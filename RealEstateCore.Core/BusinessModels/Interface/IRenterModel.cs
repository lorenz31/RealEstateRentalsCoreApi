using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IRenterModel
    {
        string Name { get; set; }
        string ContactNo { get; set; }
        string Address { get; set; }
        string Profession { get; set; }
        Guid PropertyId { get; set; }
    }
}