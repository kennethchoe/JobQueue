using System.Data.SqlClient;
using System.Management;
using System.Windows.Forms;
using FileRepository;
using JobExecutionService.Properties;
using JobQueueCore;
using SqlRepository;

namespace JobExecutionService
{
    public class JobQueueFinder
    {
        public static JobQueue GetJobQueue(string jobExecutionServiceName, string repositoryType)
        {
            if ((repositoryType ?? Settings.Default.RepositoryType) == "file")
                return AcquireFileJobQueue(jobExecutionServiceName);

            // repositoryType is "sql"
            return AcquireSqlJobQueue();

        }

        private static JobQueue AcquireFileJobQueue(string jobExecutionServiceName)
        {
            string jobExecutionServiceLocation = GetPathOfService(jobExecutionServiceName);
            var fileRep = new FileQueueRepository<Job>(jobExecutionServiceLocation + "\\queue");
            var fileErrorRep = new FileQueueRepository<Job>(jobExecutionServiceLocation + "\\queue-error");
            var fileExecutedRep = new FileQueueRepository<Job>(jobExecutionServiceLocation + "\\queue-executed");
            var logger = new Logger(jobExecutionServiceLocation + "\\log");

            return new JobQueue { Repository = fileRep, ErroredJobs = fileErrorRep, ExecutedJobs = fileExecutedRep, LoggerDelegate = logger };
        }

        private static JobQueue AcquireSqlJobQueue()
        {
            var conn = new SqlConnection { ConnectionString = Settings.Default.ConnectionString };

            var sqlRep = new SqlQueueRepository<Job>(conn, "ActiveItems");
            var sqlErrorRep = new SqlQueueRepository<Job>(conn, "ErroredItems");
            var sqlExecutedRep = new SqlQueueRepository<Job>(conn, "ExecutedItems");
            var logger = new SqlLogger(conn);

            return new JobQueue { Repository = sqlRep, ErroredJobs = sqlErrorRep, ExecutedJobs = sqlExecutedRep, LoggerDelegate = logger };
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
