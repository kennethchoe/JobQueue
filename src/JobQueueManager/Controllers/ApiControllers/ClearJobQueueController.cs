﻿using System.Web.Http;

namespace JobQueueManager.Controllers.ApiControllers
{
    public class ClearJobQueueController : ApiController
    {
        //example URL: http://localhost:49555/api/clearjobqueue
        public string Get()
        {
            JobQueueConfig.JobQueue.Clear();
            JobQueueConfig.JobQueue.ErroredJobs.Clear();

            return "Job Queue cleared.";
        }
    }
}