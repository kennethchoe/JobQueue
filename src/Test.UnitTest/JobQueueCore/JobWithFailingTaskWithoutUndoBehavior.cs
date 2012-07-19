using System;
using JobQueueCore;
using NUnit.Framework;
using Should;

namespace UnitTest.JobQueueCore
{
    class JobWithFailingTaskWithoutUndoBehavior
    {
        [Test]
        public void FailedJobLogShouldShowUndoTasksAreExecuted()
        {
            var logger = new LoggerMock();

            var job = new Job { LoggerDelegate = logger };
            job.JobTasks.Add(new JobTaskToFailWithoutUndo());

            try
            {
                job.Execute();
            }
            catch (Exception)
            {
                job.Undo();

                logger.IsLogged(LogActivity.SkippingUndoNotDefined, job.JobTasks[0]).ShouldBeTrue("JobTask without undo instruction should log as skipping undo not defined.");

            }
        }

    }
}
