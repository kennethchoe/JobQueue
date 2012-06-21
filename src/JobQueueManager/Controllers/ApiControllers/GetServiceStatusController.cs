using System.ServiceProcess;
using System.Web.Http;
using JobQueueManager.App_Start;
using JobQueueManager.Models;

namespace JobQueueManager.Controllers.ApiControllers
{
    public class GetServiceStatusController : ApiController
    {
        //example URL: http://localhost:49555/api/getservicestatus
        public JobExecutionServiceStatus Get()
        {
            var status = new JobExecutionServiceStatus();

            var serviceController = new ServiceController(JobQueueConfig.JobExecutionServiceName);
            status.ServiceStatus = serviceController.Status;

            status.JobCount = JobQueueConfig.JobQueue.Count;
            status.ErroredJobCount = JobQueueConfig.JobQueue.ErroredJobs.Count();

            return status;
        }
    }
}