using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimpleSoft.AspNetCore.Middleware.Metadata;

namespace SimpleSoft.AspNetCore.Middleware.ExampleApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMetadata(new MetadataOptions
            {
                Path = "_meta",
                IndentJson = true,
                Name = "SimpleSoft Middleware ExampleApi",
                Environment = env.EnvironmentName,
                StartedOn = DateTimeOffset.UtcNow,
                Version = new MetadataVersionOptions
                {
                    Major = 1,
                    Minor = 2,
                    Patch = 3,
                    Revision = 4,
                    Alias = "1.2.3-rc01"
                }
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync(
                    "This is an example API for SimpleSoft.AspNetCore.Middleware!");
            });
        }
    }
}
