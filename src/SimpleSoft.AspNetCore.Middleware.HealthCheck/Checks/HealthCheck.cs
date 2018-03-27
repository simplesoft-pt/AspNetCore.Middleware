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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Base class for <see cref="IHealthCheck"/> implementations.
    /// </summary>
    public abstract class HealthCheck : IHealthCheck
    {
        private static readonly Task<HealthCheckStatus> CachedHealthCheckStatusTask =
            Task.FromResult(HealthCheckStatus.Green);

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name">The health check name</param>
        /// <param name="logger">An optional logger instance</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected HealthCheck(string name, ILogger<HealthCheck> logger = null, bool required = false, params string[] tags)
            : this(new HealthCheckProperties(name, required, tags), logger)
        {

        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="properties">The health check properties</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected HealthCheck(HealthCheckProperties properties, ILogger<HealthCheck> logger = null)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            Logger = logger ?? NullLogger<HealthCheck>.Instance;
        }

        /// <summary>
        /// The health check properties
        /// </summary>
        protected virtual HealthCheckProperties Properties { get; }

        /// <summary>
        /// The health check logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc />
        public string Name => Properties.Name;

        /// <inheritdoc />
        public HealthCheckStatus Status { get; protected set; } = HealthCheckStatus.Green;

        /// <inheritdoc />
        public bool Required => Properties.Required;

        /// <inheritdoc />
        public IReadOnlyCollection<string> Tags => Properties.Tags;

        /// <inheritdoc />
        public virtual async Task UpdateStatusAsync(CancellationToken ct) => Status = await OnUpdateStatusAsync(ct);

        /// <summary>
        /// Invoked to calculate the current health check status.
        /// </summary>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A task to be awaited for the result</returns>
        public virtual Task<HealthCheckStatus> OnUpdateStatusAsync(CancellationToken ct) => CachedHealthCheckStatusTask;

        /// <summary>
        /// The health check options
        /// </summary>
        public class HealthCheckProperties
        {
            private string[] _tags;

            /// <summary>
            /// Creates a new instance
            /// </summary>
            /// <param name="name">The health check name</param>
            /// <param name="required">Is the health check required?</param>
            /// <param name="tags">The collection of tags</param>
            /// <exception cref="ArgumentNullException"></exception>
            public HealthCheckProperties(string name, bool required = false, params string[] tags)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Required = required;
                Tags = tags ?? throw new ArgumentNullException(nameof(name));
            }

            /// <summary>
            /// Health check name
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Is the health check required? Defaults to false,
            /// </summary>
            public bool Required { get; set; }

            /// <summary>
            /// The collection of tags. Defaults to empty array.
            /// </summary>
            public string[] Tags
            {
                get => _tags;
                set => _tags = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }
}