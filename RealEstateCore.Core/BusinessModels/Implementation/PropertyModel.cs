using RealEstateCore.Core.BusinessModels.Interface;

using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class PropertyModel : IPropertyModel
    {
        public Guid PropertyId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string ContactNo { get; set; }

        [Required]
        public string Owner { get; set; }

        [Required]
        public int TotalRooms { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}