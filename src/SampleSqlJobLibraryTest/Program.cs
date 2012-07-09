using System;
using JobQueueCore;
using SyncDBJobsTest.Properties;

namespace SyncDBJobsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            JobConfiguration.AppSettings = Settings.Default;

            var job = new SampleSqlJobLibrary.Jobs.SqlJobToSucceed();
            job.SetParameters(DateTime.Parse("05/01/2012"), "test", "SyncDbLogs");
            job.Execute();
        }
    }
}
