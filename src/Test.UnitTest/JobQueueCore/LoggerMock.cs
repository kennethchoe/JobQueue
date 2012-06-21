using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class LoggerMock : ILoggerDelegate
    {
        private readonly Collection<string> _log;

        #region Implementation of ILoggerDelegate

        public void Log(LogActivity logActivity, string subject)
        {
            var message = BuildMessage(logActivity, subject);
            Debug.Print(message);
            _log.Add(message);
        }

        public void LogError(string subject, Exception e)
        {
            Log(LogActivity.ErrorOccurred, subject);
            Debug.Print(e.Message);
            Debug.Print(e.StackTrace);
        }

        #endregion

        public LoggerMock()
        {
            _log = new Collection<string>();
        }

        public bool IsLogged(LogActivity logActivity, string subject)
        {
            var message = BuildMessage(logActivity, subject);
            return _log.Contains(message);
        }

        public bool IsLogged(LogActivity logActivity, CommandBase commandBase)
        {
            return IsLogged(logActivity, commandBase.CommandNameWithOrder());
        }

        private static string BuildMessage(LogActivity logActivity, string subject)
        {
            string message = logActivity.ToString() + " " + subject;
            return message;
        }

    }
}
