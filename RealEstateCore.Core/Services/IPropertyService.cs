using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.BusinessModels.Interface;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateCore.Core.Services
{
    public interface IPropertyService
    {
        Task<IResponseModel> AddPropertyAsync(IPropertyModel model);
        Task<IResponseModel> AddPropertyTermsAsync(IPropertyTermsModel model);
        Task<List<PropertiesTermsDTO>> GetOwnerPropertiesAsync(Guid userid);
        Task<PropertiesTermsDTO> GetPropertyInfoAsync(Guid userid, Guid propertyid);
        Task<IResponseModel> UpdatePropertyInfoAsync(IPropertyModel model);
    }
}