namespace Mph.EFCore.Infrastructure.Services
{
    /// <summary>
    /// 日志服务，封装了 ILogger，方便以后扩展功能（比如写入数据库）
    /// </summary>
    public class LogService
    {
        private readonly ILogger<LogService> _logger;
        private readonly IWebHostEnvironment _env;

        public LogService(ILogger<LogService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
        }
    }
}
