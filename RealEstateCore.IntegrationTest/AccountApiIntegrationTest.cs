using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Core.BusinessModels.Implementation;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Text;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class AccountApiIntegrationTest
    {
        LoggerService logService;
        HttpClient client;

        [TestInitialize]
        public void Setup()
        {
            logService = new LoggerService();

            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:59518/");
            client.DefaultRequestHeaders.Clear();
        }

        [TestMethod]
        public async Task AccountApi_RegisterUserAsync_IntegrationTest()
        {
            var user = new UserModel
            {
                Username = "user1@gmail.com",
                Password = "user1",
                FirstName = "user 1",
                LastName = "user 1"
            };

            var payload = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = await client.PostAsync("api/v1/account/register", payload);
                var response = await request.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<ResponseModel>(response);

                Assert.IsTrue(content.Status, "User registration error.");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Register User", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task AccountApi_VerifyUserAsync_IntegrationTest()
        {
            var user = new UserModel
            {
                Username = "user1@gmail.com",
                Password = "user1",
                ClientId = "webapp"
            };

            var payload = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = await client.PostAsync("api/v1/Token", payload);
                var response = await request.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<TokenModel>(response);

                Assert.IsNotNull(content.AccessToken, "Invalid User/Password");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Verify User", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }
    }
}