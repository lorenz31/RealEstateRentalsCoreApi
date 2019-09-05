using RealEstateCore.Infrastructure.DataContext;
using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Core.Helpers;
using RealEstateCore.Core.Models;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.Security;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class AuthenticationService
    {
        IConfiguration config;

        LoggerService logService;

        DatabaseContext db;
        DbContextOptionsBuilder<DatabaseContext> dbBuilder;

        [TestInitialize]
        public void Setup()
        {
            config = InitConfiguration();
            var connectionString = config["Database:ConnectionString"];

            dbBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            dbBuilder.UseSqlServer(connectionString);

            logService = new LoggerService();
        }

        [TestMethod]
        public async Task AccountService_VerifyUserIdAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbBuilder.Options))
                {
                    var userId = Guid.Parse("AB6244A0-6508-48B6-AB90-A6330E83350C");

                    var user = db.Users.Where(u => u.Id == userId).SingleOrDefaultAsync().Result;

                    Assert.IsNotNull(user, "No users exists.");
                }
            }
            catch (Exception ex)
            {
                logService.Log("Register User", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
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