using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IBaseModel
    {
        Guid UserId { get; set; }
        Guid PropertyId { get; set; }
    }
}