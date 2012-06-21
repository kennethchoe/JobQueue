using System.Web.Http;
using JobQueueCore;
using JobQueueManager.App_Start;

namespace JobQueueManager.Controllers.ApiControllers
{
    public class EnqueueJobController : ApiController
    {
        //example URL: http://localhost:49555/api/enqueuejob/?assemblyName=SyncDBJobs&className=SyncDBJobs.SyncTransactions&parameters=01/01/2012,bcde
        public string Get(string assemblyName, string className, string parameters)
        {
            var job = JobFactory.Build(assemblyName, className, parameters);
            var jobId = JobQueueConfig.JobQueue.Enqueue(job);

            return jobId;
        }

    }
}