using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
        /// <param name="healthChecks">The collection of health checks</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HealthCheckMiddleware(RequestDelegate next, 
            IEnumerable<IHealthCheck> healthChecks, ILogger<HealthCheckMiddleware> logger = null) 
            : base(next, logger)
        {
            _healthChecks = healthChecks?.ToArray() ?? throw new ArgumentNullException(nameof(healthChecks));
        }

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
                using (Logger.BeginScope("Name:'{name}'", healthCheck.Name))
                {
                    await healthCheck.UpdateStatusAsync(context.RequestAborted);

                    if (healthCheck.Status == HealthCheckStatus.Red)
                    {
                        if (healthCheck.Required)
                            result.Status = HealthCheckGlobalStatus.Red;
                        else if (result.Status == HealthCheckGlobalStatus.Green)
                            result.Status = HealthCheckGlobalStatus.Yellow;
                    }

                    result.Dependencies.Add(healthCheck.Name, new HealthCheckDependencyModel
                    {
                        Status = healthCheck.Status,
                        Required = healthCheck.Required,
                        Tags = healthCheck.Tags.ToArray()
                    });
                }
            }

            Logger.LogDebug("All health checks statuses have been updated");

            result.TerminatedOn = DateTimeOffset.Now;

            context.Response.Clear();
            context.Response.StatusCode = result.Status == HealthCheckGlobalStatus.Red ? 500 : 200;
            await context.Response.WriteJsonAsync(result, true);
        }
    }
}