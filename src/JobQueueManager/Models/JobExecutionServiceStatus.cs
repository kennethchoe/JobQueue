using System.ServiceProcess;

namespace JobQueueManager.Models
{
    public class JobExecutionServiceStatus
    {
        public int JobCount;
        public int ErroredJobCount;
        public ServiceControllerStatus ServiceStatus;
    }
}