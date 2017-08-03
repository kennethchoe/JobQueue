using System;

using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    internal class JobTaskToFailWithoutUndo : JobTaskBase
    {
        public override string JobTaskName()
        {
            return "Failing JobTask";
        }

        public override void Execute()
        {
            throw new Exception("Failing JobTask executed.");
        }
    }
}