using System;

namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IUserModel
    {
        Guid UserId { get; set; }
        string ClientId { get; set; }
        string Secret { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string Salt { get; set; }
    }
}
