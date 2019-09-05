using System;

namespace RealEstateCore.Core.Models
{
    public class RoomRented
    {
        public Room Room { get; set; }
        public Guid RoomId { get; set; }

        public Renter Renter { get; set; }
        public Guid RenterId { get; set; }
    }
}