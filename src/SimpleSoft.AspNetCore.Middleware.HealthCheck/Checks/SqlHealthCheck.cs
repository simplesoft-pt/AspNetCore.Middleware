#region License
// The MIT License (MIT)
// 
// Copyright (c) 2018 Simplesoft.pt
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

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
