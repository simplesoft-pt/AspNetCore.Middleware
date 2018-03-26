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
        /// Invoked before the delegate logic. If a response written,
        /// the delegate won't execute.
        /// </summary>
        public Func<HttpContext, Task> BeforeInvoke { get; set; }
    }
}