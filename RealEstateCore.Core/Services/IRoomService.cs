using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.BusinessModels.Interface;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateCore.Core.Services
{
    public interface IRoomService
    {
        #region Rooms
        Task<IResponseModel> AddRoomAsync(IRoomModel model);
        Task<List<RoomsListInfoDTO>> GetRoomsPerPropertyAsync(Guid userid, Guid propertyid);
        Task<List<RoomPriceDTO>> GetRoomsWithPricesAsync(Guid userid, Guid propertyid);
        Task<RoomInfoDTO> GetRoomInfoAsync(Guid userid, Guid propertyid, Guid roomid);
        Task<AvailableBedsDTO> GetAvailableBedsPerRoomAsync(IAvailableBedsModel model);
        #endregion

        #region Room Features
        Task<IResponseModel> AddRoomFeatureAsync(IRoomFeaturesModel model);
        #endregion

        #region Room Floor Plan
        Task<IResponseModel> AddRoomFloorPlanAsync(IRoomFloorPlanModel model);
        #endregion

        #region Room Types
        Task<IResponseModel> AddRoomTypesAsync(IRoomTypeModel model);
        Task<List<IRoomTypeModel>> GetRoomTypesPerProperty(Guid propertyid);
        #endregion
    }
}