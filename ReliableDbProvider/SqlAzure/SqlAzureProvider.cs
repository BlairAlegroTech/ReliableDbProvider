﻿using System;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
using Microsoft.Practices.TransientFaultHandling;

namespace ReliableDbProvider.SqlAzure
{
    /// <summary>
    /// Db provider that will result in reliable connections to a SQL Azure database.
    /// Will not retry for timeouts, if you need that then use the SqlAzureProvider in the ReliableDbProvider.SqlAzureWithTimeoutRetries namespace.
    /// </summary>
    public class SqlAzureProvider : ReliableSqlClientProvider<SqlAzureTransientErrorDetectionStrategy>
    {
        /// <summary>
        /// Singleton instance of the provider.
        /// </summary>
        public static readonly SqlAzureProvider Instance = new SqlAzureProvider();

        /// <summary>
        /// Event that is fired when a command is retried using this provider.
        /// </summary>
        public static event EventHandler<RetryingEventArgs> CommandRetry;

        /// <summary>
        /// Event that is fired when a connection is retried using this provider.
        /// </summary>
        public static event EventHandler<RetryingEventArgs> ConnectionRetry;

        protected override RetryStrategy GetCommandRetryStrategy()
        {
            return RetryStrategies.DefaultCommandStrategy;
        }

        protected override RetryStrategy GetConnectionRetryStrategy()
        {
            return RetryStrategies.DefaultConnectionStrategy;
        }

        protected override DbConnection GetConnection(ReliableSqlConnection connection)
        {
            connection.CommandRetryPolicy.Retrying += CommandRetry;
            connection.ConnectionRetryPolicy.Retrying += ConnectionRetry;
            return new SqlAzureConnection(connection);
        }
    }

    /// <summary>
    /// Wraps a <see cref="ReliableSqlConnection"/> in a <see cref="DbConnection"/> for the <see cref="SqlAzureProvider"/> Db Provider.
    /// </summary>
    public class SqlAzureConnection : ReliableSqlDbConnection
    {
        /// <summary>
        /// Create a <see cref="SqlAzureConnection"/>.
        /// </summary>
        /// <param name="connection">The <see cref="ReliableSqlConnection"/> to wrap</param>
        public SqlAzureConnection(ReliableSqlConnection connection) : base(connection) { }
        protected override DbProviderFactory GetProviderFactory()
        {
            return SqlAzureProvider.Instance;
        }
    }
}
