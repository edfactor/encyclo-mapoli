# Part 5c: Telemetry Customization & Business Metrics

**Estimated Time:** 15 minutes  
**Prerequisites:** [Part 5b Complete](./05b-security-services.md)  
**Next:** [Part 6a: Health Checks](./06a-health-checks.md)

---

## 🎯 Overview

**Demoulas.Common.Api provides automatic telemetry** through:

- `ConfigureDefaultEndpoints()` - OpenTelemetry setup with ASP.NET Core, HttpClient instrumentation
- `DemoulasEndpoint<TRequest, TResponse>` base classes - Automatic activity wrapping, metrics, logging
- `UseCommonMiddleware()` - Status codes, server timing, version headers

**This guide covers:**

- ✅ Choosing endpoint base classes for automatic telemetry
- ✅ Adding custom tags for domain-specific tracking
- ✅ Marking sensitive fields for security auditing
- ✅ Recording business metrics for operational insights
- ✅ Optional EF Core instrumentation for database tracing

**What you DON'T need to build:**

- ❌ Custom ActivitySource - Built into Common.Api
- ❌ Custom middleware for instrumentation - DemoulasEndpoint handles it
- ❌ Manual activity creation - Sealed HandleAsync enforces consistency

---

## � Step 1: Configure OpenTelemetry in Program.cs

### Adopt ConfigureDefaultEndpoints()

**Location:** `Program.cs` (after service registrations, before `var app = builder.Build()`)

```csharp
// After AddSecurityPoliciesAndHandlers(), AddProjectServices(), etc.

// Configure Common.Api telemetry with multiple meters for granular filtering
builder.ConfigureDefaultEndpoints(
    addOktaSecurity: false,  // If you already configured Okta separately
    meterNames: [
        "Demoulas.Smart.StoreOrdering.Endpoints",
        "Demoulas.Smart.StoreOrdering.Services",
        "Demoulas.Smart.StoreOrdering.Data"
    ],
    activitySourceNames: [
        "Demoulas.Smart.StoreOrdering.Endpoints"
    ]);

// Optional: Add EF Core instrumentation (if using Entity Framework)
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddEntityFrameworkCoreInstrumentation(options =>
        {
            options.SetDbStatementForText = true;
            options.SetDbStatementForStoredProcedure = true;
        }));

var app = builder.Build();
```

**What ConfigureDefaultEndpoints() provides:**

- ✅ OpenTelemetry with ActivitySource registration
- ✅ ASP.NET Core instrumentation (requests, responses)
- ✅ HttpClient instrumentation (outbound calls)
- ✅ Resource attributes (service name, environment)
- ✅ Okta JWT authentication (if addOktaSecurity = true)
- ✅ Structured logging with Serilog
- ✅ Security headers (HSTS, X-Frame-Options, etc.)
- ✅ Request/response compression
- ✅ API versioning support

---

## 🏗️ Step 2: Endpoint Base Class Selection

### Choose the Right DemoulasEndpoint Variant

| Base Class                                  | Request | Response                       | Use Case                                   |
| ------------------------------------------- | ------- | ------------------------------ | ------------------------------------------ |
| `DemoulasEndpoint<TRequest, TResponse>`     | ✓       | ✓                              | Standard request/response endpoints        |
| `DemoulasRequestEndpoint<TRequest>`         | ✓       | ✗                              | Commands with no response (204 No Content) |
| `DemoulasResponseEndpoint<TResponse>`       | ✗       | ✓                              | Queries without request body               |
| `DemoulasResultResponseEndpoint<TResponse>` | ✗       | Results<Ok, NotFound, Problem> | Read-only endpoints with Result types      |
| `DemoulasEndpoint`                          | ✗       | ✗                              | Fire-and-forget operations                 |

### Standard Request/Response Endpoint

```csharp
using Demoulas.Common.Api.Endpoints;
using MySolution.Services;
using MySolution.Contracts;

namespace MySolution.Endpoints;

public class GetUserInfoEndpoint : DemoulasEndpoint<GetUserInfoRequest, UserInfoResponse>
{
    private readonly IUserService _service;

    public GetUserInfoEndpoint(IUserService service) => _service = service;

    public override void Configure()
    {
        Get("/users/{id}");
        Policies(Policy.CanViewUsers);
    }

    protected override async Task<UserInfoResponse> HandleRequestAsync(
        GetUserInfoRequest req, CancellationToken ct)
    {
        // Telemetry is AUTOMATIC!
        // - Activity started with endpoint.name, correlation.id, user tags
        // - Request/response metrics recorded
        // - Exceptions automatically captured

        var user = await _service.GetByIdAsync(req.Id, ct);
        return new UserInfoResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}
```

