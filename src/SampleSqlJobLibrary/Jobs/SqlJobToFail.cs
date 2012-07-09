using System;

namespace SampleSqlJobLibrary.Jobs
{
    public class SqlJobToFail: SqlJobExtension.SqlJob
    {
        public SqlJobToFail()
        {
            Parameters.Add("targetDate", null);
        }

        public void SetParameters(DateTime targetDate)
        {
            Parameters["targetDate"] = targetDate;
        }

    }
}
