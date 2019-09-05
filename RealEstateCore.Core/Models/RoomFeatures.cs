using System;

namespace RealEstateCore.Core.Models
{
    public class RoomFeatures : BaseClass
    {
        public string Name { get; set; }

        public Room Room { get; set; }
        public Guid RoomId { get; set; }
    }
}