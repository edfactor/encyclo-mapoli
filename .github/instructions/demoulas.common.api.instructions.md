# Demoulas.Common.Api - REST API Development Package

**Package:** `Demoulas.Common.Api`  
**Namespace:** `Demoulas.Common.Api.Endpoints`, `Demoulas.Common.Api.Extensions`

This package provides comprehensive utilities for building REST APIs with FastEndpoints, including automatic telemetry, exception handling, middleware, and Swagger documentation.

## Table of Contents

1. [API Endpoint Base Classes](#api-endpoint-base-classes)
2. [Exception Extensions](#exception-extensions)
3. [Middleware Extensions](#middleware-extensions)
4. [Swagger Extensions](#swagger-extensions)
5. [Configuration Extensions](#configuration-extensions)

---

## API Endpoint Base Classes

**Namespace:** `Demoulas.Common.Api.Endpoints`

The Demoulas.Common.Api package provides a suite of base endpoint classes that automatically wrap your endpoint logic with comprehensive telemetry, metrics, and logging.

### Overview

All Demoulas endpoint base classes:

- **Automatically wrap your logic with telemetry** (distributed tracing, metrics, logging)
- **Track sensitive field access** for audit and compliance
- **Record request/response metrics** (size, timing, errors)
- **Handle exceptions** with proper activity recording
- **Support custom telemetry tags** via virtual methods
- **Sealed HandleAsync** to enforce consistent telemetry wrapping

### Available Base Classes

| Base Class                                  | Request | Response                       | Use Case                              |
| ------------------------------------------- | ------- | ------------------------------ | ------------------------------------- |
| `DemoulasEndpoint<TRequest, TResponse>`     | ✓       | ✓                              | Standard request/response endpoints   |
| `DemoulasRequestEndpoint<TRequest>`         | ✓       | ✗                              | Commands with no response payload     |
| `DemoulasResponseEndpoint<TResponse>`       | ✗       | ✓                              | Queries without request body          |
| `DemoulasResultResponseEndpoint<TResponse>` | ✗       | Results<Ok, NotFound, Problem> | Read-only endpoints with Result types |
| `DemoulasEndpoint`                          | ✗       | ✗                              | Fire-and-forget operations            |

### DemoulasEndpoint<TRequest, TResponse>

Base endpoint for scenarios with both request and response.

**Abstract Method:**

```csharp
protected abstract Task<TResponse> HandleRequestAsync(TRequest req, CancellationToken ct);
```

**Virtual Methods:**

```csharp
protected virtual void AddCustomTelemetryTags(Activity activity, TRequest request) { }
protected virtual string[] GetSensitiveFields(TRequest request) => Array.Empty<string>();
```

**Example:**

```csharp
public class GetCustomerEndpoint : DemoulasEndpoint<GetCustomerRequest, CustomerResponse>
{
    private readonly ICustomerService _customerService;

    public GetCustomerEndpoint(ICustomerService customerService) =>
        _customerService = customerService;

    public override void Configure()
    {
        Get("/customers/{id}");
        AllowAnonymous();
    }

    protected override async Task<CustomerResponse> HandleRequestAsync(
        GetCustomerRequest req, CancellationToken ct)
    {
        var customer = await _customerService.GetByIdAsync(req.Id, ct);
        return new CustomerResponse { /* ... */ };
    }

    protected override string[] GetSensitiveFields(GetCustomerRequest request) =>
        new[] { "SSN", "CreditCard" };

    protected override void AddCustomTelemetryTags(Activity activity, GetCustomerRequest request)
    {
        activity.SetTag("customer.id", request.Id);
    }
}
```

### Telemetry Features

All endpoint base classes automatically provide:

1. **Distributed Tracing**: Activities with endpoint.name, correlation.id, session.id, user tags
2. **Metrics Recording**: Request/response sizes, timing, sensitive field access counts
3. **Structured Logging**: Request processing, sensitive field access, exceptions
4. **Test Detection**: Skips expensive operations in test environments

### Best Practices

1. **Use the right base class**: Choose based on your endpoint's request/response pattern
2. **Never override HandleAsync**: It's sealed to ensure telemetry wrapping
3. **Implement HandleRequestAsync**: Put business logic here
4. **Mark sensitive fields**: Override `GetSensitiveFields()` for PII, SSN, etc.
5. **Add custom tags**: Use `AddCustomTelemetryTags()` for domain-specific tracking
6. **Let exceptions bubble**: Base class handles exception recording

---

## Exception Extensions

**Namespace:** `Demoulas.Common.Api.Extensions`

### ToProblemDetails()

Converts exceptions to HTTP ProblemDetails responses.

**Syntax:**

```csharp
// General exceptions
public static ProblemDetails ToProblemDetails(
    this Exception ex, string? title = null, string? details = null,
    string? instance = null, string? type = null, int? statusCode = null)

// Validation exceptions
public static ValidationProblemDetails ToProblemDetails(
    this ValidationException ex, string? title = null, string? details = null,
    string? instance = null, string? type = null)
```

**Status Code Mapping:**

- ValidationException / ArgumentException → 400
- KeyNotFoundException → 404
- UnauthorizedAccessException → 401
- NotImplementedException → 501
- UniqueConstraintException / ReferenceConstraintException → 409
- Others → 500

**Example:**

```csharp
try
{
    // operation
}
catch (ValidationException ex)
{
    return BadRequest(ex.ToProblemDetails(
        title: "Validation Failed",
        instance: "/api/customers"));
}
```

---

## Middleware Extensions

**Namespace:** `Demoulas.Common.Api.Extensions`

### UseCommonMiddleware()

Adds standard middleware including status codes, server timing, and version headers.

**Example:**

```csharp
var app = builder.Build();
app.UseCommonMiddleware();
app.MapControllers();
app.Run();
```

---

## Swagger Extensions

**Namespace:** `Demoulas.Common.Api.Extensions`

### AddSwagger()

Adds Swagger/OpenAPI documentation with security, versioning, and customization.

**Syntax:**

```csharp
public static WebApplicationBuilder AddSwagger(
    this WebApplicationBuilder builder,
    int version = 1,
    string title = "Demoulas Super Markets, Inc",
    string? description = null,
    ContactDetails? contactDetails = null,
    Action<DocumentSettings>? tagDescriptions = null,
    Action<AspNetCoreOpenApiDocumentGeneratorSettings>? documentSettingsAction = null,
    Action<OktaSettings>? oktaSettingsAction = null,
    List<IOperationProcessor>? operationProcessors = null,
    bool excludeNonFastEndpoints = false,
    ILogger? logger = null)
```

**Example:**

```csharp
builder.AddSwagger(
    version: 2,
    title: "Customer API",
    contactDetails: new ContactDetails
    {
        Name = "Support Team",
        Email = "support@example.com"
    });
```

---

## Configuration Extensions

**Namespace:** `Demoulas.Common.Api.Extensions`

### ConfigureDefaultEndpoints()

Configures comprehensive default settings including security, logging, telemetry, and compression.

**Example:**

```csharp
builder.ConfigureDefaultEndpoints(
    addOktaSecurity: true,
    meterNames: new[] { "MyApp.Metrics" });
```

---

## Best Practices

1. **Endpoint Base Classes**: Always inherit from Demoulas endpoint base classes for automatic telemetry
2. **Implement HandleRequestAsync**: Put business logic here, not in Configure() or HandleAsync()
3. **Sensitive Data**: Mark sensitive fields using GetSensitiveFields() for compliance
4. **Custom Telemetry**: Add domain-specific tags for better observability
5. **Exception Handling**: Use ToProblemDetails() for consistent error responses
6. **Middleware**: Apply UseCommonMiddleware() for standard features

## Import Statements

```csharp
using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Api.Endpoints;
using FastEndpoints;
```

---

**See Also:**

- [Main Documentation](./demoulas.common.instructions.md)
- [RESTful API Guidelines](./restful-api-guidelines.instructions.md)
- [Data Extensions](./demoulas.common.data.instructions.md)
- [Util Extensions](./demoulas.util.instructions.md)
- [FastEndpoints Documentation](https://fast-endpoints.com/)
