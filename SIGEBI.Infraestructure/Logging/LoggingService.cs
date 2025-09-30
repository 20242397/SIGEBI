

using Microsoft.Extensions.Logging;

namespace SIGEBI.Infrastructure.Logging
{
    public class LoggerService<T> : ILoggerService
    {
        private readonly ILogger<T> _logger;

        public LoggerService(ILogger<T>  logger)
        {
            _logger = logger;
        }

        public void LogInfo(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarn(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }
    }
}

