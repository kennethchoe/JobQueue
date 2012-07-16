using System;
using FileRepository;
using IntegrationTest.Properties;
using JobQueueCore;
using NUnit.Framework;

namespace IntegrationTest.RepositoryTest
{
    [TestFixture]
    class FileRepositoryBehavior: RepositoryBehavior
    {
        [SetUp]
        public void SetupQueue()
        {
            JobConfiguration.AppSettings = Settings.Default;

            Repository = new FileQueueRepository<Job>(Environment.CurrentDirectory + "\\queue");
            var fileErrorRep = new FileQueueRepository<Job>(Environment.CurrentDirectory + "\\queue-error");
            var fileExecutedRep = new FileQueueRepository<Job>(Environment.CurrentDirectory + "\\queue-executed");
            
            JobQueue = new JobQueue { Repository = Repository, ErroredJobs = fileErrorRep, ExecutedJobs = fileExecutedRep, LoggerDelegate = Logger};
            JobQueue.Clear();
            JobQueue.ErroredJobs.Clear();
            JobQueue.ExecutedJobs.Clear();
        }

    }
}
