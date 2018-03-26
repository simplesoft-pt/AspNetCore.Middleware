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

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// The medatata model
    /// </summary>
    public class MetadataModel
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="environment"></param>
        /// <param name="startedOn"></param>
        /// <param name="version"></param>
        public MetadataModel(string name, string environment, DateTimeOffset? startedOn = null, MetadataVersionModel version = null)
        {
            Name = name;
            Environment = environment;
            StartedOn = startedOn;
            Version = version;
        }

        /// <summary>
        /// The API name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The API environment
        /// </summary>
        public string Environment { get; }

        /// <summary>
        /// The API startup date and time
        /// </summary>
        public DateTimeOffset? StartedOn { get; }

        /// <summary>
        /// The API version
        /// </summary>
        public MetadataVersionModel Version { get; }
    }
}