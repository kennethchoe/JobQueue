using System;
using FileRepository;
using JobQueueCore;
using NUnit.Framework;
using SampleSqlJobLibrary;
using Should;

namespace IntegrationTest.JobExecutionServiceTest
{
    [TestFixture]
    class JobExecutorBehavior
    {
        private JobQueue _fileQueue;

        [SetUp]
        public void InitQueue()
        {
            var fileRep = new FileQueueRepository<Job>(Environment.CurrentDirectory + "\\queue");

            _fileQueue = new JobQueue { Repository = fileRep };

            //_fileQueue.Clear();
        }

        [Test]
        public void JobExecutorShouldWait()
        {
            var job = new SqlJobToSucceed(DateTime.Parse("1/1/2012"), "abcd");
            _fileQueue.Enqueue(job);

            var jobExecutor = new JobExecutor(_fileQueue);

            jobExecutor.MonitorJobQueue(null);
            _fileQueue.Count.ShouldEqual(0);

            jobExecutor.MonitorJobQueue(null);
        }
    }
}
