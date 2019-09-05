using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IAvailableBedsModel
    {
        Guid UserId { get; set; }
        Guid PropertyId { get; set; }
        Guid RoomId { get; set; }
    }
}