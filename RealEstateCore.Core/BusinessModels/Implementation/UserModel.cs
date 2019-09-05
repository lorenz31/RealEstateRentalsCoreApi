using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Infrastructure.Validation;

using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class UserModel : IUserModel
    {
        public Guid UserId { get; set; }

        public string ClientId { get; set; }

        public string Secret { get; set; }

        [UserModelNullChecker]
        public string Username { get; set; }

        [UserModelNullChecker]
        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Salt { get; set; }
    }
}
