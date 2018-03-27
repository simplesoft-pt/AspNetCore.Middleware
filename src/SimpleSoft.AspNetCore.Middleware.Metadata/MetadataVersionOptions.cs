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
using System.Diagnostics;
using System.Reflection;

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// Version information
    /// </summary>
    public class MetadataVersionOptions
    {
        private static readonly uint DefaultMajor;
        private static readonly uint DefaultMinor;
        private static readonly uint DefaultPatch;
        private static readonly uint DefaultRevision;
        private static readonly string DefaultAlias;

        static MetadataVersionOptions()
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            try
            {
                var fvi = FileVersionInfo.GetVersionInfo(entryAssembly.Location);

                DefaultMajor = (uint) fvi.FileMajorPart;
                DefaultMinor = (uint) fvi.FileMinorPart;
                DefaultPatch = (uint) fvi.FileBuildPart;
                DefaultRevision = (uint) fvi.FilePrivatePart;
                DefaultAlias = fvi.ProductVersion;
            }
            catch (Exception)
            {
                DefaultMajor = DefaultMinor = DefaultPatch = DefaultRevision = 0;
                DefaultAlias = null;
            }
        }

        /// <summary>
        /// Major version. Defaults to entry assembly <see cref="FileVersionInfo.FileMajorPart"/>.
        /// </summary>
        public uint Major { get; set; } = DefaultMajor;

        /// <summary>
        /// Minor version. Defaults to entry assembly <see cref="FileVersionInfo.FileMinorPart"/>.
        /// </summary>
        public uint? Minor { get; set; } = DefaultMinor;

        /// <summary>
        /// Patch version. Defaults to entry assembly <see cref="FileVersionInfo.FileBuildPart"/>.
        /// </summary>
        public uint? Patch { get; set; } = DefaultPatch;

        /// <summary>
        /// Revision version. Defaults to entry assembly <see cref="FileVersionInfo.FilePrivatePart"/>.
        /// </summary>
        public uint? Revision { get; set; } = DefaultRevision;

        /// <summary>
        /// Version alias. Defaults to entry assembly <see cref="FileVersionInfo.ProductVersion"/>.
        /// </summary>
        public string Alias { get; set; } = DefaultAlias;
    }
}