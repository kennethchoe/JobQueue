using System;
using System.Data.SqlClient;
using JobQueueCore;
using NUnit.Framework;
using SampleSqlJobLibrary;
using Should;
using UnitTest.Properties;

namespace UnitTest.SqlJobExtensionTest
{
    [TestFixture]
    class SqlJobBehavior
    {
        [SetUp]
        public void SetupQueue()
        {
            JobConfiguration.AppSettings = Settings.Default;
        }

        [Test]
        public void SqlJobShouldExecuteSqlStatements()
        {
            var job = new SqlJobToSucceed(DateTime.Parse("1/1/2012"), "abcd");
            job.Execute();

            var cmd = new SqlCommand("select top 1 LogText from SyncDBLogs order by id desc", job.Connection);
            var returnValue = cmd.ExecuteScalar();
            returnValue.ShouldEqual("02 - 01/01/2012 message: abcd");
        }

        [Test]
        public void FailedSqlJobShouldExecuteUndoCommand()
        {
            var job = new SqlJobToFail(DateTime.Parse("1/1/2012"));
            try
            {
                job.Execute();
            } catch
            {
                job.Undo();
            }

            var cmd = new SqlCommand("select top 1 LogText from SyncDBLogs order by id desc", job.Connection);
            var returnValue = cmd.ExecuteScalar();
            returnValue.ShouldEqual("01 Undo");
        }

        [Test]
        public void SqlWithOverridingWrongConnectionStringShouldFail()
        {
            var job = new SqlJobWithWrongConnectionString();
            try
            {
                job.Execute();
            }
            catch (SqlException)
            {
                // this is expected behavior.
            }
        }
    }
}
