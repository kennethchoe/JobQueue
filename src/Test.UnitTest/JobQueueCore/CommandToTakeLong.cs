using System.Threading;
using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class CommandToTakeLong: CommandBase
    {
        public override string CommandName()
        {
            return "Command that takes 100 milliseconds to run";
        }

        public override void Execute()
        {
            Thread.Sleep(100);
        }

        public override void Undo()
        {
            // do nothing
        }
    }
}
