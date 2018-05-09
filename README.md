# ASP.NET Core - Middleware
Collection of utilitary middleware for ASP.NET Core 2+ applications.
Check the [documentation](https://github.com/simplesoft-pt/AspNetCore.Middleware/wiki/) for more details.

## Installation
The collection is available via [NuGet](https://www.nuget.org/packages?q=simplesoft.aspnetcore.middleware) packages:

| NuGet | Description | Version |
| --- | --- | --- |
| [SimpleSoft.AspNetCore.Middleware](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware) | base middleware and other utility code | [![NuGet](https://img.shields.io/nuget/vpre/simplesoft.aspnetcore.middleware.svg)](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware) |
| [SimpleSoft.AspNetCore.Middleware.HealthCheck](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware.healthcheck) | middleware for health check the application status, helpful for monitoring or load balance | [![NuGet](https://img.shields.io/nuget/vpre/simplesoft.aspnetcore.middleware.healthcheck.svg)](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware.healthcheck) |
| [SimpleSoft.AspNetCore.Middleware.Metadata](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware.metadata) | middleware that exposes a metadata endpoint, helpul as an _is alive_ and to know the application version, environment or name | [![NuGet](https://img.shields.io/nuget/vpre/simplesoft.aspnetcore.middleware.metadata.svg)](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware.metadata) |

### Package Manager
```powershell
Install-Package SimpleSoft.AspNetCore.Middleware
Install-Package SimpleSoft.AspNetCore.Middleware.HealthCheck
Install-Package SimpleSoft.AspNetCore.Middleware.Metadata
```

### .NET CLI
```powershell
dotnet add package SimpleSoft.AspNetCore.Middleware
dotnet add package SimpleSoft.AspNetCore.Middleware.HealthCheck
dotnet add package SimpleSoft.AspNetCore.Middleware.Metadata
```
## Compatibility
The middlewares were implemented having ASP.NET Core 2 in mind, so they support `.NETStandard 2.0` and up.

## Examples
Simple code snippets showing how to use the middlewares. For a more detailed usage, just check the wiki.

### Metadata [[wiki]](https://github.com/simplesoft-pt/AspNetCore.Middleware/wiki/Metadata)
```csharp
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.2.0.18001")]
[assembly: AssemblyProduct("1.2.0-rc01")]

namespace ExampleApi
{
    public class Startup
    {   
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMetadata(new MetadataOptions
            {
                BeforeInvoke = ctx =>
                {
                    if (!"localhost".Equals(ctx.Request.Host.Host))
                        ctx.Abort();
                    return Task.CompletedTask;
                },
                IndentJson = env.IsDevelopment(),
                Environment = env.EnvironmentName
            });
        }
    }
}
```
```json
//    GET http://localhost:5000/api/_meta
//    200 OK
{
    "name": "ExampleApi",
    "environment": "Development",
    "startedOn": "2018-04-11T23:13:33.0606389+01:00",
    "version": {
        "major": 1,
        "minor": 2,
        "patch": 0,
        "revision": 18001,
        "alias": "1.2.0.rc01"
    }
}
```

### Health Checks [[wiki]](https://github.com/simplesoft-pt/AspNetCore.Middleware/wiki/HealthCheck)
```csharp
namespace ExampleApi
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
            app.UseHealthCheck(new HealthCheckOptions
            {
                BeforeInvoke = ctx =>
                {
                    if ("localhost".Equals(ctx.Request.Host.Host))
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
        }
    }
}
```
```json
//    GET http://localhost:5000/api/_health
//    500 Internal Server Error
{
  "status": "red",
  "startedOn": "2018-04-11T23:42:26.54719+01:00",
  "terminatedOn": "2018-04-11T23:42:28.7647715+01:00",
  "dependencies": {
    "db-sql-server": {
      "status": "green",
      "required": true,
      "tags": [
        "sql-server",
        "database",
        "sql"
      ]
    },
    "db-mysql": {
      "status": "red",
      "required": false,
      "tags": [
        "mysql",
        "database",
        "sql"
      ]
    },
    "http-stat-200": {
      "status": "green",
      "required": true,
      "tags": [
        "cached",
        "httpstat",
        "http"
      ]
    },
    "http-stat-500": {
      "status": "red",
      "required": true,
      "tags": [
        "cached",
        "httpstat",
        "http"
      ]
    },
    "http-stat-timeout": {
      "status": "red",
      "required": true,
      "tags": [
        "cached",
        "httpstat",
        "http"
      ]
    },
    "delegate": {
      "status": "green",
      "required": false,
      "tags": [
        "custom"
      ]
    }
  }
}
```
