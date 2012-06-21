using JobQueueCore;
using NUnit.Framework;
using Should;

namespace UnitTest.JobQueueCore
{
    public class JobQueueProgressBehavior: IProgressDelegate
    {
        private JobQueue _jobQueue;

        [Test]
        public void StoppedJobShouldRemainInTheQueue()
        {
            _jobQueue = new JobQueue { ProgressDelegate = this };

            _jobQueue.Enqueue(new JobToSucceed());

            _jobQueue.Execute();
            _jobQueue.Count.ShouldEqual(1, "Stopping after first job's first command execution should leave 1 job in the queue.");
        }

        public bool ShouldStop()
        {
            return true;
        }
    }
}