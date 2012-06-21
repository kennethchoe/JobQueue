using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            var sqlWithParameter = ApplyParameters(Sql);
            var cmd = new SqlCommand(sqlWithParameter, Connection);
            cmd.ExecuteNonQuery();
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
                var sqlWithParameter = ApplyParameters(UndoSql);
                var cmd = new SqlCommand(sqlWithParameter, Connection);
                cmd.ExecuteNonQuery();
            }
        }

        private string ApplyParameters(string sql)
        {
            var param = new object[Parameters.Count];

            int i = 0;
            foreach (var parameter in Parameters)
            {
                param[i] = parameter.Value;
                i++;
            }

            return string.Format(sql, param);
        }
    }
}