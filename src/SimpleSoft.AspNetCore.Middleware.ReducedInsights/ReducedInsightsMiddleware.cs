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
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SimpleSoft.AspNetCore.Middleware.ReducedInsights
{
    /// <summary>
    /// Insights middleware
    /// </summary>
    public class ReducedInsightsMiddleware : SimpleSoftMiddleware
    {
        private readonly string _insightsPath;
        private (string Key, int StatusCode, long ElapsedMs)? _lastRequest;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The request delegate</param>
        /// <param name="options">The middleware options</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ReducedInsightsMiddleware(RequestDelegate next, 
            IOptions<ReducedInsightsOptions> options, ILogger<ReducedInsightsMiddleware> logger = null) 
            : base(next, options, logger)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            _insightsPath = string.IsNullOrWhiteSpace(Options.Path)
                ? "/"
                : (Options.Path.StartsWith("/") ? Options.Path : "/" + Options.Path);
        }

        /// <summary>
        /// The middleware options
        /// </summary>
        protected new ReducedInsightsOptions Options { get; }

        /// <inheritdoc />
        protected override async Task OnInvoke(HttpContext context)
        {
            if (IsInsightsRequest(context))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteJsonAsync(new
                {
                    LastRequest = _lastRequest == null
                        ? null
                        : new
                        {
                            _lastRequest.Value.Key,
                            _lastRequest.Value.StatusCode,
                            _lastRequest.Value.ElapsedMs
                        }
                }).ConfigureAwait(false);
            }
            else
            {
                var sw = new Stopwatch();
                try
                {
                    sw.Start();
                    await Task.Delay(new Random().Next(0, 100));
                    await Next(context).ConfigureAwait(false);
                }
                finally
                {
                    sw.Stop();
                    var key = context.Request.Method + "->" + context.Request.Path;
                    _lastRequest = (key, context.Response.StatusCode, sw.ElapsedMilliseconds);
                }
            }
        }

        private bool IsInsightsRequest(HttpContext context)
        {
            if ("GET" == context.Request.Method)
            {
                var requestPath = context.Request.Path;
                return requestPath.HasValue
                    ? requestPath.Value.Equals(_insightsPath, StringComparison.OrdinalIgnoreCase)
                    : "/".Equals(_insightsPath);
            }

            return false;
        }
    }
}