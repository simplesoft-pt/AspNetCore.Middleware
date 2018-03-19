using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected SimpleSoftMiddleware(RequestDelegate next, ILogger<SimpleSoftMiddleware> logger = null)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
            Logger = logger ?? (ILogger) NullLogger.Instance;
        }

        /// <summary>
        /// The request delegate
        /// </summary>
        protected RequestDelegate Next { get; }

        /// <summary>
        /// The middleware logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc />
        public abstract Task Invoke(HttpContext context);
    }
}
