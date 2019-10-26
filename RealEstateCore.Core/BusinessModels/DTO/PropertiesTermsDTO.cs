using System;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class PropertiesTermsDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactNo { get; set; }
        public string Owner { get; set; }
        public int TotalRooms { get; set; }
        public decimal MonthDeposit { get; set; }
        public decimal MonthAdvance { get; set; }
    }
}
