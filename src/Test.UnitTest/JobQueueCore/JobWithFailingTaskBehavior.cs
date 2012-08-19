using System;
using JobQueueCore;
using NUnit.Framework;
using Should;

namespace UnitTest.JobQueueCore
{
    class JobWithFailingTaskBehavior
    {
        private LoggerMock _logger;
        private Job _job;

        [SetUp]
        public void GivenThatJobHasFailingTask()
        {
            _logger = new LoggerMock();
            _job = new Job {LoggerDelegate = _logger};

            _job.JobTasks.Add(new JobTaskToSucceed());
            _job.JobTasks.Add(new JobTaskToFail());
            _job.JobTasks.Add(new JobTaskToSucceed());
        }

        [Test]
        public void FailedJobShouldRaiseException()
        {
            try
            {
                _job.Execute();
                Assert.Fail("Failed job should raise an exception.");
            }
            catch (AssertionException) { throw; }
            catch (Exception)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void FailedJobLogShouldShowUndoCommandsAreExecuted()
        {
            try
            {
                _job.Execute();
            }
            catch (Exception)
            {
                _job.Undo();

                _logger.IsLogged(LogActivity.TaskFinished, _job.JobTasks[1]).ShouldBeFalse("Failing JobTask should not have finished log.");

                _logger.IsLogged(LogActivity.ErrorOccurred, _job.JobTasks[1]).ShouldBeTrue("Failing JobTask should have error log.");

                _logger.IsLogged(LogActivity.TaskUndone, _job.JobTasks[0]).ShouldBeTrue("JobTask before failing JobTask should be undone.");
                _logger.IsLogged(LogActivity.TaskUndone, _job.JobTasks[1]).ShouldBeTrue("JobTask before failing JobTask should be undone.");

            }
        }

    }
}
