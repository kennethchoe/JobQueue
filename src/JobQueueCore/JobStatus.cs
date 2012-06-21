namespace JobQueueCore
{
    public enum JobStatus
    {
        Queued,
        Executing,
        Failed,
        InvalidJobId,
        NotDeserializable
    }
}