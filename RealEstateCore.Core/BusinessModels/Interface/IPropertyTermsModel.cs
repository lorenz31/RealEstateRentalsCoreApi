using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IPropertyTermsModel
    {
        decimal MonthAdvance { get; set; }
        decimal MonthDeposit { get; set; }
        Guid PropertyId { get; set; }
    }
}