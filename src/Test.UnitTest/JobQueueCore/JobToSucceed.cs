using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class JobToSucceed: Job
    {
        public JobToSucceed()
        {
            Commands.Add(new CommandToSucceed());
            Commands.Add(new CommandToSucceed());
        }
    }
}
