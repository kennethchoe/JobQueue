using System;
using JobQueueCore;
using NUnit.Framework;
using SampleSqlJobLibrary;
using Should;

namespace IntegrationTest.RepositoryTest
{
    abstract class RepositoryBehavior
    {
        protected JobQueue JobQueue;
        protected IQueueRepository<Job> Repository;

        [Test]
        public void PeekOnEmptyJobQueueShouldReturnNull()
        {
            JobQueue.Peek().ShouldBeNull();
        }

        [Test]
        public void EnqueueShouldIncreaseCountBy1()
        {

            var job = new Job();
            JobQueue.Enqueue(job);

            JobQueue.Count.ShouldEqual(1);
        }

        [Test]
        public void SqlJobWithParametersShouldBePreservedAfterEnqueued()
        {
            var job = new SqlJobToSucceed(DateTime.Parse("1/1/2012"), "abcd1234");
            JobQueue.Enqueue(job);

            JobQueue.Count.ShouldEqual(1);

            var job2 = JobQueue.Peek();

            job2.Parameters.Count.ShouldEqual(2);
        }

        [Test]
        public void SqlJobDequeueShouldDecreaseCountBy1()
        {
            var job = new SqlJobToSucceed(DateTime.Parse("1/1/2012"), "abc123");
            JobQueue.Enqueue(job);
            JobQueue.Dequeue();

            JobQueue.Count.ShouldEqual(0);
        }

        [Test]
        public void ErroredSqlJobShouldNotHaltJobQeueueExecution()
        {
            var job = new SqlJobToFail(DateTime.Parse("1/1/2012"));
            JobQueue.Enqueue(job);

            JobQueue.Execute();
            JobQueue.Count.ShouldEqual(0);
            JobQueue.ErroredJobs.Count().ShouldEqual(1);
        }

        [Test]
        public void DeserializationShouldMarkBadJobsToSkipNextTime()
        {
            Repository.Enqueue(new NotDeserializableJob());

            try
            {
                JobQueue.Execute();
            }
            catch (InvalidOperationException e)
            {
                if (!(e.InnerException is NotDeserializableJobException))
                    throw;
            }
            JobQueue.Count.ShouldEqual(0);
        }

        [Test]
        public void JobShouldBeSearchableById()
        {
            var itemId = Repository.Enqueue(new Job());
            Repository.FindItemById(itemId).ItemId.ShouldEqual(itemId);
        }

    }
}
