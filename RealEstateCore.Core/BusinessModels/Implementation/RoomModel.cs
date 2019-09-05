using RealEstateCore.Core.BusinessModels.Interface;

using System;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class RoomModel : IRoomModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int TotalBeds { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid PropertyId { get; set; }
    }
}