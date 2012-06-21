namespace JobQueueCore
{
    public interface IProgressDelegate
    {
        bool ShouldStop();
    }
}