using JobQueueCore;
using SampleSqlJobLibrary.Jobs;

namespace SampleSqlJobLibrary.JobGroups
{
    class JobGroupWith2Jobs: JobGroup
    {
        public override string[] EnqueueOnJobQueue(JobQueue jobQueue)
        {
            var job1 = new SqlJobToSucceed {Parameters = Parameters};
            var job1Id = jobQueue.Enqueue(job1);

            var job2 = new SqlJobToSucceed {Parameters = Parameters};
            var job2Id = jobQueue.Enqueue(job2);

            return new string[2] { job1Id, job2Id };
        }
    }
}
