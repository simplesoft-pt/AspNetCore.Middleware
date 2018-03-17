using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SimpleSoft.AspNetCore.Middleware
{
    /// <summary>
    /// Base class for all SimpleSoft middleware
    /// </summary>
    public abstract class SimpleSoftMiddleware : ISimpleSoftMiddleware
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="next">The request delegate</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected SimpleSoftMiddleware(RequestDelegate next)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// The request delegate
        /// </summary>
        protected RequestDelegate Next { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task Invoke(HttpContext context);
    }
}
