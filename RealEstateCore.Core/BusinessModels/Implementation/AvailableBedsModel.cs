using RealEstateCore.Core.BusinessModels.Interface;

using System;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class AvailableBedsModel : IAvailableBedsModel
    {
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid RoomId { get; set; }
    }
}