using RealEstateCore.Infrastructure.DataContext;
using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Core.Models;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.BusinessModels.Interface;

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
    public class RoomServiceTest
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

            logService = new LoggerService();
        }

        #region Room Tests
        [TestMethod]
        public async Task RoomService_AddRoomAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var room = new Room
                    {
                        Id = Guid.NewGuid(),
                        Name = "6 Room",
                        TotalBeds = 2,
                        RoomTypeId = Guid.Parse("05BD8AD3-8C7A-4F23-ADCF-A09D5639B42D"),
                        PropertyId = Guid.Parse("A63FE370-B325-40C5-8EA3-90B49CCD9FE9")
                    };

                    db.Rooms.Add(room);
                    var result = await db.SaveChangesAsync();

                    Assert.IsTrue(result == 1);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Room", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RoomService_AddRoomBulkAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                    var propertyId = Guid.Parse("673F8BED-9909-4667-B546-014406B530A0");
                    Room room = null;
                    var singleBed = 1;
                    var doubleBed = 2;
                    var dorm4 = 4;
                    var dorm6 = 6;

                    var property = await db.RealEstateProperties
                        .Where(p => p.UserId == userId)
                        .Select(p => new { p.Id, p.Name })
                        .ToListAsync();

                    foreach (var propty in property)
                    {
                        var roomTypeIds = await db.RoomTypes
                            .AsNoTracking()
                            .Where(r => r.PropertyId == propty.Id)
                            .Select(r => new { r.Id, r.Type })
                            .ToListAsync();

                        foreach (var roomids in roomTypeIds)
                        {
                            if (roomids.Type.Contains("Single"))
                            {
                                room = new Room
                                {
                                    Id = Guid.NewGuid(),
                                    Name = $"{propty.Name} Single Room",
                                    TotalBeds = singleBed,
                                    RoomTypeId = roomids.Id,
                                    PropertyId = propty.Id
                                };
                            }

                            if (roomids.Type.Contains("Double"))
                            {
                                room = new Room
                                {
                                    Id = Guid.NewGuid(),
                                    Name = $"{propty.Name} Double Room",
                                    TotalBeds = doubleBed,
                                    RoomTypeId = roomids.Id,
                                    PropertyId = propty.Id
                                };
                            }

                            if (roomids.Type.Contains("4"))
                            {
                                room = new Room
                                {
                                    Id = Guid.NewGuid(),
                                    Name = $"{propty.Name} 4 Room",
                                    TotalBeds = dorm4,
                                    RoomTypeId = roomids.Id,
                                    PropertyId = propty.Id
                                };
                            }

                            if (roomids.Type.Contains("6"))
                            {
                                room = new Room
                                {
                                    Id = Guid.NewGuid(),
                                    Name = $"{propty.Name} 6 Room",
                                    TotalBeds = dorm6,
                                    RoomTypeId = roomids.Id,
                                    PropertyId = propty.Id
                                };
                            }

                            db.Rooms.Add(room);
                            var result = await db.SaveChangesAsync();

                            Assert.IsTrue(result == 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Bulk Room", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RoomService_GetRoomsPerPropertyAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                    var propertyId = Guid.Parse("A52944AA-392E-4240-A6A5-4BC8A734DB61");

                    var properties = await db.RealEstateProperties
                        .AsNoTracking()
                        .Join(
                            db.Rooms,
                            property => property.Id,
                            room => room.PropertyId,
                            (prop, room) => new { prop, room }
                        )
                        .Join(
                            db.RoomTypes,
                            rooms => rooms.room.RoomTypeId,
                            roomtype => roomtype.Id,
                            (rooms, roomtype) => new { rooms, roomtype }
                        )
                        .Where(p => p.rooms.prop.UserId == userId && p.rooms.prop.Id == propertyId)
                        .Select(p => new RoomsListInfoDTO
                        {
                            UserId = p.rooms.prop.UserId,
                            PropertyId = p.rooms.prop.Id,
                            PropertyName = p.rooms.prop.Name,
                            RoomName = p.rooms.room.Name,
                            RoomTypeName = p.roomtype.Type,
                            TotalBeds = p.rooms.room.TotalBeds
                        })
                        .ToListAsync();

                    Assert.IsNotNull(properties);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Rooms Per Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RoomService_GetRoomsWithPricesAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                    var propertyId = Guid.Parse("96F8443D-388E-42B5-BD1E-6CAA72E2ADAF");

                    var roomList = await db.RealEstateProperties
                        .AsNoTracking()
                        .Join(
                            db.Rooms,
                            property => property.Id,
                            room => room.PropertyId,
                            (prop, room) => new { prop, room }
                        )
                        .Join(
                            db.RoomTypes,
                            rooms => rooms.room.RoomTypeId,
                            roomtype => roomtype.Id,
                            (rooms, roomtype) => new { rooms, roomtype }
                        )
                        .Where(p => p.rooms.prop.UserId == userId && p.rooms.prop.Id == propertyId
                        )
                        .Select(p => new RoomPriceDTO
                        {
                            RoomName = p.rooms.room.Name,
                            Price = p.roomtype.Price
                        })
                        .ToListAsync();

                    Assert.IsNotNull(roomList);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Rooms With Prices", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RoomService_GetRoomInfoAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                    var propertyId = Guid.Parse("96F8443D-388E-42B5-BD1E-6CAA72E2ADAF");
                    var roomId = Guid.Parse("A2037E8C-3518-4007-AA32-6B2960390503");

                    var roomInfo = await db.RealEstateProperties
                        .AsNoTracking()
                        .Join(
                            db.Rooms,
                            property => property.Id,
                            room => room.PropertyId,
                            (prop, room) => new { prop, room }
                        )
                        .Join(
                            db.RoomTypes,
                            rooms => rooms.room.RoomTypeId,
                            roomtype => roomtype.Id,
                            (rooms, roomtype) => new { rooms, roomtype }
                        )
                        .Where(
                            p => p.rooms.prop.UserId == userId &&
                            p.rooms.prop.Id == propertyId &&
                            p.rooms.room.Id == roomId
                        )
                        .Select(p => new RoomInfoDTO
                        {
                            RoomName = p.rooms.room.Name,
                            RoomTypeName = p.roomtype.Type,
                            TotalBeds = p.rooms.room.TotalBeds,
                            Price = p.roomtype.Price,
                            Features = db.RoomFeatures.Where(f => f.RoomId == p.rooms.room.Id).Select(f => f.Name).ToList(),
                            FloorPlans = db.FloorPlans.Where(f => f.RoomId == p.rooms.room.Id).Select(f => f.Img).ToList()
                        })
                        .SingleOrDefaultAsync();

                    Assert.IsNotNull(roomInfo);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Room Info", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RoomService_GetAvailableBedsPerRoomAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                    var propertyId = Guid.Parse("D95FF806-C50A-4A54-A476-02D350B2C6FA");
                    var roomId = Guid.Parse("61110F4B-E943-4A51-A264-FC58C8ADD008");

                    var rooms = await db.RealEstateProperties
                        .AsNoTracking()
                        .Join(
                            db.Rooms,
                            property => property.Id,
                            room => room.PropertyId,
                            (prop, room) => new { prop, room }
                        )
                        .Where(
                            p => p.prop.UserId == userId && p.prop.Id == propertyId
                        )
                        .Select(p => p)
                        .ToListAsync();

                    var occupiedRooms = db.RoomsRented.Where(rr => rr.RoomId == roomId).Count();

                    var roomStats = rooms
                        .Where(r => r.room.Id == roomId)
                        .Select(rs => new AvailableBedsDTO
                        {
                            RoomId = rs.room.Id,
                            BedsOccupied = occupiedRooms,
                            BedsAvailable = rs.room.TotalBeds - occupiedRooms
                        })
                        .SingleOrDefault();

                    Assert.IsNotNull(roomStats);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Room Statistics", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }
        #endregion

        #region Room Features
        [TestMethod]
        public async Task RoomService_AddRoomFeaturesBulkAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var propertyId = Guid.Parse("673F8BED-9909-4667-B546-014406B530A0");

                    var rooms = await db.Rooms
                        .Where(p => p.PropertyId == propertyId)
                        .Select(r => new { r.Id, r.Name })
                        .ToListAsync();

                    foreach (var room in rooms)
                    {
                        List<RoomFeatures> features = null;

                        features = new List<RoomFeatures>()
                        {
                            new RoomFeatures
                            {
                                Id = Guid.NewGuid(),
                                Name = "Air Conditioner",
                                RoomId = room.Id
                            },
                            new RoomFeatures
                            {
                                Id = Guid.NewGuid(),
                                Name = "Beds",
                                RoomId = room.Id
                            },
                            new RoomFeatures
                            {
                                Id = Guid.NewGuid(),
                                Name = "Individual Lockers",
                                RoomId = room.Id
                            }
                        };

                        db.RoomFeatures.AddRange(features);
                        var result = await db.SaveChangesAsync();

                        Assert.IsTrue(result == 1);
                    }
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Room Features Bulk", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }
        #endregion

        #region Room Floor Plan
        [TestMethod]
        public async Task RoomService_AddRoomFloorPlanBulkAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var propertyId = Guid.Parse("673F8BED-9909-4667-B546-014406B530A0");

                    var rooms = await db.Rooms
                        .Where(p => p.PropertyId == propertyId)
                        .Select(r => new { r.Id, r.Name })
                        .ToListAsync();

                    foreach (var room in rooms)
                    {
                        List<RoomFloorPlan> floorPlans = null;
                        var floorPlan = @"D:\Repository\RealEstateCoreApi\RealEstateCore\wwwroot\Images\Properties\FloorPlan\floorplan.png";

                        floorPlans = new List<RoomFloorPlan>()
                        {
                            new RoomFloorPlan
                            {
                                Id = Guid.NewGuid(),
                                Img = floorPlan,
                                RoomId = room.Id
                            },
                            new RoomFloorPlan
                            {
                                Id = Guid.NewGuid(),
                                Img = floorPlan,
                                RoomId = room.Id
                            },
                            new RoomFloorPlan
                            {
                                Id = Guid.NewGuid(),
                                Img = floorPlan,
                                RoomId = room.Id
                            }
                        };

                        db.FloorPlans.AddRange(floorPlans);
                        var result = await db.SaveChangesAsync();

                        Assert.IsTrue(result == 1);
                    }
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Room Floor Plan Bulk", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }
        #endregion

        #region Room Types Tests
        [TestMethod]
        public async Task RoomService_AddRoomTypeAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                    var propertyId = Guid.Parse("D95FF806-C50A-4A54-A476-02D350B2C6FA");

                    var roomTypeModel = new RoomTypeModel
                    {
                        Type = "Double Occupancy",
                        Price = 6000,
                        PropertyId = propertyId
                    };

                    var roomType = new RoomTypes
                    {
                        Id = Guid.NewGuid(),
                        Type = roomTypeModel.Type,
                        Price = roomTypeModel.Price,
                        PropertyId = roomTypeModel.PropertyId
                    };

                    db.RoomTypes.Add(roomType);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Room Type", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RoomService_AddRoomTypesBulkAsync_Test()
        {
            try
            {
                using (var db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("5812C4B1-80D6-4000-A0D0-839C6AE91421");
                    var propertyIds = await db.RealEstateProperties.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();

                    foreach (var id in propertyIds)
                    {
                        List<RoomTypes> roomTypes = new List<RoomTypes>()
                        {
                            new RoomTypes
                            {
                                Id = Guid.NewGuid(),
                                Type = "Single Occupancy",
                                Price = 3000,
                                PropertyId = id
                            },
                            new RoomTypes
                            {
                                Id = Guid.NewGuid(),
                                Type = "Double Occupancy",
                                Price = 6000,
                                PropertyId = id
                            },
                            new RoomTypes
                            {
                                Id = Guid.NewGuid(),
                                Type = "Dorm Room (4 person occupancy)",
                                Price = 3350,
                                PropertyId = id
                            },
                            new RoomTypes
                            {
                                Id = Guid.NewGuid(),
                                Type = "Dorm Room (6 person occupancy)",
                                Price = 2750,
                                PropertyId = id
                            }
                        };

                        db.RoomTypes.AddRange(roomTypes);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Room Types Bulk", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task RoomService_GetRoomTypesPerPropertyDapperAsync_Test()
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();

                    var propertyId = Guid.Parse("6B4621F3-7102-4953-8D5F-75F71B1729E6");

                    var roomTypes = await con.QueryAsync<RoomTypeModel>("sp_GetRoomTypesPerProperty", new { PropertyId = propertyId }, commandType: CommandType.StoredProcedure);

                    Assert.IsTrue(roomTypes.AsList().Count > 0);

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Property Room Types", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }
        #endregion

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            return config;
        }

        //[TestMethod]
        //public async Task RoomService_UpdateRoomInfoAsync_Test()
        //{
        //    try
        //    {
        //        using (var db = new DatabaseContext(dbContextOpt.Options))
        //        {
        //            var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
        //            var propertyId = Guid.Parse("A52944AA-392E-4240-A6A5-4BC8A734DB61");
        //            var roomId = Guid.Parse("BD0FEAC7-ED44-4236-A09E-000877A9A99E");

        //            var roomInfo = await db.RealEstateProperties
        //                .AsNoTracking()
        //                .Join(
        //                    db.Rooms,
        //                    property => property.Id,
        //                    room => room.PropertyId,
        //                    (prop, room) => new { prop, room }
        //                )
        //                .Join(
        //                    db.RoomTypes,
        //                    rooms => rooms.room.RoomTypeId,
        //                    roomtype => roomtype.Id,
        //                    (rooms, roomtype) => new { rooms, roomtype }
        //                )
        //                .Where(
        //                    p => p.rooms.prop.UserId == userId &&
        //                    p.rooms.prop.Id == propertyId &&
        //                    p.rooms.room.Id == roomId
        //                )
        //                .Select(p => new RoomInfoDTO
        //                {
        //                    PropertyId = p.rooms.prop.Id,
        //                    RoomId = p.rooms.room.Id,
        //                    RoomName = p.rooms.room.Name,
        //                    RoomTypeName = p.roomtype.Type,
        //                    TotalBeds = p.rooms.room.TotalBeds,
        //                    Price = p.roomtype.Price,
        //                    Features = db.RoomFeatures.Where(f => f.RoomId == p.rooms.room.Id).Select(f => f.Name).ToList(),
        //                    FloorPlans = db.FloorPlans.Where(f => f.RoomId == p.rooms.room.Id).Select(f => f.Img).ToList()
        //                })
        //                .SingleOrDefaultAsync();

        //            Assert.IsNotNull(roomInfo);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logService.Log("Error", ex.Message);
        //        logService.Log("Stack Trace", ex.StackTrace);
        //        Assert.Fail();
        //    }
        //}
    }
}
