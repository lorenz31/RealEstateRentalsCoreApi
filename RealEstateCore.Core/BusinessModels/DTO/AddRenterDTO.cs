using System;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class AddRenterDTO
    {
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string Profession { get; set; }
        public Guid PropertyId { get; set; }
    }
}