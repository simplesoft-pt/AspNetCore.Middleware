using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using SimpleSoft.AspNetCore.Middleware.HealthCheck;
using SimpleSoft.AspNetCore.Middleware.Metadata;

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.2.0.18001")]
[assembly: AssemblyProduct("1.2.0-rc01")]

namespace SimpleSoft.AspNetCore.Middleware.ExampleApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            services.AddHealthCheck(cfg =>
            {
                cfg.AddSql("db-sql-server",
                    () => new SqlConnection("Data Source=localhost;Database=Master;Integrated Security=true"),
                    "SELECT 1", true, "sql-server");

                cfg.AddSql("db-mysql",
                    () => new MySqlConnection("Server=localhost;Database=mysql;IntegratedSecurity=yes"),
                    "SELECT 1", false, "mysql");

                cfg.AddDelegate("example-delegate", async ct =>
                {
                    await Task.Delay(200, ct);
                    if (DateTimeOffset.Now.Millisecond % 3 == 0)
                        throw new Exception("Example health check exception");
                    return HealthCheckStatus.Green;
                }, false, "custom");
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
                Environment = env.EnvironmentName
            });

            app.UseHealthCheck(new HealthCheckOptions
            {
                BeforeInvoke = ctx =>
                {
                    //  example: only available via localhost or to an admin
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
