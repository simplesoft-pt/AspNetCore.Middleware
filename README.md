# ASP.NET Core - Middleware
Collection of utilitary middleware for ASP.NET Core 2+ applications.

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

### Metadata
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
                    if (!IsLocalhostRequest(ctx))
                        ctx.Abort();
                    return Task.CompletedTask;
                },
                IndentJson = env.IsDevelopment(),
                Environment = env.EnvironmentName
            });
        }
    }
    
    private static bool IsLocalhostRequest(HttpContext ctx)
    {
        var host = ctx.Request.Host.Host;
        return "localhost".Equals(host) || "127.0.0.1".Equals(host);
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
*Note:* Check the [wiki](https://github.com/simplesoft-pt/AspNetCore.Middleware/wiki/Metadata) for more details.
