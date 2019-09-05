using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IPropertyModel
    {
        Guid PropertyId { get; set; }
        string Name { get; set; }
        string Address { get; set; }
        string City { get; set; }
        string ContactNo { get; set; }
        string Owner { get; set; }
        int TotalRooms { get; set; }
        Guid UserId { get; set; }
    }
}
