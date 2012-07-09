using System.Collections.Generic;
using System.Web.Mvc;

namespace JobQueueManager.ViewModels
{
    public class ControlTaskModel
    {
        public string ControlCommand { get; set; }

        public string WebApiUrl { get; set; }
    }
}