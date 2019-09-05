using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.BusinessModels.Interface;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateCore.Core.Services
{
    public interface ITransactionService
    {
        Task<IResponseModel> AddCheckInTransactionAsync(ICheckinModel model);
        Task<List<PropertyTransactionsDTO>> GetTransactionPerPropertyAsync(IBaseModel model);
    }
}