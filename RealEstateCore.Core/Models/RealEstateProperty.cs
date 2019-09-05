using System;
using System.Collections.Generic;

namespace RealEstateCore.Core.Models
{
    public class RealEstateProperty : BaseClass
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactNo { get; set; }
        public string Owner { get; set; }
        public int TotalRooms { get; set; }

        public List<Renter> Renters { get; set; }
        public List<Room> Rooms { get; set; }
        public List<RoomTypes> RoomTypes { get; set; }

        public PropertySettings Settings { get; set; }

        public ApplicationUser User { get; set; }
        public Guid UserId { get; set; }
    }
}