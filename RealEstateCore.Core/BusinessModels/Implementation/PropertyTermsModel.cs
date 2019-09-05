using RealEstateCore.Core.BusinessModels.Interface;

using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class PropertyTermsModel : IPropertyTermsModel
    {
        [Required]
        public decimal MonthAdvance { get; set; }

        [Required]
        public decimal MonthDeposit { get; set; }

        [Required]
        public Guid PropertyId { get; set; }
    }
}