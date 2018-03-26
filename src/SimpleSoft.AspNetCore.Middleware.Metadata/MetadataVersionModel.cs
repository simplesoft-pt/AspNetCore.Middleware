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

namespace SimpleSoft.AspNetCore.Middleware.Metadata
{
    /// <summary>
    /// The metadata version model
    /// </summary>
    public class MetadataVersionModel
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        /// <param name="revision"></param>
        /// <param name="alias"></param>
        public MetadataVersionModel(uint major, uint? minor = null, uint? patch = null, uint? revision = null, string alias = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            Alias = alias;
        }

        /// <summary>
        /// Major version
        /// </summary>
        public uint Major { get; }

        /// <summary>
        /// Minor version
        /// </summary>
        public uint? Minor { get; }

        /// <summary>
        /// Patch version
        /// </summary>
        public uint? Patch { get; }

        /// <summary>
        /// Revision version
        /// </summary>
        public uint? Revision { get; }

        /// <summary>
        /// Version alias
        /// </summary>
        public string Alias { get; }
    }
}