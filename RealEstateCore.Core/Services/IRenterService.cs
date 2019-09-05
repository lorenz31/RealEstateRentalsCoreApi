using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.BusinessModels.Interface;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateCore.Core.Services
{
    public interface IRenterService
    {
        Task<IResponseModel> AddRenterAsync(IRenterModel model);
        Task<List<RenterListDTO>> GetRentersPerPropertyAsync(Guid propertyid);
    }
}