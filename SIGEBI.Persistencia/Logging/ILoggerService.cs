
namespace SIGEBI.Persistence.Logging
{
    public interface ILoggerService<T>
    {
        void LogInfo(string message, params object[] args);
        void LogWarn(string message, params object[] args);
        void LogError(Exception ex, string message, params object[] args);
    }
}
