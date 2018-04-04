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

//  example: will be used by the metadata endpoint by default
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.2.0.18001")]
[assembly: AssemblyProduct("1.2.0-rc01")]

namespace SimpleSoft.AspNetCore.Middleware.ExampleApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //  needed for middleware routes
            services.AddRouting();

            //  needed if using cached health checks
            services.AddMemoryCache();

            services.AddHealthCheck(builder =>
            {
                builder.AddSql("db-sql-server",
                    () => new SqlConnection("Data Source=localhost;Database=Master;Integrated Security=true"),
                    "SELECT 1", true, "sql-server");

                builder.AddSql("db-mysql",
                    p => new MySqlConnection("Server=localhost;Database=mysql;Integrated Security=yes"),
                    "SELECT 1", false, "mysql");

                builder.AddCached(cachedBuilder =>
                {
                    cachedBuilder.AddHttp("http-stat-200",
                        "https://httpstat.us/200", 2000, true, true, "httpstat");

                    cachedBuilder.AddHttp("http-stat-500",
                        p => "https://httpstat.us/500", 2000, true, true, "httpstat");

                    cachedBuilder.AddHttp("http-stat-timeout",
                        new Uri("https://httpstat.us/200?sleep=5000"), 2000, true, true, "httpstat");
                }, TimeSpan.FromSeconds(30));

                builder.AddDelegate("delegate", async ct =>
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
                await context.Response.WriteAsync(IndexHtml);
            });
        }

        private static bool IsLocalhostRequest(HttpContext ctx)
        {
            var host = ctx.Request.Host.Host;
            return "localhost".Equals(host) || "127.0.0.1".Equals(host);
        }

        private const string IndexHtml = @"
<html>
    <head>
        <title>SimpleSoft - AspNetCore Middleware Examples</title>
        <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css"" integrity=""sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm"" crossorigin=""anonymous"">
    </head>
    <body>
        <div class=""container"">
            <h2>AspNetCore Middleware Examples</h2><br/>
            <p>This is an example API for SimpleSoft.AspNetCore.Middleware!</p>
            <p>Example endpoints:</p>
            <ul>
                <li><a href=""/_meta"" target=""_blank"">Metadata (GET /_meta)</a></li>
                <li><a href=""/_health"" target=""_blank"">Health Checks (GET /_health)</a></li>
            </ul>
        </div>
    </body>
</html>";
    }
}
