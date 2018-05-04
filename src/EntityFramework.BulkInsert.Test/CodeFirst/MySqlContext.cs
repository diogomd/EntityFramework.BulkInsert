using EntityFramework.BulkInsert.Test.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.BulkInsert.Test.CodeFirst
{
    [DbConfigurationType(typeof(MySqlContextConfig))]
    public class MySqlContext : TestBaseContext
    {
        public MySqlContext() : base("MySqlTestContext")
        {
            
        }
    }

    public class MySqlContextConfig : DbConfiguration
    {
        public MySqlContextConfig()
        {
            SetProviderServices(
                nameof(global::MySql.Data.MySqlClient),
                new global::MySql.Data.MySqlClient.MySqlProviderServices()
            );
            
            SetExecutionStrategy(nameof(global::MySql.Data.MySqlClient), () => new DefaultExecutionStrategy());
        }
    }
}
