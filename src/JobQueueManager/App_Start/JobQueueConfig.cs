using JobQueueCore;
using JobQueueManager.Properties;

namespace JobQueueManager
{
    public static class JobQueueConfig
    {
        public static JobQueue JobQueue;
        public static string JobExecutionServiceName;

        public static void ConfigSettings()
        {
            JobConfiguration.AppSettings = Settings.Default;
            JobExecutionServiceName = Settings.Default.JobExecutionServiceName;
            JobQueue = JobExecutionService.JobQueueFinder.GetJobQueue(JobExecutionServiceName, null);
        }
    }
}