using System.Collections.Generic;
using Newtonsoft.Json;

namespace JobQueueCore
{
    public abstract class JobGroup
    {
        public Dictionary<string, object> Parameters;

        public JobGroup()
        {
            Parameters = new Dictionary<string, object>();
        }

        public abstract string[] EnqueueOnJobQueue(JobQueue jobQueue);

        public string EnqueueJob<T>(JobQueue jobQueue) where T : Job, new()
        {
            var job = new T();
            foreach (var param in Parameters)
            {
                if (param.Value != null)
                    job.Parameters[param.Key] = param.Value;
            }

            var jobId = jobQueue.Enqueue(job);
            return jobId;
        }

        public void LogJobGroup(ILoggerDelegate logger, string note)
        {
            logger.Log(LogActivity.EnqueueJobGroup, this + " " + note + " " + JsonConvert.SerializeObject(Parameters));
        }
    }
}