namespace JobQueueCore
{
    public enum JobStatus
    {
        Queued,
        Executing,
        Executed,
        Failed,
        InvalidJobId,
        NotDeserializable
    }
}