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
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="properties">The health check properties</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SqlHealthCheck(SqlHealthCheckProperties properties, ILogger<SqlHealthCheck> logger = null) 
            : base(properties, logger)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <summary>
        /// The SQL health check properties
        /// </summary>
        protected new SqlHealthCheckProperties Properties { get; }

        /// <inheritdoc />
        public override async Task<HealthCheckStatus> OnUpdateStatusAsync(CancellationToken ct)
        {
            Logger.LogDebug("Accessing the database");

            using (var conn = Properties.ConnectionBuilder())
            {
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync(ct);

                if (Properties.Sql != null)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Properties.Sql;
                        Logger.LogDebug("Executing database command [Timeout:{timeout}, Type:'{type}', Text:'{text}']",
                            cmd.CommandTimeout, cmd.CommandType, cmd.CommandText);

                        await cmd.ExecuteNonQueryAsync(ct);
                    }
                }
            }

            Logger.LogDebug("Connected and query executed in the database");

            return HealthCheckStatus.Green;
        }
    }
}