**Key Points:**

- ✅ Inherit from `DemoulasEndpoint<TRequest, TResponse>`
- ✅ Implement `HandleRequestAsync()` (NOT `ExecuteAsync()`)
- ✅ Telemetry is automatic - no manual activity creation needed
- ✅ Sealed `HandleAsync()` prevents bypassing telemetry

---

## 🏷️ Step 3: Adding Custom Telemetry Tags

### Override AddCustomTelemetryTags()

Add domain-specific tags for operational insights:

```csharp
public class SubmitOrderEndpoint : DemoulasEndpoint<SubmitOrderRequest, OrderResponse>
{
    private readonly IOrderService _service;

    public SubmitOrderEndpoint(IOrderService service) => _service = service;

    public override void Configure()
    {
        Post("/orders");
        Policies(Policy.CanSubmitOrders);
    }

    protected override async Task<OrderResponse> HandleRequestAsync(
        SubmitOrderRequest req, CancellationToken ct)
    {
        var order = await _service.SubmitAsync(req, ct);
        return order;
    }

    // Add domain-specific tags to telemetry
    protected override void AddCustomTelemetryTags(Activity activity, SubmitOrderRequest request)
    {
        activity.SetTag("order.store_id", request.StoreId);
        activity.SetTag("order.item_count", request.Items.Count);
        activity.SetTag("order.total_amount", request.TotalAmount);
        activity.SetTag("order.payment_method", request.PaymentMethod);
    }
}
```

**When to use custom tags:**

- ✅ Domain identifiers (store ID, order ID, user ID)
- ✅ Operation categories (bulk vs single, admin vs user)
- ✅ Business metrics (order amount, item count, discount applied)
- ✅ Feature flags or experiment IDs

---

## 🔒 Step 4: Sensitive Field Tracking

### Override GetSensitiveFields()

Mark PII access for security auditing and compliance:

```csharp
public class GetEmployeeEndpoint : DemoulasEndpoint<GetEmployeeRequest, EmployeeResponse>
{
    private readonly IEmployeeService _service;

    public GetEmployeeEndpoint(IEmployeeService service) => _service = service;

    protected override async Task<EmployeeResponse> HandleRequestAsync(
        GetEmployeeRequest req, CancellationToken ct)
    {
        var employee = await _service.GetByIdAsync(req.Id, ct);
        return employee;
    }

    // Declare sensitive fields accessed for audit trail
    protected override string[] GetSensitiveFields(GetEmployeeRequest request) =>
        new[] { "SSN", "DateOfBirth", "Email", "HomeAddress" };
}
```

**Why it matters:**

- ✅ **Security Auditing** - Track who accessed PII and when
- ✅ **Compliance** - GDPR, FISMA Moderate, PCI DSS requirements
- ✅ **Anomaly Detection** - Alert on unusual access patterns
- ✅ **Incident Response** - Trace data breaches

**Common sensitive fields:**

- SSN, Tax ID, Passport Number
- Credit Card, Bank Account
- Email, Phone Number
- Date of Birth, Home Address
- Medical Records, Biometric Data

---

## 📊 Step 5: Recording Business Metrics

### Use EndpointTelemetry.BusinessOperationsTotal

Track business operations for operational insights:

```csharp
public class ProcessPaymentEndpoint : DemoulasEndpoint<PaymentRequest, PaymentResponse>
{
    private readonly IPaymentService _service;

    public ProcessPaymentEndpoint(IPaymentService service) => _service = service;

    protected override async Task<PaymentResponse> HandleRequestAsync(
        PaymentRequest req, CancellationToken ct)
    {
        var result = await _service.ProcessAsync(req, ct);

        // Record business metric for operational monitoring
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "payment-processed"),
            new("endpoint", nameof(ProcessPaymentEndpoint)),
            new("payment.method", req.PaymentMethod),
            new("payment.status", result.Status));

        return result;
    }
}
```

**Common business operations:**

- `user-info-lookup`, `user-created`, `user-updated`
- `order-submission`, `order-cancelled`, `order-fulfilled`
- `store-list`, `store-created`, `store-updated`
- `report-generation`, `data-export`, `admin-action`
- `payment-processed`, `refund-issued`

**When to record business metrics:**

- ✅ Every significant business operation (create, update, delete)
- ✅ High-value transactions (orders, payments, refunds)
- ✅ Security-sensitive actions (permission changes, admin operations)
- ✅ Operations with compliance requirements (PII access, data exports)

