using System;

namespace SampleSqlJobLibrary
{
    public class SqlJobToFail: SqlJobExtension.SqlJob
    {
        public SqlJobToFail(): this(DateTime.Now)
        {
            
        }

        public SqlJobToFail(DateTime targetDate)
        {
            Parameters.Add("{0} = target date", targetDate);
        }

    }
}
