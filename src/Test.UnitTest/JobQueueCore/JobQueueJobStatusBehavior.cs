using JobQueueCore;
using NUnit.Framework;
using Should;

namespace UnitTest.JobQueueCore
{
    [TestFixture]
    class JobQueueJobStatusBehavior: IProgressDelegate
    {
        private JobQueue _jobQueue;
        private string _executingJobId;

        [SetUp]
        public void InitJobQueue()
        {
            _jobQueue = new JobQueue();
            _jobQueue.Clear();
        }

        [Test]
        public void FailedJobShouldShowFailed()
        {
            var jobId = _jobQueue.Enqueue(new JobToFail());

            _jobQueue.Execute();
            _jobQueue.CheckJobStatusById(jobId).ShouldEqual(JobStatus.Failed);
        }

        [Test]
        public void SuccessfulJobShouldShowQueuedAndExecutingAndThenExecuted()
        {
            _executingJobId = _jobQueue.Enqueue(new JobToSucceed());
            _jobQueue.CheckJobStatusById(_executingJobId).ShouldEqual(JobStatus.Queued);

            _jobQueue.ProgressDelegate = this;

            _jobQueue.Execute();
            _jobQueue.CheckJobStatusById(_executingJobId).ShouldEqual(JobStatus.Executed);
        }

        #region Implementation of IProgressDelegate

        public bool ShouldStop()
        {
            _jobQueue.CheckJobStatusById(_executingJobId).ShouldEqual(JobStatus.Executing);
            return false;
        }

        #endregion
    }
}
