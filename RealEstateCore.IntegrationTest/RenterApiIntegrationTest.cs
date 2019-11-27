using RealEstateCore.Infrastructure.Services;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.DTO;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class RenterApiIntegrationTest
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
        public async Task RenterApi_AddRenterAsync_IntegrationTest()
        {
            var propertyId = Guid.Parse("6B4621F3-7102-4953-8D5F-75F71B1729E6");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            var renter = new RenterModel
            {
                Name = "Juan Dela Cruz",
                ContactNo = "09165626321",
                Address = "Cebu",
                Profession = "Doctor",
                PropertyId = propertyId
            };

            var payload = new StringContent(
                JsonConvert.SerializeObject(renter),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = await client.PostAsync("api/v1/Renter/add", payload);
                var response = await request.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<ResponseModel>(response);

                Assert.IsTrue(content.Status, "Error occurred while adding renter");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Add Renter", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task RenterApi_GetRentersPerPropertyAsync_IntegrationTest()
        {
            var propertyId = Guid.Parse("6B4621F3-7102-4953-8D5F-75F71B1729E6");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            try
            {
                var request = await client.GetAsync($"api/v1/Renter?propertyid={propertyId}");
                var response = await request.Content.ReadAsStringAsync();

                var renters = JsonConvert.DeserializeObject<List<RenterListDTO>>(response);

                Assert.IsTrue(renters.Count > 0, "No renter added yet.");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Get Renters Per Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        private async Task<TokenModel> GenerateTokenAsync()
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

                return JsonConvert.DeserializeObject<TokenModel>(response);
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Verify User", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }
    }
}