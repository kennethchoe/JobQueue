using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace JobExecutionService
{
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {
        private readonly ServiceProcessInstaller _processInstaller;
        private readonly System.ServiceProcess.ServiceInstaller _serviceInstaller;

        public ServiceInstaller()
        {
            _processInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            _processInstaller.Account = ServiceAccount.LocalSystem;
            _serviceInstaller.StartType = ServiceStartMode.Manual;
            _serviceInstaller.ServiceName = "JobExecutionService";

            Installers.Add(_serviceInstaller);
            Installers.Add(_processInstaller);
        }
    }
}
