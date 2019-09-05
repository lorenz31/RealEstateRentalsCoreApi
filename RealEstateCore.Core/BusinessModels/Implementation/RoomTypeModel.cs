using RealEstateCore.Core.BusinessModels.Interface;

using System;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class RoomTypeModel : IRoomTypeModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public Guid PropertyId { get; set; }
    }
}