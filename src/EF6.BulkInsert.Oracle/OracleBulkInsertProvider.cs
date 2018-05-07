using EF6.BulkInsert.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF6.BulkInsert.Oracle
{
    public class OracleBulkInsertProvider : DefaultBulkInsertProvider
    {
        public OracleBulkInsertProvider()
        {
            SetProviderIdentifier("Oracle.ManagedDataAccess.Client.OracleConnection");
        }
        protected override string CreateInsertBatchText(string insertHeader, List<string> rows)
        {
            return $"BEGIN \n{base.CreateInsertBatchText(insertHeader, rows)}\nEND;";
        }

        protected override void AddParameter(Type type, object value, List<string> values)
        {
            if (this.IsDateType(type))
            {
                if (value is DateTime)
                {
                    values.Add($"TO_DATE('{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS')");
                }
                else if (value is DateTimeOffset)
                {
                    values.Add($"TO_DATE('{((DateTimeOffset)value).ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS')");
                }
            }
            else
                base.AddParameter(type, value, values);
        }
    }
}
