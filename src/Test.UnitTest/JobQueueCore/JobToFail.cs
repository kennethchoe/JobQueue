using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    public class JobToFail : Job
    {
         public JobToFail()
        {
            JobTasks.Add(new JobTaskToSucceed());
            JobTasks.Add(new JobTaskToFail());
        }
    }
}