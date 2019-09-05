using RealEstateCore.Core.BusinessModels.Interface;

using System;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class CheckinModel : ICheckinModel
    {
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid RoomId { get; set; }
        public Guid RenterId { get; set; }
        public double DownPayment { get; set; }
    }
}