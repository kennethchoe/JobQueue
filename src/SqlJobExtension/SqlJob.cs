using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JobQueueCore;

namespace SqlJobExtension
{
    public class SqlJob: Job
    {
        public static readonly Regex CurlyParameterPattern = new Regex("\\{(.*)\\}");
        public SqlConnection Connection;

        public SqlJob()
        {
            AddSqlJobTasksFromResourcesIfAny();
        }

        private void AddSqlJobTasksFromResourcesIfAny()
        {
            var allResourceNames = GetType().Assembly.GetManifestResourceNames();
            Array.Sort(allResourceNames);

            IEnumerable<string> sqlResourceNames = GetSqlFileNames(allResourceNames);

            foreach (var resourceName in sqlResourceNames)
            {
                var sqlJobTask = new SqlJobTask(resourceName, GetSqlScript(resourceName))
                                     {UndoSql = GetUndoSqlScriptIfFound(allResourceNames, resourceName)};

                JobTasks.Add(sqlJobTask);
            }
        }

        private IEnumerable<string> GetSqlFileNames(IEnumerable<string> allResourceNames)
        {
            var resourceNamePattern = ".JobSqls." + GetType().Name + ".";
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

        protected override void EnrichJobTaskBeforeExecution(JobTaskBase jobTask)
        {
            var sqlCmd = jobTask as SqlJobTask;
            if (sqlCmd != null)
            {
                SetConnection();

                sqlCmd.Connection = Connection;
                ApplyParameters(sqlCmd);
            }
        }

        private void ApplyParameters(SqlJobTask sqlCmd)
        {
            var sqlParameters = new Dictionary<string, object>();
            foreach (var parameter in Parameters)
            {
                if (CurlyParameterPattern.IsMatch(parameter.Key))
                {
                    sqlCmd.Sql = sqlCmd.Sql.Replace(parameter.Key, parameter.Value.ToString());
                    sqlCmd.UndoSql = sqlCmd.UndoSql.Replace(parameter.Key, parameter.Value.ToString());
                }
                else
                    sqlParameters.Add(parameter.Key, parameter.Value);
            }

            sqlCmd.Parameters = sqlParameters;
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
