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
using Newtonsoft.Json;
using SimpleSoft.AspNetCore.Middleware;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// Extension methods for <see cref="HttpResponse"/> instances.
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Serializes and writes the given object as a JSON into the HTTP response.
        /// </summary>
        /// <param name="response">The response to be used</param>
        /// <param name="value">The value to be serialized</param>
        /// <param name="indent">Indent the JSON?</param>
        /// <returns>A task to be awaited</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task WriteJsonAsync(this HttpResponse response, object value, bool indent = false)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            response.ContentType = MiddlewareSingletons.DefaultJsonContentType;
            return response.WriteAsync(JsonConvert.SerializeObject(
                value, indent ? Formatting.Indented : Formatting.None));
        }

        /// <summary>
        /// Serializes and writes the given object as a JSON into the HTTP response.
        /// </summary>
        /// <param name="response">The response to be used</param>
        /// <param name="value">The value to be serialized</param>
        /// <param name="settings">The serializer settings to be used</param>
        /// <returns>A task to be awaited</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task WriteJsonAsync(this HttpResponse response, object value, JsonSerializerSettings settings)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            response.ContentType = MiddlewareSingletons.DefaultJsonContentType;
            return response.WriteAsync(JsonConvert.SerializeObject(value, settings));
        }
    }
}