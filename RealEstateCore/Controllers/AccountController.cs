using RealEstateCore.Core.Services;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.Implementation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace RealEstateCore.Controllers
{
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/v1/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _acctService;
        private readonly IResponseModel _responseModel;

        public AccountController(
            IAccountService accountService,
            IResponseModel responseModel)
        {
            _acctService = accountService;
            _responseModel = responseModel;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<IResponseModel>> PostRegisterAsync([FromBody] UserModel obj)
        {
            var request = await _acctService.RegisterUserAsync(obj);

            _responseModel.Status = request.Status;
            _responseModel.Message = request.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }
    }
}