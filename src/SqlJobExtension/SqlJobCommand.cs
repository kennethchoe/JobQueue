using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using JobQueueCore;

namespace SqlJobExtension
{
    public class SqlJobCommand: CommandBase
    {
        public SqlConnection Connection;
        public string Sql;
        public string UndoSql;
        public Dictionary<string, object> Parameters;
        private readonly string _commandName;

        public SqlJobCommand(string commandName, string sql)
        {
            Sql = sql;
            UndoSql = "";
            _commandName = commandName;
            Parameters = new Dictionary<string, object>();
        }

        public override string CommandName()
        {
            return _commandName;
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

            if (JobConfiguration.LogDebugInfo())
            {
                LoggerDelegate.LogDebugInfo(CommandName(), "Parameters:\r\n" + cmd.Parameters);
                LoggerDelegate.LogDebugInfo(CommandName(), "Sql:\r\n" + sql);
            }

            cmd.ExecuteNonQuery();
        }

    }
}