# Part 6a: Health Checks

**Estimated Time:** 10 minutes  
**Prerequisites:** [Part 5c Complete](./05c-telemetry-middleware.md)  
**Next:** [Part 6b: Testing Patterns](./06b-testing-patterns.md)

---

## 🎯 Overview

Health checks provide diagnostics for:

- **Environment Information** - OS, framework, uptime
- **Database Connectivity** - EF Core health
- **Cache Availability** - Redis/distributed cache
- **Custom Checks** - Application-specific validation

---

## 🏥 EnvironmentHealthCheck Implementation

### HealthCheck/EnvironmentHealthCheck.cs

```csharp
using System.Runtime.InteropServices;
using Demoulas.Common.Api.Utilities;
using Demoulas.Common.Contracts.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MySolution.Endpoints.HealthCheck;

public class EnvironmentHealthCheck : IHealthCheck
{
    private static readonly DateTime _startupTime = DateTime.UtcNow;
    private readonly AppVersionInfo _appVersion;
    private readonly OktaConfiguration _config;
    private readonly IWebHostEnvironment _env;

    public EnvironmentHealthCheck(
        IWebHostEnvironment env,
        AppVersionInfo appVersion,
        OktaConfiguration config)
    {
        _env = env;
        _appVersion = appVersion;
        _config = config;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, object> data = new()
        {
            { "Environment", _env.EnvironmentName },
            { "ApplicationName", _env.ApplicationName },
            { "MachineName", Environment.MachineName },
            { "WorkingSet", Environment.WorkingSet },
            { "OSVersion", Environment.OSVersion.ToString() },
            { "Framework", RuntimeInformation.FrameworkDescription },
            { "AppVersion", _appVersion.BuildNumber },
            { "CurrentDirectory", Environment.CurrentDirectory },
            { "Uptime", (DateTime.UtcNow - _startupTime).ToString(@"dd\.hh\:mm\:ss") },
            { "UtcNow", DateTimeOffset.UtcNow.ToString("o") },
            { "OktaEnvironmentName", _config.EnvironmentName ?? string.Empty },
            { "OktaRolePrefix", _config.RolePrefix }
        };

        return Task.FromResult(HealthCheckResult.Healthy("Environment check", data));
    }
}
```

**Key Points:**

- ✅ Injects `AppVersionInfo` for build version tracking
- ✅ Injects `OktaConfiguration` for environment validation
- ✅ Tracks startup time for uptime calculation
- ✅ Includes 12 diagnostic fields for troubleshooting
- ✅ Uses dictionary initializer for cleaner syntax

---

## 🔧 Health Check Registration

### In Program.cs

**CRITICAL:** Ensure required dependencies are registered before health checks.

```csharp
// Register dependencies for EnvironmentHealthCheck
// AppVersionInfo is typically registered earlier in Program.cs
// OktaConfiguration is registered in AddSecurityServices

// Register health checks
builder.Services.AddHealthChecks()
    .AddCheck<EnvironmentHealthCheck>("environment")
    .AddDbContextCheck<MyDbContext>("database")
    .AddDistributedMemoryCache()  // If using IDistributedCache
    .AddCheck("cache", () => HealthCheckResult.Healthy("Cache available"));

// Configure health check publishing
builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromMinutes(1);      // Initial delay
    options.Period = TimeSpan.FromMinutes(15);    // Check interval
});
```

### Endpoint Mapping

```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                data = e.Value.Data,
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        await context.Response.WriteAsync(result);
    }
});
```

---

## 📊 Sample Health Check Response

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "environment",
      "status": "Healthy",
      "description": "Environment check",
      "data": {
        "Environment": "Development",
        "ApplicationName": "MySolution.Api",
        "MachineName": "DEV-SERVER",
        "OSVersion": "Microsoft Windows NT 10.0.22631.0",
        "Framework": ".NET 10.0.0",
        "Uptime": "02.14:32:15",
        "ProcessorCount": 8
      },
      "duration": 2.5
    },
    {
      "name": "database",
      "status": "Healthy",
      "description": "Database connection",
      "duration": 45.2
    }
  ],
  "totalDuration": 47.7
}
```

---

## 🔍 Custom Health Checks

### CacheHealthCheck.cs

```csharp
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MySolution.Api.HealthCheck;

public class CacheHealthCheck : IHealthCheck
{
    private readonly IDistributedCache _cache;

    public CacheHealthCheck(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var testKey = "health_check_" + Guid.NewGuid();
            var testValue = "test"u8.ToArray();

            await _cache.SetAsync(testKey, testValue, cancellationToken);
            var retrieved = await _cache.GetAsync(testKey, cancellationToken);
            await _cache.RemoveAsync(testKey, cancellationToken);

            if (retrieved != null && retrieved.SequenceEqual(testValue))
            {
                return HealthCheckResult.Healthy("Cache read/write successful");
            }

            return HealthCheckResult.Degraded("Cache read/write failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cache unavailable", ex);
        }
    }
}
```

---

## ✅ Validation Checklist - Part 6a

- [ ] **EnvironmentHealthCheck** implemented
- [ ] **Database health check** registered
- [ ] **Cache health check** registered (if using)
- [ ] **/health endpoint** mapped with JSON writer
- [ ] **Health check publishing** configured
- [ ] **Custom checks** added for critical dependencies

---

## 🎓 Key Takeaways - Part 6a

1. **Environment Diagnostics** - OS, framework, uptime info
2. **Database Connectivity** - EF Core health validation
3. **Custom Checks** - Application-specific validation
4. **JSON Response** - Structured health data

---

**Next:** [Part 6b: Testing Patterns](./06b-testing-patterns.md)
