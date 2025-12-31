---
applyTo: "src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/**/*.*"
paths: "src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/**/*.*"
---

# FastEndpoints API Development Guide

**Project:** Demoulas Profit Sharing
**Framework:** .NET 10, FastEndpoints 7.x
**Last Updated:** December 2025

Comprehensive guide for building REST API endpoints. Combines architectural patterns with Zalando RESTful API guidelines.

---

## Table of Contents

1. [Quick Reference](#quick-reference)
2. [Architecture Overview](#architecture-overview)
3. [Base Endpoint Classes](#base-endpoint-classes)
4. [URL & Path Design](#url--path-design)
5. [HTTP Methods](#http-methods)
6. [Request & Response Formats](#request--response-formats)
7. [Result Pattern & HTTP Mapping](#result-pattern--http-mapping)
8. [Telemetry (MANDATORY)](#telemetry-mandatory)
9. [Validation](#validation)
10. [Security & Authorization](#security--authorization)
11. [Pagination](#pagination)
12. [Error Handling](#error-handling)
13. [Groups & Route Organization](#groups--route-organization)
14. [CSV Report Endpoints](#csv-report-endpoints)
15. [Creating New Endpoints](#creating-new-endpoints)
16. [New Endpoint Checklist](#new-endpoint-checklist)

---

## Quick Reference

### DO

- Use **resource-oriented design** (nouns, not verbs): `/distributions` not `/create-distribution`
- Use **kebab-case** for paths: `/profit-sharing-distributions`
- Use **plural** resource names: `/distributions`, `/beneficiaries`
- Use **snake_case** for JSON properties and query parameters
- Return **JSON objects** as top-level (not arrays)
- Implement **pagination** on all collection endpoints
- Add **comprehensive telemetry** with business metrics
- Inherit from **base endpoint classes** for automatic telemetry
- Declare **sensitive fields** in telemetry calls

### DON'T

- Use verbs in URLs: ~~`POST /update-enrollment`~~ → `PUT /profit-years/{id}`
- Use trailing slashes: ~~`/distributions/`~~ → `/distributions`
- Return arrays as top-level: ~~`[{...}]`~~ → `{"results": [{...}]}`
- Expose stack traces in error responses
- Trust client-provided roles (always validate server-side)
- Access `DbContext` directly from endpoints (use services)
- Skip telemetry or input validation

---

## Architecture Overview

```
HTTP Request
    ↓
FastEndpoints Middleware
    ↓
TelemetryProcessor (Pre-Process)
    ↓
Endpoint.ExecuteAsync()
    ├─ RecordRequestMetrics
    ├─ Call Service Layer (returns Result<T>)
    ├─ Record Business Metrics
    ├─ RecordResponseMetrics
    └─ Return typed result
    ↓
TelemetryProcessor (Post-Process)
    ↓
HTTP Response
```

**Layer Responsibilities:**

| Layer     | Responsibility                                  |
| --------- | ----------------------------------------------- |
| Endpoints | HTTP concerns, route config, telemetry, mapping |
| Services  | Business logic, data orchestration, `Result<T>` |
| Data      | EF Core queries, entity mapping, persistence    |
| Common    | DTOs, domain errors, validators                 |

**Critical:** Endpoints MUST NOT access `DbContext` directly. All data operations go through services returning `Result<T>`.

---

## Base Endpoint Classes

All endpoints inherit from base classes in `Base/ProfitSharingEndpoint.cs`:

| Base Class                                   | Request | Response                     | Use Case                     |
| -------------------------------------------- | ------- | ---------------------------- | ---------------------------- |
| `ProfitSharingEndpoint<TReq, TResp>`         | ✓       | ✓                            | Standard request/response    |
| `ProfitSharingRequestEndpoint<TReq>`         | ✓       | ✗                            | Commands with no response    |
| `ProfitSharingResponseEndpoint<TResp>`       | ✗       | ✓                            | Queries without request body |
| `ProfitSharingResultResponseEndpoint<TResp>` | ✗       | Results<Ok,NotFound,Problem> | Read-only with typed results |

**Example:**

```csharp
public sealed class GetMemberEndpoint : ProfitSharingEndpoint<GetMemberRequest, Results<Ok<MemberDetails>, NotFound, ProblemHttpResult>>
{
    private readonly IMemberService _service;
    private readonly ILogger<GetMemberEndpoint> _logger;

    public GetMemberEndpoint(IMemberService service, ILogger<GetMemberEndpoint> logger)
        : base(Navigation.Constants.MasterInquiry)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("members/{id}");
        Group<MasterInquiryGroup>();
    }

    public override Task<Results<Ok<MemberDetails>, NotFound, ProblemHttpResult>> ExecuteAsync(
        GetMemberRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.GetMemberAsync(req.Id, ct);
            return result.ToHttpResult(Error.MemberNotFound);
        }, "Ssn", "OracleHcmId");
    }
}
```

---

## URL & Path Design

### Resource-Oriented (Nouns, Not Verbs)

| Wrong                       | Correct                             | Method |
| --------------------------- | ----------------------------------- | ------ |
| `POST /update-enrollment`   | `PUT /profit-years/{id}/enrollment` | PUT    |
| `POST /create-disbursement` | `POST /beneficiary-disbursements`   | POST   |
| `GET /search-members`       | `GET /members?name=Smith`           | GET    |

### Kebab-Case for Path Segments

```csharp
// Correct
Get("profit-year-distributions");
Post("beneficiary-disbursements");

// Wrong
Get("profitYearDistributions");  // camelCase
Get("profit_year_distributions"); // snake_case
```

### Plural Resource Names

```
/distributions         # Collection
/distributions/{id}    # Single item
/members/{id}/enrollments  # Nested collection
```

### Limit Nesting to 3 Levels

```
✅ /profit-years/{id}/members/{id}/enrollments
❌ /profit-years/{id}/members/{id}/enrollments/{id}/history/{id}
```

---

## HTTP Methods

| Method | Use Case         | Idempotent | Cacheable |
| ------ | ---------------- | ---------- | --------- |
| GET    | Read resource    | Yes        | Yes       |
| POST   | Create resource  | No\*       | No        |
| PUT    | Replace resource | Yes        | No        |
| PATCH  | Partial update   | No\*       | No        |
| DELETE | Remove resource  | Yes        | No        |

\*Can be designed for idempotency using idempotency keys.

**Response Codes:**

| Method | Success     | Not Found | Validation Error |
| ------ | ----------- | --------- | ---------------- |
| GET    | 200 OK      | 404       | 400              |
| POST   | 201 Created | -         | 400              |
| PUT    | 200/204     | 404       | 400              |
| DELETE | 200/204     | 404       | -                |

---

## Request & Response Formats

### Snake_Case for JSON (Configured Globally)

```csharp
// C# class (PascalCase)
public class MemberDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
}

// JSON output (snake_case)
{ "id": 123, "first_name": "John" }
```

### Snake_Case for Query Parameters

```
GET /members?profit_year=2024&member_type=employee
```

### Always Return Objects (Not Arrays)

```json
// Wrong
[{ "id": 1 }, { "id": 2 }]

// Correct
{ "results": [{ "id": 1 }, { "id": 2 }] }
```

---

## Result Pattern & HTTP Mapping

Services return `Result<T>`, endpoints convert to HTTP results:

```csharp
// Service returns Result<T>
public async Task<Result<MemberDetails>> GetMemberAsync(int id, CancellationToken ct)
{
    var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
    return member is null
        ? Result<MemberDetails>.Failure(Error.MemberNotFound)
        : Result<MemberDetails>.Success(_mapper.ToDto(member));
}

// Endpoint maps to HTTP
var result = await _service.GetMemberAsync(req.Id, ct);
return result.ToHttpResult(Error.MemberNotFound);
```

**Mapping:**

- `Result.Success(value)` → `200 OK`
- `Result.Failure(Error.NotFound)` → `404 Not Found`
- `Result.Failure(otherError)` → `500 Problem`

**For validation errors, use `ToHttpResultWithValidation()`:**

```csharp
return result.ToHttpResultWithValidation(Error.DistributionNotFound, Error.BadgeNumberNotFound);
```

---

## Telemetry (MANDATORY)

All endpoints MUST implement telemetry using `TelemetryExtensions`.

### Pattern 1: ExecuteWithTelemetry (Recommended)

```csharp
public override Task<Results<...>> ExecuteAsync(TRequest req, CancellationToken ct)
{
    return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
    {
        var result = await _service.ProcessAsync(req, ct);

        // Record business metrics
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "employee-lookup"),
            new("endpoint", nameof(MyEndpoint)));

        // Record counts if applicable
        EndpointTelemetry.RecordCountsProcessed.Record(result.Count,
            new("record_type", "employee"),
            new("endpoint", nameof(MyEndpoint)));

        return result.ToHttpResult(Error.NotFound);
    }, "Ssn", "OracleHcmId"); // Declare sensitive fields
}
```

### Pattern 2: Manual Telemetry (Advanced)

```csharp
public override async Task HandleAsync(TRequest req, CancellationToken ct)
{
    using var activity = this.StartEndpointActivity(HttpContext);
    try
    {
        this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");
        var result = await _service.ProcessAsync(req, ct);
        this.RecordResponseMetrics(HttpContext, _logger, result);
        await Send.OkAsync(result, ct);
    }
    catch (Exception ex)
    {
        this.RecordException(HttpContext, _logger, ex, activity);
        throw;
    }
}
```

### Logger Injection (REQUIRED)

```csharp
public MyEndpoint(IMyService service, ILogger<MyEndpoint> logger)
    : base(Navigation.Constants.SomeId)
{
    _logger = logger; // MUST inject for telemetry
}
```

### Sensitive Field Declarations

Always declare sensitive fields accessed by the endpoint:

```csharp
this.ExecuteWithTelemetry(..., "Ssn", "OracleHcmId", "Salary");
```

Common sensitive fields: `Ssn`, `OracleHcmId`, `BadgeNumber`, `Salary`, `BeneficiaryInfo`

---

## Validation

### FluentValidation (in Common.Validators)

**CRITICAL:** Validators MUST be in `Demoulas.ProfitSharing.Common.Validators`, NOT in Endpoints.

```csharp
namespace Demoulas.ProfitSharing.Common.Validators;

public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
{
    public PaginationRequestValidator()
    {
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize must be between 1 and 1000.");

        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.BadgeNumbers)
            .Must(b => b == null || !b.All(n => n == 0))
            .WithMessage("All-zero badge numbers not allowed.");
    }
}
```

### Required Validations

- Numeric ranges (min/max)
- String lengths
- Collection sizes (max 1000 items)
- Date ranges
- Enum validation
- Degenerate query guards

---

## Security & Authorization

### Policy-Based Authorization

```csharp
public override void Configure()
{
    Get("distributions");
    Policies(Policy.CanViewDistributions);
    Group<DistributionsGroup>();
}
```

### Server-Side Validation (CRITICAL)

```csharp
// WRONG: Trust client header
var roles = req.Headers["x-impersonating-roles"];

// CORRECT: Re-validate server-side
var authenticatedUserId = HttpContext.User.FindFirst("sub")?.Value;
var allowedRoles = await _authService.GetAllowedRolesAsync(authenticatedUserId, ct);
if (!requestedRoles.All(r => allowedRoles.Contains(r)))
    throw new UnauthorizedAccessException();
```

---

## Pagination

All collection endpoints MUST support pagination:

```csharp
public class PaginatedResponseDto<T>
{
    public List<T> Results { get; set; }
    public int TotalCount { get; set; }
    public string? Cursor { get; set; }
    public bool HasMore { get; set; }
}
```

**Query parameters:** `limit` (default 20, max 1000), `cursor`

**Prefer cursor-based pagination** over offset-based for stability with data changes.

---

## Error Handling

### Problem JSON Format (RFC 7807)

```json
{
  "type": "https://api.example.com/errors/validation-error",
  "title": "Validation Failed",
  "status": 400,
  "detail": "The 'profit_year' field must be between 2000 and 2099"
}
```

### Never Expose Stack Traces

```csharp
// Log internally
_logger.LogError(ex, "Error processing {MemberId}", memberId);

// Return generic message
return Results.Problem(
    detail: "An unexpected error occurred.",
    statusCode: 500);
```

---

## Groups & Route Organization

Groups provide route prefixing and shared configuration:

```csharp
public sealed class DistributionsGroup : GroupBase
{
    protected override string Route => "distributions";
    protected override string RouteName => "Distributions";
}
```

**Available Groups:**

| Group              | Route             | Purpose                     |
| ------------------ | ----------------- | --------------------------- |
| LookupGroup        | `/lookup`         | Reference data              |
| MasterInquiryGroup | `/master-inquiry` | Employee/beneficiary search |
| BeneficiariesGroup | `/beneficiaries`  | Beneficiary CRUD            |
| DistributionGroup  | `/distributions`  | Distribution management     |
| YearEndGroup       | `/year-end`       | Year-end operations         |
| NavigationGroup    | `/navigation`     | Navigation tree             |

---

## CSV Report Endpoints

Extend `EndpointWithCsvBase` for reports supporting JSON and CSV:

```csharp
public class MyReportEndpoint : EndpointWithCsvBase<MyRequest, MyResponse, MyReportEndpoint.MyMap>
{
    public override string ReportFileName => "My Report";

    public override async Task<ReportResponseBase<MyResponse>> GetResponse(MyRequest req, CancellationToken ct)
    {
        // Generate report...
    }

    public sealed class MyMap : ClassMap<MyResponse>
    {
        public MyMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE");
            Map(m => m.FullName).Index(1).Name("NAME");
        }
    }
}
```

Content negotiation:

- `Accept: application/json` → JSON response
- `Accept: text/csv` → CSV file download

---

## Creating New Endpoints

### Step 1: Define DTOs

```csharp
// Request
public sealed record MyFeatureRequest
{
    public int ProfitYear { get; init; }
    public int? BadgeNumber { get; init; }
}

// Response
public sealed record MyFeatureResponse
{
    public int Id { get; init; }
    [MaskSensitive]
    public string FullName { get; init; } = string.Empty;
}
```

### Step 2: Create Validator (in Common.Validators)

```csharp
namespace Demoulas.ProfitSharing.Common.Validators;

public sealed class MyFeatureRequestValidator : AbstractValidator<MyFeatureRequest>
{
    public MyFeatureRequestValidator()
    {
        RuleFor(x => x.ProfitYear).InclusiveBetween(2000, 2100);
    }
}
```

### Step 3: Create Endpoint

```csharp
public sealed class MyFeatureEndpoint : ProfitSharingEndpoint<MyFeatureRequest, Results<Ok<MyFeatureResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IMyFeatureService _service;
    private readonly ILogger<MyFeatureEndpoint> _logger;

    public MyFeatureEndpoint(IMyFeatureService service, ILogger<MyFeatureEndpoint> logger)
        : base(Navigation.Constants.MyFeatureId)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("my-feature/search");
        Summary(s =>
        {
            s.Summary = "Search for my feature data";
            s.ExampleRequest = new MyFeatureRequest { ProfitYear = 2024 };
        });
        Group<MyFeatureGroup>();
    }

    public override Task<Results<Ok<MyFeatureResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(
        MyFeatureRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.GetDataAsync(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "my-feature-search"),
                new("endpoint", nameof(MyFeatureEndpoint)));

            return result.ToHttpResult(Error.MyFeatureNotFound);
        }, "Ssn");
    }
}
```

---

## New Endpoint Checklist

### Design

- [ ] Resource-based URL (nouns, not verbs)
- [ ] Correct HTTP method (GET/POST/PUT/DELETE)
- [ ] Kebab-case path segments
- [ ] Plural resource names
- [ ] ≤3 nesting levels

### Implementation

- [ ] Inherits from base endpoint class
- [ ] Logger injected for telemetry
- [ ] Telemetry implemented (`ExecuteWithTelemetry` or manual)
- [ ] Sensitive fields declared
- [ ] Business metrics recorded
- [ ] Result pattern used (`ToHttpResult`)
- [ ] Validator in `Common.Validators`
- [ ] Input validation complete (ranges, lengths, degenerate guards)

### API Format

- [ ] Snake_case JSON properties
- [ ] Snake_case query parameters
- [ ] Returns object (not array) at top level
- [ ] Pagination on collections
- [ ] Problem JSON for errors

### Security

- [ ] Policy applied via `Policies()`
- [ ] Not `AllowAnonymous`
- [ ] Server-side role validation
- [ ] No PII in logs (masked in telemetry)
- [ ] No stack traces in responses

### Documentation

- [ ] `Summary()` configured
- [ ] Example request provided
- [ ] Response examples for each status code

### Testing

- [ ] Unit tests (happy path, error cases)
- [ ] Authorization tests
- [ ] Validation tests

---

## References

- [Zalando RESTful API Guidelines](https://opensource.zalando.com/restful-api-guidelines/)
- [FastEndpoints Documentation](https://fast-endpoints.com/)
- Project telemetry: `TELEMETRY_GUIDE.md`, `TELEMETRY_QUICK_REFERENCE.md`
- Security: `security.instructions.md`
