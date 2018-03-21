using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleSoft.AspNetCore.Middleware.HealthCheck;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/> instances.
    /// </summary>
    public static class HealthCheckAppBuilderExtensions
    {
        /// <summary>
        /// Registers the health check middleware into the HTTP pipeline.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app)
        {
            return app.UseHealthCheck(app.ApplicationServices.GetService<IOptions<HealthCheckOptions>>());
        }

        /// <summary>
        /// Registers the health check middleware into the HTTP pipeline.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="options">The middleware options</param>
        /// <returns>The builder after changes</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, HealthCheckOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            return app.UseHealthCheck(Options.Create(options));
        }

        private static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, IOptions<HealthCheckOptions> options)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var route = string.IsNullOrWhiteSpace(options.Value.Path) ? string.Empty : options.Value.Path;

            return app.UseRouter(r =>
            {
                r.MapMiddlewareRoute(route, builder =>
                {
                    builder.UseMiddleware<HealthCheckMiddleware>(options);
                });
            });
        }
    }
}
