using RealEstateCore.Core.BusinessModels.Interface;

namespace RealEstateCore.Core.BusinessModels.Implementation
{
    public class ResponseModel : IResponseModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}