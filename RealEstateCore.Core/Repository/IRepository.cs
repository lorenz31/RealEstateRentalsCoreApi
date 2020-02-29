using RealEstateCore.Core.BusinessModels.DTO;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateCore.Core.Repository
{
    public interface IRepository
    {
        #region Property Repository
        Task<List<PropertiesTermsDTO>> GetOwnerPropertiesAsync(Guid userid);
        Task<PropertiesTermsDTO> GetPropertyInfoAsync(Guid userid, Guid propertyid);
        #endregion
    }
}
