using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Health check for SQL databases.
    /// </summary>
    public class SqlHealthCheck : HealthCheck
    {
        private readonly Func<DbConnection> _connectionBuilder;
        private readonly string _sql;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name">The health check name</param>
        /// <param name="connectionBuilder">The connection builder function</param>
        /// <param name="sql">The SQL to be executed agains the database</param>
        /// <param name="logger">An optional logger instance</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SqlHealthCheck(
            string name, Func<DbConnection> connectionBuilder, string sql = "SELECT 1",
            ILogger<SqlHealthCheck> logger = null, bool required = false, params string[] tags) 
            : base(name, logger, required, tags)
        {
            _connectionBuilder = connectionBuilder ?? throw new ArgumentNullException(nameof(connectionBuilder));
            _sql = sql ?? throw new ArgumentNullException(nameof(sql));
        }

        /// <inheritdoc />
        public override async Task<HealthCheckStatus> OnUpdateStatusAsync(CancellationToken ct)
        {
            Logger.LogDebug("Accessing the database");

            using (var conn = _connectionBuilder())
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync(ct);

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = _sql;
                    await command.ExecuteNonQueryAsync(ct);
                }
            }

            Logger.LogDebug("Connected and query executed in the database");

            return HealthCheckStatus.Green;
        }
    }
}
