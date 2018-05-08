using EF6.BulkInsert.MySql;
using EF6.BulkInsert.Providers;

namespace EF6.BulkInsert.Test.CodeFirst.BulkInsert.MySql
{
    public class MySqlBulkInsertPerformanceTest : PerformanceTestBase<MySqlBulkInsertProvider, MySqlContext>
    {
        protected override string ProviderConnectionType
        {
            get { return "MySql.Data.MySqlClient.MySqlConnection"; }
        }

        protected override MySqlContext GetContext()
        {
            return new MySqlContext();
        }
    }
}
