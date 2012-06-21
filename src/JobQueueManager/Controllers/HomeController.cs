using System.Web.Mvc;
using JobQueueManager.ViewModels;

namespace JobQueueManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new JobQueueControlViewModel());
        }

        public string GetHttpApiUrl(JobQueueControlViewModel model)
        {
            var url = Request.Url.GetLeftPart(System.UriPartial.Authority) + GetApiUrl(model);
            return url;
        }

        public ActionResult InvokeControlCommand(JobQueueControlViewModel model)
        {
            return Redirect(GetHttpApiUrl(model));
        }

        private string GetApiUrl(JobQueueControlViewModel model)
        {
            string apistring;
            switch (model.ControlCommand)
            {
                case ControlCommand.EnqueueJob:
                    apistring = "enqueuejob/?assemblyName=" + model.EnqueueJobAssemblyName + "&className=" + model.EnqueueJobClassName + "&parameters=" + model.EnqueueJobParameters;
                    break;

                case ControlCommand.GetJobStatusById:
                    apistring = "getjobstatus/" + model.GetJobStatusJobId;
                    break;

                default:
                    apistring = model.ControlCommand.ToString().ToLower();
                    break;
            }

            return Url.Content("~/api/" + apistring);
        }
    }
}