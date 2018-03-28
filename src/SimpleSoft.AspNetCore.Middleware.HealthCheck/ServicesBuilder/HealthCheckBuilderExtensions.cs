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
        /// Adds a <see cref="DelegatingHealthCheck"/> to the services.
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="name">The health check name</param>
        /// <param name="action">The action to execute to get the health check status</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <returns>The builder after changes</returns>
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
        /// <param name="properties">The delegate properties</param>
        /// <returns>The builder after changes</returns>
        public static IHealthCheckBuilder AddDelegate(this IHealthCheckBuilder builder, DelegatingHealthCheckProperties properties)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            builder.Register(s =>
            {
                s.AddSingleton<IHealthCheck>(p =>
                    new DelegatingHealthCheck(properties, p.GetService<ILogger<DelegatingHealthCheck>>()));
            });
            return builder;
        }
    }
}