using JobQueueManager;
using JobQueueManager.Controllers.ApiControllers;
using NUnit.Framework;

namespace FullSystemTest.JobQueueManagerTest
{
    [TestFixture]
    public class WebApiBehaviorWithSqlRepository: WebApiBehavior
    {
        [OneTimeSetUp]
        public void InitJobQueue()
        {
            JobQueueConfig.ConfigSettings();
            JobQueueConfig.JobQueue = JobExecutionService.JobQueueFinder.GetJobQueue(JobQueueConfig.JobExecutionServiceName, "sql");
        }

        protected override void StartService()
        {
            new StartServiceController().Get("sql");
        }
    }
}
