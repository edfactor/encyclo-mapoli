---
applyTo: "src/services/src/**/*.*"
paths: "src/services/src/**/*.*"
---

# Demoulas.Common.Logging - Structured Logging and Observability

**Package:** `Demoulas.Common.Logging`  
**Namespace:** `Demoulas.Common.Logging.Extensions`

This package provides comprehensive structured logging with built-in security features, distributed tracing, and support for multiple observability platforms including SumoLogic, Dynatrace, and OpenTelemetry.

## Table of Contents

1. [Quick Start](#quick-start)
2. [Configuration](#configuration)
3. [Observability Platforms](#observability-platforms)
4. [Security Features](#security-features)
5. [OpenTelemetry Integration](#opentelemetry-integration)
6. [Health Monitoring](#health-monitoring)
7. [Migration from v1.x](#migration-from-v1x)

---

## Quick Start

### Basic Setup

```csharp
using Demoulas.Common.Logging.Extensions;
using Demoulas.Common.Contracts.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with structured logging
builder.SetDefaultLoggerConfiguration(new LoggingConfig
{
    ProjectName = "MyApplication",
    Namespace = "MyCompany.MyTeam",

    // SumoLogic integration
    SumoLogic = new SumoLogicConfig
    {
        Url = builder.Configuration["SumoLogic:Url"]!,
        BufferBaseFileName = "sumo-buffer",
        UseSensitiveDataMasking = true
    }
});

// Add OpenTelemetry with service defaults
builder.Services.AddServiceDefaults(builder.Configuration);

var app = builder.Build();
app.MapDefaultHealthChecks(); // Add health endpoints
app.Run();
```

---

## Configuration

### LoggingConfig

**Namespace:** `Demoulas.Common.Contracts.Configuration`

Primary configuration object for logging setup.

```csharp
public class LoggingConfig
{
    public string? ProjectName { get; set; }          // Application name
    public string? Namespace { get; set; }            // Organizational namespace
    public FileSystemLogConfig? FileSystem { get; set; }
    public SumoLogicConfig? SumoLogic { get; set; }
    public DynatraceConfig? Dynatrace { get; set; }
    public bool UseSensitiveDataMasking { get; set; } = true;
    public ushort? DestructureDepth { get; set; } = 3;
}
```

### FileSystemLogConfig

Configure file-based logging (text and/or JSON).

```csharp
public class FileSystemLogConfig
{
    public string? FilePath { get; set; }             // Base path for logs
    public bool WriteTextLogs { get; set; } = true;
    public bool WriteJsonLogs { get; set; } = false;  // Opt-in structured JSON
    public int? RetainedFileCountLimit { get; set; } = 31;
}
```

**Example:**

```csharp
FileSystem = new FileSystemLogConfig
{
    FilePath = "C:\\Logs\\MyApp",
    WriteTextLogs = true,
    WriteJsonLogs = true,  // Enable JSON for log aggregation
    RetainedFileCountLimit = 90  // Keep 90 days of logs
}
```

---

## Observability Platforms

### SumoLogic

Cloud-native log management with HTTP collector integration.

#### SumoLogicConfig

```csharp
public class SumoLogicConfig
{
    public string Url { get; set; }                         // HTTP collector URL
    public string? SourceName { get; set; }
    public string? SourceCategory { get; set; }
    public string? SourceHost { get; set; }
    public string? BufferBaseFileName { get; set; }          // For buffered mode
    public bool UseSensitiveDataMasking { get; set; } = true;
    public Action<HttpClient>? ConfigureHttpClient { get; set; }  // Custom HTTP settings
}
```

#### Buffered vs Unbuffered Mode

**Buffered (Recommended for Production):**

```csharp
SumoLogic = new SumoLogicConfig
{
    Url = configuration["SumoLogic:Url"]!,
    BufferBaseFileName = "sumo-buffer",  // Enables buffering
    SourceCategory = "MyApp/Production"
}
```

**Unbuffered (Development/Testing):**

```csharp
SumoLogic = new SumoLogicConfig
{
    Url = configuration["SumoLogic:Url"]!,
    BufferBaseFileName = null  // Direct HTTP delivery
}
```

#### Custom HTTP Client Configuration

```csharp
SumoLogic = new SumoLogicConfig
{
    Url = configuration["SumoLogic:Url"]!,
    ConfigureHttpClient = client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("X-Custom-Header", "Value");
    }
}
```

---

### Dynatrace

APM integration with custom attribute enrichment.

#### DynatraceConfig

```csharp
public class DynatraceConfig
{
    public string? EntityId { get; set; }                // Dynatrace entity ID
    public bool UseFileIngestion { get; set; }           // OneAgent file ingestion
    public Dictionary<string, string>? CustomAttributes { get; set; }
}
```

#### Example Configuration

```csharp
Dynatrace = new DynatraceConfig
{
    EntityId = "CUSTOM_DEVICE-ABC123",
    UseFileIngestion = true,
    CustomAttributes = new Dictionary<string, string>
    {
        ["dt.entity.custom_device"] = "CUSTOM_DEVICE-ABC123",
        ["environment"] = "Production",
        ["region"] = "US-East"
    }
}
```

#### When to Specify EntityId

Specify `EntityId` manually when:

- Running in **serverless functions** (Lambda, Azure Functions)
- Using **containerized** deployments without OneAgent
- **Legacy systems** without OneAgent auto-injection
- Need **custom entity** grouping in Dynatrace UI

If OneAgent is installed, `EntityId` is auto-discovered.

#### File Ingestion

Enable for OneAgent integration:

```csharp
UseFileIngestion = true  // Logs written to OneAgent monitored path
```

Prerequisites:

- OneAgent installed and active
- File monitoring configured in Dynatrace
- Proper file permissions

---

## Security Features

### Automatic Data Masking

**Always enabled** when `UseSensitiveDataMasking = true` (default).

#### Masked Data Types

- **SSN:** `XXX-XX-1234` → `***MASKED***`
- **Credit Cards:** `4111-1111-1111-1111` → `***MASKED***`
- **Email Addresses:** `user@example.com` → `***MASKED***`
- **IBAN:** `DE89370400440532013000` → `***MASKED***`
- **Database Connections:**
  - Hostnames: `dbserver.company.com` → `***MASKED***`
  - Ports: `:1521` → `***MASKED***`
  - Service Names: `SERVICE_NAME=PROD` → `***MASKED***`
  - Oracle descriptors: `(DESCRIPTION=...)` → `***MASKED***`

#### Configuration

```csharp
builder.SetDefaultLoggerConfiguration(new LoggingConfig
{
    UseSensitiveDataMasking = true,  // Default

    SumoLogic = new SumoLogicConfig
    {
        UseSensitiveDataMasking = true  // Per-sink override
    }
});
```

### Custom Masking Operators

Leverage `Serilog.Enrichers.Sensitive` for advanced masking:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithSensitiveDataMasking(options =>
    {
        options.MaskProperties.Add("Password");
        options.MaskProperties.Add("ApiKey");
        options.MaskingOperators.Add(new CustomMaskingOperator());
    })
    .CreateLogger();
```

---

## OpenTelemetry Integration

### Service Defaults

**Method:** `AddServiceDefaults(IConfiguration configuration)`  
**Namespace:** `Demoulas.Common.Logging.Extensions.AspireServiceDefaultExtensions`

Configures OpenTelemetry for logs, metrics, and traces with Aspire-compatible defaults.

```csharp
builder.Services.AddServiceDefaults(builder.Configuration);
```

### Environment Variables

Set via configuration or environment:

```bash
# Service identification
OTEL_SERVICE_NAME=MyApplication
OTEL_SERVICE_VERSION=1.0.0

# OTLP endpoint
OTEL_EXPORTER_OTLP_ENDPOINT=http://collector:4318

# Dynatrace-specific
OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE=delta
OTEL_EXPORTER_OTLP_METRICS_DEFAULT_HISTOGRAM_AGGREGATION=exponential
```

### Service Name Priority

Resolved in this order:

1. `OTEL_SERVICE_NAME` environment variable
2. `service.name` in `OTEL_RESOURCE_ATTRIBUTES`
3. `LoggingConfig.ProjectName`
4. Assembly name

### Instrumentation

Automatically instruments:

- **ASP.NET Core** - HTTP request/response
- **HttpClient** - Outbound HTTP calls
- **Runtime** - GC, CPU, memory metrics
- **Oracle Database** - SQL queries and commands (via OpenTelemetry.Instrumentation.EntityFrameworkCore)

### Custom Spans

```csharp
using System.Diagnostics;

private static readonly ActivitySource ActivitySource = new("MyApp.Operations");

public async Task ProcessOrderAsync(int orderId)
{
    using var activity = ActivitySource.StartActivity("ProcessOrder");
    activity?.SetTag("order.id", orderId);

    await _orderService.ProcessAsync(orderId);
}
```

---

## Health Monitoring

### MapDefaultHealthChecks

**Extension Method:** `MapDefaultHealthChecks(this WebApplication app)`

Adds standard health check endpoints:

- `/health` - Basic health check
- `/health/ready` - Readiness probe (Kubernetes)
- `/health/live` - Liveness probe (Kubernetes)

```csharp
var app = builder.Build();

app.MapDefaultHealthChecks();  // Adds all health endpoints

app.Run();
```

### Custom Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("Database", () =>
    {
        // Custom database health check
        return HealthCheckResult.Healthy();
    })
    .AddCheck("Redis", () =>
    {
        // Custom Redis health check
        return HealthCheckResult.Healthy();
    });
```

### Health Check UI (Optional)

```csharp
builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

app.MapHealthChecksUI(options => options.UIPath = "/health-ui");
```

---

## Migration from v1.x

### Breaking Changes

1. **Configuration Object:**

   - `ElasticSearchConfig` → `LoggingConfig`
   - ElasticSearch removed (use SumoLogic or OpenTelemetry)

2. **OpenTelemetry Setup:**

   - Now configured via `AddServiceDefaults()` instead of `ConfigureLogging()`

3. **Method Signatures:**
   - `ConfigureLogging()` removed
   - Use `SetDefaultLoggerConfiguration(LoggingConfig)` instead

### v1.x Code

```csharp
// OLD - v1.x
builder.Host.ConfigureLogging(new ElasticSearchConfig
{
    ApplicationName = "MyApp",
    Uri = "https://es.company.com",
    UseElasticSearch = true
});
```

### v2.0 Code

```csharp
// NEW - v2.0
builder.SetDefaultLoggerConfiguration(new LoggingConfig
{
    ProjectName = "MyApp",
    Namespace = "MyTeam",

    SumoLogic = new SumoLogicConfig
    {
        Url = configuration["SumoLogic:Url"]!
    }
});

builder.Services.AddServiceDefaults(builder.Configuration);
```

---

## Best Practices

### Structured Logging

Use structured logging with named properties:

```csharp
// ✅ GOOD - Structured
_logger.LogInformation("User {UserId} placed order {OrderId} for ${Amount:F2}",
    userId, orderId, amount);

// ❌ BAD - String interpolation
_logger.LogInformation($"User {userId} placed order {orderId} for ${amount}");
```

### Log Levels

- **Trace:** Detailed diagnostic (e.g., method entry/exit)
- **Debug:** Development-time debugging
- **Information:** General application flow
- **Warning:** Unexpected but handled situations
- **Error:** Errors and exceptions
- **Critical:** Application/system failures

### Exception Logging

```csharp
try
{
    await ProcessOrderAsync(orderId);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to process order {OrderId}", orderId);
    throw;
}
```

### Performance Considerations

- Use **structured logging** for efficient querying
- Enable **JSON logs** for file-based sinks used by aggregators
- Configure **retention policies** to manage disk usage
- Use **buffered mode** for SumoLogic in high-throughput scenarios

---

## Multi-Targeting Support

This package supports **net8.0, net9.0, and net10.0**. No framework-specific considerations.

---

## Related Documentation

- [Demoulas.Common.Contracts Instructions](demoulas.common.contracts.instructions.md) - Configuration DTOs
- [Demoulas.Common.Api Instructions](demoulas.common.api.instructions.md) - API logging integration
- [Security Instructions](demoulas.common.security.instructions.md) - Security best practices
- [Code Review Instructions](code-review.instructions.md) - Review standards
