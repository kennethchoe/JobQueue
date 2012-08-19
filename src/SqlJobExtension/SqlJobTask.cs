using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using JobQueueCore;

namespace SqlJobExtension
{
    public class SqlJobTask: JobTaskBase
    {
        public SqlConnection Connection;
        public string Sql;
        public string UndoSql;
        public Dictionary<string, object> Parameters;
        private readonly string _jobTaskName;

        public SqlJobTask(string jobTaskName, string sql)
        {
            Sql = sql;
            UndoSql = "";
            _jobTaskName = jobTaskName;
            Parameters = new Dictionary<string, object>();
        }

        public override string JobTaskName()
        {
            return _jobTaskName;
        }

        public override void Execute()
        {
            EnsureConnectionIsOpen();
            ExecuteNonQuery(Sql);
        }

        private void EnsureConnectionIsOpen()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        public override void Undo()
        {
            if (UndoSql != "")
            {
                EnsureConnectionIsOpen();
                ExecuteNonQuery(UndoSql);
            }
            else
            {
                throw new UndoTaskNotDefinedExcepton();
            }
        }

        private void ExecuteNonQuery(string sql)
        {
            var cmd = new SqlCommand(sql, Connection);
            if (JobConfiguration.AppSettings != null)
            {
                try
                {
                    var timeout = JobConfiguration.AppSettings["CommandTimeout"];
                    cmd.CommandTimeout = Int32.Parse(timeout.ToString());
                }
                catch
                {
                }
            }

            foreach (var parameter in Parameters)
                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);

            LoggerDelegate.LogDebugInfo(JobTaskName(), "Parameters:\r\n" + cmd.Parameters);
            LoggerDelegate.LogDebugInfo(JobTaskName(), "Sql:\r\n" + sql);

            cmd.ExecuteNonQuery();
        }

    }
}