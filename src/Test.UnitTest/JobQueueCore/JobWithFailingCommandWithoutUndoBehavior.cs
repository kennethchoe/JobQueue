using System;
using JobQueueCore;
using NUnit.Framework;
using Should;

namespace UnitTest.JobQueueCore
{
    class JobWithFailingCommandWithoutUndoBehavior
    {
        [Test]
        public void FailedJobLogShouldShowUndoCommandsAreExecuted()
        {
            var logger = new LoggerMock();

            var job = new Job { LoggerDelegate = logger };
            job.Commands.Add(new CommandToFailWithoutUndo());

            try
            {
                job.Execute();
            }
            catch (Exception)
            {
                job.Undo();

                logger.IsLogged(LogActivity.SkippingUndoNotDefined, job.Commands[0]).ShouldBeTrue("Command without undo instruction should log as skipping undo not defined.");

            }
        }

    }
}
