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
    public class RenterServiceTest
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
        public async Task RenterService_AddRenterAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var renterModel = new RenterModel
                    {
                        Name = "Jhon",
                        ContactNo = "9876543210",
                        Address = "General Maxilom Avenue",
                        Profession = "Front Desk",
                        PropertyId = Guid.Parse("6B4621F3-7102-4953-8D5F-75F71B1729E6")
                    };

                    var renter = new Renter
                    {
                        Id = Guid.NewGuid(),
                        Name = renterModel.Name,
                        ContactNo = renterModel.ContactNo,
                        Address = renterModel.Address,
                        Profession = renterModel.Profession,
                        PropertyId = renterModel.PropertyId
                    };

                    db.Renter.Add(renter);
                    var result = await db.SaveChangesAsync();

                    Assert.IsTrue(result == 1);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Renter", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RenterService_AddBulkRenterAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                    var properties = await db.RealEstateProperties.Where(p => p.UserId == userId).ToListAsync();

                    var propertyList = properties.Take(5);

                    foreach (var item in propertyList)
                    {
                        var renterDto = new AddRenterDTO
                        {
                            Name = $"Renter Sample",
                            ContactNo = "123456789",
                            Address = $"Cebu Sample",
                            Profession = $"Prof Sample",
                            PropertyId = item.Id
                        };

                        var renter = new Renter
                        {
                            Id = Guid.NewGuid(),
                            Name = renterDto.Name,
                            ContactNo = renterDto.ContactNo,
                            Address = renterDto.Address,
                            Profession = renterDto.Profession,
                            PropertyId = renterDto.PropertyId
                        };

                        db.Renter.Add(renter);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Bulk Renter", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RenterService_GetRentersPerPropertyAsync_Test()
        {
            try
            {
                using (var con = new SqlConnection(config["Database:ConnectionString"]))
                {
                    con.Open();

                    var propertyId = Guid.Parse("6B4621F3-7102-4953-8D5F-75F71B1729E6");

                    var renters = await con.QueryAsync<RenterListDTO>("sp_GetRentersPerProperty", new { PropertyId = propertyId }, commandType: CommandType.StoredProcedure);

                    Assert.IsTrue(renters.Count() > 0);

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Renters Per Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        //[TestMethod]
        //public async Task RenterService_AssignRenterRoomAsync_Test()
        //{
        //    try
        //    {
        //        using (var db = new DatabaseContext(dbContextOpt.Options))
        //        {
        //            var assignRoom = new RoomRented
        //            {
        //                RenterId = Guid.Parse("2AC48057-8D5D-4C5A-8B1F-05464C62CEED"),
        //                RoomId = Guid.Parse("61110F4B-E943-4A51-A264-FC58C8ADD008")
        //            };

        //            db.RoomsRented.Add(assignRoom);
        //            var result = await db.SaveChangesAsync();

        //            Assert.IsTrue(result == 1);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logService.Log("Assign Room To Renter", ex.InnerException.Message, ex.Message, ex.StackTrace);
        //        Assert.Fail();
        //    }
        //}

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            return config;
        }
    }
}