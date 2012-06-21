using System.ServiceProcess;
using System.Threading;
using JobQueueCore;
using JobQueueManager.App_Start;
using JobQueueManager.Controllers.ApiControllers;
using JobQueueManager.Models;
using NUnit.Framework;
using Should;

namespace FullSystemTest.JobQueueManagerTest
{
    [TestFixture]
    public class WebApiBehaviorWithFileRepository: WebApiBehavior
    {
        [TestFixtureSetUp]
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
