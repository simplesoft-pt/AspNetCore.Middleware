using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Base class for <see cref="IHealthCheck"/> implementations.
    /// </summary>
    public abstract class HealthCheck : IHealthCheck
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name">The health check name</param>
        /// <param name="logger">The health check logger</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected HealthCheck(string name, ILogger<HealthCheck> logger = null, bool required = false, params string[] tags)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Logger = logger ?? NullLogger<HealthCheck>.Instance;
            Required = required;
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        }

        /// <summary>
        /// The health check logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public HealthCheckStatus Status { get; protected set; } = HealthCheckStatus.Green;

        /// <inheritdoc />
        public bool Required { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Tags { get; }

        /// <inheritdoc />
        public virtual async Task UpdateStatusAsync(CancellationToken ct)
        {
            using (Logger.BeginScope("CheckId:'{checkId}'", Guid.NewGuid().ToString("N")))
            {
                try
                {
                    Logger.LogDebug("Updating health check status");
                    Status = await OnUpdateStatusAsync(ct);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Health check failed");
                    Status = HealthCheckStatus.Red;
                }
            }
        }

        /// <summary>
        /// Invoked to calculate the current health check status.
        /// Exceptions thrown will be catch by the underline implementation.
        /// </summary>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A task to be awaited for the result</returns>
        public abstract Task<HealthCheckStatus> OnUpdateStatusAsync(CancellationToken ct);
    }
}