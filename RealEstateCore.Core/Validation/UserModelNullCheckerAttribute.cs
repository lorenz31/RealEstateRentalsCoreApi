using RealEstateCore.Core.BusinessModels.Implementation;

using System.ComponentModel.DataAnnotations;

namespace RealEstateCore.Infrastructure.Validation
{
    public class UserModelNullCheckerAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var userModel = (UserModel)validationContext.ObjectInstance;

            if (userModel.Username.Length == 0)
            {
                return new ValidationResult("Please provide Username");
            }

            if (userModel.Password.Length == 0)
            {
                return new ValidationResult("Please provide Password");
            }

            return ValidationResult.Success;
        }
    }
}