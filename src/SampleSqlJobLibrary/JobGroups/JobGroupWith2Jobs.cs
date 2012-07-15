using System.Collections.Generic;
using JobQueueCore;
using SampleSqlJobLibrary.Jobs;

namespace SampleSqlJobLibrary.JobGroups
{
    class JobGroupWith2Jobs: JobGroup
    {
        public override string[] EnqueueOnJobQueue(JobQueue jobQueue)
        {
            LogJobGroup(jobQueue.LoggerDelegate, "");

            var jobIds = new List<string>()
                {
                    EnqueueJob<SqlJobToSucceed>(jobQueue), 
                    EnqueueJob<SqlJobToSucceed>(jobQueue)
                };
            return jobIds.ToArray();
        }
    }
}
