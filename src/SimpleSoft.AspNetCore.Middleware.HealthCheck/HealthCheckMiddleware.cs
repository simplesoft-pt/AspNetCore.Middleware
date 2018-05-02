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
            : base(next, options, logger)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// The middleware options
        /// </summary>
        protected new HealthCheckOptions Options { get; }

        /// <inheritdoc />
        protected override async Task OnInvoke(HttpContext context)
        {
            Logger.LogDebug("Checking all health checks");

            var healthCheck = await RunHealthChecksAsync(
                context.RequestServices.GetServices<IHealthCheck>(), Options.ParallelExecution, context, context.RequestAborted);

            Logger.LogDebug("All health checks statuses have been updated");

            if (Options.BeforeSerialization != null)
                healthCheck = await Options.BeforeSerialization(context, healthCheck);

            context.Response.Clear();
            context.Response.StatusCode = healthCheck.Status == HealthCheckGlobalStatus.Red ? 500 : 200;

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Options.IndentJson ? Formatting.Indented : Formatting.None,
                ContractResolver = MiddlewareSingletons.CamelCaseResolver
            };
            if (Options.StringEnum)
                jsonSettings.Converters.Add(new StringEnumConverter(true));
            await context.Response.WriteJsonAsync(healthCheck, jsonSettings);
        }

        /// <summary>
        /// Runs the health checks
        /// </summary>
        /// <param name="healthChecks">The collection of health checks</param>
        /// <param name="parallelExecution">Run the health checks in parallel?</param>
        /// <param name="context">The HTTP context</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A task to be awaited for the result</returns>
        protected virtual async Task<HealthCheckModel> RunHealthChecksAsync(
            IEnumerable<IHealthCheck> healthChecks, bool parallelExecution, HttpContext context, CancellationToken ct)
        {
            var result = new HealthCheckModel
            {
                Status = HealthCheckGlobalStatus.Green,
                ServerStartedOn = Options.ServerStartedOn,
                Dependencies = new Dictionary<string, HealthCheckDependencyModel>(StringComparer.OrdinalIgnoreCase)
            };

            if (parallelExecution)
            {
                var tasks = healthChecks.Select(healthCheck => RunAsync(healthCheck, ct)).ToList();

                Logger.LogDebug("Running all health checks in parallel");
                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                {
                    var dependency = task.Result.Value;
                    result.Dependencies.Add(task.Result.Key, dependency);

                    if (dependency.Status == HealthCheckStatus.Red)
                    {
                        if (dependency.Required)
                            result.Status = HealthCheckGlobalStatus.Red;
                        else if(result.Status == HealthCheckGlobalStatus.Green)
                            result.Status = HealthCheckGlobalStatus.Yellow;
                    }
                }
            }
            else
            {
                foreach (var healthCheck in healthChecks)
                {
                    var dependency = await RunAsync(healthCheck, ct);
                    result.Dependencies.Add(dependency.Key, dependency.Value);
                }
            }

            return result;
        }

        private async Task<KeyValuePair<string, HealthCheckDependencyModel>> RunAsync(IHealthCheck healthCheck, CancellationToken ct)
        {
            using (Logger.BeginScope("HealthCheckName:'{name}'", healthCheck.Name))
            {
                Logger.LogDebug("Performing an health check");

                HealthCheckStatus status;
                try
                {
                    await healthCheck.UpdateStatusAsync(ct);
                    status = healthCheck.Status;

                    if (Logger.IsEnabled(LogLevel.Information))
                        Logger.LogInformation("Health check status '{status}'", status.ToString("G"));
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Health check failed");
                    status = HealthCheckStatus.Red;
                }

                return new KeyValuePair<string, HealthCheckDependencyModel>(
                    healthCheck.Name, new HealthCheckDependencyModel
                    {
                        Status = status,
                        Required = healthCheck.Required,
                        Tags = healthCheck.Tags.ToArray()
                    });
            }
        }
    }
}