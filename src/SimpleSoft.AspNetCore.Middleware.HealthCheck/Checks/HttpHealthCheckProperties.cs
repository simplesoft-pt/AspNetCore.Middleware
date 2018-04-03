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

// ReSharper disable once CheckNamespace
namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// The HTTP health check properties.
    /// </summary>
    public class HttpHealthCheckProperties : HealthCheckProperties
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name">The health check name</param>
        /// <param name="url">The url address</param>
        /// <param name="timeoutInMs">The request timeout</param>
        /// <param name="ensureSuccessfulStatus">Ensure a successful HTTP status code of 2XX?</param>
        /// <param name="required">Is the health check required?</param>
        /// <param name="tags">The collection of tags</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpHealthCheckProperties(
            string name, Uri url, int timeoutInMs = 5000, bool ensureSuccessfulStatus = true,
            bool required = false, params string[] tags)
            : base(name, required, tags)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            TimeoutInMs = timeoutInMs;
            EnsureSuccessfulStatus = ensureSuccessfulStatus;
            Tags.Add("http");
        }

        /// <summary>
        /// The url address
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        /// The request timeout
        /// </summary>
        public int TimeoutInMs { get; }

        /// <summary>
        /// Ensure a successful HTTP status code of 2XX?
        /// </summary>
        public bool EnsureSuccessfulStatus { get; }
    }
}