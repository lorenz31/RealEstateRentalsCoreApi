using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class RenterInfoDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string Profession { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public Guid PropertyId { get; set; }
    }
}
