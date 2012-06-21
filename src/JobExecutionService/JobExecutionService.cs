using System.ServiceProcess;
using System.Threading;
using JobExecutionService.Properties;
using JobQueueCore;

namespace JobExecutionService
{
    public partial class JobExecutionService : ServiceBase
    {
        private JobExecutor _jobExecutor;
        private Timer _stateTimer;
        private TimerCallback _timerDelegate;
        private readonly ILoggerDelegate _logger;

        public JobExecutionService(JobQueue jobQueue)
        {
            ServiceName = Settings.Default.JobExecutionServiceName;
            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = true;
            _jobExecutor = new JobExecutor(jobQueue);
            _logger = jobQueue.LoggerDelegate;

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            if (args.Length > 0)
                if (Settings.Default.RepositoryType != args[0])
                    // OnStart argument overrides repository type setting in app.config
                    _jobExecutor.JobQueue = JobQueueFinder.GetJobQueue(Settings.Default.JobExecutionServiceName, args[0]);

            _timerDelegate = _jobExecutor.MonitorJobQueue;
            _stateTimer = new Timer(_timerDelegate, null, 1000, 1000);
            _logger.Log(LogActivity.ServiceStarted, "");
        }

        protected override void OnStop()
        {
            _stateTimer.Dispose();
            _jobExecutor.ShouldStopNow();
            _jobExecutor.WaitUntilIdle();

            base.OnStop();
            _logger.Log(LogActivity.ServiceStopped, "");
        }
    }
}
