using RealEstateCore.Infrastructure.DataContext;
using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Core.Models;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.DTO;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class TransactionServiceTest
    {
        IConfiguration config;

        LoggerService logService;

        string connectionString;
        
        DbContextOptionsBuilder<DatabaseContext> dbContextOpt;

        [TestInitialize]
        public void Setup()
        {
            config = InitConfiguration();
            connectionString = config["Database:ConnectionString"];
            
            dbContextOpt = new DbContextOptionsBuilder<DatabaseContext>();
            dbContextOpt.UseSqlServer(connectionString);

            logService = new LoggerService();
        }

        [TestMethod]
        public async Task TransactionService_AddCheckInTransactionAsync_Test()
        {
            var userId = Guid.Parse("1923610F-A467-40F3-8652-773A86DE4314");
            var propertyId = Guid.Parse("6B4621F3-7102-4953-8D5F-75F71B1729E6");
            var roomId = Guid.Parse("D054BDF9-6AD8-41A6-B56C-63F1A0829208");
            var renterId = Guid.Parse("7C9C5B77-379F-4B02-95FC-A8D60959D110");
            var downPayment = 6000;

            using (var db = new DatabaseContext(dbContextOpt.Options))
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    var renterInfo = await con.QueryAsync<RenterInfoDTO>("sp_GetRenterInfo", new { PropertyId = propertyId, RenterId = renterId }, commandType: CommandType.StoredProcedure);

                    var selectedRoom = await con.QueryAsync<RoomPriceDTO>("sp_GetRoomInfo", new { UserId = userId, PropertyId = propertyId }, commandType: CommandType.StoredProcedure);

                    var terms = await con.QueryAsync<decimal>("sp_GetPropertyTerms", new { PropertyId = propertyId }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    var checkIn = DateTime.Now;
                    renterInfo.FirstOrDefault().CheckIn = checkIn;

                    db.Renter.Update(new Renter
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

                    var renterInfoUpdated = await db.SaveChangesAsync();

                    if (renterInfoUpdated == 1)
                    {
                        await ComputeTransactionAsync(db, selectedRoom.FirstOrDefault().Price, (int)terms.FirstOrDefault(), downPayment, checkIn, renterId);
                        await AssignRoomAsync(db, renterId, roomId);

                        Assert.IsTrue(renterInfoUpdated == 1, "Error updating renter checkin info.");
                    }
                    else
                    {
                        Assert.Fail("Error updating renter checkin info.");
                    }
                }
                catch (Exception ex)
                {
                    logService.Log("Renter Checkin", ex.InnerException.Message, ex.Message, ex.StackTrace);
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public async Task TransactionService_RecurringMonthlyBillAsync_Test()
        {
            try
            {
                var renterId = Guid.Parse("2AC48057-8D5D-4C5A-8B1F-05464C62CEED");
                var payment = 4500;

                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var lastTransaction = db.Transactions
                        .Where(t => t.RenterId == renterId)
                        .OrderByDescending(t => t.DatePaid)
                        .First();

                    var monthlyRent = await db.RoomsRented
                        .Join(
                            db.Rooms,
                            roomRented => roomRented.RoomId,
                            room => room.Id,
                            (roomRented, room) => new { roomRented, room }
                        )
                        .Join(
                            db.RoomTypes,
                            room => room.room.RoomTypeId,
                            roomType => roomType.Id,
                            (room, roomType) => new { room, roomType.Price }
                        )
                        .Where(renter => renter.room.roomRented.RenterId == renterId)
                        .Select(room => room.Price)
                        .SingleOrDefaultAsync();
                    
                    Assert.IsNotNull(monthlyRent);

                    decimal amountDue = lastTransaction.Balance + monthlyRent;
                    decimal balance = amountDue - payment;

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

                    db.Transactions.Add(transaction);
                    var saveTransaction = await db.SaveChangesAsync();

                    Assert.IsTrue(saveTransaction == 1, "Error saving transaction.");
                }
            }
            catch (Exception ex)
            {
                logService.Log("Recurring Monthly Billing", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task TransactionService_GetTransactionPerPropertyAsync_Test()
        {
            try
            {
                var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                var propertyId = Guid.Parse("D95FF806-C50A-4A54-A476-02D350B2C6FA");

                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var transactionHist = await db.RealEstateProperties
                        .Join(
                            db.Renter,
                            propty => propty.Id,
                            renter => renter.PropertyId,
                            (propty, renter) => new { propty, renter }
                        )
                        .Join(
                            db.Transactions,
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
                            AmountDue = (double) t.transaction.AmountDue,
                            NextDateDue = t.transaction.NextDateDue,
                            RenterId = t.transaction.RenterId,
                            Balance = (double) t.transaction.Balance,
                            PaymentFor = t.transaction.PaymentFor,
                            AmountPaid = (double) t.transaction.AmountPaid,
                        })
                        .Where(p => p.UserId == userId && p.PropertyId == propertyId)
                        .OrderByDescending(t => t.DatePaid)
                        .ToListAsync();

                    Assert.IsTrue(transactionHist.Count > 0, "No transaction history.");
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Property Transactions", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        // TODO: Revisit
        [TestMethod]
        public async Task TransactionService_CheckRenterDueAsync_Test()
        {
            try
            {
                var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                var propertyId = Guid.Parse("D95FF806-C50A-4A54-A476-02D350B2C6FA");

                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var transactionHist = await db.RealEstateProperties
                        .Join(
                            db.Renter,
                            propty => propty.Id,
                            renter => renter.PropertyId,
                            (propty, renter) => new { propty, renter }
                        )
                        .Join(
                            db.Transactions,
                            renter => renter.renter.Id,
                            transaction => transaction.RenterId,
                            (renter, transaction) => new { renter, transaction }
                        )
                        .Select(t => t)
                        .Where(p => p.renter.propty.UserId == userId && p.renter.propty.Id == propertyId)
                        .ToListAsync();

                    Assert.IsTrue(transactionHist.Count > 0, "No transaction history.");
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Property Transactions", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        private async Task ComputeTransactionAsync(DatabaseContext db, decimal price, int terms, decimal downpayment, DateTime checkin, Guid renterid)
        {
            decimal checkInAmtDue = price * terms;
            decimal balance = checkInAmtDue - downpayment;
            DateTime nextDateDue = checkin.AddMonths(1);
            DateTime daysBeforeDue = nextDateDue.Subtract(TimeSpan.FromDays(3));

            var transaction = new TransactionHistory
            {
                Id = Guid.NewGuid(),
                DatePaid = DateTime.Now,
                AmountDue = checkInAmtDue,
                AmountPaid = downpayment,
                PaymentFor = "Rental",
                Balance = balance,
                NextDateDue = nextDateDue,
                DaysBeforeDue = daysBeforeDue,
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

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            return config;
        }
    }
}
