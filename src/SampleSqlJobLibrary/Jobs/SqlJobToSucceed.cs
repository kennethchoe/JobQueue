using System;

namespace SampleSqlJobLibrary.Jobs
{
    public class SqlJobToSucceed: SqlJobExtension.SqlJob
    {
        public SqlJobToSucceed()
        {
            Parameters.Add("@TargetDate", null);
            Parameters.Add("@arg2", null);
            Parameters.Add("{TargetTable}", null);
        }

        public void SetParameters(DateTime targetDate, string arg2, string targetTable)
        {
            Parameters["@TargetDate"] = targetDate;
            Parameters["@arg2"] = arg2;
            Parameters["{TargetTable}"] = targetTable;
        }
    }
}
