using RealEstateCore.Infrastructure.DataContext;
using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Core.Helpers;
using RealEstateCore.Core.Models;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.Security;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class AccountServiceTest
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
        public async Task AccountService_RegisterUserAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbBuilder.Options))
                {
                    var userModel = new UserModel
                    {
                        Username = null,
                        Password = "demo123!",
                        FirstName = "Test",
                        LastName = "Test"
                    };

                    var salt = PasswordHash.GenerateSalt();
                    var passwordHash = PasswordHash.ComputeHash(userModel.Password, salt);

                    var user = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        ClientId = config["AppConfiguration:Audience"],
                        Email = userModel.Username,
                        Password = Convert.ToBase64String(passwordHash),
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        EmailConfirmed = false,
                        DateRegistered = DateTime.UtcNow,
                        Salt = Convert.ToBase64String(salt)
                    };

                    db.Users.Add(user);
                    var result = await db.SaveChangesAsync();

                    Assert.IsTrue(result == 1);
                }
            }
            catch (Exception ex)
            {
                logService.Log("Register User", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task AccountService_VerifyUserAsync_Test()
        {
            try
            {
                using (db = new DatabaseContext(dbBuilder.Options))
                {
                    var model = new UserModel
                    {
                        Username = "user1",
                        Password = "demo123!",
                        ClientId = "webapp"
                    };

                    var userInfo = await db.Users.Where(u => u.ClientId == model.ClientId && u.Email == model.Username).SingleOrDefaultAsync();

                    if (userInfo == null) Assert.Fail();

                    var salt = Convert.FromBase64String(userInfo.Salt);
                    var hashPassword = Convert.FromBase64String(userInfo.Password);
                    var isVerified = PasswordHash.VerifyPassword(model.Password, salt, hashPassword);

                    if (isVerified)
                    {
                        var token = GenerateJwt(model);

                        Assert.IsNotNull(token.AccessToken);
                    }
                    else
                        Assert.Fail();
                }
            }
            catch (Exception ex)
            {
                logService.Log("Verify User", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        private ITokenModel GenerateJwt(IUserModel model)
        {
            var token = new JwtTokenBuilder(config);

            return new TokenModel
            {
                AccessToken = token.GenerateToken(),
                UserId = model.UserId,
                Email = model.Username
            };
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
