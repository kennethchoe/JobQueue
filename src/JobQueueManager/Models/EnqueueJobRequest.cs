namespace JobQueueManager.Models
{
    public class EnqueueJobRequest
    {
        public string JobLibraryAssemblyName { get; set; }
        public string JobClassName { get; set; }
        public object[] Parameters { get; set; }
    }
}