using System;
using System.Collections.Generic;

namespace RealEstateCore.Core.Models
{
    public class Room : BaseClass
    {
        public string Name { get; set; }
        public int TotalBeds { get; set; }        
        public Guid RoomTypeId { get; set; }

        public List<RoomFloorPlan> FloorPlans { get; set; }
        public List<RoomFeatures> Features { get; set; }

        public ICollection<RoomRented> RoomsRented { get; set; }

        public RealEstateProperty Property { get; set; }
        public Guid PropertyId { get; set; }
    }
}