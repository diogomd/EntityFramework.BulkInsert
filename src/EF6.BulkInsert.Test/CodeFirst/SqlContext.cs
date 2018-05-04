using System.Data.Entity;
using EF6.BulkInsert.Test.CodeFirst.Domain;
using EF6.BulkInsert.Test.Domain;
using System.Data.Entity.Infrastructure;

namespace EF6.BulkInsert.Test.CodeFirst
{
    [DbConfigurationType(typeof(SqlContextConfig))]
    public class SqlContext : TestBaseContext
    {
        public DbSet<PinPoint> PinPoints { get; set; }
    }

    public class SqlContextConfig : DbConfiguration
    {
        public SqlContextConfig()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new DefaultExecutionStrategy());
        }
    }
}
