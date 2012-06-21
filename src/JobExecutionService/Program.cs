using System.ServiceProcess;
using JobExecutionService.Properties;
using JobQueueCore;

namespace JobExecutionService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static void Main()
        {
            JobConfiguration.AppSettings = Settings.Default;

            var jobExecutionServiceName = Settings.Default.JobExecutionServiceName;
            var jobQueue = JobQueueFinder.GetJobQueue(jobExecutionServiceName, null);

            var servicesToRun = new ServiceBase[] 
            { 
                new JobExecutionService(jobQueue)
            };
            ServiceBase.Run(servicesToRun);
        }

    }
}
