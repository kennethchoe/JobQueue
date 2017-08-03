using System;
using System.ServiceProcess;
using System.Web.Http;

namespace JobQueueManager.Controllers.ApiControllers
{
    public class StopServiceController : ApiController
    {
        //example URL: http://localhost:49555/api/stopservice
        public string Get()
        {
            var serviceController = new ServiceController(JobQueueConfig.JobExecutionServiceName);
            if (serviceController.Status == ServiceControllerStatus.Stopped)
                return "Service is not running.";

            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
            return "Service is stopped.";
        }
    }
}