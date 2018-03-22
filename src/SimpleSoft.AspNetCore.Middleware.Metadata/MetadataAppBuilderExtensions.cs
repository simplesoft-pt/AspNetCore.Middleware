using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleSoft.AspNetCore.Middleware.Metadata;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/> instances.
    /// </summary>
    public static class MetadataAppBuilderExtensions
    {
        /// <summary>
        /// Registers the metadata middleware into the HTTP pipeline.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseMetadata(this IApplicationBuilder app)
        {
            return app.UseMetadata(
                app.ApplicationServices.GetService<IOptions<MetadataOptions>>() ??
                Options.Create(new MetadataOptions()));
        }

        /// <summary>
        /// Registers the metadata middleware into the HTTP pipeline.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="options">The middleware options</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseMetadata(this IApplicationBuilder app, MetadataOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            return app.UseMetadata(Options.Create(options));
        }

        private static IApplicationBuilder UseMetadata(this IApplicationBuilder app, IOptions<MetadataOptions> options)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var route = string.IsNullOrWhiteSpace(options.Value.Path) ? string.Empty : options.Value.Path;

            return app.UseRouter(r =>
            {
                r.MapMiddlewareRoute(route, builder =>
                {
                    builder.UseMiddleware<MetadataMiddleware>(options);
                });
            });
        }
    }
}
