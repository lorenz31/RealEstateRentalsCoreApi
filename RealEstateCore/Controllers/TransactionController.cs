using RealEstateCore.Core.Services;
using RealEstateCore.Core.BusinessModels.Interface;
using RealEstateCore.Core.BusinessModels.Implementation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace RealEstateCore.Controllers
{
    // Resume implementation after the rest of the api endpoints
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/v1/Transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IResponseModel _responseModel;

        public TransactionController(
            ITransactionService transactionService,
            IResponseModel responseModel)
        {
            _transactionService = transactionService;
            _responseModel = responseModel;
        }

        [HttpPost]
        [Route("checkin")]
        public async Task<IActionResult> PostCheckInTransactionAsync([FromBody] CheckinModel obj)
        {
            if (!ModelState.IsValid) return BadRequest();

            var response = await _transactionService.AddCheckInTransactionAsync(obj);

            _responseModel.Status = response.Status;
            _responseModel.Message = response.Message;

            if (_responseModel.Status)
                return Ok(_responseModel);
            else
                return BadRequest(_responseModel);
        }
    }
}