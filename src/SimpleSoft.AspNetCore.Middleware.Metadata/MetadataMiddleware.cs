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
            : base(next, options, logger)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// The middleware options
        /// </summary>
        protected new MetadataOptions Options { get; }

        /// <inheritdoc />
        protected override Task OnInvoke(HttpContext context)
        {
            Logger.LogDebug("Returning application metadata");

            var metadata = GetMetadata(context);

            context.Response.Clear();
            context.Response.StatusCode = 200;

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Options.IndentJson ? Formatting.Indented : Formatting.None,
                NullValueHandling = Options.IncludeNullProperties ? NullValueHandling.Include : NullValueHandling.Ignore,
                ContractResolver = MiddlewareSingletons.CamelCaseResolver
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
