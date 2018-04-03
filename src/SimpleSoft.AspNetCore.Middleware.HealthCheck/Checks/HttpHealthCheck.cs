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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Health check for HTTP endpoints.
    /// </summary>
    public class HttpHealthCheck : HealthCheck
    {
        private static readonly Lazy<HttpClient> LazyHttpClient = new Lazy<HttpClient>(() => new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan
        });
        private static HttpClient HttpClient => LazyHttpClient.Value;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="properties">The health check properties</param>
        /// <param name="logger">An optional logger instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpHealthCheck(HttpHealthCheckProperties properties, ILogger<HttpHealthCheck> logger = null) 
            : base(properties, logger)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <summary>
        /// The HTTP health check properties
        /// </summary>
        protected new HttpHealthCheckProperties Properties { get; }

        /// <inheritdoc />
        public override async Task<HealthCheckStatus> OnUpdateStatusAsync(CancellationToken ct)
        {
            using (Logger.BeginScope("Url:'{url}'", Properties.Url))
            {
                Logger.LogDebug("Performing health check");

                using (var response = await RequestEndpointAsync(ct))
                {
                    Logger.LogDebug("Response status ", response.StatusCode);

                    if (Properties.EnsureSuccessfulStatus)
                        response.EnsureSuccessStatusCode();
                }

                Logger.LogDebug("Endpoint was successful reached");
            }

            return HealthCheckStatus.Green;
        }

        private async Task<HttpResponseMessage> RequestEndpointAsync(CancellationToken ct)
        {
            using (var cts = new CancellationTokenSource(Properties.TimeoutInMs))
            using (ct.Register(() =>
            {
                try
                {
                    // ReSharper disable once AccessToDisposedClosure
                    cts.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    Logger.LogWarning(
                        "CancellationTokenSource is already disposed, but it shouldn't be a problem since it means the request has already ended");
                }
            }))
            using (var request = new HttpRequestMessage(HttpMethod.Get, Properties.Url))
            {
                try
                {
                    return await HttpClient.SendAsync(request, cts.Token);
                }
                catch (TaskCanceledException e)
                {
                    if (ct.IsCancellationRequested)
                        throw;
                    throw new TimeoutException(
                        $"The endpoint '{Properties.Url}' too more than {Properties.TimeoutInMs} ms to send a response",
                        e);
                }
            }
        }
    }
}
