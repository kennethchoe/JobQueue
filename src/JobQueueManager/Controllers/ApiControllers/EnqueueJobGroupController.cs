using System.Collections.Generic;
using System.Web.Http;
using JobQueueCore;
using JobQueueManager.App_Start;
using Newtonsoft.Json;

namespace JobQueueManager.Controllers.ApiControllers
{
    public class EnqueueJobGroupController : ApiController
    {
        //example URL: http://localhost:49555/api/enqueuejobgroup/?assemblyName=SyncDBJobs&className=SyncDBJobs.SyncHost&parameters=01/01/2012,bcde
        public string[] Get(string assemblyName, string className, string parameters)
        {
            var parsedParameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(parameters);

            var jobGroup = JobFactory.BuildJobGroup(assemblyName, className);
            jobGroup.Parameters = parsedParameters;

            var jobIds = jobGroup.EnqueueOnJobQueue(JobQueueConfig.JobQueue);

            return jobIds;
        }
    }
}