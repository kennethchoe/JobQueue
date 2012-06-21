using System;

namespace JobQueueCore
{
    public class NullLoggerDelegate : ILoggerDelegate
    {
        public void Log(LogActivity activity, string subject)
        {
            // do nothing
        }

        public void LogError(string subject, Exception e)
        {
            // do nothing
        }
    }
}