using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IRoomModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        int TotalBeds { get; set; }
        Guid RoomTypeId { get; set; }
        Guid PropertyId { get; set; }
    }
}