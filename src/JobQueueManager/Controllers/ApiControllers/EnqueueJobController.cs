using System;
using System.Collections.Generic;
using System.Web.Http;
using JobQueueCore;
using JobQueueManager.App_Start;
using Newtonsoft.Json;

namespace JobQueueManager.Controllers.ApiControllers
{
    public class EnqueueJobController : ApiController
    {
        //example URL: http://localhost:49555/api/enqueuejob/?assemblyName=SyncDBJobs&className=SyncDBJobs.SyncTransactions&parameters=01/01/2012,bcde
        public string Get(string assemblyName, string className, string parameters)
        {
            var parsedParameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(parameters);
            
            var job = JobFactory.Build(assemblyName, className);
            job.Parameters = parsedParameters;

            var jobId = JobQueueConfig.JobQueue.Enqueue(job);

            return jobId;
        }

    }
}