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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Health check wrapper to cache <see cref="HealthCheckStatus"/>.
    /// </summary>
    public class CachedHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="healthCheck">The health check to cache</param>
        /// <param name="cache">The memory cache</param>
        /// <param name="properties">The health check properties</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CachedHealthCheck(IHealthCheck healthCheck, IMemoryCache cache, CachedHealthCheckProperties properties, ILogger<CachedHealthCheck> logger = null)
        {
            HealthCheck = healthCheck ?? throw new ArgumentNullException(nameof(healthCheck));
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            Logger = logger ?? NullLogger<CachedHealthCheck>.Instance;

            CacheKey = nameof(CachedHealthCheck) + "->" + HealthCheck.Name;
        }

        /// <summary>
        /// The health check logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// The health check to cache
        /// </summary>
        protected IHealthCheck HealthCheck { get; }

        /// <summary>
        /// The memory cache
        /// </summary>
        protected IMemoryCache Cache { get; }

        /// <summary>
        /// The SQL health check properties
        /// </summary>
        protected CachedHealthCheckProperties Properties { get; }

        /// <summary>
        /// The cache key
        /// </summary>
        protected string CacheKey { get; }

        /// <inheritdoc />
        public string Name => HealthCheck.Name;

        /// <inheritdoc />
        public HealthCheckStatus Status { get; protected set; } = HealthCheckStatus.Green;

        /// <inheritdoc />
        public bool Required => HealthCheck.Required;

        /// <inheritdoc />
        public IReadOnlyCollection<string> Tags => HealthCheck.Tags;

        /// <inheritdoc />
        public async Task UpdateStatusAsync(CancellationToken ct)
        {
            Status = await Cache.GetOrCreateAsync(CacheKey, async k =>
            {
                Logger.LogDebug(
                    "Expired cache status. Updating to the most recent status [CacheExpiration:'{expiration}']...",
                    Properties.Expiration);

                k.AbsoluteExpirationRelativeToNow = Properties.Expiration;

                if (Properties.CacheExceptions)
                {
                    try
                    {
                        await HealthCheck.UpdateStatusAsync(ct);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e, "Inner health check failed but status 'Red' will be cached");
                        return HealthCheckStatus.Red;
                    }
                }
                else
                {
                    await HealthCheck.UpdateStatusAsync(ct);
                }

                return HealthCheck.Status;
            });
        }
    }
}
