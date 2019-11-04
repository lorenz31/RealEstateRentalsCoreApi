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
using System.Net;

namespace RealEstateCore.IntegrationTest
{
    [TestClass]
    public class RoomApiIntegrationTest
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

        #region Room Type Endpoints
        [TestMethod]
        public async Task RoomApi_AddRoomTypeAsync_IntegrationTest()
        {
            var propertyId = Guid.Parse("BCEDAB95-5E70-4A0B-83DC-0053D28A7D8F");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            var roomTypeModel = new RoomTypeModel
            {
                Type = "4 person dorm aircon room",
                Price = 3800,
                PropertyId = propertyId
            };

            var payload = new StringContent(
                JsonConvert.SerializeObject(roomTypeModel),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = await client.PostAsync("api/v1/Room/type/add", payload);
                var response = await request.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<ResponseModel>(response);

                Assert.IsTrue(content.Status, "Error occurred while adding room type");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Add Room Type", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task RoomApi_GetRoomTypesAsync_IntegrationTest()
        {
            var propertyId = Guid.Parse("6B9A26D3-A043-4007-82AC-02AD91762F45");

            try
            {
                var token = await GenerateTokenAsync();

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

                var request = await client.GetAsync($"api/v1/Room/type?propertyid={propertyId}");
                var response = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(response))
                    {
                        var roomTypes = JsonConvert.DeserializeObject<List<RoomTypeModel>>(response);

                        Assert.IsTrue(roomTypes.Count > 0);
                    }
                    else
                    {
                        Assert.Fail($"No room type added for {propertyId} property yet.");
                    }
                }
                else
                {
                    Assert.Fail("An error has occurred.");
                }
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Get Room Types", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }
        #endregion

        #region Room Endpoints
        [TestMethod]
        public async Task RoomApi_AddRoomAsync_IntegrationTest()
        {
            var propertyId = Guid.Parse("BCEDAB95-5E70-4A0B-83DC-0053D28A7D8F");
            var roomTypeId = Guid.Parse("398D20A1-B3FA-4580-AB04-369B132B1E4F");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            var room = new RoomModel
            {
                Name = "Chevrolet",
                TotalBeds = 6,
                RoomTypeId = roomTypeId,
                PropertyId = propertyId
            };

            var payload = new StringContent(
                JsonConvert.SerializeObject(room),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = await client.PostAsync("api/v1/Room/add", payload);
                var response = await request.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<ResponseModel>(response);

                Assert.IsTrue(content.Status, "Error occurred while adding room");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Add Room", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task RoomApi_GetRoomsPerPropertyAsync_IntegrationTest()
        {
            var userdId = Guid.Parse("1923610F-A467-40F3-8652-773A86DE4314");
            var propertyId = Guid.Parse("03EF7E92-AC36-4A45-9574-AC149371EF05");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            try
            {
                var request = await client.GetAsync($"api/v1/Room/list?userid={userdId}&propertyid={propertyId}");
                var response = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    var rooms = JsonConvert.DeserializeObject<List<RoomsListInfoDTO>>(response);

                    Assert.IsTrue(rooms.Count > 0);
                }
                else
                {
                    var resp = JsonConvert.DeserializeObject<ResponseModel>(response);

                    Assert.Fail(resp.Message);
                }
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Get Rooms Per Property", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task RoomApi_GetRoomsWithPricesAsync_IntegrationTest()
        {
            var userdId = Guid.Parse("1923610F-A467-40F3-8652-773A86DE4314");
            var propertyId = Guid.Parse("45639725-C992-4DE2-875D-EAB4D567A02E");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            try
            {
                var request = await client.GetAsync($"api/v1/Room/prices?userid={userdId}&propertyid={propertyId}");
                var response = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    var rooms = JsonConvert.DeserializeObject<List<RoomPriceDTO>>(response);

                    Assert.IsTrue(rooms.Count > 0);
                }
                else
                {
                    var resp = JsonConvert.DeserializeObject<ResponseModel>(response);

                    Assert.Fail(resp.Message);
                }
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Get Room Prices", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task RoomApi_GetRoomInfoAsync_IntegrationTest()
        {
            var userdId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");
            var propertyId = Guid.Parse("BCEDAB95-5E70-4A0B-83DC-0053D28A7D8F");
            var roomId = Guid.Parse("C909DDE0-97BF-49C9-976B-DCE398E533FF");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            try
            {
                var request = await client.GetAsync($"api/v1/Room/info?userid={userdId}&propertyid={propertyId}&roomid={roomId}");
                var response = await request.Content.ReadAsStringAsync();

                var roomInfo = JsonConvert.DeserializeObject<RoomInfoDTO>(response);

                Assert.IsNotNull(roomInfo, "Error occurred while getting room info");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Get Room Info", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task RoomApi_GetAvailableBedsPerRoomAsync_IntegrationTest()
        {
            var userId = Guid.Parse("10445DB1-C5B0-478A-89F6-613450414ED4");
            var propertyId = Guid.Parse("BCEDAB95-5E70-4A0B-83DC-0053D28A7D8F");
            var roomId = Guid.Parse("C909DDE0-97BF-49C9-976B-DCE398E533FF");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            try
            {
                var request = await client.GetAsync($"api/v1/Room/available?userid={userId}&propertyid={propertyId}&roomid={roomId}");
                var response = await request.Content.ReadAsStringAsync();

                var availableBeds = JsonConvert.DeserializeObject<AvailableBedsDTO>(response);

                Assert.IsNotNull(availableBeds, "Error occurred while retrieving available rooms");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Get Available Rooms", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }
        #endregion

        #region Room Features Endpoints
        [TestMethod]
        public async Task RoomApi_AddRoomFeatureAsync_IntegrationTest()
        {
            var roomId = Guid.Parse("C909DDE0-97BF-49C9-976B-DCE398E533FF");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            var roomFeatures = new RoomFeaturesModel
            {
                RoomId = roomId,
                Name = "2 Double Deck Beds"
            };

            var payload = new StringContent(
                JsonConvert.SerializeObject(roomFeatures),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = await client.PostAsync("api/v1/Room/features/add", payload);
                var response = await request.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<ResponseModel>(response);

                Assert.IsTrue(content.Status, "Error occurred while adding room feature");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Add Room Features", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }
        #endregion

        #region Room Floor Plan Endpoints
        [TestMethod]
        public async Task RoomApi_AddRoomFloorPlanAsync_IntegrationTest()
        {
            var roomId = Guid.Parse("C909DDE0-97BF-49C9-976B-DCE398E533FF");
            var token = await GenerateTokenAsync();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

            var roomFloorPlan = new RoomFloorPlanModel
            {
                RoomId = roomId,
                Img = @"D:\Repository\RealEstateCoreApi\RealEstateCore\wwwroot\Images\Properties\FloorPlan\floorplan.png"
            };

            var payload = new StringContent(
                JsonConvert.SerializeObject(roomFloorPlan),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var request = await client.PostAsync("api/v1/Room/floorplan/add", payload);
                var response = await request.Content.ReadAsStringAsync();

                var content = JsonConvert.DeserializeObject<ResponseModel>(response);

                Assert.IsTrue(content.Status, "Error occurred while adding room floor plan");
            }
            catch (Exception ex)
            {
                logService.Log("API Endpoint: Add Room Floor Plan", ex.InnerException.Message, ex.Message, ex.StackTrace);
                Assert.Fail(ex.Message);
            }
        }
        #endregion

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
