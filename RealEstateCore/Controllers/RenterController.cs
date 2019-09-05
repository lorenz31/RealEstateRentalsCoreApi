using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.Implementation;
using RealEstateCore.Core.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System;

namespace RealEstateCore.Controllers
{
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/v1/Renter")]
    [ApiController]
    public class RenterController : ControllerBase
    {
        private readonly IRenterService _renterService;
        private readonly IResponseModel _responseModel;

        public RenterController(
            IRenterService renterService,
            IResponseModel responseModel)
        {
            _renterService = renterService;
            _responseModel = responseModel;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> PostAddRenterAsync([FromBody] RenterModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _renterService.AddRenterAsync(obj);

            _responseModel.Status = response.Status;
            _responseModel.Message = response.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetRentersPerPropertyAsync([FromQuery] string propertyid)
        {
            var propertyId = Guid.Parse(propertyid);

            var renters = await _renterService.GetRentersPerPropertyAsync(propertyId);

            if (renters.Count == 0)
                return Ok(new ResponseModel { Status = false, Message = "No renters added yet." });

            return Ok(renters);
        }
    }
}