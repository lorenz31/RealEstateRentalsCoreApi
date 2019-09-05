using RealEstateCore.Core.BusinessModels.Interface;

using Newtonsoft.Json;
using System;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class TokenModel : ITokenModel
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}