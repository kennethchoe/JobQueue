using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class JobToTakeLong: Job
    {
        public JobToTakeLong()
        {
            Commands.Add(new CommandToTakeLong());
        }
    }
}
