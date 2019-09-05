using RealEstateCore.Infrastructure.DataContext;
using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Core.Helpers;
using RealEstateCore.Core.Models;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.Security;
using RealEstateCore.Core.BusinessModels.DTO;

using Dapper;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class PropertyServiceDapperTest
    {
        IConfiguration config;

        LoggerService logService;
        
        string connectionString;

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            return config;
        }

        [TestInitialize]
        public void Setup()
        {
            config = InitConfiguration();
            connectionString = config["Database:ConnectionString"];

            logService = new LoggerService();
        }

        #region USING DAPPER
        [TestMethod]
        public async Task PropertyService_GetPropertiesDapperAsync_Test()
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();

                    var userId = Guid.Parse("B0B20FA9-E37D-47C7-9C8C-32784F2F3EE7");

                    var properties = await con.QueryAsync<PropertiesDTO>("sp_GetOwnerProperties", new { UserId = userId }, commandType: CommandType.StoredProcedure);

                    Assert.IsTrue(properties.AsList().Count > 0);

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                logService.Log("Get Owner Properties", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task PropertyService_AddPropertyBulkDapperAsync_Test()
        {
            try
            {
                using (var con = new SqlConnection(connectionString))
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

                        var propertyData = @"INSERT INTO RealEstateProperties (Id, UserId, Name, Address, City, ContactNo, Owner, TotalRooms) VALUES (@PropertyID, @UserId, @Name, @Address, @City, @ContactNo, @Owner, @TotalRooms)";
                        
                    }
                }
            }
            catch (Exception ex)
            {
                logService.Log("Add New Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail();
            }
        }
        #endregion
    }
}
