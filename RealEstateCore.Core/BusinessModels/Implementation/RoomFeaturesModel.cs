using RealEstateCore.Core.BusinessModels.Interface;

using System;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class RoomFeaturesModel : IRoomFeaturesModel
    {
        public string Name { get; set; }
        public Guid RoomId { get; set; }
    }
}