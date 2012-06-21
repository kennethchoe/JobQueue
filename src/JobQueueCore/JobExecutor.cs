using System;
using System.Threading;

namespace JobQueueCore
{
    internal enum JobExecutorStatus
    {
        Idle,
        Executing
    }

    public class JobExecutor: IProgressDelegate
    {
        private bool _shouldStop;
        private JobExecutorStatus _status;
        public JobQueue JobQueue;

        public JobExecutor(JobQueue jobQueue)
        {
            _status = JobExecutorStatus.Idle;
            _shouldStop = false;
            JobQueue = jobQueue;
            JobQueue.ProgressDelegate = this;
        }

        #region Implementation of IProgressDelegate

        public bool ShouldStop()
        {
            return _shouldStop;
        }

        #endregion

        public void ShouldStopNow()
        {
            _shouldStop = true;
        }

        public void WaitUntilIdle()
        {
            do
            {
                Thread.Sleep(100);
            } while (_status != JobExecutorStatus.Idle);
        }

        public void MonitorJobQueue(object stateObject)
        {
            if (_status != JobExecutorStatus.Idle)
                return;

            if (JobQueue.Count > 0)
            {
                _status = JobExecutorStatus.Executing;
                TryExecute();
            }

            _status = JobExecutorStatus.Idle;
        }

        private void TryExecute()
        {
            try
            {
                JobQueue.Execute();
            }
            catch (Exception e)
            {
                JobQueue.LoggerDelegate.LogError("JobExecutor", e);
            }
        }
    }
}
