# Telemetry and Metrics Implementation

This document describes the telemetry and metrics implementation added to the Profit Sharing application.

## Overview

The telemetry system provides comprehensive monitoring and observability for the application while protecting sensitive data. It includes:

- **Performance monitoring** with request duration and response size tracking
- **Error tracking** by type and endpoint category
- **User access patterns** (by role, not individual users)
- **Sensitive field access** monitoring (configurable)
- **Large response detection** and alerting
- **Distributed tracing** with correlation IDs

## Architecture

### Components

1. **EndpointTelemetry** - Centralized metrics and activity source
2. **TelemetryMiddleware** - HTTP request/response monitoring
3. **TelemetryConfiguration** - Configurable settings
4. **ServiceCollectionExtensions** - DI registration

### Key Features

- **Low-cardinality metrics** to avoid metric explosion
- **Sensitive data protection** through masking and categorization
- **Configurable sensitive field tracking** (disabled by default in production)
- **Integration with existing GlobalMeter** patterns
- **OpenTelemetry compliance** for distributed tracing

## Configuration

### appsettings.json

```json
{
  "Telemetry": {
    "Enabled": true,
    "EnableSensitiveFieldTracking": false,
    "LargeResponseThresholdBytes": 5000000,
    "IncludeUserRole": true,
    "DetailedTraceSampleRate": 0.1
  }
}
```

### Configuration Options

- **Enabled**: Master switch for telemetry collection
- **EnableSensitiveFieldTracking**: Enable/disable sensitive field access counting (use carefully)
- **LargeResponseThresholdBytes**: Threshold for large response alerts (default: 5MB)
- **IncludeUserRole**: Include user role in metrics (low cardinality)
- **DetailedTraceSampleRate**: Sampling rate for detailed traces (0.0 to 1.0)

## Metrics Captured

### Request Metrics

- `ps_endpoint_requests_total` - Total endpoint requests by category, method, status, user role
- `ps_endpoint_duration_seconds` - Request duration histogram
- `ps_response_size_bytes` - Response size histogram

### Security & Monitoring Metrics

- `ps_sensitive_field_access_total` - Sensitive field access counts (when enabled)
- `ps_large_responses_total` - Large response counter
- `ps_endpoint_errors_total` - Error counter by type and category

### Endpoint Categories (Low Cardinality)

- `demographics` - Demographics endpoints
- `beneficiaries` - Beneficiary management
- `reports` - Reporting endpoints
- `lookups` - Reference data endpoints
- `master-inquiry` - Master inquiry operations
- `health` - Health check endpoints
- `other` - All other endpoints

## Logging and Tracing

### Structured Logging

- Correlation IDs for request tracking
- Masked sensitive data (SSNs, user IDs)
- Error details with context
- Performance metrics

### Example Log Entry

```json
{
  "timestamp": "2025-01-20T10:30:45.123Z",
  "level": "Information",
  "message": "Large response detected: 6291456 bytes for endpoint beneficiaries with correlation 1a2b3c4d for user role authenticated",
  "correlation_id": "1a2b3c4d",
  "endpoint_category": "beneficiaries",
  "user_role": "authenticated",
  "response_size_bytes": 6291456
}
```

### Distributed Tracing

- Activity source: `demoulas.profitsharing`
- Operation tags: `operation`, `success`, `error_type`
- Masked sensitive data in tags
- Correlation with OpenTelemetry standards

## Security & Privacy

### Data Protection

- **SSN masking**: Only first 3 digits + hash shown in telemetry
- **User ID masking**: First 3 chars + hash for correlation
- **No PII in metrics**: Only role-based aggregation
- **Configurable sensitive tracking**: Disabled by default in production

### Example Masked Data

- SSN `123456789` → `123***A1B2C3D4` (in logs/traces)
- User ID `john.doe@company.com` → `joh***A1B2C3D4` (in traces)

## Usage Examples

### Adding Telemetry to New Endpoints

