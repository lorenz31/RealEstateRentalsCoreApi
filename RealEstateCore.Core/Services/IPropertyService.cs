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
        Task<List<PropertiesDTO>> GetOwnerPropertiesAsync(Guid userid);
        Task<PropertiesDTO> GetPropertyInfoAsync(Guid userid, Guid propertyid);
        Task<IResponseModel> UpdatePropertyInfoAsync(IPropertyModel model);
    }
}