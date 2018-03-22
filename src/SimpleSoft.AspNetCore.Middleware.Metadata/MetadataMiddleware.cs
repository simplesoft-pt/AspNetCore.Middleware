using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// Metadata middleware
    /// </summary>
    public class MetadataMiddleware : SimpleSoftMiddleware
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The request delegate</param>
        /// <param name="options">The middleware options</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MetadataMiddleware(RequestDelegate next,
            IOptions<MetadataOptions> options, ILogger<MetadataMiddleware> logger = null) 
            : base(next, logger)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            Options = options.Value;
        }

        /// <summary>
        /// The middleware options
        /// </summary>
        protected MetadataOptions Options { get; }

        /// <inheritdoc />
        public override Task Invoke(HttpContext context)
        {
            if (context.Response.HasStarted)
            {
                Logger.LogWarning("The response has already started, the middleware will not be executed.");
                return Task.CompletedTask;
            }

            Logger.LogDebug("Returning application metadata");

            var metadata = GetMetadata(context);

            context.Response.Clear();
            context.Response.StatusCode = 200;

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Options.IndentJson ? Formatting.Indented : Formatting.None,
                NullValueHandling = Options.IncludeNullProperties ? NullValueHandling.Include : NullValueHandling.Ignore
            };
            return context.Response.WriteJsonAsync(metadata, jsonSettings);
        }

        /// <summary>
        /// Gets the medatata model that will be serialized
        /// as the JSON response.
        /// </summary>
        /// <returns>The metadata instance</returns>
        protected virtual MetadataModel GetMetadata(HttpContext context)
        {
            var version = Options.Version == null
                ? null
                : new MetadataVersionModel(
                    Options.Version.Major, Options.Version.Minor, Options.Version.Patch,
                    Options.Version.Revision, Options.Version.Alias);

            return new MetadataModel(Options.Name, Options.Environment, Options.StartedOn, version);
        }
    }
}
