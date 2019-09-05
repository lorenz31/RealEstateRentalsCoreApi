using RealEstateCore.Infrastructure.DataContext;
using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Core.Helpers;
using RealEstateCore.Core.Models;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.Security;
using RealEstateCore.Core.BusinessModels.DTO;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class PropertyServiceTest
    {
        IConfiguration config;

        LoggerService logService;

        DatabaseContext db;
        DbContextOptionsBuilder<DatabaseContext> dbContextOpt;

        string connectionString;

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
        public async Task PropertyService_AddPropertyAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbContextOpt.Options))
                {
                    Guid PropertyId = Guid.NewGuid();
                    Guid UserId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");
                    string Name = $"Trail";
                    string Address = $"Trail Address";
                    string City = $"Trail City";
                    string ContactNo = "9632587410";
                    string Owner = $"Trail Owner";
                    int TotalRooms = 0;

                    var propertyModel = new PropertyModel
                    {
                        PropertyId = Guid.NewGuid(),
                        UserId = UserId,
                        Name = Name,
                        Address = Address,
                        City = City,
                        ContactNo = ContactNo,
                        Owner = Owner,
                        TotalRooms = TotalRooms
                    };

                    var property = new RealEstateProperty
                    {
                        Id = Guid.NewGuid(),
                        UserId = propertyModel.UserId,
                        Name = propertyModel.Name,
                        Address = propertyModel.Address,
                        City = propertyModel.City,
                        ContactNo = propertyModel.ContactNo,
                        Owner = propertyModel.Owner,
                        TotalRooms = propertyModel.TotalRooms
                    };

                    db.RealEstateProperties.Add(property);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add New Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task PropertyService_AddPropertyBulkAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbContextOpt.Options))
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        Guid PropertyId = Guid.NewGuid();
                        Guid UserId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");
                        string Name = $"Trail {i}";
                        string Address = $"Trail Address {i}";
                        string City = $"Trail City {i}";
                        string ContactNo = "9632587410";
                        string Owner = $"Trail Owner {i}";
                        int TotalRooms = 0;

                        var propertyModel = new PropertyModel
                        {
                            PropertyId = Guid.NewGuid(),
                            UserId = UserId,
                            Name = Name,
                            Address = Address,
                            City = City,
                            ContactNo = ContactNo,
                            Owner = Owner,
                            TotalRooms = TotalRooms
                        };

                        var property = new RealEstateProperty
                        {
                            Id = Guid.NewGuid(),
                            UserId = propertyModel.UserId,
                            Name = propertyModel.Name,
                            Address = propertyModel.Address,
                            City = propertyModel.City,
                            ContactNo = propertyModel.ContactNo,
                            Owner = propertyModel.Owner,
                            TotalRooms = propertyModel.TotalRooms
                        };

                        db.RealEstateProperties.Add(property);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add New Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task PropertyService_AddPropertyTermsAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbContextOpt.Options))
                {
                    Guid PropertyId = Guid.Parse("D95FF806-C50A-4A54-A476-02D350B2C6FA");

                    var propertyTerms = new PropertyTermsModel
                    {
                        PropertyId = PropertyId,
                        MonthDeposit = 2,
                        MonthAdvance = 1
                    };

                    var settings = new PropertySettings
                    {
                        Id = Guid.NewGuid(),
                        MonthDeposit = propertyTerms.MonthDeposit,
                        MonthAdvance = propertyTerms.MonthAdvance,
                        PropertyId = propertyTerms.PropertyId
                    };

                    db.Settings.Add(settings);
                    var result = await db.SaveChangesAsync();

                    Assert.IsTrue(result == 1);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Property Terms", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task PropertyService_GetPropertiesAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");

                    var properties = await db.RealEstateProperties
                        .AsNoTracking()
                        .Select(p => new PropertiesDTO
                        {
                            Id = p.Id,
                            UserId = p.UserId,
                            Name = p.Name,
                            Address = p.Address,
                            City = p.City,
                            ContactNo = p.ContactNo,
                            Owner = p.Owner,
                            TotalRooms = p.TotalRooms
                        })
                        .Where(p => p.UserId == userId)
                        .ToListAsync();

                    Assert.IsTrue(properties.Count > 0);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Owner Properties", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task PropertyService_GetPropertyInfoAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("B0B20FA9-E37D-47C7-9C8C-32784F2F3EE7");
                    var propertyId = Guid.Parse("CE463664-25C7-4F97-AFE0-0011A548F5A3");

                    var property = await db.RealEstateProperties
                        .AsNoTracking()
                        .Select(p => new PropertiesDTO
                        {
                            Id = p.Id,
                            UserId = p.UserId,
                            Name = p.Name,
                            Address = p.Address,
                            City = p.City,
                            ContactNo = p.ContactNo,
                            Owner = p.Owner,
                            TotalRooms = p.TotalRooms
                        })
                        .Where(p => p.UserId == userId && p.Id == propertyId)
                        .SingleOrDefaultAsync();

                    Assert.IsNotNull(property);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Property Info", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task PropertyService_UpdatePropertyInfoAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbContextOpt.Options))
                {
                    var userId = Guid.Parse("AC087774-4D7C-47CA-A926-373D9A6C580A");
                    var propertyId = Guid.Parse("673F8BED-9909-4667-B546-014406B530A0");

                    var propertyInfo = new PropertyModel
                    {
                        PropertyId = propertyId,
                        Name = "Test",
                        Address = "Test Address",
                        City = "Test City",
                        ContactNo = "12345678901",
                        Owner = "Test Owner",
                        TotalRooms = 10,
                        UserId = userId
                    };

                    var property = await db.RealEstateProperties.SingleOrDefaultAsync(p => p.UserId == propertyInfo.UserId && p.Id == propertyInfo.PropertyId);

                    if (property != null)
                    {
                        property.Name = propertyInfo.Name;
                        property.Address = propertyInfo.Address;
                        property.City = propertyInfo.City;
                        property.ContactNo = propertyInfo.ContactNo;
                        property.Owner = propertyInfo.Owner;
                        property.TotalRooms = propertyInfo.TotalRooms;

                        db.RealEstateProperties.Update(property);
                        var result = await db.SaveChangesAsync();

                        Assert.IsTrue(result == 1, "Error updating property info.");
                    }
                    else
                        Assert.Fail("No property found.");
                }
            }
            catch (Exception ex)
            {
                logService.Log("Update Property Info", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        #region Using ADO.NET
        [TestMethod]
        public void PropertyService_AddPropertyADO_Test()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    Guid PropertyId = Guid.NewGuid();
                    Guid UserId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");
                    string Name = $"Hey Fellas";
                    string Address = $"22-K General Maxilom Avenue";
                    string City = $"Cebu City";
                    string ContactNo = "0123456789";
                    string Owner = $"Secret";
                    int TotalRooms = 5;

                    var propertyModel = new PropertyModel
                    {
                        PropertyId = PropertyId,
                        UserId = UserId,
                        Name = Name,
                        Address = Address,
                        City = City,
                        ContactNo = ContactNo,
                        Owner = Owner,
                        TotalRooms = TotalRooms
                    };

                    var property = new RealEstateProperty
                    {
                        Id = propertyModel.PropertyId,
                        UserId = propertyModel.UserId,
                        Name = propertyModel.Name,
                        Address = propertyModel.Address,
                        City = propertyModel.City,
                        ContactNo = propertyModel.ContactNo,
                        Owner = propertyModel.Owner,
                        TotalRooms = propertyModel.TotalRooms
                    };

                    SqlCommand cmd = new SqlCommand("sp_AddRealEstateProperty", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@Id", property.Id),
                        new SqlParameter("@UserId", property.UserId),
                        new SqlParameter("@Name", property.Name),
                        new SqlParameter("@Address", property.Address),
                        new SqlParameter("@City", property.City),
                        new SqlParameter("@ContactNo", property.ContactNo),
                        new SqlParameter("@Owner", property.Owner),
                        new SqlParameter("@TotalRooms", property.TotalRooms)
                    };

                    cmd.Parameters.AddRange(parameters);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    //Assert.IsTrue(rows > 1);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add New Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void PropertyService_AddPropertyBulkADO_Test()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        Guid PropertyId = Guid.NewGuid();
                        Guid UserId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");
                        string Name = $"Test Property {i}";
                        string Address = $"Test Address {i}";
                        string City = $"Test City {i}";
                        string ContactNo = "9632587410";
                        string Owner = $"Test Owner {i}";
                        int TotalRooms = 10;

                        var propertyModel = new PropertyModel
                        {
                            PropertyId = PropertyId,
                            UserId = UserId,
                            Name = Name,
                            Address = Address,
                            City = City,
                            ContactNo = ContactNo,
                            Owner = Owner,
                            TotalRooms = TotalRooms
                        };

                        var property = new RealEstateProperty
                        {
                            Id = Guid.NewGuid(),
                            UserId = propertyModel.UserId,
                            Name = propertyModel.Name,
                            Address = propertyModel.Address,
                            City = propertyModel.City,
                            ContactNo = propertyModel.ContactNo,
                            Owner = propertyModel.Owner,
                            TotalRooms = propertyModel.TotalRooms
                        };

                        SqlCommand cmd = new SqlCommand("sp_AddRealEstateProperty", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", property.Id);
                        cmd.Parameters.AddWithValue("@UserId", property.UserId);
                        cmd.Parameters.AddWithValue("@Name", property.Name);
                        cmd.Parameters.AddWithValue("@Address", property.Address);
                        cmd.Parameters.AddWithValue("@City", property.City);
                        cmd.Parameters.AddWithValue("@ContactNo", property.ContactNo);
                        cmd.Parameters.AddWithValue("@Owner", property.Owner);
                        cmd.Parameters.AddWithValue("@TotalRooms", property.TotalRooms);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add New Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void PropertyService_AddPropertyTermsADO_Test()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    Guid PropertyId = Guid.Parse("1897C7A7-EB70-4A11-9EF1-C6714DF34D5A");

                    var propertyTerms = new PropertyTermsModel
                    {
                        PropertyId = PropertyId,
                        MonthDeposit = 2,
                        MonthAdvance = 1
                    };

                    var settings = new PropertySettings
                    {
                        Id = Guid.NewGuid(),
                        MonthDeposit = propertyTerms.MonthDeposit,
                        MonthAdvance = propertyTerms.MonthAdvance,
                        PropertyId = propertyTerms.PropertyId
                    };

                    SqlCommand cmd = new SqlCommand("sp_AddPropertyTerms", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", settings.Id);
                    cmd.Parameters.AddWithValue("@PropertyId", settings.PropertyId);
                    cmd.Parameters.AddWithValue("@MonthAdv", settings.MonthAdvance);
                    cmd.Parameters.AddWithValue("@MonthDep", settings.MonthDeposit);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    //Assert.IsTrue(rows > 1);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add Property Terms", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void PropertyService_GetPropertiesADO_Test()
        {
            try
            {
                List<PropertiesDTO> properties = new List<PropertiesDTO>();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    var userId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");

                    SqlCommand cmd = new SqlCommand("sp_GetOwnerProperties", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        PropertiesDTO property = new PropertiesDTO();

                        property.Id = Guid.Parse(rdr["Id"].ToString());
                        property.UserId = Guid.Parse(rdr["UserId"].ToString());
                        property.Name = rdr["Name"].ToString();
                        property.Address = rdr["Address"].ToString();
                        property.City = rdr["City"].ToString();
                        property.ContactNo = rdr["ContactNo"].ToString();
                        property.Owner = rdr["Owner"].ToString();
                        property.TotalRooms = Convert.ToInt16(rdr["TotalRooms"]);

                        properties.Add(property);
                    }
                    con.Close();

                    Assert.IsTrue(properties.Count > 0);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Owner Properties", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void PropertyService_GetPropertyInfoADO_Test()
        {
            try
            {
                PropertiesDTO property = new PropertiesDTO();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    var userId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");
                    var propertyId = Guid.Parse("76589131-9FF7-40CE-A4FD-F5F1C78AD18B");

                    SqlCommand cmd = new SqlCommand("sp_GetPropertyInfo", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter[] qparams =
                    {
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@PropertyId", propertyId)
                    };

                    cmd.Parameters.AddRange(qparams);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        property.Id = Guid.Parse(rdr["Id"].ToString());
                        property.UserId = Guid.Parse(rdr["UserId"].ToString());
                        property.Name = rdr["Name"].ToString();
                        property.Address = rdr["Address"].ToString();
                        property.City = rdr["City"].ToString();
                        property.ContactNo = rdr["ContactNo"].ToString();
                        property.Owner = rdr["Owner"].ToString();
                        property.TotalRooms = Convert.ToInt16(rdr["TotalRooms"]);
                    }

                    Assert.IsNotNull(property);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Property Info", ex.InnerException.Message, ex.Message, ex.StackTrace);
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
    }
}