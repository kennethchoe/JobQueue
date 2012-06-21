namespace JobQueueCore
{
    public abstract class CommandBase
    {
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
