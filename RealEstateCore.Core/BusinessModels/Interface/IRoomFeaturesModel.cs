using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IRoomFeaturesModel
    {
        string Name { get; set; }
        Guid RoomId { get; set; }
    }
}