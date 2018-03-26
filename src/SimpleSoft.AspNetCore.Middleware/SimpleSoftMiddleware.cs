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

            if (context.Response.HasStarted)
            {
                Logger.LogWarning("The response has already started, the middleware will not be executed.");
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
