using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.Interface;

using System.Threading.Tasks;

namespace RealEstateCore.Core.Services
{
    public interface IAccountService
    {
        Task<IResponseModel> RegisterUserAsync(IUserModel model);
        Task<IUserModel> VerifyUserAsync(IUserModel model);
        ITokenModel GenerateJwt(IUserModel model);
    }
}