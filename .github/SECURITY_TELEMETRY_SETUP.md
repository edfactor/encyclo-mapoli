# Security Telemetry Setup Guide

Advanced OpenTelemetry configuration for detecting abusive data access patterns, suspicious user behavior, and security threats in the Profit Sharing application.

## Overview

This guide describes **recommended** security telemetry patterns that help IT Security detect:
- Large-scale data downloads or exports
- Repeated access to sensitive fields (SSN, salary data)
- Unusually high request rates from individual accounts
- Suspicious query patterns or data access anomalies

**Scope**: Low-risk, low-cardinality metrics with high-fidelity traces/logs for investigation.

**Status**: Recommended patterns for future implementation. Basic telemetry is handled by `TelemetryExtensions` (see `TELEMETRY_GUIDE.md`).

## Architecture

### Centralized Telemetry Bootstrap

The repository provides centralized OpenTelemetry/logging setup via:
```
Demoulas.Common.Logging.Extensions.AspireServiceDefaultExtensions
```

**Critical**: Do NOT duplicate `AddOpenTelemetry()` calls. Extend the existing pipeline instead.

### Telemetry Layers

1. **Base Layer** (already implemented):
   - HTTP request/response metrics
   - Endpoint activity tracing
   - Exception tracking
   - Business operation counters

2. **Security Layer** (this document):
   - Response size monitoring
   - Sensitive field access tracking
   - Per-user rate limiting signals
   - Bulk export detection

## Required NuGet Packages

Add via centralized `Directory.Packages.props`:

```xml
<ItemGroup>
  <PackageVersion Include="OpenTelemetry" Version="1.x.x" />
  <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.x.x" />
  <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.x.x" />
  <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.x.x" />
  <PackageVersion Include="OpenTelemetry.Instrumentation.SqlClient" Version="1.x.x" />
  <PackageVersion Include="OpenTelemetry.Exporter.Otlp" Version="1.x.x" />
  <PackageVersion Include="OpenTelemetry.Exporter.Console" Version="1.x.x" /> <!-- Dev only -->
</ItemGroup>
```

## Configuration

### appsettings.json

```json
{
  "OpenTelemetry": {
    "Enabled": true,
    "Otlp": {
      "Endpoint": "http://otel-collector:4317"
    },
    "Exporter": {
      "Console": {
        "Enabled": false  // Dev only
      }
    },
    "Tracing": {
      "Sampler": "traceidratio",
      "SamplerRate": 0.1  // 10% sampling
    },
    "Metrics": {
      "Enabled": true
    }
  },
  "SecurityTelemetry": {
    "EnableSensitiveFieldTracking": false,     // MUST be false in production by default
    "LargeResponseThresholdBytes": 5242880,   // 5 MB
    "BulkExportThreshold": 10000,             // Records
    "EnablePerUserMetrics": false              // High cardinality - requires approval
  }
}
```

### Environment Variables

```bash
# Production overrides
OPENTELEMETRY__ENABLED=true
OPENTELEMETRY__OTLP__ENDPOINT=https://otel-collector.prod.example.com:4317
SECURITYTELEMETRY__ENABLESENSITIVEFIELDTRACKING=false  # Explicit false
```

## Implementation Patterns

### Extending the Telemetry Pipeline

**DO NOT** call `builder.Services.AddOpenTelemetry()` directly. Instead, register security-specific components:

```csharp
// Program.cs (after AspireServiceDefaultExtensions.ApplyDefaults)

// Register security telemetry service
builder.Services.AddSingleton<SecurityTelemetryService>();

// Register middleware for response size tracking
builder.Services.AddTransient<ResponseSizeTrackingMiddleware>();

// Use middleware
var app = builder.Build();
app.UseMiddleware<ResponseSizeTrackingMiddleware>();
```

### Security Metrics (Low Cardinality)

Define security-focused metrics using `System.Diagnostics.Metrics`:

```csharp
public class SecurityMetrics
{
    private static readonly Meter s_meter = new("Demoulas.ProfitSharing.Security");
    
    // Response size histogram (low cardinality: endpoint_category only)
    public static readonly Histogram<long> ResponseBytesHistogram = s_meter.CreateHistogram<long>(
        "ps_response_bytes",
        unit: "bytes",
        description: "Distribution of response sizes by endpoint category");
    
    // Large download counter
    public static readonly Counter<long> LargeDownloadsTotal = s_meter.CreateCounter<long>(
        "ps_large_downloads_total",
        description: "Count of responses exceeding size threshold");
    
    // Sensitive field access counter (low cardinality: field type + endpoint category)
    public static readonly Counter<long> SensitiveFieldAccessTotal = s_meter.CreateCounter<long>(
        "ps_sensitive_field_access_total",
        description: "Count of sensitive field accesses by field type and endpoint");
    
    // Bulk export counter
    public static readonly Counter<long> BulkExportOperationsTotal = s_meter.CreateCounter<long>(
        "ps_bulk_export_operations_total",
        description: "Count of bulk export operations by export type");
    
    // User request rate (aggregate by role, NOT by user_id)
    public static readonly Counter<long> UserRequestsByRole = s_meter.CreateCounter<long>(
        "ps_user_requests_by_role_total",
        description: "Request count aggregated by user role");
}
```

