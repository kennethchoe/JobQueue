using System.Data.SqlClient;
using IntegrationTest.Properties;
using JobQueueCore;
using NUnit.Framework;
using SqlRepository;

namespace IntegrationTest.RepositoryTest
{
    [TestFixture]
    class SqlRepositoryBehavior: RepositoryBehavior
    {
        private SqlLogger _logger;

        [SetUp]
        public void SetupQueue()
        {
            JobConfiguration.AppSettings = Settings.Default;

            var conn = new SqlConnection { ConnectionString = Settings.Default.ConnectionString };

            Repository = new SqlQueueRepository<Job>(conn);
            _logger = new SqlLogger(conn);

            JobQueue = new JobQueue { Repository = Repository, LoggerDelegate = _logger };
            JobQueue.Clear();
            JobQueue.ErroredJobs.Clear();
        }

    }
}
