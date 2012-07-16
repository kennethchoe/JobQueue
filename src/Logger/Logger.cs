using System;
using System.Diagnostics;
using JobQueueCore;
using log4net;

namespace Log4NetLogger
{
    public class Logger : ILoggerDelegate
    {
        public void Log(LogActivity activity, string subject)
        {
            PrepareLogger(subject).Info(activity.ToString());
        }

        public void LogError(string subject, Exception e)
        {
            PrepareLogger(subject).Error(LogActivity.ErrorOccurred, e);
        }

        public void LogDebugInfo(string subject, string debugInfo)
        {
            PrepareLogger(subject).Debug(debugInfo);
        }

        private ILog PrepareLogger(string subject)
        {
            var callerMethod = (new StackFrame(2, true).GetMethod());

            string loggerName = "";
            if (callerMethod.DeclaringType != null)
            {
                loggerName = callerMethod.DeclaringType.FullName;
            }

            var logger = LogManager.GetLogger(loggerName);

            ThreadContext.Properties["subject"] = subject;

            return logger;
        }
    }
}
