using System;
using System.Diagnostics;

using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    internal class JobTaskToFail : JobTaskBase
    {
        public override string JobTaskName()
        {
            return "Failing JobTask";
        }

        public override void Execute()
        {
            throw new Exception("Failing JobTask executed.");
        }

        public override void Undo()
        {
            Debug.Print("Undo from " + GetType());
        }
    }
}