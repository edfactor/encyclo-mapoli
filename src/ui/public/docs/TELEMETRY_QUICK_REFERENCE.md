# Telemetry Quick Reference Card

## 🚀 Quick Start for Developers

### Add to New Endpoint (3 steps)

1. **Add using statements**:

```csharp
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;
```

2. **Inject logger**:

```csharp
private readonly ILogger<MyEndpoint> _logger;

public MyEndpoint(IMyService service, ILogger<MyEndpoint> logger)
    : base(Navigation.Constants.MyNavigationId)
{
    _service = service;
    _logger = logger;
}
```

3. **Choose your pattern**:

**Option A: Automatic (Recommended)**

```csharp
public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
{
    return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
    {
        // Your logic here
        return await _service.ProcessAsync(req, ct);
    }, "Ssn"); // Mark sensitive fields
}
```

**Option B: Manual Control**

```csharp
public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
{
    using var activity = this.StartEndpointActivity(HttpContext);

    try
    {
        this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

        var response = await _service.ProcessAsync(req, ct);

        // Optional business metrics
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "my-operation"),
            new("endpoint", "MyEndpoint"));

        this.RecordResponseMetrics(HttpContext, _logger, response);
        return response;
    }
    catch (Exception ex)
    {
        this.RecordException(HttpContext, _logger, ex, activity);
        throw;
    }
}
```

## 📊 Common Business Metrics

```csharp
// Year-end operations
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "year-end-calculation"),
    new("endpoint", "YearEndEndpoint"),
    new("profit_year", req.ProfitYear.ToString()));

// Report generation
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "report-generation"),
    new("report_type", "beneficiaries"),
    new("endpoint", "BeneficiaryReportEndpoint"));

// Record counts
var recordCount = response?.Items?.Count ?? 0;
EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
    new("record_type", "employees"),
    new("endpoint", "EmployeeEndpoint"));
```

## 🔒 Sensitive Fields

Always mark when accessing:

- `"Ssn"` - Social Security Numbers
- `"Email"` - Email addresses
- `"BankAccount"` - Banking info
- `"Phone"` - Phone numbers

## ✅ Testing Checklist

- [ ] Logger injected in constructor
- [ ] Using statements added
- [ ] Sensitive fields marked in telemetry calls
- [ ] Business metrics added where appropriate
- [ ] Exception handling includes `RecordException`
- [ ] Unit tests verify telemetry calls

## 🐛 Troubleshooting

**No telemetry data?**

- Check logger injection
- Verify using statements
- Enable console exporter for debugging

**Performance issues?**

- Don't nest ExecuteWithTelemetry calls
- Check sampling rates
- Avoid synchronous I/O in telemetry

**High cardinality warnings?**

- Use user_role, not user_id in labels
- Keep label values low cardinality

## 📈 What Gets Measured

✅ **Automatically**:

- Request/response times
- Request/response sizes
- Error rates and types
- User activity by role

✅ **When You Add**:

- Business operation counts
- Record processing volumes
- Sensitive data access
- Custom metrics

## 🎯 For QA Teams

Monitor during testing:

```promql
# Error rate (should be < 1%)
rate(ps_endpoint_errors_total[5m]) / rate(app_requests_total[5m])

# Large responses (check reasonableness)
increase(ps_large_responses_total[10m])

# SSN access (should only occur when expected)
ps_sensitive_field_access_total{field="Ssn"}
```

## 🚨 For DevOps

Key alerts to set up:

```yaml
# High error rate
rate(ps_endpoint_errors_total[5m]) / rate(app_requests_total[5m]) > 0.05

# Excessive large responses
increase(ps_large_responses_total[10m]) > 10

# Unusual SSN access
rate(ps_sensitive_field_access_total{field="Ssn"}[1h]) > 100
```

## 📝 Log Correlation

Every request gets a correlation ID for debugging:

```
correlationId:"abc123-def456"
```

All logs include structured data:

- endpoint name
- user role
- operation type
- masked sensitive data

---

**Need Help?** See full documentation in `TELEMETRY_GUIDE.md`
