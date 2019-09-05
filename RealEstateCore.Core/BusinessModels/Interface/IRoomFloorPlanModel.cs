using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IRoomFloorPlanModel
    {
        string Img { get; set; }
        Guid RoomId { get; set; }
    }
}