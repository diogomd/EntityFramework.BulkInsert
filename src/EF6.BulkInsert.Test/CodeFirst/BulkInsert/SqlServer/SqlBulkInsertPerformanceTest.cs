using EF6.BulkInsert.Providers;

namespace EF6.BulkInsert.Test.CodeFirst.BulkInsert.SqlServer
{
    public class SqlBulkInsertPerformanceTest : PerformanceTestBase<SqlBulkInsertProvider, SqlContext>
    {
        protected override string ProviderConnectionType
        {
            get { return "System.Data.SqlClient.SqlConnection"; }
        }

        protected override SqlContext GetContext()
        {
            return new SqlContext();
        }
    }
}