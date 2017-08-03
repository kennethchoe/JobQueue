using System.Web.Http;

namespace JobQueueManager.Controllers.ApiControllers
{
    public class GetJobStatusController : ApiController
    {
        // example URL: http://localhost:49555/api/getjobstatus/0b4c0404-4200-4276-bfa9-c5d6a13b1b25
        public JobQueueCore.JobStatus Get(string id)
        {
            return JobQueueConfig.JobQueue.CheckJobStatusById(id);
        }
   }
}