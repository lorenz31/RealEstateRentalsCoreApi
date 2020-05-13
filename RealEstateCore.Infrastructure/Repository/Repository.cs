using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.Repository;
using RealEstateCore.Core.Services;
using RealEstateCore.Infrastructure.DataContext;

using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace RealEstateCore.Infrastructure.Repository
{
    public class Repository : IRepository
    {
        private DatabaseContext _db;
        private IConfiguration _config;
        private ILoggerService _loggerService;

        public Repository(
            DatabaseContext db,
            IConfiguration config,
            ILoggerService loggerService)
        {
            _db = db;
            _config = config;
            _loggerService = loggerService;
        }

        public async Task<List<PropertiesTermsDTO>> GetOwnerPropertiesAsync(Guid userid)
        {
            try
            {
                using (var con = new SqlConnection(_config["Database:ConnectionString"]))
                {
                    con.Open();

                    var userId = Guid.Parse(userid.ToString());

                    var properties = await con.QueryAsync<PropertiesTermsDTO>("sp_GetOwnerPropertiesWithTerms", new { UserId = userId }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    return properties.AsList() ?? null;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Owner Properties", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }

        public async Task<PropertiesTermsDTO> GetPropertyInfoAsync(Guid userid, Guid propertyid)
        {
            try
            {
                using (var con = new SqlConnection(_config["Database:ConnectionString"]))
                {
                    con.Open();

                    var properties = await con.QueryAsync<PropertiesTermsDTO>("sp_GetPropertyInfo", new { UserId = userid, PropertyId = propertyid }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    return properties.SingleOrDefault() ?? null;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Property Info", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }

        public async Task<List<RoomTypeModel>> GetRoomTypesPerPropertyAsync(Guid propertyid)
        {
            try
            {
                using (var con = new SqlConnection(_config["Database:ConnectionString"]))
                {
                    con.Open();

                    var roomTypes = await con.QueryAsync<RoomTypeModel>("sp_GetRoomTypesPerProperty", new { PropertyId = propertyid }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    return roomTypes.Count() > 0 ? roomTypes.AsList() : null;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Property Room Types", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }

        public async Task<List<RoomsListInfoDTO>> GetRoomsPerPropertyAsync(Guid userid, Guid propertyid)
        {
            try
            {
                using (var con = new SqlConnection(_config["Database:ConnectionString"]))
                {
                    con.Open();

                    var roomList = await con.QueryAsync<RoomsListInfoDTO>("sp_GetRoomsPerProperty", new { UserId = userid, PropertyId = propertyid }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    return roomList.ToList() ?? null;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Rooms Per Property", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }

        public async Task<List<RoomPriceDTO>> GetRoomsWithPricesAsync(Guid userid, Guid propertyid)
        {
            try
            {
                using (var con = new SqlConnection(_config["Database:ConnectionString"]))
                {
                    con.Open();

                    var roomPrices = await con.QueryAsync<RoomPriceDTO>("sp_GetRoomsWithPrices", new { UserId = userid, PropertyId = propertyid }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    return roomPrices.ToList() ?? null;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Rooms With Prices", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }

        public async Task<List<RoomFeaturesDTO>> GetRoomFeaturesAsync(Guid propertyid)
        {
            try
            {
                using (var con = new SqlConnection(_config["Database:ConnectionString"]))
                {
                    con.Open();

                    var features = await con.QueryAsync<RoomFeaturesDTO>("sp_GetRoomFeatures", new { PropertyId = propertyid }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    return features.ToList() ?? null;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Rooms Features", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }
    }
}