### Response Size Tracking Middleware

```csharp
public class ResponseSizeTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseSizeTrackingMiddleware> _logger;
    private readonly IConfiguration _config;

    public ResponseSizeTrackingMiddleware(
        RequestDelegate next,
        ILogger<ResponseSizeTrackingMiddleware> logger,
        IConfiguration config)
    {
        _next = next;
        _logger = logger;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);

            var responseSize = responseBodyStream.Length;
            var endpointCategory = GetEndpointCategory(context.Request.Path);

            // Record histogram (low cardinality)
            SecurityMetrics.ResponseBytesHistogram.Record(
                responseSize,
                new("endpoint_category", endpointCategory),
                new("status_code", context.Response.StatusCode.ToString()));

            // Check threshold
            var threshold = _config.GetValue<long>("SecurityTelemetry:LargeResponseThresholdBytes", 5242880);
            if (responseSize > threshold)
            {
                SecurityMetrics.LargeDownloadsTotal.Add(1,
                    new("endpoint_category", endpointCategory));

                // Emit structured log for investigation (includes user_id)
                _logger.LogWarning(
                    "Large response detected: {SizeBytes} bytes from {Endpoint} (User: {UserId}, CorrelationId: {CorrelationId})",
                    responseSize,
                    context.Request.Path,
                    context.User.Identity?.Name ?? "anonymous",
                    Activity.Current?.Id ?? "none");
            }

            // Copy response back
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private string GetEndpointCategory(PathString path)
    {
        // Categorize endpoints to keep cardinality low
        if (path.StartsWithSegments("/api/employees")) return "employees";
        if (path.StartsWithSegments("/api/reports")) return "reports";
        if (path.StartsWithSegments("/api/distributions")) return "distributions";
        if (path.StartsWithSegments("/api/year-end")) return "year-end";
        return "other";
    }
}
```

### Sensitive Field Access Tracking

When service methods access sensitive fields:

```csharp
public class EmployeeService
{
    private readonly ILogger<EmployeeService> _logger;
    private readonly IConfiguration _config;

    public async Task<Result<EmployeeDto>> GetEmployeeAsync(string oracleHcmId, CancellationToken ct)
    {
        var employee = await _dbContext.Employees
            .Include(e => e.Demographics)
            .FirstOrDefaultAsync(e => e.OracleHcmId == oracleHcmId, ct);

        if (employee == null)
            return Result<EmployeeDto>.Failure(Error.EmployeeNotFound);

        // Track sensitive field access (if enabled)
        if (_config.GetValue<bool>("SecurityTelemetry:EnableSensitiveFieldTracking"))
        {
            TrackSensitiveFieldAccess("Ssn", "employee-lookup", oracleHcmId);
            TrackSensitiveFieldAccess("OracleHcmId", "employee-lookup", oracleHcmId);
        }

        return Result<EmployeeDto>.Success(MapToDto(employee));
    }

    private void TrackSensitiveFieldAccess(string fieldType, string operation, string entityId)
    {
        // Increment counter (low cardinality: field type + operation)
        SecurityMetrics.SensitiveFieldAccessTotal.Add(1,
            new("field_type", fieldType),
            new("operation", operation));

        // Emit structured log with user details for investigation
        var userId = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        var correlationId = Activity.Current?.Id;

        _logger.LogInformation(
            "Sensitive field access: {FieldType} in {Operation} (EntityId: {EntityId}, UserId: {UserId}, CorrelationId: {CorrelationId})",
            fieldType,
            operation,
            MaskEntityId(entityId),  // Mask or hash entity ID
            userId ?? "anonymous",
            correlationId ?? "none");
    }

    private string MaskEntityId(string id)
    {
        // Mask all but last 4 characters
        if (string.IsNullOrEmpty(id) || id.Length <= 4)
            return "****";
        return new string('*', id.Length - 4) + id.Substring(id.Length - 4);
    }
}
```

### Bulk Export Detection

