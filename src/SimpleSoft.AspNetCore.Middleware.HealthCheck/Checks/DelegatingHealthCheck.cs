using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Health check thar invokes a funcion to calculate the current <see cref="HealthCheckStatus"/>.
    /// Exceptions thrown by the function will set the status to <see cref="HealthCheckStatus.Red"/>.
    /// </summary>
    public class DelegatingHealthCheck : HealthCheck
    {
        private readonly Func<CancellationToken, Task<HealthCheckStatus>> _action;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name">The health check name</param>
        /// <param name="action">The action invoked to calculate the current status</param>
        /// <param name="logger">An optional logger instance</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DelegatingHealthCheck(
            string name, Func<CancellationToken, Task<HealthCheckStatus>> action,
            ILogger<DelegatingHealthCheck> logger = null, bool required = false, params string[] tags) 
            : base(name, logger, required, tags)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <inheritdoc />
        public override Task<HealthCheckStatus> OnUpdateStatusAsync(CancellationToken ct) => _action(ct);
    }
}