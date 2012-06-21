using System;
using System.IO;
using System.Text;
using JobQueueCore;

namespace FileRepository
{
    public class Logger: ILoggerDelegate

    {
        private readonly DirectoryInfo _logfileDirectory;

        public Logger(string folder)
        {
            if (!folder.EndsWith("\\"))
                folder += "\\";

            _logfileDirectory = Directory.CreateDirectory(folder);
        }

        public void Log(LogActivity activity, string subject)
        {
            string fileName = _logfileDirectory.FullName + DateTime.Now.ToString("yyyy_MMM_dd") + ".txt";
            var content = DateTime.Now + "\t" + activity.ToString() + "\t" + subject + "\n";

            Stream oStream = new FileStream(fileName, FileMode.Append);
            oStream.Write(Encoding.ASCII.GetBytes(content), 0, Encoding.ASCII.GetByteCount(content));
            oStream.Close();
        }

        public void LogError(string subject, Exception e)
        {
            Log(LogActivity.ErrorOccurred, subject);
            Log(LogActivity.ErrorOccurred, e.Message);

            foreach (var line in e.StackTrace.Split('\n'))
            {
                Log(LogActivity.ErrorOccurred, line);
            }
        }
    }
}
