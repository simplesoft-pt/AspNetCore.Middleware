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
using System.Reflection;

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// Metadata middleware options
    /// </summary>
    public class MetadataOptions : SimpleSoftMiddlewareOptions
    {
        private static readonly string DefaultName;
        private static readonly DateTimeOffset DefaultStartedOn;

        static MetadataOptions()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            DefaultName = entryAssembly.GetName().Name;
            DefaultStartedOn = DateTimeOffset.Now;
        }

        /// <summary>
        /// Path for which the middleware responds. Defaults to 'api/_meta'.
        /// </summary>
        public string Path { get; set; } = "api/_meta";

        /// <summary>
        /// Indent the JSON response? Defaults to 'true'.
        /// </summary>
        public bool IndentJson { get; set; } = true;

        /// <summary>
        /// Exclude null properties? Defaults to 'false'.
        /// </summary>
        public bool IncludeNullProperties { get; set; } = false;

        /// <summary>
        /// The API name. Defaults to entry assembly name.
        /// </summary>
        public string Name { get; set; } = DefaultName;

        /// <summary>
        /// The API environment. Defaults to 'null'.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The API startup date and time. Defaults to this class first static initialization date and time.
        /// </summary>
        public DateTimeOffset? StartedOn { get; set; } = DefaultStartedOn;

        /// <summary>
        /// The API version. Defaults to entry assembly file and product versions.
        /// </summary>
        public MetadataVersionOptions Version { get; set; } = new MetadataVersionOptions();
    }
}
