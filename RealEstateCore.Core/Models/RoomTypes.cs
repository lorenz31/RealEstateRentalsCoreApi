using System;

namespace RealEstateCore.Core.Models
{
    public class RoomTypes : BaseClass
    {
        public string Type { get; set; }
        public decimal Price { get; set; }

        public RealEstateProperty Property { get; set; }
        public Guid PropertyId { get; set; }
    }
}