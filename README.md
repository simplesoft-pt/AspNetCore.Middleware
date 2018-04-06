# ASP.NET Core - Middleware
Collection of utilitary middleware for ASP.NET Core 2+ applications.

## Installation
The collection is available via [NuGet](https://www.nuget.org/packages?q=simplesoft.aspnetcore.middleware) packages:

| NuGet | Description | Version |
| --- | --- | --- |
| [SimpleSoft.AspNetCore.Middleware](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware) | base middleware and utilitaries for the more concrete implementations | [![NuGet](https://img.shields.io/nuget/vpre/simplesoft.aspnetcore.middleware.svg)](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware) |
| [SimpleSoft.AspNetCore.Middleware.HealthCheck](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware.healthcheck) | library offering a middleware for health check the application status, helpful for monitoring or load balance | [![NuGet](https://img.shields.io/nuget/vpre/simplesoft.aspnetcore.middleware.healthcheck.svg)](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware.healthcheck) |
| [SimpleSoft.AspNetCore.Middleware.Metadata](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware.metadata) | library offering a middleware that exposes a metadata endpoint, helpul as an _is alive_ and to know the application version, environment or name | [![NuGet](https://img.shields.io/nuget/vpre/simplesoft.aspnetcore.middleware.metadata.svg)](https://www.nuget.org/packages/simplesoft.aspnetcore.middleware.metadata) |

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
