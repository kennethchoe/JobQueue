using System;
using System.Collections.Generic;
using System.Web.Http;
using JobQueueCore;
using JobQueueManager.App_Start;

namespace JobQueueManager.Controllers
{
    public class EnqueueJobController : ApiController
    {
        //example URL: http://localhost:49555/api/enqueuejob/?assemblyName=SyncDBJobs&className=SyncDBJobs.SyncTransactions&parameters=01/01/2012,bcde
        public string Get(string assemblyName, string className, string parameters)
        {
            var obj = Activator.CreateInstance(assemblyName, className).Unwrap();
            var job = obj as Job;

            if (job == null)
            {
                throw new Exception("Job class not found.");
            }

            job.Parameters = BuildParameters(job, parameters);

            var jobId = JobQueueService.JobQueue.Enqueue(job);

            return jobId;
        }

        private static Dictionary<string, object> BuildParameters(Job job, string parameters)
        {
            int i = 0;
            var newParams = new Dictionary<string, object>();

            var parameterArray = parameters.Split(',');
            foreach (var parameter in job.Parameters)
            {
                newParams.Add(parameter.Key, parameterArray[i]);
                i++;
            }
            return newParams;
        }
    }
}