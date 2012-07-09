namespace SampleSqlJobLibrary.Jobs
{
    public class SqlJobWithWrongConnectionString: SqlJobExtension.SqlJob
    {
        protected override string ConnectionStringKey
        {
            get
            {
                return "WrongConnectionString";
            }
        }
    }
}
