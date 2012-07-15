using System;
using System.Diagnostics;

using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    internal class CommandToFailWithoutUndo : CommandBase
    {
        public override string CommandName()
        {
            return "Failing Command";
        }

        public override void Execute()
        {
            throw new Exception("Failing Command executed.");
        }
    }
}