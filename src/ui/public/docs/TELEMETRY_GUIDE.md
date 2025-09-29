# Profit Sharing Application Telemetry Guide

## Overview

The Profit Sharing application includes comprehensive telemetry and observability infrastructure designed to help development, QA, and production support teams understand how the application is used while protecting sensitive data.

## Table of Contents

- [Quick Start](#quick-start)
- [For Developers](#for-developers)
- [For QA Teams](#for-qa-teams)
- [For DevOps/Production Support](#for-devopsproduction-support)
- [Telemetry Architecture](#telemetry-architecture)
- [Metrics Reference](#metrics-reference)
- [Sensitive Data Protection](#sensitive-data-protection)
- [Configuration](#configuration)
- [Troubleshooting](#troubleshooting)

## Quick Start

### What We Measure
- **API Usage**: Request counts, response times, user activity
- **Business Operations**: Year-end processes, report generation, data processing volumes
- **Performance**: Request/response sizes, execution times, large response detection
- **Security**: Sensitive field access (SSN, PII) with proper masking
- **Errors**: Exception tracking with correlation IDs for debugging

### Key Benefits
- **Usage Analytics**: Understand which endpoints are used most frequently
- **Performance Monitoring**: Identify slow endpoints and large data transfers
- **Security Auditing**: Track access to sensitive data with proper PII protection
- **Error Analysis**: Correlate errors across requests with structured logging
- **Business Intelligence**: Monitor year-end operations and report generation metrics

---

## For Developers

### Adding Telemetry to New Endpoints

#### Option 1: ExecuteWithTelemetry Wrapper (Recommended)
For most endpoints, use the `ExecuteWithTelemetry` wrapper for automatic comprehensive telemetry:

```csharp
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;

public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
{
    return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
    {
        // Your endpoint logic here
        var result = await _service.ProcessAsync(req, ct);
        
        // Optional: Add business-specific metrics
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "my-operation"),
            new("endpoint", "MyEndpoint"));
            
        return result;
    }, "Ssn", "Email"); // List sensitive fields accessed
}
```

#### Option 2: Manual Telemetry (For Complex Scenarios)
For endpoints requiring fine-grained control:

```csharp
public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
{
    using var activity = this.StartEndpointActivity(HttpContext);
    
    try
    {
        // Record request metrics (mark sensitive fields)
        this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");
        
        // Your endpoint logic
        var response = await _service.ProcessAsync(req, ct);
        
        // Business metrics
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "my-business-operation"),
            new("endpoint", "MyEndpoint"));
            
        // Record counts processed
        var recordCount = response?.Items?.Count ?? 0;
        EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
            new("record_type", "my-records"),
            new("endpoint", "MyEndpoint"));
        
        // Record response metrics
        this.RecordResponseMetrics(HttpContext, _logger, response);
        
        return response;
    }
    catch (Exception ex)
    {
        // Record exception with correlation
        this.RecordException(HttpContext, _logger, ex, activity);
        throw;
    }
}
```

### Required Using Statements
```csharp
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Common.Telemetry;
using Microsoft.Extensions.Logging;
```

### Constructor Pattern
Ensure your endpoint has logger injection:
```csharp
private readonly ILogger<MyEndpoint> _logger;

public MyEndpoint(/* other services */, ILogger<MyEndpoint> logger)
    : base(Navigation.Constants.MyNavigationId)
{
    _logger = logger;
    // other initialization
}
```

### Sensitive Field Guidelines
When accessing sensitive data, always mark it in telemetry calls:
- `"Ssn"` - Social Security Numbers
- `"Email"` - Email addresses  
- `"BankAccount"` - Banking information
- `"Phone"` - Phone numbers

### Business Metrics Examples
```csharp
// Year-end operations
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "year-end-profit-calculation"),
    new("endpoint", "YearEndProcessEndpoint"),
    new("profit_year", "2025"));

// Report generation
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "report-generation"),
    new("report_type", "beneficiaries"),
    new("endpoint", "BeneficiaryReportEndpoint"));

// Data processing volumes
EndpointTelemetry.RecordCountsProcessed.Record(employeeCount,
    new("record_type", "employees-processed"),
    new("endpoint", "EmployeeProcessingEndpoint"));
```

### Testing Telemetry
In unit tests, verify telemetry is called:
```csharp
[Test]
public async Task MyEndpoint_ShouldRecordTelemetry()
{
    // Arrange
    var mockLogger = new Mock<ILogger<MyEndpoint>>();
    var endpoint = new MyEndpoint(mockService, mockLogger.Object);
    
    // Act
    await endpoint.ExecuteAsync(request, CancellationToken.None);
    
    // Assert
    mockLogger.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Processing request")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

---

## For QA Teams

### What Telemetry Provides for Testing

#### 1. Request/Response Validation
- **Request Size Monitoring**: Verify large payloads don't exceed limits
- **Response Time Tracking**: Identify performance regressions
- **Error Correlation**: Link errors across distributed requests

#### 2. Business Logic Validation
- **Operation Counts**: Verify correct number of records processed
- **Year-End Metrics**: Validate profit sharing calculations and report generation
- **User Activity**: Confirm role-based access patterns

#### 3. Security Testing
- **Sensitive Data Access**: Verify SSN/PII access is properly logged and masked
- **Role-Based Access**: Monitor user role patterns in telemetry
- **Large Response Detection**: Identify potential data exfiltration attempts

### Testing Scenarios

#### Performance Testing
Monitor these metrics during load testing:
```
# Request duration (should be < 5 seconds for most endpoints)
app_request_duration_seconds_bucket{endpoint_category="year-end"}

# Large responses (should be reasonable for business requirements)
ps_large_responses_total{endpoint_category="reports"}

# Error rates (should be < 1% under normal load)
ps_endpoint_errors_total / app_requests_total
```

#### Security Testing
Verify sensitive data handling:
```
# SSN access should be logged when appropriate
ps_sensitive_field_access_total{field="Ssn"}

# User roles should be tracked correctly
app_requests_total{user_role="ADMINISTRATOR"}
```

#### Business Logic Testing
Validate business operations:
```
# Year-end operations should complete successfully
ps_business_operations_total{operation="year-end-profit-calculation"}

# Record counts should match expectations
ps_record_counts_processed{record_type="employees"}
```

### Test Data Considerations
- Use test SSNs that don't appear in production logs
- Verify test user roles generate appropriate telemetry
- Test large dataset scenarios to trigger large response metrics

### Integration with Test Automation
Add telemetry validation to automated tests:
```csharp
[Test]
public async Task YearEndProcess_ShouldRecordBusinessMetrics()
{
    // Arrange
    var testRequest = new YearEndRequest { ProfitYear = 2025 };
    
    // Act  
    var response = await CallEndpoint(testRequest);
    
    // Assert
    Assert.IsTrue(response.IsSuccess);
    
    // Verify telemetry was recorded (check logs or metrics endpoint)
    await VerifyMetricRecorded("ps_business_operations_total", 
        new[] { ("operation", "year-end-processing") });
}
```

---

## For DevOps/Production Support

### Monitoring and Alerting

#### Key Metrics to Monitor

**1. Request Volume and Performance**
```promql
# Request rate by endpoint category
rate(app_requests_total[5m])

# 95th percentile response time
histogram_quantile(0.95, rate(app_request_duration_seconds_bucket[5m]))

# Error rate (should be < 1%)
rate(ps_endpoint_errors_total[5m]) / rate(app_requests_total[5m])
```

**2. Business Operations**
```promql
# Year-end operations per hour
increase(ps_business_operations_total{operation=~"year-end.*"}[1h])

# Large responses (potential performance issue)
increase(ps_large_responses_total[10m])

# Sensitive data access rate
rate(ps_sensitive_field_access_total[1h])
```

**3. System Health**
```promql
# Request size distribution
histogram_quantile(0.95, rate(ps_request_size_bytes_bucket[5m]))

# Response size distribution  
histogram_quantile(0.95, rate(ps_response_size_bytes_bucket[5m]))

# User activity by role
sum by (user_role) (rate(app_requests_total[5m]))
```

#### Recommended Alerts

**High Error Rate**
```yaml
alert: HighErrorRate
expr: rate(ps_endpoint_errors_total[5m]) / rate(app_requests_total[5m]) > 0.05
for: 2m
description: "Error rate is {{ $value | humanizePercentage }} for the last 5 minutes"
```

**Large Response Volume**
```yaml
alert: ExcessiveLargeResponses  
expr: increase(ps_large_responses_total[10m]) > 10
for: 1m
description: "{{ $value }} large responses detected in last 10 minutes"
```

**Sensitive Data Access Spike**
```yaml
alert: SensitiveDataAccessSpike
expr: rate(ps_sensitive_field_access_total{field="Ssn"}[1h]) > 100
for: 5m
description: "High SSN access rate: {{ $value }} per hour"
```

### Log Analysis

#### Structured Log Patterns
All endpoints generate structured logs with correlation IDs:

```json
{
  "timestamp": "2025-09-25T10:30:00Z",
  "level": "Information", 
  "message": "Processing request in YearEndReportEndpoint for user role ADMINISTRATOR",
  "correlationId": "abc123-def456",
  "endpoint": "YearEndReportEndpoint",
  "userRole": "ADMINISTRATOR",
  "requestSize": 1024,
  "operation": "year-end-report-generation"
}
```

#### Log Queries for Troubleshooting

**Find all requests for a specific correlation ID:**
```
correlationId:"abc123-def456"
```

**Find errors in the last hour:**
```
level:"Error" AND timestamp:[now-1h TO now]
```

**Track sensitive data access:**
```
message:"Sensitive field accessed" AND field:"Ssn"
```

**Monitor year-end operations:**
```
operation:"year-end*" AND timestamp:[now-24h TO now]
```

### Configuration Management

#### Environment Variables
```bash
# OpenTelemetry Configuration
OTEL_ENABLED=true
OTEL_OTLP_ENDPOINT=http://your-collector:4317
OTEL_TRACING_SAMPLER=parentbased_traceidratio
OTEL_TRACING_SAMPLER_RATE=0.1

# Security Telemetry (use with caution)
TELEMETRY_ENABLE_SENSITIVE_FIELD_TRACKING=false
TELEMETRY_LARGE_RESPONSE_THRESHOLD_BYTES=5242880
```

#### appsettings.json
```json
{
  "OpenTelemetry": {
    "Enabled": true,
    "Otlp": {
      "Endpoint": "http://otel-collector:4317"
    },
    "Tracing": {
      "Sampler": "parentbased_traceidratio",
      "SamplerRate": 0.1
    },
    "Metrics": {
      "Enabled": true
    }
  },
  "Telemetry": {
    "EnableSensitiveFieldTracking": false,
    "LargeResponseThresholdBytes": 5242880,
    "PiiMaskingEnabled": true
  }
}
```

### Capacity Planning

Use telemetry data for capacity planning:

**Peak Usage Analysis**
```promql
# Peak requests per hour by endpoint category
max_over_time(rate(app_requests_total[1h])[7d])

# Peak response sizes
max_over_time(histogram_quantile(0.95, rate(ps_response_size_bytes_bucket[1h]))[7d])
```

**Growth Trends**
```promql
# 7-day growth rate
(rate(app_requests_total[1d]) - rate(app_requests_total[1d] offset 7d)) / rate(app_requests_total[1d] offset 7d)
```

### Troubleshooting Common Issues

#### High Memory Usage
Check for large responses:
```promql
histogram_quantile(0.99, rate(ps_response_size_bytes_bucket[5m]))
```

#### Slow Performance
Identify slow endpoints:
```promql
histogram_quantile(0.95, rate(app_request_duration_seconds_bucket[5m])) by (endpoint_category)
```

#### Security Incidents
Track unusual access patterns:
```promql
# Unusual user accessing sensitive data
ps_sensitive_field_access_total{field="Ssn"} by (user_role)

# Large number of records accessed by single user  
ps_record_counts_processed by (user_role)
```

---

## Telemetry Architecture

### Components Overview

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Endpoints     │───▶│ TelemetryExt.cs  │───▶│ OpenTelemetry   │
│                 │    │                  │    │                 │
│ - Manual calls  │    │ - StartActivity  │    │ - Activities    │
│ - ExecuteWith   │    │ - RecordMetrics  │    │ - Metrics       │
│   Telemetry     │    │ - RecordException│    │ - Logs          │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                 │
                                 ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│ TelemetryProc.  │    │ EndpointTelemetry│    │ Middleware      │
│                 │    │                  │    │                 │
│ - PreProcess    │    │ - BusinessOps    │    │ - Request/Resp  │
│ - PostProcess   │    │ - RecordCounts   │    │ - Error tracking│
│ - Auto tracking │    │ - SensitiveField │    │ - User activity │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

### Data Flow

1. **Request Arrives** → TelemetryMiddleware captures basic metrics
2. **Endpoint Execution** → TelemetryProcessor or manual telemetry calls
3. **Business Logic** → Custom business metrics recorded
4. **Response** → Response metrics and correlation logged
5. **Export** → OpenTelemetry exports to configured backends

### Metrics Categories

**Standard Metrics (GlobalMeter)**
- `app_requests_total` - Request counts by endpoint
- `app_request_duration_seconds` - Request duration histogram

**Extended Metrics (EndpointTelemetry)**
- `ps_business_operations_total` - Business operation counts
- `ps_record_counts_processed` - Data volume processed
- `ps_sensitive_field_access_total` - Sensitive data access
- `ps_request_size_bytes` - Request payload sizes
- `ps_response_size_bytes` - Response payload sizes
- `ps_large_responses_total` - Large response counter
- `ps_endpoint_errors_total` - Error counts by type

---

## Metrics Reference

### Complete Metrics List

| Metric Name | Type | Description | Labels |
|-------------|------|-------------|---------|
| `app_requests_total` | Counter | Total API requests | `endpoint.name`, `navigation.id`, `user.role` |
| `app_request_duration_seconds` | Histogram | Request duration | `endpoint_category`, `method`, `status_code`, `user_role` |
| `ps_business_operations_total` | Counter | Business operations performed | `operation`, `endpoint`, `profit_year`, `report_type` |
| `ps_record_counts_processed` | Histogram | Number of records processed | `record_type`, `endpoint`, `entity_type` |
| `ps_sensitive_field_access_total` | Counter | Sensitive field access events | `field`, `endpoint.name`, `user.role` |
| `ps_request_size_bytes` | Histogram | Request payload size | `endpoint.name`, `navigation.id`, `user.role` |
| `ps_response_size_bytes` | Histogram | Response payload size | `endpoint_category`, `method`, `status_code`, `user_role` |
| `ps_large_responses_total` | Counter | Large responses (>5MB) | `endpoint_category`, `user_role` |
| `ps_endpoint_errors_total` | Counter | Endpoint errors | `endpoint.name`, `error.type`, `user.role` |
| `ps_user_activity_total` | Counter | User activity events | `user.role`, `endpoint_category` |
| `ps_validation_failures_total` | Counter | Validation failures | `endpoint.name`, `validation_type` |

### Label Values

**endpoint_category**: `master-inquiry`, `year-end`, `reports`, `beneficiaries`, `distributions`, `lookups`, `military`, `it-operations`

**user.role**: `ADMINISTRATOR`, `FINANCEMANAGER`, `READONLY`, `ITOPERATIONS`

**operation**: `year-end-profit-calculation`, `report-generation`, `employee-lookup`, `beneficiary-management`, `distribution-processing`

**record_type**: `employees`, `beneficiaries`, `distributions`, `year-end-participants`, `reports`

**field**: `Ssn`, `Email`, `BankAccount`, `Phone`

---

## Sensitive Data Protection

### PII Masking Strategy

The telemetry system automatically masks sensitive data in logs while preserving access tracking:

#### Masking Rules
- **SSN**: `123-45-6789` → `***-**-6789`
- **Email**: `user@company.com` → `u***@c***.com`
- **Phone**: `(555) 123-4567` → `(***) ***-4567`
- **Bank Account**: `123456789` → `******789`

#### What Gets Logged vs Masked

**Logged (Safe for Telemetry)**:
- That SSN field was accessed (count)
- Which endpoint accessed it
- User role that accessed it
- Correlation ID for debugging

**Masked (Never in Telemetry)**:
- Actual SSN values
- Actual email addresses
- Actual phone numbers
- Actual bank account numbers

#### Configuration
```json
{
  "Telemetry": {
    "EnableSensitiveFieldTracking": false,  // Disable in prod initially
    "PiiMaskingEnabled": true,              // Always true
    "SensitiveFields": ["Ssn", "Email", "BankAccount", "Phone"]
  }
}
```

### Compliance Considerations

**GDPR/Privacy**:
- All PII is masked in telemetry data
- Correlation IDs allow debugging without exposing PII
- Sensitive field access is tracked for audit purposes
- Data retention policies apply to telemetry data

**SOX/Financial Compliance**:
- Financial operations are tracked for audit trails
- User access to sensitive financial data is logged
- Year-end operations have comprehensive tracking

**Security Auditing**:
- Unusual access patterns can be detected
- User role-based access monitoring
- Large data export detection

---

## Configuration

### Production Configuration

#### Recommended Settings
```json
{
  "OpenTelemetry": {
    "Enabled": true,
    "Otlp": {
      "Endpoint": "https://your-otel-collector:4317"
    },
    "Tracing": {
      "Sampler": "parentbased_traceidratio",
      "SamplerRate": 0.01  // 1% sampling for production
    },
    "Metrics": {
      "Enabled": true
    }
  },
  "Telemetry": {
    "EnableSensitiveFieldTracking": false,  // Start disabled
    "LargeResponseThresholdBytes": 5242880, // 5MB
    "PiiMaskingEnabled": true
  }
}
```

#### Development Settings
```json
{
  "OpenTelemetry": {
    "Enabled": true,
    "Exporter": {
      "Console": {
        "Enabled": true  // For local debugging
      }
    },
    "Tracing": {
      "Sampler": "always_on"  // Full sampling for dev
    }
  },
  "Telemetry": {
    "EnableSensitiveFieldTracking": true,  // OK for dev
    "LargeResponseThresholdBytes": 1048576 // 1MB for dev
  }
}
```

### Feature Flags

Use feature flags to control telemetry rollout:

```csharp
// In endpoint code
if (_featureManager.IsEnabledAsync("DetailedTelemetry").Result)
{
    // Record additional business metrics
    EndpointTelemetry.BusinessOperationsTotal.Add(1, tags);
}
```

### Environment-Specific Overrides

```bash
# Production
export TELEMETRY_ENABLE_SENSITIVE_FIELD_TRACKING=false
export OTEL_TRACING_SAMPLER_RATE=0.01

# Staging  
export TELEMETRY_ENABLE_SENSITIVE_FIELD_TRACKING=true
export OTEL_TRACING_SAMPLER_RATE=0.1

# Development
export OTEL_EXPORTER_CONSOLE_ENABLED=true
export OTEL_TRACING_SAMPLER=always_on
```

---

## Troubleshooting

### Common Issues

#### 1. No Telemetry Data
**Symptoms**: No metrics appearing in monitoring system

**Checklist**:
- [ ] `OpenTelemetry:Enabled` is `true`
- [ ] OTLP endpoint is accessible
- [ ] Endpoint has logger injection
- [ ] Using correct `using` statements
- [ ] TelemetryExtensions methods are called

**Debug Steps**:
```csharp
// Enable console exporter for debugging
services.AddOpenTelemetry()
    .WithMetrics(builder => builder.AddConsoleExporter());
```

#### 2. Missing Business Metrics
**Symptoms**: Standard metrics work but business metrics missing

**Common Causes**:
- Forgetting to call `EndpointTelemetry.BusinessOperationsTotal.Add()`
- Exception thrown before metrics recorded
- Wrong metric labels

**Fix**:
```csharp
// Always record business metrics, even on errors
try
{
    var result = await ProcessAsync();
    
    // Record success metrics
    EndpointTelemetry.BusinessOperationsTotal.Add(1, successTags);
    return result;
}
catch (Exception ex)
{
    // Record failure metrics  
    EndpointTelemetry.BusinessOperationsTotal.Add(1, failureTags);
    throw;
}
```

#### 3. High Cardinality Warnings
**Symptoms**: Monitoring system warns about high cardinality

**Causes**:
- Using user IDs in metric labels
- Too many unique label values

**Fix**:
```csharp
// BAD: High cardinality
new("user_id", userId)  // Don't do this

// GOOD: Low cardinality  
new("user_role", userRole)  // Use this instead
```

#### 4. Sensitive Data in Logs
**Symptoms**: Actual SSN/PII appearing in logs

**Immediate Actions**:
1. Stop log collection
2. Purge affected logs
3. Review telemetry code

**Prevention**:
```csharp
// Always use masked values
var maskedSsn = TelemetryExtensions.MaskSensitiveValue(ssn, "Ssn");
_logger.LogInformation("Processing SSN: {MaskedSsn}", maskedSsn);
```

#### 5. Performance Impact
**Symptoms**: Application slower after adding telemetry

**Analysis**:
- Check if `ExecuteWithTelemetry` is nested
- Verify sampling rates aren't too high
- Look for synchronous I/O in telemetry code

**Optimization**:
```csharp
// Use async logging
_logger.LogInformation("Message");  // Not _logger.LogInformationAsync()

// Optimize sampling for high-volume endpoints
services.Configure<TelemetryConfiguration>(config =>
{
    config.SamplingRate = 0.01; // 1% for high-volume endpoints
});
```

### Diagnostic Queries

#### Check Telemetry Health
```promql
# Are we receiving any metrics?
up{job="profit-sharing-api"}

# Metric collection rate
rate(app_requests_total[5m])

# Error rate in telemetry
rate(ps_endpoint_errors_total[5m])
```

#### Validate Business Metrics
```promql
# Business operations being recorded?
increase(ps_business_operations_total[1h])

# Record counts make sense?
histogram_quantile(0.95, rate(ps_record_counts_processed_bucket[5m]))
```

#### Security Validation
```promql
# Sensitive field access patterns
ps_sensitive_field_access_total by (field, user_role)

# Large responses (potential data exfiltration)
increase(ps_large_responses_total[10m])
```

### Support Contacts

**Development Issues**: Contact development team with:
- Correlation ID from logs
- Endpoint name and request details
- Expected vs actual telemetry behavior

**Infrastructure Issues**: Contact DevOps team with:
- Metrics query results
- Time range of issue
- Affected environments

**Security Concerns**: Contact security team immediately with:
- Evidence of PII in logs
- Unusual access patterns
- Suspected data exfiltration

---

## Appendix

### Sample Queries

#### Business Intelligence
```promql
# Year-end operations by month
increase(ps_business_operations_total{operation=~"year-end.*"}[30d])

# Report generation trends
sum by (report_type) (increase(ps_business_operations_total{operation="report-generation"}[7d]))

# Employee processing volumes
histogram_quantile(0.95, rate(ps_record_counts_processed_bucket{record_type="employees"}[1d]))
```

#### Performance Analysis
```promql
# Slowest endpoints (95th percentile)
histogram_quantile(0.95, rate(app_request_duration_seconds_bucket[5m])) by (endpoint_category)

# Largest responses by endpoint category
histogram_quantile(0.99, rate(ps_response_size_bytes_bucket[5m])) by (endpoint_category)

# Request rate by user role
sum by (user_role) (rate(app_requests_total[5m]))
```

#### Security Monitoring
```promql
# SSN access by user role
sum by (user_role) (rate(ps_sensitive_field_access_total{field="Ssn"}[1h]))

# Large data exports by user
ps_large_responses_total by (user_role)

# Failed authentication attempts (if available)
increase(ps_endpoint_errors_total{error_type="UnauthorizedAccessException"}[5m])
```

### Integration Examples

#### Grafana Dashboard
```json
{
  "dashboard": {
    "title": "Profit Sharing Application Metrics",
    "panels": [
      {
        "title": "Request Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(app_requests_total[5m])",
            "legendFormat": "{{endpoint_category}}"
          }
        ]
      },
      {
        "title": "Business Operations",
        "type": "stat", 
        "targets": [
          {
            "expr": "increase(ps_business_operations_total[1h])",
            "legendFormat": "{{operation}}"
          }
        ]
      }
    ]
  }
}
```

#### Alertmanager Configuration
```yaml
groups:
- name: profit-sharing
  rules:
  - alert: HighErrorRate
    expr: rate(ps_endpoint_errors_total[5m]) / rate(app_requests_total[5m]) > 0.05
    for: 2m
    annotations:
      summary: "High error rate detected"
      description: "Error rate is {{ $value | humanizePercentage }}"
      
  - alert: SensitiveDataAccessSpike
    expr: rate(ps_sensitive_field_access_total{field="Ssn"}[1h]) > 100
    for: 5m
    annotations:
      summary: "Unusual SSN access pattern"
      description: "SSN access rate: {{ $value }} per hour"
```

### Code Examples Repository

All telemetry examples and templates are available in the codebase:

- **TelemetryExtensions.cs**: Core extension methods
- **EndpointTelemetry.cs**: Business metrics definitions
- **Sample Endpoints**: Working examples in Reports/YearEnd folder
- **Unit Tests**: Telemetry testing patterns in test projects

---

*This documentation is maintained by the development team. Last updated: September 25, 2025*