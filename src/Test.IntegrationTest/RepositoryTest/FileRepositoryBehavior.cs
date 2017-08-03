using System.IO;
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

            var path = Path.GetTempPath();

            Repository = new FileQueueRepository<Job>(path + "\\queue");
            var fileErrorRep = new FileQueueRepository<Job>(path + "\\queue-error");
            var fileExecutedRep = new FileQueueRepository<Job>(path + "\\queue-executed");
            
            JobQueue = new JobQueue { Repository = Repository, ErroredJobs = fileErrorRep, ExecutedJobs = fileExecutedRep, LoggerDelegate = Logger};
            JobQueue.Clear();
            JobQueue.ErroredJobs.Clear();
            JobQueue.ExecutedJobs.Clear();
        }

    }
}
