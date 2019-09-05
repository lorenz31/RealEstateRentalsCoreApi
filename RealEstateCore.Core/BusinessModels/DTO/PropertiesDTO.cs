using System;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class PropertiesDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactNo { get; set; }
        public string Owner { get; set; }
        public int TotalRooms { get; set; }
    }
}