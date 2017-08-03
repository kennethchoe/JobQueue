using System;
using System.Data.SqlClient;
using JobQueueCore;
using NUnit.Framework;
using SampleSqlJobLibrary.Jobs;
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
            var job = new SqlJobToSucceed();
            job.SetParameters(DateTime.Parse("1/1/2012"), "abcd", "TestLogs");
            job.Execute();

            var cmd = new SqlCommand("select top 1 LogText from TestLogs order by id desc", job.Connection);
            var returnValue = cmd.ExecuteScalar();
            returnValue.ShouldEqual("Macro parameter will be replaced anywhere. Inserting into TestLogs");
        }

        [Test]
        public void FailedSqlJobShouldExecuteUndoTask()
        {
            var job = new SqlJobToFail();
            job.SetParameters(DateTime.Parse("1/1/2012"));
            try
            {
                job.Execute();
            } catch
            {
                job.Undo();
            }

            var cmd = new SqlCommand("select top 1 LogText from TestLogs order by id desc", job.Connection);
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
