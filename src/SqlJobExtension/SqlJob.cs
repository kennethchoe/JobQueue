using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using JobQueueCore;

namespace SqlJobExtension
{
    public class SqlJob: Job
    {
        public SqlConnection Connection;

        public SqlJob()
        {
            LoadCommandsFromResourcesIfAny();
        }

        private void LoadCommandsFromResourcesIfAny()
        {
            var allResourceNames = GetType().Assembly.GetManifestResourceNames();
            Array.Sort(allResourceNames);

            IEnumerable<string> sqlResourceNames = GetSqlFileNames(allResourceNames);

            foreach (var resourceName in sqlResourceNames)
            {
                var sqlCommand = new SqlJobCommand(resourceName, GetSqlScript(resourceName))
                                     {UndoSql = GetUndoSqlScriptIfFound(allResourceNames, resourceName)};

                Commands.Add(sqlCommand);
            }
        }

        private IEnumerable<string> GetSqlFileNames(IEnumerable<string> allResourceNames)
        {
            var resourceNamePattern = ".Sql." + GetType().Name + ".";
            return allResourceNames
                .Where(rn => rn.Contains(resourceNamePattern) && rn.EndsWith(".sql")).ToList();
        }

        private string GetUndoSqlScriptIfFound(IEnumerable<string> allResourceNames, string resourceName)
        {
            var undoResourceName = allResourceNames.FirstOrDefault(rn => rn == resourceName + ".Undo");
            if (undoResourceName != null)
                return GetSqlScript(undoResourceName);

            return "";
        }

        private string GetSqlScript(string resourceName)
        {
            Assembly assembly = GetType().Assembly;
            var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
                return "";

            var streamReader = new StreamReader(stream);

            string data = streamReader.ReadToEnd();
            streamReader.Close();

            return data;
        }

        protected override void EnrichCommandBeforeExecution(CommandBase command)
        {
            var sqlCmd = command as SqlJobCommand;
            if (sqlCmd != null)
            {
                SetConnection();

                sqlCmd.Connection = Connection;
                sqlCmd.Parameters = Parameters;
            }
        }

        protected virtual string ConnectionStringKey
        {
            get { return "ConnectionString"; }
        }

        private void SetConnection()
        {
            if (Connection == null)
            {
                if (JobConfiguration.AppSettings == null)
                    throw new Exception("Cannot retrieve SQL connection string. Set 'JobConfiguration.AppSettings = Settings.Default' and provide your SQL connection string under application property 'ConnectionString'.");

                var connectionString = JobConfiguration.AppSettings[ConnectionStringKey].ToString();
                Connection = new SqlConnection(connectionString);
            }
        }
    }
}
