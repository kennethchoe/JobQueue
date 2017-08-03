using System;
using System.IO;
using FileRepository;
using JobQueueCore;
using NUnit.Framework;
using SampleSqlJobLibraryTest.Properties;
using log4net.Config;

namespace SampleSqlJobLibraryTest
{
    [TestFixture]
    class SampleSqlJobBehavior
    {
        private ILoggerDelegate _logger;
        private JobQueue _jobQueue;

        [OneTimeSetUp]
        public void InitJobQueue()
        {
            XmlConfigurator.Configure();

            JobConfiguration.AppSettings = Settings.Default;

            var path = Path.GetTempPath();

            var repository = new FileQueueRepository<Job>(path + "\\queue");
            var fileErrorRep = new FileQueueRepository<Job>(path + "\\queue-error");
            var fileExecutedRep = new FileQueueRepository<Job>(path + "\\queue-executed");
            _logger = new Log4NetLogger.Logger();

            _jobQueue = new JobQueue { Repository = repository, ErroredJobs = fileErrorRep, ExecutedJobs = fileExecutedRep, LoggerDelegate = _logger };
            _jobQueue.Clear();

            _jobQueue.LoggerDelegate = _logger;
        }

        [Test]
        public void RunSampleJob()
        {
            var jobGroup = new SampleSqlJobLibrary.JobGroups.JobGroupWith2Jobs();
            jobGroup.SetParameters(DateTime.Parse("05/01/2012"), "test", "TestLogs");
            jobGroup.EnqueueOnJobQueue(_jobQueue);

            _jobQueue.Execute();
        }

    }
}
