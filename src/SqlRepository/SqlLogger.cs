using System;
using System.Data.SqlClient;
using DapperExtensions;
using DapperExtensions.Mapper;
using JobQueueCore;

namespace SqlRepository
{
    public class SqlLogger: ILoggerDelegate
    {
        public SqlConnection Connection;

        public SqlLogger(SqlConnection connection)
        {
            Connection = connection;
            DapperExtensions.DapperExtensions.DefaultMapper = typeof(PluralizedAutoClassMapper<>);
        }

        public void Log(LogActivity activity, string subject)
        {
            Connection.Open();
            var newItem = new SqlQueueLog { LogText = activity.ToString() + " " + subject, CreatedDate = DateTime.Now };
            Connection.Insert(newItem);
            Connection.Close();
        }

        public void LogError(string subject, Exception e)
        {
            Connection.Open();
            var newItem = new SqlQueueLog { LogText = LogActivity.ErrorOccurred + " " + subject + ", " + e.Message, ErrorDetails = e.StackTrace, CreatedDate = DateTime.Now };
            Connection.Insert(newItem);
            Connection.Close();
        }
    }
}
