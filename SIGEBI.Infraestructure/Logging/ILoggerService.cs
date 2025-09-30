namespace SIGEBI.Infrastructure.Logging
{
    public interface ILoggerService
    {
        void LogInfo(string message, params object[] args);
        void LogWarn(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
    }
}
