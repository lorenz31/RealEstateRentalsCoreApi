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

                    var userId = Guid.Parse("1923610F-A467-40F3-8652-773A86DE4314");

                    var properties = await con.QueryAsync<PropertiesTermsDTO>("sp_GetOwnerPropertiesWithTerms", new { UserId = userId }, commandType: CommandType.StoredProcedure);

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
        #endregion
    }
}