```csharp
public class ReportService
{
    public async Task<Result<ReportData>> GenerateReportAsync(ReportRequest request, CancellationToken ct)
    {
        var data = await LoadReportDataAsync(request, ct);

        // Track bulk export
        var threshold = _config.GetValue<int>("SecurityTelemetry:BulkExportThreshold", 10000);
        if (data.Records.Count > threshold)
        {
            SecurityMetrics.BulkExportOperationsTotal.Add(1,
                new("export_type", request.ReportType),
                new("record_count_bucket", GetCountBucket(data.Records.Count)));

            _logger.LogWarning(
                "Bulk export: {RecordCount} records in {ReportType} report (User: {UserId}, CorrelationId: {CorrelationId})",
                data.Records.Count,
                request.ReportType,
                _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "anonymous",
                Activity.Current?.Id ?? "none");
        }

        return Result<ReportData>.Success(data);
    }

    private string GetCountBucket(int count)
    {
        // Bucket counts to keep cardinality low
        if (count < 1000) return "small";
        if (count < 10000) return "medium";
        if (count < 100000) return "large";
        return "xlarge";
    }
}
```

## Cardinality Management

### Low-Cardinality Dimensions (Safe)

Use these as metric labels:
- `service` (e.g., "profit-sharing-api")
- `environment` (e.g., "production", "staging")
- `endpoint_category` (e.g., "employees", "reports", "distributions")
- `method` (e.g., "GET", "POST")
- `response_status` (e.g., "200", "400", "500")
- `user_role` (e.g., "admin", "manager", "employee")
- `field_type` (e.g., "Ssn", "Salary", "OracleHcmId")
- `export_type` (e.g., "profit-sharing-report", "demographics-export")

### High-Cardinality Data (Use Logs/Traces)

**NEVER** use these as metric labels:
- ❌ `user_id` (thousands of unique values)
- ❌ `badge_number` (thousands of unique values)
- ❌ `ssn` (PII + high cardinality)
- ❌ `correlation_id` (unique per request)
- ❌ Specific endpoint paths with parameters

For per-user investigation, use structured logs with correlation IDs.

## Alerting Rules

### Prometheus PromQL Examples

**Large Download Spike**:
```promql
# Alert when large downloads exceed 5 in 10 minutes
increase(ps_large_downloads_total[10m]) > 5
```

**Sensitive Field Access Spike**:
```promql
# Alert when SSN access exceeds 100 in 1 hour
increase(ps_sensitive_field_access_total{field_type="Ssn"}[1h]) > 100
```

**User Role Request Rate**:
```promql
# Alert when admin role requests exceed 10,000 in 5 minutes
sum by (user_role) (increase(ps_user_requests_by_role_total{user_role="admin"}[5m])) > 10000
```

**Bulk Export Frequency**:
```promql
# Alert when bulk exports exceed 3 in 1 hour
increase(ps_bulk_export_operations_total[1h]) > 3
```

### SIEM / Log Search Examples

**Find users with excessive SSN lookups**:
```
event.type:"sensitive_field_access" 
AND field_type:"Ssn" 
| stats count() by user_id 
| where count > 100
```

**Detect potential data exfiltration**:
```
message:"Large response detected" 
AND size_bytes:>10485760 
| timechart span=1h count() by user_id
```

## Privacy & Compliance

### PII Masking

Always mask direct identifiers before emitting telemetry:

```csharp
private string MaskSsn(string ssn)
{
    if (string.IsNullOrEmpty(ssn) || ssn.Length != 11)
        return "***-**-****";
    
    return $"***-**-{ssn.Substring(7)}";  // Show last 4 digits only
}

private string HashOracleId(string oracleHcmId)
{
    using var sha256 = SHA256.Create();
    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(oracleHcmId));
    return Convert.ToBase64String(hashBytes).Substring(0, 12);
}
```

### Configuration Toggles

Guard sensitive tracking behind configuration:

```csharp
if (_config.GetValue<bool>("SecurityTelemetry:EnableSensitiveFieldTracking"))
{
    TrackSensitiveFieldAccess(fieldType, operation);
}
```

**Production defaults**:
- `EnableSensitiveFieldTracking`: **false** (requires explicit opt-in)
- `EnablePerUserMetrics`: **false** (requires IT/privacy approval)

### Access Controls

- Telemetry collectors and storage require authentication
- Encryption at rest for all telemetry data
- Role-based access to query telemetry (audit trail required)
- Retention policies: 30 days for detailed logs, 1 year for aggregated metrics

## Operational Recommendations

### Implementation Phases

