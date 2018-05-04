using System.Data.Entity;
using EntityFramework.BulkInsert.Test.CodeFirst.Domain;
using EntityFramework.BulkInsert.Test.Domain;
using System.Data.Entity.Infrastructure;

namespace EntityFramework.BulkInsert.Test.CodeFirst
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
