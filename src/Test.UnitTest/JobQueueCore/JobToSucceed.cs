using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class JobToSucceed: Job
    {
        public JobToSucceed()
        {
            JobTasks.Add(new JobTaskToSucceed());
            JobTasks.Add(new JobTaskToSucceed());
        }
    }
}
