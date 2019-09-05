using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface ICheckinModel
    {
        Guid UserId { get; set; }
        Guid PropertyId { get; set; }
        Guid RoomId { get; set; }
        Guid RenterId { get; set; }
        double DownPayment { get; set; }
    }
}