using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// The health check middleware
    /// </summary>
    public class HealthCheckMiddleware : SimpleSoftMiddleware
    {
        private readonly IHealthCheck[] _healthChecks;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The request delegate</param>
        /// <param name="options"></param>
        /// <param name="healthChecks">The collection of health checks</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HealthCheckMiddleware(RequestDelegate next, 
            IOptions<HealthCheckOptions> options, IEnumerable<IHealthCheck> healthChecks, 
            ILogger<HealthCheckMiddleware> logger = null) 
            : base(next, logger)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            Options = options.Value;
            _healthChecks = healthChecks?.ToArray() ?? throw new ArgumentNullException(nameof(healthChecks));
        }

        /// <summary>
        /// The middleware options
        /// </summary>
        protected HealthCheckOptions Options { get; }

        /// <inheritdoc />
        public override async Task Invoke(HttpContext context)
        {
            if (context.Response.HasStarted)
            {
                Logger.LogWarning("The response has already started, the middleware will not be executed.");
                return;
            }

            Logger.LogDebug("Checking all health checks");

            var result = new HealthCheckModel
            {
                Status = HealthCheckGlobalStatus.Green,
                StartedOn = DateTimeOffset.Now,
                Dependencies = new Dictionary<string, HealthCheckDependencyModel>()
            };
            foreach (var healthCheck in _healthChecks)
            {
                var status = await CalculateStatusAsync(healthCheck, context.RequestAborted);
                if (status == HealthCheckStatus.Red)
                {
                    if (healthCheck.Required)
                        result.Status = HealthCheckGlobalStatus.Red;
                    else if (result.Status == HealthCheckGlobalStatus.Green)
                        result.Status = HealthCheckGlobalStatus.Yellow;
                }

                result.Dependencies.Add(healthCheck.Name, new HealthCheckDependencyModel
                {
                    Status = status,
                    Required = healthCheck.Required,
                    Tags = healthCheck.Tags.ToArray()
                });
            }

            Logger.LogDebug("All health checks statuses have been updated");

            result.TerminatedOn = DateTimeOffset.Now;

            context.Response.Clear();
            context.Response.StatusCode = result.Status == HealthCheckGlobalStatus.Red ? 500 : 200;
            await context.Response.WriteJsonAsync(result, Options.IndentJson);
        }

        /// <summary>
        /// Calculates the status for a given health check. It is also expected to handle
        /// exceptions thrown.
        /// </summary>
        /// <param name="healthCheck">The health check</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A task to be awaited for the result</returns>
        protected virtual async Task<HealthCheckStatus> CalculateStatusAsync(IHealthCheck healthCheck, CancellationToken ct)
        {
            using (Logger.BeginScope("Name:'{name}'", healthCheck.Name))
            {
                Logger.LogDebug("Performing an health check");

                try
                {
                    await healthCheck.UpdateStatusAsync(ct);
                    return healthCheck.Status;
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Health check failed");
                    return HealthCheckStatus.Red;
                }
            }
        }
    }
}