---

## 🗄️ Step 6: Database Telemetry (EF Core)

### Aspire Automatic Configuration

**If using .NET Aspire (recommended):** EF Core instrumentation is **automatically configured** by Aspire's OpenTelemetry integration. No additional configuration needed in Program.cs.

Aspire automatically includes:

- ✅ SQL query text tracing (parameterized)
- ✅ Query duration and timing
- ✅ Connection pool statistics
- ✅ Transaction tracing

### Manual Configuration (Non-Aspire Projects Only)

If **NOT** using Aspire, manually add EF Core instrumentation:

```csharp
// In Program.cs, AFTER ConfigureDefaultEndpoints()

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddEntityFrameworkCoreInstrumentation(options =>
        {
            options.SetDbStatementForText = true;          // Show SQL for text queries
            options.SetDbStatementForStoredProcedure = true; // Show SQL for stored procs
            options.EnrichWithIDbCommand = (activity, command) =>
            {
                activity.SetTag("db.operation", command.CommandType);
                activity.SetTag("db.elapsed_ms", command.CommandTimeout);
            };
        }));
```

**Performance Considerations:**

- ⚠️ `SetDbStatementForText = true` adds overhead (~5-10% for heavy queries)
- ⚠️ Enable selectively (e.g., dev/staging only) if performance critical
- ✅ Always parameterized (no SQL injection risk)

---

## 🧪 Step 7: Testing & Verification

### Manual Testing in Development

1. **Run application** in Development mode
2. **Check console logs** for structured output:

```log
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: MySolution.Endpoints.GetUserInfoEndpoint[0]
      Request started: GET /api/users/123
      TraceId: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-00
      SpanId: 00f067aa0ba902b7
      SessionId: 550e8400-e29b-41d4-a716-446655440000
info: MySolution.Endpoints.GetUserInfoEndpoint[0]
      Request completed: GET /api/users/123 - 200 OK in 45ms
```

3. **Verify telemetry fields**:
   - ✅ `TraceId` present (distributed tracing)
   - ✅ `SpanId` present (activity ID)
   - ✅ `SessionId` present (user journey correlation)
   - ✅ Custom tags visible in logs

### OpenTelemetry Collector (Optional - Advanced)

For local testing with Jaeger UI:

#### docker-compose.yml

```yaml
services:
  jaeger:
    image: jaegertracing/all-in-one:latest
    ports:
      - "16686:16686" # Jaeger UI
      - "4317:4317" # OTLP gRPC receiver
      - "4318:4318" # OTLP HTTP receiver
```

#### appsettings.Development.json

```json
{
  "OpenTelemetry": {
    "Endpoint": "http://localhost:4317",
    "Protocol": "Grpc"
  }
}
```

**Run Jaeger:**

```bash
docker-compose up -d
```

**View traces:** Open `http://localhost:16686` in browser

---

## ✅ Validation Checklist - Part 5c

- [ ] **ConfigureDefaultEndpoints()** called in Program.cs
- [ ] **Multiple meters** configured for granular filtering
- [ ] **EF Core instrumentation** added (if using Entity Framework)
- [ ] **UseCommonMiddleware()** added to pipeline
- [ ] **All endpoints** inherit from DemoulasEndpoint base classes
- [ ] **HandleRequestAsync()** implemented (NOT ExecuteAsync)
- [ ] **GetSensitiveFields()** overridden where PII accessed
- [ ] **AddCustomTelemetryTags()** overridden for domain tracking
- [ ] **BusinessOperationsTotal** recorded for key operations
- [ ] **Telemetry verified** in logs (TraceId, SpanId visible)

---

## 🎓 Key Takeaways - Part 5c

1. **Common.Api handles infrastructure** - No custom ActivitySource or middleware needed
2. **DemoulasEndpoint enforces consistency** - Sealed HandleAsync prevents telemetry bypass
3. **Focus on business value** - Custom tags, sensitive fields, business metrics
4. **Test environment support** - Automatic detection, no-op in tests
5. **Compliance ready** - Sensitive field tracking for GDPR, FISMA, PCI DSS

---

## 📚 Related Documentation

- [Demoulas.Common.Api Instructions](../../.github/instructions/demoulas.common.api.instructions.md)
- [Part 3: API Bootstrap & Middleware](./03-api-bootstrap-middleware.md)
- [Part 6a: Health Checks](./06a-health-checks.md)

---

**Next:** [Part 6a: Health Checks](./06a-health-checks.md)
