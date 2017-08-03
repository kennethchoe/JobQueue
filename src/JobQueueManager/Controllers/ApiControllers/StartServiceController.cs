using System;
using System.ServiceProcess;
using System.Web.Http;

namespace JobQueueManager.Controllers.ApiControllers
{
    public class StartServiceController : ApiController
    {
        //example URL: http://localhost:49555/api/startservice
        public string Get(string repositoryType)
        {
            var serviceController = new ServiceController(JobQueueConfig.JobExecutionServiceName);
            if (serviceController.Status == ServiceControllerStatus.Running)
                return "Service is already running.";

            if (repositoryType == null)
                serviceController.Start();
            else
                serviceController.Start(new [] { repositoryType });

            serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));

            return "Service started.";
        }
    }
}