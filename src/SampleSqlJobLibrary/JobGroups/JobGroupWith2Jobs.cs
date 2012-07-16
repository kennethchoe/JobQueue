using System;
using System.Collections.Generic;
using JobQueueCore;
using SampleSqlJobLibrary.Jobs;

namespace SampleSqlJobLibrary.JobGroups
{
    public class JobGroupWith2Jobs: JobGroup
    {
        public void SetParameters(DateTime targetDate, string arg2, string targetTable)
        {
            Parameters["@TargetDate"] = targetDate;
            Parameters["@arg2"] = arg2;
            Parameters["{TargetTable}"] = targetTable;
        }

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
