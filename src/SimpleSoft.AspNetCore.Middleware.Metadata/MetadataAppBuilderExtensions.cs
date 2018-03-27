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
