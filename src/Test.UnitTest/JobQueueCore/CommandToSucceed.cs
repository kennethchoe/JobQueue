using System.Diagnostics;
using JobQueueCore;

namespace UnitTest.JobQueueCore
{
    class CommandToSucceed: CommandBase
    {
        public override string CommandName()
        {
            return "Successful Command";
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
