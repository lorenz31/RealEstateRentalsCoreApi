namespace RealEstateCore.Core.Services
{
    public interface ILoggerService
    {
        void Log(string feature, string innerex, string message, string stacktrace);
    }
}