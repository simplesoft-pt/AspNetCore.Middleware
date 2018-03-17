using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// Metadata middleware
    /// </summary>
    public class MetadataMiddleware : SimpleSoftMiddleware
    {
        private readonly MetadataOptions _options;
        private readonly ILogger<MetadataMiddleware> _logger;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The request delegate</param>
        /// <param name="options">The middleware options</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MetadataMiddleware(RequestDelegate next,
            IOptions<MetadataOptions> options, ILogger<MetadataMiddleware> logger = null) 
            : base(next)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _options = options.Value;
            _logger = logger ?? NullLogger<MetadataMiddleware>.Instance;
        }

        /// <inheritdoc />
        public override Task Invoke(HttpContext context)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("The response has already started, the middleware will not be executed.");
                return Task.CompletedTask;
            }

            _logger.LogDebug("Returning application metadata");

            var metadata = new
            {
                _options.Name,
                _options.Environment,
                _options.StartedOn,
                Version = new
                {
                    _options.Version?.Major,
                    _options.Version?.Minor,
                    _options.Version?.Patch,
                    _options.Version?.Revision,
                    _options.Version?.Alias,
                }
            };

            context.Response.Clear();
            context.Response.StatusCode = 200;
            return context.Response.WriteJsonAsync(metadata, _options.IndentJson);
        }
    }
}
