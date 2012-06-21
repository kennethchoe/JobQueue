namespace SampleSqlJobLibrary
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
