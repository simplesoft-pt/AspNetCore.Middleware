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
using System.Data.Common;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// The SQL health check properties.
    /// </summary>
    public class SqlHealthCheckProperties : HealthCheckProperties
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name">The health check name</param>
        /// <param name="connectionBuilder">The connection builder function</param>
        /// <param name="sql">The SQL to be executed agains the database. If null or empty, the connection will only be open.</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SqlHealthCheckProperties(
            string name, Func<DbConnection> connectionBuilder, string sql = null, 
            bool required = false, params string[] tags) 
            : base(name, required, tags)
        {
            ConnectionBuilder = connectionBuilder ?? throw new ArgumentNullException(nameof(connectionBuilder));
            Sql = sql;
        }

        /// <summary>
        /// Builds database connections
        /// </summary>
        public Func<DbConnection> ConnectionBuilder { get; }

        /// <summary>
        /// SQL to be executed
        /// </summary>
        public string Sql { get; }
    }
}