using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleSoft.AspNetCore.Middleware.ExampleApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((ctx, builder) =>
                {
                    builder
                        .ClearProviders()
                        .AddDebug()
                        .AddConsole(options => { options.IncludeScopes = true; })
                        .SetMinimumLevel(LogLevel.Debug);
                })
                .Build();
    }
}
