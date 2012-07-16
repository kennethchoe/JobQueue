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
        [SetUp]
        public void SetupQueue()
        {
            JobConfiguration.AppSettings = Settings.Default;

            var conn = new SqlConnection { ConnectionString = Settings.Default.ConnectionString };

            Repository = new SqlQueueRepository<Job>(conn, "ActiveItems");
            var sqlErrorRep = new SqlQueueRepository<Job>(conn, "ErroredItems");
            var sqlExecutedRep = new SqlQueueRepository<Job>(conn, "ExecutedItems");

            JobQueue = new JobQueue { Repository = Repository, ErroredJobs = sqlErrorRep, ExecutedJobs = sqlExecutedRep, LoggerDelegate = Logger };
            JobQueue.Clear();
            JobQueue.ErroredJobs.Clear();
            JobQueue.ExecutedJobs.Clear();
        }

    }
}
