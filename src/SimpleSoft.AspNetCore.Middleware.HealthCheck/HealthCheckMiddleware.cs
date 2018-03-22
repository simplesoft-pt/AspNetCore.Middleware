using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// The health check middleware
    /// </summary>
    public class HealthCheckMiddleware : SimpleSoftMiddleware
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The request delegate</param>
        /// <param name="options">The middleware options</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HealthCheckMiddleware(RequestDelegate next, 
            IOptions<HealthCheckOptions> options, ILogger<HealthCheckMiddleware> logger = null) 
            : base(next, logger)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            Options = options.Value;
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

            var healthCheck = await RunHealthChecksAsync(
                context.RequestServices.GetServices<IHealthCheck>(), context, context.RequestAborted);

            Logger.LogDebug("All health checks statuses have been updated");

            context.Response.Clear();
            context.Response.StatusCode = healthCheck.Status == HealthCheckGlobalStatus.Red ? 500 : 200;

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Options.IndentJson ? Formatting.Indented : Formatting.None
            };
            if (Options.StringEnum)
                jsonSettings.Converters.Add(new StringEnumConverter(true));
            await context.Response.WriteJsonAsync(healthCheck, jsonSettings);
        }

        /// <summary>
        /// Runs the health checks
        /// </summary>
        /// <param name="healthChecks">The collection of health checks</param>
        /// <param name="context">The HTTP context</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A task to be awaited for the result</returns>
        protected virtual async Task<HealthCheckModel> RunHealthChecksAsync(IEnumerable<IHealthCheck> healthChecks, HttpContext context, CancellationToken ct)
        {
            var result = new HealthCheckModel
            {
                Status = HealthCheckGlobalStatus.Green,
                StartedOn = DateTimeOffset.Now,
                Dependencies = new Dictionary<string, HealthCheckDependencyModel>()
            };
            foreach (var healthCheck in healthChecks)
            {
                using (Logger.BeginScope("Name:'{name}'", healthCheck.Name))
                {
                    Logger.LogDebug("Performing an health check");

                    var status = await CalculateStatusAsync(healthCheck, context, ct);
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
            }

            result.TerminatedOn = DateTimeOffset.Now;

            return result;
        }

        /// <summary>
        /// Calculates the status for a given health check. It is also expected to handle
        /// exceptions thrown by the health check.
        /// </summary>
        /// <param name="healthCheck">The health check</param>
        /// <param name="context">The HTTP context</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A task to be awaited for the result</returns>
        protected virtual async Task<HealthCheckStatus> CalculateStatusAsync(IHealthCheck healthCheck, HttpContext context, CancellationToken ct)
        {
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