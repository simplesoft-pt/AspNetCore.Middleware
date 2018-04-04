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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    public static partial class HealthCheckBuilderExtensions
    {
        /// <summary>
        /// Adds a cache status wrapper to the contained registrations
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="cachedBuilder">The builder that will register health checks to be cached</param>
        /// <param name="expiration">The cache expiration</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddCached(this IHealthCheckBuilder builder,
            Action<IHealthCheckBuilder> cachedBuilder, TimeSpan expiration)
        {
            return builder.AddCached(cachedBuilder, new CachedHealthCheckProperties(expiration));
        }

        /// <summary>
        /// Adds a cache status wrapper to the contained registrations
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="cachedBuilder">The builder that will register health checks to be cached</param>
        /// <param name="properties">The cache properties</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddCached(this IHealthCheckBuilder builder, 
            Action<IHealthCheckBuilder> cachedBuilder, CachedHealthCheckProperties properties)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (cachedBuilder == null) throw new ArgumentNullException(nameof(cachedBuilder));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var cfg = new HealthCheckBuilder();
            cachedBuilder(cfg);

            foreach (var descriptor in cfg.Descriptors)
            {
                builder.Add(
                    p => new CachedHealthCheck(
                        descriptor.Factory(p),
                        p.GetRequiredService<IMemoryCache>(),
                        properties,
                        p.GetService<ILogger<CachedHealthCheck>>()),
                    descriptor.Lifetime);
            }

            return builder;
        }

        /// <summary>
        /// Adds a cache status wrapper to the contained registrations
        /// </summary>
        /// <param name="builder">The health check builder</param>
        /// <param name="cachedBuilder">The builder that will register health checks to be cached</param>
        /// <param name="propertiesBuilder">The cache properties builder</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IHealthCheckBuilder AddCached(this IHealthCheckBuilder builder, 
            Action<IHealthCheckBuilder> cachedBuilder, Func<IServiceProvider, CachedHealthCheckProperties> propertiesBuilder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (cachedBuilder == null) throw new ArgumentNullException(nameof(cachedBuilder));
            if (propertiesBuilder == null) throw new ArgumentNullException(nameof(propertiesBuilder));

            var cfg = new HealthCheckBuilder();
            cachedBuilder(cfg);

            foreach (var descriptor in cfg.Descriptors)
            {
                builder.Add(
                    p => new CachedHealthCheck(
                        descriptor.Factory(p),
                        p.GetRequiredService<IMemoryCache>(),
                        propertiesBuilder(p),
                        p.GetService<ILogger<CachedHealthCheck>>()),
                    descriptor.Lifetime);
            }

            return builder;
        }
    }
}