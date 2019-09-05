using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IRoomTypeModel
    {
        Guid Id { get; set; }
        string Type { get; set; }
        decimal Price { get; set; }
        Guid PropertyId { get; set; }
    }
}