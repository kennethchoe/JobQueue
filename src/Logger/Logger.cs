using System;
using JobQueueCore;
using log4net;

namespace Log4NetLogger
{
    public class Logger : ILoggerDelegate
    {
        private static ILog _logger = LogManager.GetLogger("root");

        public void Log(LogActivity activity, string subject)
        {
            log4net.ThreadContext.Properties["subject"] = subject;
            _logger.Info(activity.ToString());
        }

        public void LogError(string subject, Exception e)
        {
            log4net.ThreadContext.Properties["subject"] = subject;
            _logger.Error(LogActivity.ErrorOccurred, e);
        }

        public void LogDebugInfo(string subject, string debugInfo)
        {
            log4net.ThreadContext.Properties["subject"] = subject;
            _logger.Debug(debugInfo);
        }
    }
}
