using System;

namespace JobQueueCore
{
    public class JobQueue
    {
        public IProgressDelegate ProgressDelegate;
        public ILoggerDelegate LoggerDelegate;
        public TimeSpan DoNotRunLongerThan;

        public IQueueRepository<Job> Repository;
        public IQueueRepository<Job> ErroredJobs;

        private string _currentJobId = "";

        public JobQueue()
        {
            ProgressDelegate = new NullProgressDelegate();
            LoggerDelegate = new NullLoggerDelegate();
            Repository = new InMemoryQueueRepository<Job>();
            ErroredJobs = new InMemoryQueueRepository<Job>();
        }

        public int Count
        {
            get { return Repository.Count(); }
        }

        public Job Peek()
        {
            if (Count == 0)
                return default(Job);

            try
            {
                return Repository.Peek();
            }
            catch (ItemDeserializationException e)
            {
                LoggerDelegate.LogError("Job Deserialization Failure", e);
                Repository.MarkPeekItemAsBad();

                return Peek();
            }
        }

        public string Enqueue(Job item)
        {
            var itemId = Repository.Enqueue(item);
            item.ItemId = itemId;
            LoggerDelegate.Log(LogActivity.Enqueued, item.ItemDescription);
            return itemId;
        }

        public Job Dequeue()
        {
            var item = Repository.Dequeue();
            LoggerDelegate.Log(LogActivity.Dequeued, item.ItemDescription);
            return item;
        }

        public void Clear()
        {
            Repository.Clear();
            LoggerDelegate.Log(LogActivity.QueueCleared, "");
        }

        public void Execute()
        {
            var startTime = DateTime.Now;

            do
            {
                var job = Peek();
                if (job == default(Job))
                    break;

                _currentJobId = job.ItemId;
                ExecuteOneJob(job);

                if (job.IsStopped)
                    break;

                if (DoNotRunLongerThan != TimeSpan.Zero)
                    if (startTime.Add(DoNotRunLongerThan).CompareTo(DateTime.Now) < 0)
                        break;

            } while (true);

            _currentJobId = "";
        }

        private void ExecuteOneJob(Job job)
        {
            job.LoggerDelegate = LoggerDelegate;
            job.ProgressDelegate = ProgressDelegate;

            try
            {
                job.Execute();

                if (!job.IsStopped)
                    Dequeue();
            }
            catch (Exception)
            {
                job.Undo();

                //this approach will not work for multiple JobExecutionService environment
                Dequeue();
                ErroredJobs.Enqueue(job);

                LoggerDelegate.Log(LogActivity.JobCancelled, job.ItemDescription);
            }
        }

        public JobStatus CheckJobStatusById(string jobId)
        {
            if (jobId == _currentJobId)
                return JobStatus.Executing;

            try
            {
                var item = ErroredJobs.FindItemById(jobId);
                if (item != null)
                    return JobStatus.Failed;

                item = Repository.FindItemById(jobId);
                if (item != null)
                    return JobStatus.Queued;
            }
            catch (ItemDeserializationException)
            {
                return JobStatus.NotDeserializable;
            }

            return JobStatus.InvalidJobId;
        }
    }
}
