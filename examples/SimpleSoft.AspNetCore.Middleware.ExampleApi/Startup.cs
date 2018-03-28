using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimpleSoft.AspNetCore.Middleware.HealthCheck;
using SimpleSoft.AspNetCore.Middleware.Metadata;

namespace SimpleSoft.AspNetCore.Middleware.ExampleApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            services.AddHealthCheck(cfg =>
            {
                cfg.AddDelegate("random-exception", async ct =>
                {
                    await Task.Delay(200, ct);
                    if (DateTimeOffset.Now.Millisecond % 2 == 0)
                        throw new Exception("Random health check exception");
                    return HealthCheckStatus.Green;
                }, false, "test", "random", "custom");
                cfg.AddDelegate("random-exception-required", async ct =>
                {
                    await Task.Delay(200, ct);
                    if (DateTimeOffset.Now.Millisecond % 3 == 0)
                        throw new Exception("Random health check exception");
                    return HealthCheckStatus.Green;
                }, true, "test", "random", "custom");
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseMetadata(new MetadataOptions
            {
                BeforeInvoke = ctx =>
                {
                    //  example: only available via localhost, aborting the request if false
                    if (IsLocalhostRequest(ctx))
                        return Task.CompletedTask;
                    
                    ctx.Abort();
                    return Task.CompletedTask;
                },
                Path = "_meta",
                IndentJson = env.IsDevelopment(),
                IncludeNullProperties = true,
                Name = "SimpleSoft Middleware ExampleApi",
                Environment = env.EnvironmentName,
                StartedOn = DateTimeOffset.Now,
                Version = new MetadataVersionOptions
                {
                    Major = 1,
                    Minor = 2,
                    Patch = 3,
                    Revision = 4,
                    Alias = "1.2.3-rc01"
                }
            });

            app.UseHealthCheck(new HealthCheckOptions
            {
                BeforeInvoke = ctx =>
                {
                    //  example: only available via localhost or to an admin
                    //  returning a client error status code when false
                    if (IsLocalhostRequest(ctx))
                        return Task.CompletedTask;
                    
                    if (ctx.User.Identity.IsAuthenticated)
                    {
                        if (ctx.User.IsInRole("admin"))
                            return Task.CompletedTask;

                        ctx.Response.StatusCode = 403;
                        return ctx.Response.WriteAsync("Forbidden");
                    }

                    ctx.Response.StatusCode = 401;
                    return ctx.Response.WriteAsync("Unauthorized");
                },
                Path = "_health",
                IndentJson = env.IsDevelopment(),
                StringEnum = true
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync(
                    "This is an example API for SimpleSoft.AspNetCore.Middleware!");
            });
        }

        private static bool IsLocalhostRequest(HttpContext ctx)
        {
            var host = ctx.Request.Host.Host;
            return "localhost".Equals(host) || "127.0.0.1".Equals(host);
        }
    }
}
