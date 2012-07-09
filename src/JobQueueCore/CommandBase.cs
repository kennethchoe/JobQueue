namespace JobQueueCore
{
    public abstract class CommandBase
    {
        protected CommandBase()
        {
            LoggerDelegate = new NullLoggerDelegate();
        }

        public int Order;

        public ILoggerDelegate LoggerDelegate;

        public abstract string CommandName();
        public abstract void Execute();
        public abstract void Undo();

        public string CommandNameWithOrder()
        {
            return Order + ". " + CommandName();
        }
    }
}
