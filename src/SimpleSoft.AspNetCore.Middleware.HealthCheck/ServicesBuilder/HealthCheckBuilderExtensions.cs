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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Extensions for <see cref="IHealthCheckBuilder"/> instances.
    /// </summary>
    public static class HealthCheckBuilderExtensions
    {
        /// <summary>
        /// Adds the health check factory to the <see cref="IHealthCheckBuilder.Descriptors"/> collection.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="factory">The health check factory</param>
        /// <param name="lifetime">The service lifetime</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder Add(this IHealthCheckBuilder builder,
            Func<IServiceProvider, IHealthCheck> factory, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Add(new HealthCheckServiceDescriptor(factory, lifetime));
            return builder;
        }

        #region Delegate

        /// <summary>
        /// Adds a <see cref="DelegatingHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="name">The health check name</param>
        /// <param name="action">The action to execute to get the health check status</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddDelegate(this IHealthCheckBuilder builder,
            string name, Func<CancellationToken, Task<HealthCheckStatus>> action,
            bool required = false, params string[] tags)
        {
            return builder.AddDelegate(new DelegatingHealthCheckProperties(name, action, required, tags));
        }

        /// <summary>
        /// Adds a <see cref="DelegatingHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="name">The health check name</param>
        /// <param name="action">The action to execute to get the health check status</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddDelegate(this IHealthCheckBuilder builder,
            string name, Func<IServiceProvider, CancellationToken, Task<HealthCheckStatus>> action,
            bool required = false, params string[] tags)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (action == null) throw new ArgumentNullException(nameof(action));

            return builder.Add(p =>
                new DelegatingHealthCheck(
                    new DelegatingHealthCheckProperties(name, ct => action(p, ct), required, tags),
                    p.GetService<ILogger<DelegatingHealthCheck>>()));
        }

        /// <summary>
        /// Adds a <see cref="DelegatingHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="properties">The health check properties</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddDelegate(this IHealthCheckBuilder builder, DelegatingHealthCheckProperties properties)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            
            return builder.Add(
                p => new DelegatingHealthCheck(properties, p.GetService<ILogger<DelegatingHealthCheck>>()),
                ServiceLifetime.Singleton);
        }

        #endregion

        #region SQL

        /// <summary>
        /// Adds a <see cref="SqlHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="name">The health check name</param>
        /// <param name="connectionBuilder">The connection builder function</param>
        /// <param name="sql">The SQL to be executed agains the database. If null or empty, the connection will only be open.</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddSql(this IHealthCheckBuilder builder,
            string name, Func<DbConnection> connectionBuilder, string sql = null,
            bool required = false, params string[] tags)
        {
            return builder.AddSql(new SqlHealthCheckProperties(name, connectionBuilder, sql, required, tags));
        }

        /// <summary>
        /// Adds a <see cref="SqlHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="name">The health check name</param>
        /// <param name="connectionBuilder">The connection builder function</param>
        /// <param name="sql">The SQL to be executed agains the database. If null or empty, the connection will only be open.</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddSql(this IHealthCheckBuilder builder,
            string name, Func<IServiceProvider, DbConnection> connectionBuilder, string sql = null,
            bool required = false, params string[] tags)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (connectionBuilder == null) throw new ArgumentNullException(nameof(connectionBuilder));

            return builder.Add(p =>
                new SqlHealthCheck(
                    new SqlHealthCheckProperties(name, () => connectionBuilder(p), sql, required, tags),
                    p.GetService<ILogger<SqlHealthCheck>>()));
        }

        /// <summary>
        /// Adds a <see cref="SqlHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="properties">The health check properties</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddSql(this IHealthCheckBuilder builder, SqlHealthCheckProperties properties)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            return builder.Add(
                p => new SqlHealthCheck(properties, p.GetService<ILogger<SqlHealthCheck>>()),
                ServiceLifetime.Singleton);
        }

        #endregion

        #region HTTP

        /// <summary>
        /// Adds a <see cref="HttpHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="name">The health check name</param>
        /// <param name="url">The url address</param>
        /// <param name="timeoutInMs">The request timeout</param>
        /// <param name="ensureSuccessfulStatus">Ensure a successful HTTP status code of 2XX?</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddHttp(this IHealthCheckBuilder builder,
            string name, string url, int timeoutInMs = 5000, bool ensureSuccessfulStatus = true,
            bool required = false, params string[] tags)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddHttp(name, new Uri(url), timeoutInMs, ensureSuccessfulStatus, required, tags);
        }

        /// <summary>
        /// Adds a <see cref="HttpHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="name">The health check name</param>
        /// <param name="url">The url address</param>
        /// <param name="timeoutInMs">The request timeout</param>
        /// <param name="ensureSuccessfulStatus">Ensure a successful HTTP status code of 2XX?</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddHttp(this IHealthCheckBuilder builder,
            string name, Uri url, int timeoutInMs = 5000, bool ensureSuccessfulStatus = true,
            bool required = false, params string[] tags)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.AddHttp(new HttpHealthCheckProperties(name, url, timeoutInMs, ensureSuccessfulStatus, required, tags));
        }

        /// <summary>
        /// Adds a <see cref="HttpHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="properties">The health check properties</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddHttp(this IHealthCheckBuilder builder, HttpHealthCheckProperties properties)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            return builder.Add(
                p => new HttpHealthCheck(properties, p.GetService<ILogger<HttpHealthCheck>>()),
                ServiceLifetime.Singleton);
        }

        #endregion
    }
}