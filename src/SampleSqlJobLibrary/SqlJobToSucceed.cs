using System;

namespace SampleSqlJobLibrary
{
    public class SqlJobToSucceed: SqlJobExtension.SqlJob
    {
        // constructor without any parameters is a must so that IXmlSerialize can work.
        // it also must call this(a,b) so that parameters are populated within construction call.
        public SqlJobToSucceed(): this(DateTime.Now, "")
        {
            
        }

        public SqlJobToSucceed(DateTime targetDate, string arg2)
        {
            Parameters.Add("{0}", targetDate);
            Parameters.Add("{1}", arg2);
        }

    }
}
