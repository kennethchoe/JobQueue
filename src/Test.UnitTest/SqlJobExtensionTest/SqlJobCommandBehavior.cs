using System.Data.SqlClient;
using JobQueueCore;
using NUnit.Framework;
using Should;
using SqlJobExtension;
using UnitTest.JobQueueCore;
using UnitTest.Properties;

namespace UnitTest.SqlJobExtensionTest
{
    [TestFixture]
    class SqlJobCommandBehavior
    {
        [SetUp]
        public void SetupQueue()
        {
            JobConfiguration.AppSettings = Settings.Default;
        }

        [Test]
        public void SqlJobCommandShouldExecuteSqlStatements()
        {
            var conn = new SqlConnection {ConnectionString = Settings.Default.ConnectionString};
            conn.Open();

            var sqlCommand = new SqlJobCommand("test", "select 'executing'") {Connection = conn};
            sqlCommand.Execute();

            conn.Close();
        }

        [Test]
        public void SqlJobCommandsShouldShareConnection()
        {
            var sqlJob = new SqlJob();
            sqlJob.Commands.Add(new SqlJobCommand("test1", "select return_value = 1 into #return_value_table"));
            sqlJob.Commands.Add(new SqlJobCommand("test2", "update #return_value_table set return_value = return_value + 1"));

            sqlJob.Execute();

            var cmd = new SqlCommand("select max(return_value) from #return_value_table", sqlJob.Connection);
            var returnValue = cmd.ExecuteScalar();
            returnValue.ShouldEqual(2, "Connection is not shared throughout the commands in the SqlJobSample.");

        }

        [Test]
        public void SqlJobCommandsShouldLeaveDebugLog()
        {
            var logger = new LoggerMock();
            var sqlJob = new SqlJob() { LoggerDelegate = logger };
            sqlJob.Commands.Add(new SqlJobCommand("test1", "select 1 where 1=2"));
            sqlJob.Execute();

            logger.IsDebugLogged("test1").ShouldBeTrue();

        }

        [Test]
        public void JobParameterShouldEnrichCommands()
        {
            var sqlJob = new SqlJob();
            sqlJob.Parameters.Add("number1", 2);
            sqlJob.Parameters.Add("number2", 3);
            sqlJob.Parameters.Add("{result}", "##return_value_table");

            // when SqlCommand utilizes Parameters, # temp table does not get persistent through. ## temp table does.
            sqlJob.Commands.Add(new SqlJobCommand("test1", "select return_value = @number1 * @number2 into {result}"));
            sqlJob.Execute();

            var conn = sqlJob.Connection;

            var cmd = new SqlCommand("select max(return_value) from ##return_value_table", conn);
            var returnValue = cmd.ExecuteScalar();
            returnValue.ShouldEqual(6);

            conn.Close();
        }
    }

}
