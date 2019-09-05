using RealEstateCore.Core.Services;
using RealEstateCore.Core.BusinessModels.Implementation;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace RealEstateCore.Controllers
{
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/v1/Token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IAccountService _accountService;

        public TokenController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<TokenModel>> Create([FromBody] UserModel obj)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userVerified = await _accountService.VerifyUserAsync(obj);

            if (userVerified == null) return BadRequest(new ResponseModel { Status = false, Message = "Invalid user." });

            var token = _accountService.GenerateJwt(userVerified);

            return Ok(token);
        }
    }
}