using System;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class AvailableBedsDTO
    {
        public Guid RoomId { get; set; }
        public int BedsOccupied { get; set; }
        public int BedsAvailable { get; set; }
    }
}