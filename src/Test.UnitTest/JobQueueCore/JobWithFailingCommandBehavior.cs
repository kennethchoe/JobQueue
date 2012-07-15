using System;
using JobQueueCore;
using NUnit.Framework;
using Should;

namespace UnitTest.JobQueueCore
{
    class JobWithFailingCommandBehavior
    {
        private LoggerMock _logger;
        private Job _job;

        [SetUp]
        public void GivenThatJobHasFailingCommand()
        {
            _logger = new LoggerMock();
            _job = new Job {LoggerDelegate = _logger};

            _job.Commands.Add(new CommandToSucceed());
            _job.Commands.Add(new CommandToFail());
            _job.Commands.Add(new CommandToSucceed());
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

                _logger.IsLogged(LogActivity.CommandFinished, _job.Commands[1]).ShouldBeFalse("Failing Command should not have finished log.");

                _logger.IsLogged(LogActivity.ErrorOccurred, _job.Commands[1]).ShouldBeTrue("Failing Command should have error log.");

                _logger.IsLogged(LogActivity.CommandUndone, _job.Commands[0]).ShouldBeTrue("Command before failing Command should be undone.");
                _logger.IsLogged(LogActivity.CommandUndone, _job.Commands[1]).ShouldBeTrue("Command before failing Command should be undone.");

            }
        }

    }
}
