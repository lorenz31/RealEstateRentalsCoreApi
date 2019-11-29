using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.Services;
using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.Models;
using RealEstateCore.Infrastructure.DataContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace RealEstateCore.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly DatabaseContext _db;
        private readonly IConfiguration _config;
        private readonly IResponseModel _response;
        private readonly ILoggerService _loggerService;

        public TransactionService(
            DatabaseContext db,
            IConfiguration config,
            IResponseModel response,
            ILoggerService loggerService)
        {
            _db = db;
            _config = config;
            _response = response;
            _loggerService = loggerService;
        }

        public async Task<IResponseModel> AddCheckInTransactionAsync(ICheckinModel model)
        {
            using (var con = new SqlConnection(_config["Database:ConnectionString"]))
            {
                try
                {
                    con.Open();

                    var renterInfo = await con.QueryAsync<RenterInfoDTO>("sp_GetRenterInfo", new { PropertyId = model.PropertyId, RenterId = model.RenterId }, commandType: CommandType.StoredProcedure);

                    var selectedRoom = await con.QueryAsync<RoomPriceDTO>("sp_GetRoomInfo", new { UserId = model.UserId, PropertyId = model.PropertyId }, commandType: CommandType.StoredProcedure);

                    var terms = await con.QueryAsync<decimal>("sp_GetPropertyTerms", new { PropertyId = model.PropertyId }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    var checkIn = DateTime.Now;
                    renterInfo.FirstOrDefault().CheckIn = checkIn;

                    _db.Renter.Update(new Renter
                    {
                        Id = renterInfo.FirstOrDefault().Id,
                        Name = renterInfo.FirstOrDefault().Name,
                        ContactNo = renterInfo.FirstOrDefault().ContactNo,
                        Address = renterInfo.FirstOrDefault().Address,
                        Profession = renterInfo.FirstOrDefault().Profession,
                        CheckIn = renterInfo.FirstOrDefault().CheckIn,
                        CheckOut = renterInfo.FirstOrDefault().CheckOut,
                        PropertyId = renterInfo.FirstOrDefault().PropertyId
                    });

                    var renterInfoUpdated = await _db.SaveChangesAsync();

                    if (renterInfoUpdated == 1)
                    {
                        await ComputeTransactionAsync(_db, selectedRoom.FirstOrDefault().Price, (int)terms.FirstOrDefault(), (decimal)model.DownPayment, checkIn, model.RenterId);
                        await AssignRoomAsync(_db, model.RenterId, model.RoomId);

                        _response.Status = true;
                        _response.Message = "";

                        return _response;
                    }
                    else
                    {
                        _response.Status = false;
                        _response.Message = "Error updating renter info.";

                        return _response;
                    }
                }
                catch (Exception ex)
                {
                    _loggerService.Log("Renter Checkin", ex.InnerException.Message, ex.Message, ex.StackTrace);

                    _response.Status = false;
                    _response.Message = "";

                    return _response;
                }
            }
        }

        public async Task<List<PropertyTransactionsDTO>> GetTransactionPerPropertyAsync(IBaseModel model)
        {
            try
            {
                var transactionHist = await _db.RealEstateProperties
                    .Join(
                        _db.Renter,
                        propty => propty.Id,
                        renter => renter.PropertyId,
                        (propty, renter) => new { propty, renter }
                    )
                    .Join(
                        _db.Transactions,
                        renter => renter.renter.Id,
                        transaction => transaction.RenterId,
                        (renter, transaction) => new { renter, transaction }
                    )
                    .Select(t => new PropertyTransactionsDTO
                    {
                        UserId = t.renter.propty.UserId,
                        PropertyId = t.renter.propty.Id,
                        TransactionId = t.transaction.Id,
                        DatePaid = t.transaction.DatePaid,
                        AmountDue = (double)t.transaction.AmountDue,
                        NextDateDue = t.transaction.NextDateDue,
                        RenterId = t.transaction.RenterId,
                        Balance = (double)t.transaction.Balance,
                        PaymentFor = t.transaction.PaymentFor,
                        AmountPaid = (double)t.transaction.AmountPaid,
                    })
                    .Where(p => p.UserId == model.UserId && p.PropertyId == model.PropertyId)
                    .OrderByDescending(t => t.DatePaid)
                    .ToListAsync();

                return transactionHist ?? null;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Property Transactions", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }

        private async Task RecurringMonthlyBillAsync()
        {
            try
            {
                var renterId = Guid.Parse("2AC48057-8D5D-4C5A-8B1F-05464C62CEED");
                var payment = 4800;

                var lastTransaction = _db.Transactions
                        .Where(t => t.RenterId == renterId)
                        .OrderByDescending(t => t.DatePaid)
                        .First();

                var monthlyRent = await _db.RoomsRented
                    .Join(
                        _db.Rooms,
                        roomRented => roomRented.RoomId,
                        room => room.Id,
                        (roomRented, room) => new { roomRented, room }
                    )
                    .Join(
                        _db.RoomTypes,
                        room => room.room.RoomTypeId,
                        roomType => roomType.Id,
                        (room, roomType) => new { room, roomType.Price }
                    )
                    .Where(renter => renter.room.roomRented.RenterId == renterId)
                    .Select(room => room.Price)
                    .SingleOrDefaultAsync();

                decimal amountDue = lastTransaction.Balance + monthlyRent;
                decimal balance = payment - amountDue;

                var transaction = new TransactionHistory
                {
                    Id = Guid.NewGuid(),
                    DatePaid = DateTime.Now,
                    AmountDue = amountDue,
                    AmountPaid = payment,
                    PaymentFor = "Rental",
                    Balance = balance,
                    NextDateDue = lastTransaction.NextDateDue.AddMonths(1),
                    RenterId = renterId
                };

                _db.Transactions.Add(transaction);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _loggerService.Log("Recurring Monthly Billing", ex.InnerException.Message, ex.Message, ex.StackTrace);
                return;
            }
        }

        private async Task ComputeTransactionAsync(DatabaseContext db, decimal price, int terms, decimal downpayment, DateTime checkin, Guid renterid)
        {
            decimal checkInAmtDue = price * terms;
            decimal balance = checkInAmtDue - downpayment;

            var transaction = new TransactionHistory
            {
                Id = Guid.NewGuid(),
                DatePaid = DateTime.Now,
                AmountDue = checkInAmtDue,
                AmountPaid = downpayment,
                PaymentFor = "Rental",
                Balance = balance,
                NextDateDue = checkin.AddMonths(1),
                RenterId = renterid
            };

            db.Transactions.Add(transaction);
            await db.SaveChangesAsync();
        }

        private async Task AssignRoomAsync(DatabaseContext db, Guid renterid, Guid roomid)
        {
            var assignRoom = new RoomRented
            {
                RenterId = renterid,
                RoomId = roomid
            };

            db.RoomsRented.Add(assignRoom);
            await db.SaveChangesAsync();
        }
    }
}