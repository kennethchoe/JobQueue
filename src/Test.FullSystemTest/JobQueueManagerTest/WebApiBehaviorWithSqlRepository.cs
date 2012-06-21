using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
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
    public class WebApiBehaviorWithSqlRepository: WebApiBehavior
    {
        [TestFixtureSetUp]
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
