using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SimpleSoft.AspNetCore.Middleware
{
    /// <summary>
    /// Base interface for all SimpleSoft middleware
    /// </summary>
    public interface ISimpleSoftMiddleware
    {
        /// <summary>
        /// Invoked by the HTTP pipeline.
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>A task to be awaited</returns>
        Task Invoke(HttpContext context);
    }
}