```csharp
public override async Task<Results<Ok<Response>, NotFound, ProblemHttpResult>> ExecuteAsync(Request req, CancellationToken ct)
{
    // Start activity for detailed tracing
    using var activity = EndpointTelemetry.ActivitySource.StartActivity("operation_name");
    activity?.SetTag("operation", "operation_name");
    activity?.SetTag("entity_id", req.Id?.ToString() ?? "null");

    try
    {
        _logger.LogInformation("Starting operation for entity {EntityId}", req.Id);

        var result = await _service.DoOperation(req, ct);

        _logger.LogInformation("Successfully completed operation with ID {ResultId}", result.Id);
        activity?.SetTag("result_id", result.Id.ToString());
        activity?.SetTag("success", true);

        return TypedResults.Ok(result);
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Entity not found"))
    {
        _logger.LogWarning(ex, "Entity not found for operation");
        activity?.SetTag("success", false);
        activity?.SetTag("error_type", "entity_not_found");
        return TypedResults.NotFound();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error in operation");
        activity?.SetTag("success", false);
        activity?.SetTag("exception_type", ex.GetType().Name);
        return TypedResults.Problem($"An unexpected error occurred: {ex.Message}");
    }
}
```

### Custom Metrics

```csharp
// Record custom metrics
EndpointTelemetry.SensitiveFieldAccessTotal.Add(1,
    new("field", "ssn"),
    new("endpoint_category", "demographics"));

// Record errors
EndpointTelemetry.EndpointErrorsTotal.Add(1,
    new("endpoint_category", "beneficiaries"),
    new("error_type", "ValidationException"),
    new("user_role", "authenticated"));
```

## Monitoring and Alerting

### Recommended Alerts

#### Performance Alerts

- High request duration (95th percentile > 5 seconds)
- Large response threshold exceeded (> 5MB responses)
- Error rate spike (> 5% errors in 5 minutes)

#### Security Alerts

- Excessive sensitive field access (when enabled)
- Unusual access patterns by user role
- High volume of failed requests from single user

### Sample Prometheus Queries

```promql
# High error rate
rate(ps_endpoint_errors_total[5m]) > 0.05

# Large responses
increase(ps_large_responses_total[10m]) > 5

# Slow requests
histogram_quantile(0.95, rate(ps_endpoint_duration_seconds_bucket[5m])) > 5

# Sensitive field access spike (when enabled)
increase(ps_sensitive_field_access_total[1h]) > 100
```

## Development vs Production

### Development Environment

- Higher trace sampling rate (0.5)
- Sensitive field tracking enabled
- Lower large response threshold (1MB)
- Detailed logging enabled

### Production Environment

- Conservative trace sampling (0.1)
- Sensitive field tracking disabled by default
- Higher large response threshold (5MB)
- Security-focused logging

## Best Practices

1. **Keep metrics low-cardinality** - Use endpoint categories, not full paths
2. **Mask sensitive data** - Never log raw SSNs, user IDs, or PII
3. **Use correlation IDs** - Link logs and traces for investigation
4. **Configure thresholds** - Adjust based on your application patterns
5. **Monitor metric cardinality** - Avoid metric explosion
6. **Test thoroughly** - Verify telemetry doesn't impact performance
7. **Document custom metrics** - Maintain inventory of metrics and their purpose

## Troubleshooting

### Common Issues

1. **High metric cardinality**: Check for user IDs or other high-cardinality tags
2. **Performance impact**: Reduce trace sampling or disable expensive features
3. **Missing metrics**: Verify service registration and middleware placement
4. **Sensitive data leaks**: Review log output and trace tags

### Configuration Validation

The telemetry configuration is validated at startup. Invalid configurations will prevent the application from starting with clear error messages.

## Future Enhancements

- Integration with APM platforms (DataDog, New Relic)
- Custom dashboards and visualizations
- Machine learning-based anomaly detection
- Enhanced security telemetry and threat detection
- Performance optimization recommendations
