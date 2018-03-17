using System;
using Microsoft.AspNetCore.Http;

namespace SimpleSoft.AspNetCore.Middleware
{
    /// <summary>
    /// Base class for all SimpleSoft middleware
    /// </summary>
    public class SimpleSoftMiddleware
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The request delegate</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SimpleSoftMiddleware(RequestDelegate next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// The request delegate
        /// </summary>
        protected RequestDelegate Next { get; }
    }
}
