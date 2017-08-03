using JobQueueManager.App_Start;
using JobQueueManager.Controllers.ApiControllers;
using NUnit.Framework;

namespace FullSystemTest.JobQueueManagerTest
{
    [TestFixture]
    public class WebApiBehaviorWithFileRepository: WebApiBehavior
    {
        [OneTimeSetUp]
        public void InitJobQueue()
        {
            JobQueueConfig.ConfigSettings();
            JobQueueConfig.JobQueue = JobExecutionService.JobQueueFinder.GetJobQueue(JobQueueConfig.JobExecutionServiceName, "file");
        }

        protected override void StartService()
        {
            new StartServiceController().Get("file");
        }
    }
}
