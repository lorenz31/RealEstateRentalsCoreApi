using System;
using System.Collections.Generic;

namespace RealEstateCore.Core.Models
{
    public class Renter : BaseClass
    {
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string Profession { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public List<TransactionHistory> Transactions { get; set; }

        public ICollection<RoomRented> RoomsRented { get; set; }

        public RealEstateProperty Property { get; set; }
        public Guid PropertyId { get; set; }
    }
}