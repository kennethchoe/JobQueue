using System;
using JobQueueCore;
using NUnit.Framework;
using Should;

namespace UnitTest.JobQueueCore
{
    [TestFixture]
    class JobQueueBehavior
    {
        private JobQueue _jobQueue;

        [SetUp]
        public void Init()
        {
            _jobQueue = new JobQueue();
            _jobQueue.Clear();
        }

        [Test]
        public void JobShouldBeSearchableById()
        {
            var itemId = _jobQueue.Enqueue(new JobToSucceed());
            _jobQueue.CheckJobStatusById(itemId).ShouldEqual(JobStatus.Queued);
        }

        [Test]
        public void FailedJobShouldNotStopTheQueueAndBeRegisteredToErroredJobs()
        {
            _jobQueue.Enqueue(new JobToSucceed());
            _jobQueue.Enqueue(new JobToFail());
            _jobQueue.Enqueue(new JobToSucceed());

            _jobQueue.Execute();
            _jobQueue.Count.ShouldEqual(0, "All successful and failure jobs should be processed.");
            _jobQueue.ErroredJobs.Count().ShouldEqual(1, "Failed job should be registered to ErroredJobs.");
        }

        [Test]
        public void DoNotRunLongerThanAttributeShouldStopTheQueue()
        {
            _jobQueue.Enqueue(new JobToTakeLong());
            _jobQueue.Enqueue(new JobToTakeLong());
            _jobQueue.Enqueue(new JobToTakeLong());

            _jobQueue.DoNotRunLongerThan = new TimeSpan(0, 0, 0, 0, 150);
            _jobQueue.Execute();
            _jobQueue.Count.ShouldEqual(1, ".15 seconds execution time should leave 1 job in the queue.");
        }

    }
}
