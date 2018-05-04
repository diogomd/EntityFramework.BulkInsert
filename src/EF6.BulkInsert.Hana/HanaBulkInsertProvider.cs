using System.Collections.Generic;
using EF6.BulkInsert.Helpers;

using System.Threading.Tasks;
using Sap.Data.Hana;
using System;

namespace EF6.BulkInsert.Providers
{
    public class HanaBulkInsertProvider : ProviderBase<HanaConnection, HanaTransaction>
    {
        public HanaBulkInsertProvider()
        {
            SetProviderIdentifier("Sap.Data.Hana.HanaConnection");
        }

        /// <summary>
        /// Runs sql bulk insert using custom IDataReader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        public override void Run<T>(IEnumerable<T> entities, HanaTransaction transaction)
        {
            var sqlBulkCopyOptions = ToHanaBulkCopyOptions(Options.BulkCopyOptions);
            var keepIdentity = (HanaBulkCopyOptions.KeepIdentity & sqlBulkCopyOptions) > 0;
            using (var reader = new MappedDataReader<T>(entities, this))
            {
                using (var bulkCopy = new HanaBulkCopy(transaction.Connection, sqlBulkCopyOptions, transaction))
                {
                    bulkCopy.BulkCopyTimeout = Options.TimeOut;
                    bulkCopy.BatchSize = Options.BatchSize;
                    bulkCopy.DestinationTableName = string.Format("[{0}].[{1}]", reader.SchemaName, reader.TableName);                    

                    bulkCopy.NotifyAfter = Options.NotifyAfter;
                    if (Options.Callback != null)
                    {
                        bulkCopy.HanaRowsCopied += (sender, args) =>
                        {
                            Options.Callback.Invoke(sender, new RowsCopiedEventArgs(args.RowsCopied));
                        };
                    }

                    foreach (var kvp in reader.Cols)
                    {
                        if (kvp.Value.IsIdentity && !keepIdentity)
                        {
                            continue;
                        }
                        bulkCopy.ColumnMappings.Add(kvp.Value.ColumnName, kvp.Value.ColumnName);
                    }

                    bulkCopy.WriteToServer(reader);
                }
            }
        }

        public override object GetSqlGeography(string wkt, int srid)
        {
            throw new NotImplementedException();
        }

        public override object GetSqlGeometry(string wkt, int srid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs sql bulk insert using custom IDataReader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="transaction"></param>
        public override async Task RunAsync<T>(IEnumerable<T> entities, HanaTransaction transaction)
        {
            var sqlBulkCopyOptions = ToHanaBulkCopyOptions(Options.BulkCopyOptions);
            var keepIdentity = (HanaBulkCopyOptions.KeepIdentity & sqlBulkCopyOptions) > 0;
            using (var reader = new MappedDataReader<T>(entities, this))
            {
                using (var bulkCopy = new HanaBulkCopy(transaction.Connection, sqlBulkCopyOptions, transaction))
                {
                    bulkCopy.BulkCopyTimeout = Options.TimeOut;
                    bulkCopy.BatchSize = Options.BatchSize;
                    bulkCopy.DestinationTableName = string.Format("[{0}].[{1}]", reader.SchemaName, reader.TableName);

                    bulkCopy.NotifyAfter = Options.NotifyAfter;
                    if (Options.Callback != null)
                    {
                        bulkCopy.HanaRowsCopied += (sender, args) =>
                        {
                            Options.Callback.Invoke(sender, new RowsCopiedEventArgs(args.RowsCopied));
                        };
                    }

                    foreach (var kvp in reader.Cols)
                    {
                        if (kvp.Value.IsIdentity && !keepIdentity)
                        {
                            continue;
                        }
                        bulkCopy.ColumnMappings.Add(kvp.Value.ColumnName, kvp.Value.ColumnName);
                    }

                    await bulkCopy.WriteToServerAsync(reader);
                }
            }
        }

        /// <summary>
        /// Create new sql connection
        /// </summary>
        /// <returns></returns>
        protected override HanaConnection CreateConnection()
        {
            return new HanaConnection(ConnectionString);
        }

        private HanaBulkCopyOptions ToHanaBulkCopyOptions(BulkCopyOptions bulkCopyOptions)
        {
            return (HanaBulkCopyOptions)(int)bulkCopyOptions;
        }
    }
}