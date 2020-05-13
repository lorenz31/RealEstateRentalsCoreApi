using RealEstateCore.Core.Services;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.Implementation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Cors;
using RealEstateCore.Filters;

namespace RealEstateCore.Controllers
{
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/v1/Property")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly IResponseModel _responseModel;

        public PropertyController(
            IPropertyService propertyService,
            IResponseModel responseModel)
        {
            _propertyService = propertyService;
            _responseModel = responseModel;
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> PostAddPropertyAsync([FromBody] PropertyModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _propertyService.AddPropertyAsync(obj);

            _responseModel.Status = response.Status;
            _responseModel.Message = response.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }

        [HttpPost]
        [Route("terms/add")]
        public async Task<IActionResult> PostAddPropertyTermsAsync([FromBody] PropertyTermsModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var request = await _propertyService.AddPropertyTermsAsync(obj);

            _responseModel.Status = request.Status;
            _responseModel.Message = request.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }

        [HttpGet]
        [Route("")]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> GetOwnerPropertiesAsync([FromQuery] Guid userid)
        {
            var ids = HttpContext.Items;

            if (ids.ContainsKey("uid"))
            {
                Guid userId = (Guid)HttpContext.Items["uid"];

                var response = await _propertyService.GetOwnerPropertiesAsync(userId);

                if (response.Count == 0) return NoContent();

                return Ok(response);
            }

            return NoContent();
        }

        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> GetPropertyInfoAsync([FromQuery] string userid, [FromQuery] string propertyid)
        {
            var userId = Guid.Parse(userid);
            var propertyId = Guid.Parse(propertyid);

            var response = await _propertyService.GetPropertyInfoAsync(userId, propertyId);

            if (response == null) return NoContent();

            return Ok(response);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> PutPropertyInfoAsync([FromBody] PropertyModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _propertyService.UpdatePropertyInfoAsync(obj);

            _responseModel.Status = response.Status;
            _responseModel.Message = response.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }
    }
}