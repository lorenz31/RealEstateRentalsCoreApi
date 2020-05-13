using System;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class RoomsListInfoDTO
    {
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public string PropertyName { get; set; }
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomTypeName { get; set; }
        public decimal Price { get; set; }
        public int TotalBeds { get; set; }
    }
}