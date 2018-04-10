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
                StartedOn = DateTimeOffset.Now,
                Dependencies = new Dictionary<string, HealthCheckDependencyModel>()
            };

            if (parallelExecution)
            {
                var tasks = healthChecks.Select(async healthCheck =>
                {
                    var status = await CalculateStatusAsync(healthCheck, context, ct);
                    return new
                    {
                        healthCheck.Name,
                        Value = new HealthCheckDependencyModel
                        {
                            Status = status,
                            Required = healthCheck.Required,
                            Tags = healthCheck.Tags.ToArray()
                        }
                    };
                }).ToList();

                Logger.LogDebug("Running all health checks in parallel");
                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                    result.Dependencies.Add(task.Result.Name, task.Result.Value);
            }
            else
            {
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