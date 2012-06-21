using System;
using System.Configuration;
using JobQueueCore;
using SyncDBJobsTest.Properties;

namespace SyncDBJobsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            JobConfiguration.AppSettings = Settings.Default;

            var job = new SampleSqlJobLibrary.SqlJobToSucceed(DateTime.Parse("05/01/2012"), "test");
            job.Execute();
        }
    }
}
