using EntityFramework.BulkInsert.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EntityFramework.BulkInsert.Providers
{
    public class DefaultBulkInsertProvider : ProviderBase<DbConnection, DbTransaction>
    {
        public DefaultBulkInsertProvider()
        {
            SetProviderIdentifier("*");
        }

        public override object GetSqlGeography(string wkt, int srid)
        {
            throw new NotImplementedException();
        }

        public override object GetSqlGeometry(string wkt, int srid)
        {
            throw new NotImplementedException();
        }

        protected override DbConnection CreateConnection()
        {
            return DbConfiguration.DependencyResolver.GetService<IDbConnectionFactory>().CreateConnection(ConnectionString);
        }

        protected override string ConnectionString => DbConnection.ConnectionString;

        private bool IsValidIdentityType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }

        public override void Run<T>(IEnumerable<T> entities, DbTransaction transaction)
        {
            bool keepIdentity = (BulkCopyOptions.KeepIdentity & Options.BulkCopyOptions) > 0;
            bool keepNulls = (BulkCopyOptions.KeepNulls & Options.BulkCopyOptions) > 0;

            using (var reader = new MappedDataReader<T>(entities, this))
            {
                var columns = reader.Cols
                    .Where(x => !x.Value.Computed && (!x.Value.IsIdentity || keepIdentity))
                    .ToArray();

                // INSERT INTO [TableName] (column list)
                var insert = new StringBuilder()
                    .Append($" INSERT INTO {reader.TableName} ")
                    .Append("(")
                    .Append(string.Join(",", columns.Select(col => col.Value.ColumnName)))
                    .Append(")")
                    .Append(" VALUES")
                    .ToString();

                int i = 0;
                long rowsCopied = 0;
                var rows = new List<string>();
                while (reader.Read())
                {
                    var values = new List<string>();
                    foreach (var col in columns)
                    {
                        var value = reader.GetValue(col.Key);
                        var type = col.Value.Type;

                        AddParameter(type, value, values);
                    }

                    rows.Add("(" + string.Join(",", values) + ")");

                    i++;

                    if (i == Options.BatchSize || i == Options.NotifyAfter)
                    {
                        using (var cmd = CreateCommand(CreateInsertBatchText(insert, rows), transaction.Connection, transaction))
                            cmd.ExecuteNonQuery();

                        if (Options.Callback != null)
                        {
                            int batches = Options.BatchSize / Options.NotifyAfter;

                            rowsCopied += i;
                            Options.Callback(this, new RowsCopiedEventArgs(rowsCopied));
                        }

                        i = 0;
                        rows.Clear();
                    }
                }

                if (rows.Any())
                {
                    using (var cmd = CreateCommand(CreateInsertBatchText(insert, rows), transaction.Connection, transaction))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        public override Task RunAsync<T>(IEnumerable<T> entities, DbTransaction transaction)
        {
            return Task.Run(() => Run(entities, transaction));
        }

        private void AddParameter(Type type, object value, List<string> values)
        {
            if (type == null
                || type == typeof(string)
                || type == typeof(Guid?)
                || type == typeof(Guid))
            {
                if (value == null)
                {
                    values.Add("NULL");
                }
                else
                {
                    values.Add($"'{value.ToString().Replace("'", "''")}'");
                }
            }
            else if (IsDateType(type))
            {
                if (value == null)
                {
                    values.Add("NULL");
                }
                else
                {
                    const string dateTimePattern = "yyyy-MM-dd HH:mm:ss.ffffff";
                    if (value is DateTime)
                    {
                        values.Add($"'{((DateTime)value).ToString(dateTimePattern)}'");
                    }
                    else if (value is DateTimeOffset)
                    {
                        values.Add($"'{((DateTimeOffset)value).ToString(dateTimePattern)}'");
                    }
                }
            }
            else if (type.IsEnum)
            {
                if (value == null)
                {
                    values.Add("NULL");
                }
                else
                {
                    var enumUnderlyingType = type.GetEnumUnderlyingType();
                    values.Add(Convert.ChangeType(value, enumUnderlyingType).ToString());
                }
            }
            else
            {
                if (value == null)
                {
                    values.Add("NULL");
                }
                else
                {
                    values.Add(value.ToString());
                }
            }
        }

        private IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandTimeout = Options.TimeOut;

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }
            return cmd;
        }

        private string CreateInsertBatchText(string insertHeader, List<string> rows)
        {
            return insertHeader + " " + string.Join(",", rows) + ";";
        }

        private bool IsDateType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsDateType(Nullable.GetUnderlyingType(type));

            return type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }

    }
}
