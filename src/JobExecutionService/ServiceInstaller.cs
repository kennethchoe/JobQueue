using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace JobExecutionService
{
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "JobExecutionService";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}
