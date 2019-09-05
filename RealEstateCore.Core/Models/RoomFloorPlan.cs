using System;

namespace RealEstateCore.Core.Models
{
    public class RoomFloorPlan : BaseClass
    {
        public string Img { get; set; }

        public Room Room { get; set; }
        public Guid RoomId { get; set; }
    }
}