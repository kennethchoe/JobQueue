namespace JobQueueCore
{
    class NullProgressDelegate: IProgressDelegate
    {
        public bool ShouldStop()
        {
            return false;
        }
    }
}
