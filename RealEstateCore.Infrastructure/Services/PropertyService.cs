using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.Repository;
using RealEstateCore.Core.Services;
using RealEstateCore.Core.Models;
using RealEstateCore.Infrastructure.DataContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace RealEstateCore.Infrastructure.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly DatabaseContext _db;
        private IRepository _repo;
        private readonly IConfiguration _config;
        private readonly IResponseModel _response;
        private readonly ILoggerService _loggerService;

        public PropertyService(
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

        public async Task<IResponseModel> AddPropertyAsync(IPropertyModel model)
        {
            try
            {
                var property = new RealEstateProperty
                {
                    Id = Guid.NewGuid(),
                    UserId = model.UserId,
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    ContactNo = model.ContactNo,
                    Owner = model.Owner,
                    TotalRooms = model.TotalRooms
                };

                _db.RealEstateProperties.Add(property);
                await _db.SaveChangesAsync();

                _response.Status = true;
                _response.Message = "Property added successfully.";

                return _response;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Add New Property", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "Error adding property.";

                return _response;
            }
        }

        public async Task<IResponseModel> AddPropertyTermsAsync(IPropertyTermsModel model)
        {
            try
            {
                var settings = new PropertySettings
                {
                    Id = Guid.NewGuid(),
                    MonthDeposit = model.MonthDeposit,
                    MonthAdvance = model.MonthAdvance,
                    PropertyId = model.PropertyId
                };

                _db.Settings.Add(settings);
                var result = await _db.SaveChangesAsync();

                _response.Status = true;
                _response.Message = "Property Terms added successfully.";

                return _response;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Add Property Terms", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "Error adding property terms.";

                return _response;
            }
        }

        public async Task<List<PropertiesTermsDTO>> GetOwnerPropertiesAsync(Guid userid)
        {
            return await _repo.GetOwnerPropertiesAsync(userid) ?? null;
        }

        public async Task<PropertiesTermsDTO> GetPropertyInfoAsync(Guid userid, Guid propertyid)
        {
            return await _repo.GetPropertyInfoAsync(userid, propertyid) ?? null;
        }

        public async Task<IResponseModel> UpdatePropertyInfoAsync(IPropertyModel model)
        {
            try
            {
                var property = await _db.RealEstateProperties.SingleOrDefaultAsync(p => p.UserId == model.UserId && p.Id == model.PropertyId);

                if (property != null)
                {
                    property.Name = model.Name;
                    property.Address = model.Address;
                    property.City = model.City;
                    property.ContactNo = model.ContactNo;
                    property.Owner = model.Owner;
                    property.TotalRooms = model.TotalRooms;

                    _db.RealEstateProperties.Update(property);
                    await _db.SaveChangesAsync();

                    _response.Status = true;
                    _response.Message = $"{model.Name} successfully updated.";

                    return _response;
                }
                else
                {
                    _response.Status = false;
                    _response.Message = "No property found.";

                    return _response;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log("Update Property Info", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "Error adding property.";

                return _response;
            }
        }
    }
}