using System;
using System.Collections.Generic;
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
    public abstract class WebApiBehavior
    {
        [SetUp]
        public virtual void SetUp()
        {
            new ClearJobQueueController().Get();
            new StopServiceController().Get(); // if it was running before, stop it.
        }

        protected abstract void StartService();

        [TearDown]
        public void StopJobService()
        {
            new StopServiceController().Get(); // stop it so that consequent testing can rewrite the executable.
        }

        [Test]
        public void ClearedStatusCheck()
        {
            JobExecutionServiceStatus status;

            StartService();
            status = new GetServiceStatusController().Get();
            status.ServiceStatus.ShouldEqual(ServiceControllerStatus.Running);

            new StopServiceController().Get();
            status = new GetServiceStatusController().Get();
            status.ServiceStatus.ShouldEqual(ServiceControllerStatus.Stopped);

            status.JobCount.ShouldEqual(0);
            status.ErroredJobCount.ShouldEqual(0);
        }

        [Test]
        public void EnqueuedCommonJobShouldBeQueuedStatusThenExecutedAfterExecution()
        {
            var jobId = new EnqueueJobController().Get("JobQueueCore", "JobQueueCore.Job", "{\"TargetDate\":\"01/01/2012\",\"arg2\":\"cdef\",\"{TargetTable}\":\"TestLogs\"}");

            TestAJob(jobId);
        }

        [Test]
        public void EnqueuedSqlJobShouldBeQueuedStatusThenExecutedAfterExecution()
        {
            var jobId = new EnqueueJobController().Get("SampleSqlJobLibrary", "SampleSqlJobLibrary.Jobs.SqlJobToSucceed", "{\"TargetDate\":\"01/01/2012\",\"arg2\":\"cdef\",\"{TargetTable}\":\"TestLogs\"}");

            TestAJob(jobId);
        }

        private void TestAJob(string jobId)
        {
            var jobStatus = new GetJobStatusController().Get(jobId);
            jobStatus.ShouldEqual(JobStatus.Queued);

            StartService();

            int repeatTime = 0;
            do
            {
                jobStatus = new GetJobStatusController().Get(jobId);
                if (jobStatus == JobStatus.InvalidJobId)
                    break;

                Thread.Sleep(100);
            } while (repeatTime++ < 50);

            var status = new GetServiceStatusController().Get();
            status.JobCount.ShouldEqual(0);

            jobStatus = new GetJobStatusController().Get(jobId);
            jobStatus.ShouldEqual(JobStatus.Executed);
        }

        [Test]
        public void JobGroupShouldEnqueueMultipleJobs()
        {
            StopJobService();
            string[] jobIds = new EnqueueJobGroupController().Get("SampleSqlJobLibrary", "SampleSqlJobLibrary.JobGroups.JobGroupWith2Jobs", "{\"TargetDate\":\"01/01/2012\",\"arg2\":\"cdef\"}");
            jobIds.Length.ShouldEqual(2);
        }

    }
}