**Phase 1: Basic Metrics** (Start Here)
- Response size histograms (endpoint category only)
- User request counts by role
- No sensitive field tracking

**Phase 2: Large Download Detection**
- Threshold-based alerting
- Structured logs with correlation IDs
- Role-based investigation

**Phase 3: Sensitive Field Tracking** (Requires Approval)
- Configuration toggle enabled after IT/privacy review
- Field-type counters (no user_id labels)
- Per-user logs with correlation IDs

**Phase 4: Advanced Analytics** (Optional)
- Anomaly detection on request patterns
- Machine learning models for threat detection
- Integration with SIEM platforms

### Tuning Thresholds

Start conservative, tune based on actual usage:

| Metric | Initial Threshold | Tune After |
|--------|------------------|------------|
| Large response size | 5 MB | 2 weeks |
| Bulk export record count | 10,000 | 1 month |
| Sensitive field access rate | 100/hour | 2 weeks |
| User request rate (role) | 10,000/5min | 1 week |

### Cardinality Monitoring

Monitor metric cardinality in Prometheus/Grafana:

```promql
# Check total series count
count({__name__=~"ps_.*"})

# Find high-cardinality metrics
topk(10, count by (__name__)({__name__=~"ps_.*"}))
```

If cardinality exceeds 10,000 series, review labels and aggregate further.

## Testing

### Unit Tests

```csharp
public class SecurityMetricsTests
{
    [Fact]
    public void TrackLargeResponse_IncrementsCounter()
    {
        // Arrange
        var initialCount = GetMetricValue(SecurityMetrics.LargeDownloadsTotal);
        
        // Act
        SecurityMetrics.LargeDownloadsTotal.Add(1, new("endpoint_category", "reports"));
        
        // Assert
        var finalCount = GetMetricValue(SecurityMetrics.LargeDownloadsTotal);
        finalCount.ShouldBe(initialCount + 1);
    }
}
```

### Integration Tests

```csharp
public class SecurityTelemetryMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task LargeResponse_EmitsMetricAndLog()
    {
        // Arrange - Configure large response threshold
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>(
                        "SecurityTelemetry:LargeResponseThresholdBytes", "100")
                });
            });
        });
        
        var client = factory.CreateClient();
        
        // Act - Request endpoint that returns > 100 bytes
        var response = await client.GetAsync("/api/reports/large-report");
        
        // Assert - Verify metric was incremented (use test collector)
        // Assert - Verify log was emitted (use test logger)
    }
}
```

## Wiring Example

### Complete Integration

```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

// Apply centralized Aspire telemetry defaults (DO NOT DUPLICATE)
// This is already configured by the repository's shared extensions
// DO NOT call builder.Services.AddOpenTelemetry() again

// Register security telemetry service
builder.Services.AddSingleton<SecurityTelemetryService>();
builder.Services.AddTransient<ResponseSizeTrackingMiddleware>();

var app = builder.Build();

// Use security middleware
app.UseMiddleware<ResponseSizeTrackingMiddleware>();

// ... rest of middleware pipeline

app.Run();
```

## Related Documentation

- `TELEMETRY_GUIDE.md` - Comprehensive telemetry reference (base patterns)
- `TELEMETRY_QUICK_REFERENCE.md` - Developer cheat sheet
- `TELEMETRY_DEVOPS_GUIDE.md` - Production operations guide
- OpenTelemetry docs: https://opentelemetry.io/docs/
- Prometheus best practices: https://prometheus.io/docs/practices/naming/

## Best Practices Summary

### DO:
- ✅ Extend existing telemetry pipeline (don't duplicate)
- ✅ Use low-cardinality metric labels (role, endpoint_category, field_type)
- ✅ Emit per-user details to logs/traces with correlation IDs
- ✅ Mask or hash PII before emitting telemetry
- ✅ Guard sensitive tracking behind configuration toggles
- ✅ Default to disabled in production (explicit opt-in)
- ✅ Start with basic metrics, add advanced patterns after approval
- ✅ Monitor cardinality and tune thresholds based on usage

### DO NOT:
- ❌ Duplicate `AddOpenTelemetry()` calls
- ❌ Use high-cardinality data in metric labels (user_id, badge_number)
- ❌ Emit raw PII to telemetry systems
- ❌ Enable sensitive field tracking without IT/privacy approval
- ❌ Use per-user metrics without cardinality limits
- ❌ Fail operations when telemetry fails
- ❌ Hardcode thresholds (use configuration)

---

**Last Updated**: October 2025  
**Maintained By**: Platform Team + Security Team  
**Approval Required**: IT Security, Privacy Team  
**Questions**: Contact #platform-engineering, #security
