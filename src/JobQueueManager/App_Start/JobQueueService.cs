using System.Configuration;
using System.IO;
using System.Management;
using System.ServiceProcess;
using FileRepository;
using JobQueueCore;

namespace JobQueueManager.App_Start
{
    public static class JobQueueService
    {
        public static JobQueueCore.JobQueue JobQueue;
        public static string JobExecutionServiceName;

        public static void AcquireJobQueue()
        {
            JobExecutionServiceName = JobQueueManager.Properties.Settings.Default.JobExecutionServiceName;

            string jobExecutionServiceLocation = GetPathOfService(JobExecutionServiceName);

            var fileRep = new FileQueueRepository<Job>(jobExecutionServiceLocation + "\\queue");
            var fileErrorRep = new FileQueueRepository<Job>(jobExecutionServiceLocation + "\\queue-error");
            var logger = new Logger(jobExecutionServiceLocation + "\\log");

            JobQueue = new JobQueue { Repository = fileRep, ErroredJobs = fileErrorRep, LoggerDelegate = logger };
        }

        // http://stackoverflow.com/questions/2728578/how-to-get-phyiscal-path-of-windows-service-using-net
        private static string GetPathOfService(string serviceName)
        {
            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", serviceName));
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wqlObjectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                var fullPathWithFilename = managementObject.GetPropertyValue("PathName").ToString();
                if (fullPathWithFilename.StartsWith("\""))
                    fullPathWithFilename = fullPathWithFilename.Substring(1, fullPathWithFilename.Length - 2);

                var fullPathOnly = fullPathWithFilename.Substring(0, fullPathWithFilename.LastIndexOf('\\'));
                return fullPathOnly;
            }

            return null;
        }
    }
}