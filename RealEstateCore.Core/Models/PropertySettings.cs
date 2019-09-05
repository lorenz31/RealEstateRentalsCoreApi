using System;

namespace RealEstateCore.Core.Models
{
    public class PropertySettings : BaseClass
    {
        public decimal MonthAdvance { get; set; }
        public decimal MonthDeposit { get; set; }

        public RealEstateProperty Property { get; set; }
        public Guid PropertyId { get; set; }
    }
}