namespace JobQueueManager.ViewModels
{
    public class JobQueueControlViewModel
    {
        public ControlCommand ControlCommand { get; set; }

        public string EnqueueJobAssemblyName { get; set; }
        public string EnqueueJobClassName { get; set; }
        public string EnqueueJobParameters { get; set; }

        public string GetJobStatusJobId { get; set; }
    }

    public enum ControlCommand
    {
        StartService,
        StopService,
        ClearJobQueue,
        GetServiceStatus,
        EnqueueJob,
        GetJobStatusById
    }

}