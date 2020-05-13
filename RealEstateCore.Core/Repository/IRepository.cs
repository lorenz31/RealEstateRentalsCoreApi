using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.BusinessModels.Implementation;

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

        #region Room Repository
        Task<List<RoomTypeModel>> GetRoomTypesPerPropertyAsync(Guid propertyid);
        Task<List<RoomsListInfoDTO>> GetRoomsPerPropertyAsync(Guid userid, Guid propertyid);
        Task<List<RoomPriceDTO>> GetRoomsWithPricesAsync(Guid userid, Guid propertyid);
        #endregion

        #region Room Feature Repository
        Task<List<RoomFeaturesDTO>> GetRoomFeaturesAsync(Guid propertyid);
        #endregion
    }
}
