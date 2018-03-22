using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleSoft.AspNetCore.Middleware.HealthCheck;
using SimpleSoft.AspNetCore.Middleware.Metadata;

namespace SimpleSoft.AspNetCore.Middleware.ExampleApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            services
                .AddScoped<IHealthCheck>(s => new DelegatingHealthCheck("random-exception", async ct =>
                {
                    await Task.Delay(200, ct);
                    if (DateTimeOffset.Now.Millisecond % 2 == 0)
                        throw new Exception("Random health check exception");
                    return HealthCheckStatus.Green;
                }, s.GetService<ILogger<DelegatingHealthCheck>>()))
                .AddScoped<IHealthCheck>(s => new DelegatingHealthCheck("random-exception-required", async ct =>
                {
                    await Task.Delay(200, ct);
                    if (DateTimeOffset.Now.Millisecond % 2 == 0)
                        throw new Exception("Random health check exception");
                    return HealthCheckStatus.Green;
                }, s.GetService<ILogger<DelegatingHealthCheck>>(), true));
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMetadata(new MetadataOptions
            {
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
    }
}
