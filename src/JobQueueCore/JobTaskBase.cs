using System;

namespace JobQueueCore
{
    public abstract class JobTaskBase
    {
        protected JobTaskBase()
        {
            LoggerDelegate = new NullLoggerDelegate();
        }

        public int Order;

        public ILoggerDelegate LoggerDelegate;

        public abstract string JobTaskName();
        public abstract void Execute();

        public virtual void Undo()
        {
            throw new UndoTaskNotDefinedExcepton();
        }

        public string JobTaskNameWithOrder()
        {
            return Order + ". " + JobTaskName();
        }
    }

    public class UndoTaskNotDefinedExcepton : Exception
    {
        
    }
}
