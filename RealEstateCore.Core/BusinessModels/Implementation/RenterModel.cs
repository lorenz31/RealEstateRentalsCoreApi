using RealEstateCore.Core.BusinessModels.Interface;

using System;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class RenterModel : IRenterModel
    {
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string Profession { get; set; }
        public Guid PropertyId { get; set; }
    }
}