using RealEstateCore.Core.Services;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.Implementation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Cors;

namespace RealEstateCore.Controllers
{
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/v1/Room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IResponseModel _responseModel;

        public RoomController(
            IRoomService roomService,
            IResponseModel responseModel)
        {
            _roomService = roomService;
            _responseModel = responseModel;
        }

        #region Room Endpoints
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> PostAddPropertyAsync([FromBody] RoomModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _roomService.AddRoomAsync(obj);

            _responseModel.Status = response.Status;
            _responseModel.Message = response.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetRoomsPerPropertyAsync([FromQuery] string userid, [FromQuery] string propertyid)
        {
            if (string.IsNullOrEmpty(userid)) return BadRequest("User Id is required.");

            if (string.IsNullOrEmpty(propertyid)) return BadRequest("Property Id is required.");

            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(propertyid)) return BadRequest("User Id and Property Id is required.");

            var userId = Guid.Parse(userid);
            var propertyId = Guid.Parse(propertyid);

            var response = await _roomService.GetRoomsPerPropertyAsync(userId, propertyId);

            if (response.Count == 0)
                return null;
            else
                return Ok(response);
        }

        [HttpGet]
        [Route("prices")]
        public async Task<IActionResult> GetRoomsWithPricesAsync([FromQuery] string userid, [FromQuery] string propertyid)
        {
            if (string.IsNullOrEmpty(userid)) return BadRequest("User Id is required.");

            if (string.IsNullOrEmpty(propertyid)) return BadRequest("Property Id is required.");

            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(propertyid)) return BadRequest("User Id and Property Id is required.");

            var userId = Guid.Parse(userid);
            var propertyId = Guid.Parse(propertyid);

            var response = await _roomService.GetRoomsWithPricesAsync(userId, propertyId);

            if (response.Count == 0)
                return null;
            else
                return Ok(response);
        }

        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> GetRoomInfoAsync([FromQuery] string userid, [FromQuery] string propertyid, [FromQuery] string roomid)
        {
            if (string.IsNullOrEmpty(userid)) return BadRequest("User Id is required.");

            if (string.IsNullOrEmpty(propertyid)) return BadRequest("Property Id is required.");

            if (string.IsNullOrEmpty(roomid)) return BadRequest("Room Id is required.");

            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(propertyid) && string.IsNullOrEmpty(roomid)) return BadRequest("User Id, Property Id and Room Id is required.");

            var userId = Guid.Parse(userid);
            var propertyId = Guid.Parse(propertyid);
            var roomId = Guid.Parse(roomid);

            var roomInfo = await _roomService.GetRoomInfoAsync(userId, propertyId, roomId);

            if (roomInfo != null)
                return Ok(roomInfo);
            else
                return NoContent();
        }

        [HttpGet]
        [Route("available")]
        public async Task<IActionResult> GetAvailableBedsPerRoomAsync([FromQuery] string userid, [FromQuery] string propertyid, [FromQuery] string roomid)
        {
            if (string.IsNullOrEmpty(userid)) return BadRequest("User Id is required.");

            if (string.IsNullOrEmpty(propertyid)) return BadRequest("Property Id is required.");

            if (string.IsNullOrEmpty(roomid)) return BadRequest("Room Id is required.");

            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(propertyid) && string.IsNullOrEmpty(roomid)) return BadRequest("User Id, Property Id and Room Id is required.");

            var args = new AvailableBedsModel
            {
                UserId = Guid.Parse(userid),
                PropertyId = Guid.Parse(propertyid),
                RoomId = Guid.Parse(roomid)
            };

            var availableBeds = await _roomService.GetAvailableBedsPerRoomAsync(args);

            if (availableBeds != null)
                return Ok(availableBeds);
            else
                return BadRequest();
        }
        #endregion

        #region Room Features Endpoints
        [HttpPost]
        [Route("features/add")]
        public async Task<IActionResult> PostRoomFeatureAsync([FromBody] RoomFeaturesModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _roomService.AddRoomFeatureAsync(obj);

            _responseModel.Status = response.Status;
            _responseModel.Message = response.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }
        #endregion

        #region Room Floor Plan Endpoints
        [HttpPost]
        [Route("floorplan/add")]
        public async Task<IActionResult> PostFloorPlanAsync([FromBody] RoomFloorPlanModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _roomService.AddRoomFloorPlanAsync(obj);

            _responseModel.Status = response.Status;
            _responseModel.Message = response.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }
        #endregion

        #region Room Type Endpoints
        [HttpPost]
        [Route("type/add")]
        public async Task<IActionResult> PostRoomTypeAsync([FromBody] RoomTypeModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _roomService.AddRoomTypesAsync(obj);

            _responseModel.Status = response.Status;
            _responseModel.Message = response.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }

        [HttpGet]
        [Route("type")]
        public async Task<IActionResult> GetRoomTypeAsync([FromQuery] string propertyid)
        {
            if (string.IsNullOrEmpty(propertyid)) return BadRequest("Property Id is required.");

            var propertyId = Guid.Parse(propertyid);

            var response = await _roomService.GetRoomTypesPerProperty(propertyId);

            if (response.Count == 0)
                return null;
            else
                return Ok(response);
        }
        #endregion
    }
}