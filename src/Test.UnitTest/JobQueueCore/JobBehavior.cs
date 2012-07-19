using System.Diagnostics;
using JobQueueCore;
using NUnit.Framework;
using Should;

namespace UnitTest.JobQueueCore
{
    class JobBehavior
    {
        [Test]
        public void JobExecutionShouldBeLogged()
        {
            var logger = new LoggerMock();
            var job = new Job { LoggerDelegate = logger };
            
            job.Execute();

            logger.IsLogged(LogActivity.JobStarted, job.ItemDescription).ShouldBeTrue("Job start should be logged.");
            logger.IsLogged(LogActivity.JobFinished, job.ItemDescription).ShouldBeTrue("Job finish should be logged.");
        }

        [Test]
        public void OneJobTaskInstanceCannotBeAddedTwice()
        {
            var job = new Job();
            var successfuljobTask = new JobTaskToSucceed();
            
            try
            {
                job.JobTasks.Add(successfuljobTask);
                job.JobTasks.Add(successfuljobTask);
                Assert.Fail("Job tasks should not allow adding the same JobTask instance twice.");
            }
            catch (DuplicateItemException e)
            {
                Debug.Print("exception caught successfully: " + e.Message);
            }
        }
    }
}
