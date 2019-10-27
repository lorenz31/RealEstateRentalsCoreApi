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
using System.Linq;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class PropertyApiIntegrationTest
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
        public async Task PropertyApi_AddPropertyAsync_IntegrationTest()
        {
            var userId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            for (int i = 0; i < 500; i++)
            {
                var property = new PropertyModel
                {
                    Name = $"Dorm {i}",
                    Address = $"Avenue {i}",
                    City = "Cebu",
                    ContactNo = "09175632158",
                    Owner = "Test Mandanni",
                    TotalRooms = 0,
                    UserId = userId
                };

                var payload = new StringContent(
                    JsonConvert.SerializeObject(property),
                    Encoding.UTF8,
                    "application/json"
                );

                try
                {
                    var request = await client.PostAsync("api/v1/Property/add", payload);
                    var response = await request.Content.ReadAsStringAsync();

                    var content = JsonConvert.DeserializeObject<ResponseModel>(response);

                    Assert.IsTrue(content.Status, "Error occurred while adding new property");
                }
                catch (Exception ex)
                {
                    logService.Log("API Endpoint: Add new property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                    Assert.Fail(ex.Message);
                }
            }
        }

        [TestMethod]
        public async Task PropertyApi_AddPropertyTermsAsync_IntegrationTest()
        {
            var propertyId = Guid.Parse("BCEDAB95-5E70-4A0B-83DC-0053D28A7D8F");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            var terms = new PropertyTermsModel
            {
                MonthDeposit = 1,
                MonthAdvance = 1,
                PropertyId = propertyId
            };

            var payload = new StringContent(
                JsonConvert.SerializeObject(terms),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = await client.PostAsync("api/v1/Property/terms/add", payload);
                var response = await request.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<ResponseModel>(response);

                Assert.IsTrue(content.Status, "Error occurred while adding property terms");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Add Property Terms", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task PropertyApi_GetOwnerPropertiesAsync_IntegrationTest()
        {
            var userId = Guid.Parse("1923610F-A467-40F3-8652-773A86DE4314");

            try
            {
                var token = await GenerateTokenAsync();

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

                var request = await client.GetAsync($"api/v1/Property?userId={userId}");
                var response = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    var properties = JsonConvert.DeserializeObject<List<PropertiesTermsDTO>>(response);

                    Assert.IsTrue(properties.Count > 0, "No properties added yet.");
                }
                else
                {
                    Assert.Fail("An error has occurred.");
                }
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Get Owner Properties", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task PropertyApi_GetPropertyInfoAsync_IntegrationTest()
        {
            var userId = Guid.Parse("1923610F-A467-40F3-8652-773A86DE4314");
            var propertyId = Guid.Parse("6B4621F3-7102-4953-8D5F-75F71B1729E6");

            try
            {
                var token = await GenerateTokenAsync();

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

                var request = await client.GetAsync($"api/v1/Property/info?userid={userId}&propertyid={propertyId}");
                var response = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(response))
                    {
                        var propertyInfo = JsonConvert.DeserializeObject<PropertiesDTO>(response);

                        Assert.IsNotNull(propertyInfo);
                    }
                    else
                    {
                        Assert.Fail("Property doesn't exist.");
                    }
                }
                else
                {
                    Assert.Fail("An error has occurred.");
                }
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Get Property Info", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task PropertyApi_UpdatePropertyInfoAsync_IntegrationTest()
        {
            var property = new PropertyModel
            {
                UserId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4"),
                PropertyId = Guid.Parse("BCEDAB95-5E70-4A0B-83DC-0053D28A7D8F"),
                Name = "SYBU Dorm",
                Address = "1st St. La Guardia, Lahug",
                City = "Cebu City",
                ContactNo = "(032)2567890",
                Owner = "Marlyn",
                TotalRooms = 0
            };

            try
            {
                var token = await GenerateTokenAsync();

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

                var payload = new StringContent(
                    JsonConvert.SerializeObject(property),
                    Encoding.UTF8,
                    "application/json"
                );

                var request = await client.PutAsync($"api/v1/Property", payload);
                var response = await request.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ResponseModel>(response);

                Assert.IsTrue(result.Status, "Error updating property info.");
            }
            catch (Exception ex)
            {
                logService.Log($"API Endpoint: Update Property {property.Name} - {property.PropertyId} Info", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        private async Task<TokenModel> GenerateTokenAsync()
        {
            var user = new UserModel
            {
                Username = "user1@gmail.com",
                Password = "demo123!",
                ClientId = "mobileapp"
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