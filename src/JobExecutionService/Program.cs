using System.ServiceProcess;
using JobExecutionService.Properties;
using JobQueueCore;
using log4net.Config;

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
            XmlConfigurator.Configure();

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
