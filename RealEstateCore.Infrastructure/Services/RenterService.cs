﻿using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.DTO;
using RealEstateCore.Core.Services;
using RealEstateCore.Core.Models;
using RealEstateCore.Infrastructure.DataContext;

using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace RealEstateCore.Infrastructure.Services
{
    public class RenterService : IRenterService
    {
        private readonly DatabaseContext _db;
        private readonly IConfiguration _config;
        private readonly IResponseModel _response;
        private readonly ILoggerService _loggerService;

        public RenterService(
            DatabaseContext db,
            IConfiguration config,
            IResponseModel response,
            ILoggerService loggerService)
        {
            _db = db;
            _config = config;
            _response = response;
            _loggerService = loggerService;
        }

        public async Task<IResponseModel> AddRenterAsync(IRenterModel model)
        {
            try
            {
                var renter = new Renter
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    ContactNo = model.ContactNo,
                    Address = model.Address,
                    Profession = model.Profession,
                    PropertyId = model.PropertyId
                };

                _db.Renter.Add(renter);
                await _db.SaveChangesAsync();

                _response.Status = true;
                _response.Message = "New renter added.";

                return _response;
            }
            catch (Exception ex)
            {
                _loggerService.Log("Add Renter", ex.InnerException.Message, ex.Message, ex.StackTrace);

                _response.Status = false;
                _response.Message = "Error adding renter.";

                return _response;
            }
        }

        public async Task<List<RenterListDTO>> GetRentersPerPropertyAsync(Guid propertyid)
        {
            try
            {
                using (var con = new SqlConnection(_config["Database:ConnectionString"]))
                {
                    con.Open();

                    var renters = await con.QueryAsync<RenterListDTO>("sp_GetRentersPerProperty", new { PropertyId = propertyid }, commandType: CommandType.StoredProcedure);

                    con.Close();

                    return renters.AsList() ?? null;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Log("Get Renters Per Property", ex.InnerException.Message, ex.Message, ex.StackTrace);

                return null;
            }
        }
    }
}
