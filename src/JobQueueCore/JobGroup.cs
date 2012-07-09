using System.Collections.Generic;

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
    }
}