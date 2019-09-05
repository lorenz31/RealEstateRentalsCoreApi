using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.Helpers;
using RealEstateCore.Core.Models;
using RealEstateCore.Core.Services;
using RealEstateCore.Core.Security;
using RealEstateCore.Infrastructure.DataContext;

using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RealEstateCore.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly DatabaseContext _db;
        private readonly IConfiguration _config;
        private readonly IResponseModel _response;
        private readonly ILoggerService _loggerService;

        public AccountService(
            DatabaseContext db,
            IConfiguration config,
            IResponseModel response,
            ILoggerService loggerService)
        {
            _db = db;
            _config = config;
            _response = response;
            _loggerService = loggerService;
        }

        public async Task<IResponseModel> RegisterUserAsync(IUserModel model)
        {
            try
            {
                var salt = PasswordHash.GenerateSalt();
                var passwordHash = PasswordHash.ComputeHash(model.Password, salt);

                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    ClientId = _config["AppConfiguration:Audience"],
                    Email = model.Username,
                    Password = Convert.ToBase64String(passwordHash),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = false,
                    DateRegistered = DateTime.UtcNow,
                    Salt = Convert.ToBase64String(salt)
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                _response.Status = true;
                _response.Message = "Registration Successful.";

                return _response;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Register User", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "Registration Error.";

                return _response;
            }
        }

        public async Task<IUserModel> VerifyUserAsync(IUserModel model)
        {
            try
            {
                var userInfo = await _db.Users.Where(u => u.ClientId == model.ClientId && u.Email == model.Username).SingleOrDefaultAsync();

                if (userInfo == null) return null;

                var salt = Convert.FromBase64String(userInfo.Salt);
                var hashPassword = Convert.FromBase64String(userInfo.Password);
                var isVerified = PasswordHash.VerifyPassword(model.Password, salt, hashPassword);

                return new UserModel
                {
                    UserId = userInfo.Id,
                    Username = userInfo.Email
                };
            }
            catch (Exception ex)
            {
                _loggerService.Log("Verify User", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }

        public ITokenModel GenerateJwt(IUserModel model)
        {
            var token = new JwtTokenBuilder(_config);

            return new TokenModel
            {
                AccessToken = token.GenerateToken(),
                UserId = model.UserId,
                Email = model.Username
            };
        }
    }
}
