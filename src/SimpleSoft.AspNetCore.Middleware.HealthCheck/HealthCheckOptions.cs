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

namespace SimpleSoft.AspNetCore.Middleware.HealthCheck
{
    /// <summary>
    /// Health check middleware options
    /// </summary>
    public class HealthCheckOptions : SimpleSoftMiddlewareOptions
    {
        private static readonly DateTimeOffset DefaultServerStartedOn = DateTimeOffset.Now;

        /// <summary>
        /// Path for which the middleware responds. Defaults to 'api/_health'.
        /// </summary>
        public string Path { get; set; } = "api/_health";

        /// <summary>
        /// Indent the JSON response? Defaults to 'true'.
        /// </summary>
        public bool IndentJson { get; set; } = true;

        /// <summary>
        /// Should enums be returned as strings? Defaults to 'true'.
        /// </summary>
        public bool StringEnum { get; set; } = true;

        /// <summary>
        /// Run the health checks in parallel? Defaults to 'true'.
        /// </summary>
        public bool ParallelExecution { get; set; } = true;

        /// <summary>
        /// The API startup date and time. Defaults to this class first static initialization date and time.
        /// </summary>
        public DateTimeOffset ServerStartedOn { get; set; } = DefaultServerStartedOn;

        /// <summary>
        /// Allows the manipulation of the health check result before beeing
        /// serialized and returned by the middleware. Defaults to 'null'.
        /// </summary>
        public Func<HttpContext, HealthCheckModel, Task<HealthCheckModel>> BeforeSerialization { get; set; }
    }
}