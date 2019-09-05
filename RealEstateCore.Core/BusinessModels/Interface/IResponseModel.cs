namespace RealEstateCore.Core.BusinessModels.Interface
{
    public interface IResponseModel
    {
        bool Status { get; set; }
        string Message { get; set; }
    }
}
