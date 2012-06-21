using System;
using System.Diagnostics;

using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    internal class CommandToFail : CommandBase
    {
        public override string CommandName()
        {
            return "Failing Command";
        }

        public override void Execute()
        {
            throw new Exception("Failing Command executed.");
        }

        public override void Undo()
        {
            Debug.Print("Undo from " + GetType());
        }
    }
}