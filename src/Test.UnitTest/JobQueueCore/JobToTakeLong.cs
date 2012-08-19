using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class JobToTakeLong: Job
    {
        public JobToTakeLong()
        {
            JobTasks.Add(new JobTaskToTakeLong());
        }
    }
}
