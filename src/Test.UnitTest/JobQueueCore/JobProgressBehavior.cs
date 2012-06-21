using JobQueueCore;
using NUnit.Framework;

namespace UnitTest.JobQueueCore
{
    class JobProgressBehavior : IProgressDelegate
    {
        [Test]
        public void StoppedJobShouldBeLoggedAsCancelled()
        {
            var logger = new LoggerMock();
            var job = new Job { LoggerDelegate = logger, ProgressDelegate = this };

            job.Commands.Add(new CommandToSucceed());
            job.Commands.Add(new CommandToSucceed());
            job.Execute();

            logger.IsLogged(LogActivity.JobCancelled, job.ItemDescription);
        }

        public bool ShouldStop()
        {
            return true;
        }
    }
}
