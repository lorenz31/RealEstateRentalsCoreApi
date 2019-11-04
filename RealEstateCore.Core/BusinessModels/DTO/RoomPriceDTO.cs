using System;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class RoomPriceDTO
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomType { get; set; }
        public decimal Price { get; set; }
    }
}