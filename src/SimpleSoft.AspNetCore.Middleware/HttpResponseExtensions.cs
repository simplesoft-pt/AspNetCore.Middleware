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