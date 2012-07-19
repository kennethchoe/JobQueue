using System.Diagnostics;
using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class JobTaskToSucceed: JobTaskBase
    {
        public override string JobTaskName()
        {
            return "Successful JobTask";
        }

        public override void Execute()
        {
            Debug.Print("Execute from " + GetType());
        }

        public override void Undo()
        {
            Debug.Print("Undo from " + GetType());
        }
    }
}
