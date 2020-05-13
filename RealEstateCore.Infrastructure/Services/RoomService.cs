using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.Services;
using RealEstateCore.Core.Models;
using RealEstateCore.Core.Repository;
using RealEstateCore.Infrastructure.DataContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace RealEstateCore.Infrastructure.Services
{
    public class RoomService : IRoomService
    {
        private readonly DatabaseContext _db;
        private IRepository _repo;
        private readonly IConfiguration _config;
        private readonly IResponseModel _response;
        private readonly ILoggerService _loggerService;

        public RoomService(
            DatabaseContext db,
            IRepository repo,
            IConfiguration config,
            IResponseModel response,
            ILoggerService loggerService)
        {
            _db = db;
            _repo = repo;
            _config = config;
            _response = response;
            _loggerService = loggerService;
        }

        #region Room Services
        public async Task<IResponseModel> AddRoomAsync(IRoomModel model)
        {
            try
            {
                var room = new Room
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    TotalBeds = model.TotalBeds,
                    RoomTypeId = model.RoomTypeId,
                    PropertyId = model.PropertyId
                };

                _db.Rooms.Add(room);
                await _db.SaveChangesAsync();

                _response.Status = true;
                _response.Message = "New room added.";

                return _response;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Add Room", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "An error occurred while adding new room.";

                return _response;
            }
        }

        public async Task<List<RoomsListInfoDTO>> GetRoomsPerPropertyAsync(Guid userid, Guid propertyid) => await _repo.GetRoomsPerPropertyAsync(userid, propertyid) ?? null;

        public async Task<List<RoomPriceDTO>> GetRoomsWithPricesAsync(Guid userid, Guid propertyid) => await _repo.GetRoomsWithPricesAsync(userid, propertyid);

        public async Task<RoomInfoDTO> GetRoomInfoAsync(Guid userid, Guid propertyid, Guid roomid)
        {
            try
            {
                var roomInfo = await _db.RealEstateProperties
                    .AsNoTracking()
                    .Join(
                        _db.Rooms,
                        property => property.Id,
                        room => room.PropertyId,
                        (prop, room) => new { prop, room }
                    )
                    .Join(
                        _db.RoomTypes,
                        rooms => rooms.room.RoomTypeId,
                        roomtype => roomtype.Id,
                        (rooms, roomtype) => new { rooms, roomtype }
                    )
                    .Where(
                        p => p.rooms.prop.UserId == userid &&
                        p.rooms.prop.Id == propertyid &&
                        p.rooms.room.Id == roomid
                    )
                    .Select(p => new RoomInfoDTO
                    {
                        RoomName = p.rooms.room.Name,
                        RoomTypeName = p.roomtype.Type,
                        TotalBeds = p.rooms.room.TotalBeds,
                        Price = p.roomtype.Price,
                        Features = _db.RoomFeatures.Where(f => f.RoomId == p.rooms.room.Id).Select(f => f.Name).ToList(),
                        FloorPlans = _db.FloorPlans.Where(f => f.RoomId == p.rooms.room.Id).Select(f => f.Img).ToList()
                    })
                    .SingleOrDefaultAsync();

                return roomInfo ?? null;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Room Info", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }

        public async Task<AvailableBedsDTO> GetAvailableBedsPerRoomAsync(IAvailableBedsModel model)
        {
            try
            {
                var rooms = await _db.RealEstateProperties
                    .AsNoTracking()
                    .Join(
                        _db.Rooms,
                        property => property.Id,
                        room => room.PropertyId,
                        (prop, room) => new { prop, room }
                    )
                    .Where(
                        p => p.prop.UserId == model.UserId && p.prop.Id == model.PropertyId
                    )
                    .Select(p => p)
                    .ToListAsync();

                var occupiedRooms = _db.RoomsRented.Where(rr => rr.RoomId == model.RoomId).Count();

                var roomStats = rooms
                    .Where(r => r.room.Id == model.RoomId)
                    .Select(rs => new AvailableBedsDTO
                    {
                        RoomId = rs.room.Id,
                        BedsOccupied = occupiedRooms,
                        BedsAvailable = rs.room.TotalBeds - occupiedRooms
                    })
                    .SingleOrDefault();

                return roomStats;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Room Statistics", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }
        #endregion

        #region Room Features
        public async Task<IResponseModel> AddRoomFeatureAsync(IRoomFeaturesModel model)
        {
            try
            {
                var features = new RoomFeatures
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    RoomId = model.RoomId
                };

                _db.RoomFeatures.Add(features);
                await _db.SaveChangesAsync();

                _response.Status = true;
                _response.Message = "Room feature added.";

                return _response;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Add Room Feature", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "An error occurred while adding room feature.";

                return _response;
            }
        }

        public async Task<List<RoomFeaturesDTO>> GetRoomFeaturesAsync(Guid propertyid) => await _repo.GetRoomFeaturesAsync(propertyid) ?? null;
        #endregion

        #region Room Floor Plan
        public async Task<IResponseModel> AddRoomFloorPlanAsync(IRoomFloorPlanModel model)
        {
            try
            {
                var floorPlan = new RoomFloorPlan
                {
                    Id = Guid.NewGuid(),
                    Img = model.Img,
                    RoomId = model.RoomId
                };

                _db.FloorPlans.AddRange(floorPlan);
                await _db.SaveChangesAsync();

                _response.Status = true;
                _response.Message = "Room floor plan added.";

                return _response;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Add Room Floor Plan", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "Error occurred while adding room floor plan.";

                return _response;
            }
        }
        #endregion

        #region Room Type
        public async Task<IResponseModel> AddRoomTypesAsync(IRoomTypeModel model)
        {
            try
            {
                var roomType = new RoomTypes
                {
                    Id = Guid.NewGuid(),
                    Type = model.Type,
                    Price = model.Price,
                    PropertyId = model.PropertyId
                };

                _db.RoomTypes.Add(roomType);
                await _db.SaveChangesAsync();

                _response.Status = true;
                _response.Message = "New room type added.";

                return _response;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Add Room Type", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "Error adding room type.";

                return _response;
            }
        }

        public async Task<List<RoomTypeModel>> GetRoomTypesPerPropertyAsync(Guid propertyid) => await _repo.GetRoomTypesPerPropertyAsync(propertyid) ?? null;
        #endregion
    }
}