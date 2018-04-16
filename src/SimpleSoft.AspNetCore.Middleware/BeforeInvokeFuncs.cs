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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SimpleSoft.AspNetCore.Middleware
{
    /// <summary>
    /// Helper functions that can be used for <see cref="SimpleSoftMiddlewareOptions.BeforeInvoke"/>.
    /// </summary>
    public static class BeforeInvoke
    {
        /// <summary>
        /// Does nothing with the <see cref="HttpContext"/> allowing the request to proceed.
        /// </summary>
        public static readonly Func<HttpContext, Task> DoNothing = ctx => Task.CompletedTask;

        /// <summary>
        /// Aborts the request if not from a local client.
        /// </summary>
        public static readonly Func<HttpContext, Task> LocalOnly = ctx =>
        {
            if(!ctx.Request.IsLocal())
                ctx.Abort();

            return Task.CompletedTask;
        };

        /// <summary>
        /// Checks if the user has any of the given roles, writing HTTP 403
        /// or HTTP 401 if not authenticated.
        /// </summary>
        /// <param name="role">Role to match</param>
        /// <param name="roles">Other opcional roles to match</param>
        /// <returns>The function to be used</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<HttpContext, Task> HasAnyRole(string role, params string[] roles)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (roles == null) throw new ArgumentNullException(nameof(roles));

            return ctx =>
            {
                int statusCode;
                string content;
                if (ctx.User.Identity.IsAuthenticated)
                {
                    if (ctx.User.IsInRole(role) || roles.Length > 0 && roles.Any(r => ctx.User.IsInRole(r)))
                        return Task.CompletedTask;

                    statusCode = 403;
                    content = "Forbidden";
                }
                else
                {
                    statusCode = 401;
                    content = "Unauthorized";
                }

                ctx.Response.StatusCode = statusCode;
                return ctx.Response.WriteAsync(content);
            };
        }

        /// <summary>
        /// Checks if the request is local, otherwise if the user has any of the given roles, writing HTTP 403 
        /// or HTTP 401 if not authenticated.
        /// </summary>
        /// <param name="role">Role to match</param>
        /// <param name="roles">Other opcional roles to match</param>
        /// <returns>The function to be used</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Func<HttpContext, Task> IsLocalOrHasAnyRole(string role, params string[] roles)
        {
            var hasAnyRole = HasAnyRole(role, roles);
            return ctx => ctx.Request.IsLocal() ? Task.CompletedTask : hasAnyRole(ctx);
        }

        private static bool IsLocal(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            return !connection.RemoteIpAddress.IsSet() ||
                   (connection.LocalIpAddress.IsSet()
                       ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                       : IPAddress.IsLoopback(connection.RemoteIpAddress));
        }

        private static bool IsSet(this IPAddress address) => address != null && address.ToString() != "::1";
    }
}