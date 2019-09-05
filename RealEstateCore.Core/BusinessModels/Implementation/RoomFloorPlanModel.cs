using RealEstateCore.Core.BusinessModels.Interface;

using System;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class RoomFloorPlanModel : IRoomFloorPlanModel
    {
        public string Img { get; set; }
        public Guid RoomId { get; set; }
    }
}