using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface ITokenModel
    {
        string AccessToken { get; set; }
        Guid UserId { get; set; }
        string Email { get; set; }
    }
}
