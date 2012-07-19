using System.Threading;
using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class JobTaskToTakeLong: JobTaskBase
    {
        public override string JobTaskName()
        {
            return "JobTask that takes 100 milliseconds to run";
        }

        public override void Execute()
        {
            Thread.Sleep(100);
        }
    }
}
