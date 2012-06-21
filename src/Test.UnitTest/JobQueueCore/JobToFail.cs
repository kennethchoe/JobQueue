using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    public class JobToFail : Job
    {
         public JobToFail()
        {
            Commands.Add(new CommandToSucceed());
            Commands.Add(new CommandToFail());
        }
    }
}