using System.Data.SqlClient;
using JobQueueCore;
using NUnit.Framework;
using Should;
using SqlJobExtension;
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
        public void JobParameterShouldEnrichCommands()
        {
            var sqlJob = new SqlJob();
            sqlJob.Parameters.Add("{0} - number1", 2);
            sqlJob.Parameters.Add("{1} - number2", 3);

            sqlJob.Commands.Add(new SqlJobCommand("test1", "select return_value = {0} * {1} into #return_value_table"));
            sqlJob.Execute();

            var conn = sqlJob.Connection;

            var cmd = new SqlCommand("select max(return_value) from #return_value_table", conn);
            var returnValue = cmd.ExecuteScalar();
            returnValue.ShouldEqual(6);

            conn.Close();
        }
    }

}
