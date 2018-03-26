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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace SimpleSoft.AspNetCore.Middleware
{
    /// <summary>
    /// Base class for all SimpleSoft middleware
    /// </summary>
    public abstract class SimpleSoftMiddleware : ISimpleSoftMiddleware
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The request delegate</param>
        /// <param name="options">The middleware options</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected SimpleSoftMiddleware(RequestDelegate next, 
            IOptions<SimpleSoftMiddlewareOptions> options, ILogger<SimpleSoftMiddleware> logger = null)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            Logger = logger ?? (ILogger) NullLogger.Instance;
        }

        /// <summary>
        /// The request delegate
        /// </summary>
        protected RequestDelegate Next { get; }

        /// <summary>
        /// The middleware options.
        /// </summary>
        protected virtual SimpleSoftMiddlewareOptions Options { get; }

        /// <summary>
        /// The middleware logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc />
        public virtual async Task Invoke(HttpContext context)
        {
            if (Options.BeforeInvoke != null)
                await Options.BeforeInvoke(context);

            if (context.Response.HasStarted || context.RequestAborted.IsCancellationRequested)
            {
                Logger.LogWarning(
                    "Middleware will not be executed [Response.HasStarted={responseHasStarted} Request.Aborted={requestAborted}]",
                    context.Response.HasStarted, context.RequestAborted.IsCancellationRequested);
                return;
            }

            Logger.LogDebug("Invoking the middleware logic");
            await OnInvoke(context);
        }

        /// <summary>
        /// Invoked by the HTTP pipeline only if allowed by the
        /// <see cref="SimpleSoftMiddlewareOptions.BeforeInvoke"/> delegate.
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>A task to be awaited</returns>
        protected virtual Task OnInvoke(HttpContext context) => Next(context);
    }
}
