using System.Text;
using Microsoft.Net.Http.Headers;

namespace SimpleSoft.AspNetCore.Middleware
{
    /// <summary>
    /// Contains all helper singletons that can be used
    /// by the middleware
    /// </summary>
    public static class MiddlewareSingletons
    {
        /// <summary>
        /// String representation for JSON content type
        /// </summary>
        public static readonly string DefaultJsonContentType = new MediaTypeHeaderValue("application/json")
        {
            Encoding = Encoding.UTF8
        }.ToString();
    }
}