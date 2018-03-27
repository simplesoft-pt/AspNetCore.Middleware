using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SimpleSoft.AspNetCore.Middleware
{
    /// <summary>
    /// Middleware options
    /// </summary>
    public class SimpleSoftMiddlewareOptions
    {
        /// <summary>
        /// Invoked before the delegate logic. If a response written or the request is aborted,
        /// the middleware will stop the execution.
        /// </summary>
        public Func<HttpContext, Task> BeforeInvoke { get; set; }
    }
}