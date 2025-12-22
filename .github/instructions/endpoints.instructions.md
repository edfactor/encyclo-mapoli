---
applyTo: "src/services/src/Demoulas.ProfitSharing.Endpoints/**/*.*"
paths: "src/services/src/Demoulas.ProfitSharing.Endpoints/**/*.*"
---

# ENDPOINTS.md

Comprehensive technical documentation for the FastEndpoints-based API layer in the Demoulas Profit Sharing application.

**Last Updated:** 2025-10-23
**Project:** Demoulas Profit Sharing
**Framework:** .NET 9, FastEndpoints 5.x

---

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Project Structure](#project-structure)
4. [Base Endpoint Classes](#base-endpoint-classes)
5. [Endpoint Categories](#endpoint-categories)
6. [Telemetry Integration (MANDATORY)](#telemetry-integration-mandatory)
7. [Result Pattern & HTTP Mapping](#result-pattern--http-mapping)
8. [Validation Patterns](#validation-patterns)
9. [Groups & Route Organization](#groups--route-organization)
10. [CSV Report Endpoints](#csv-report-endpoints)
11. [Authentication & Authorization](#authentication--authorization)
12. [Creating New Endpoints](#creating-new-endpoints)
13. [Testing Endpoints](#testing-endpoints)
14. [Common Patterns & Best Practices](#common-patterns--best-practices)
15. [Reference Examples](#reference-examples)

---

## Overview

The `Demoulas.ProfitSharing.Endpoints` project contains all HTTP API endpoints for the profit sharing application, built on **FastEndpoints** rather than traditional ASP.NET Core controllers. This provides:

- **Type-safe minimal API style** with automatic model binding and validation
- **Explicit request/response contracts** with no hidden magic
- **Built-in OpenAPI/Swagger generation** with detailed documentation
- **Comprehensive telemetry integration** via `TelemetryExtensions` and `TelemetryProcessor`
- **Grouped endpoint organization** for logical route prefixing
- **Result-oriented error handling** with domain `Result<T>` to HTTP status mapping

**Key Statistics:**

- **100+ endpoints** across 13+ functional areas
- **~3,900 lines** of endpoint code
- **Standardized base classes** for consistency
- **Mandatory telemetry** on all endpoints

---

## Architecture

### Conceptual Flow

```
HTTP Request
    ↓
FastEndpoints Middleware
    ↓
TelemetryProcessor (Pre-Process) ← Starts Activity, Records Request Metrics
    ↓
Endpoint.ExecuteAsync() or HandleAsync()
    ↓
├─ ExecuteWithTelemetry wrapper (Recommended)
│  ├─ RecordRequestMetrics (sensitive fields declared)
│  ├─ Business Logic (calls Service Layer)
│  ├─ BusinessOperationsTotal.Add (domain metrics)
│  ├─ RecordResponseMetrics
│  └─ Return typed result
│
└─ Manual telemetry (Advanced control)
   ├─ StartEndpointActivity
   ├─ RecordRequestMetrics
   ├─ try { business logic } catch { RecordException }
   ├─ RecordResponseMetrics
   └─ Return typed result
    ↓
TelemetryProcessor (Post-Process) ← Records Duration, Errors, User Activity
    ↓
HTTP Response (Ok<T>, NotFound, ProblemHttpResult, etc.)
```

### Layer Responsibilities

| Layer                | Responsibility                                                   | Examples                                                 |
| -------------------- | ---------------------------------------------------------------- | -------------------------------------------------------- |
| **Endpoints**        | HTTP concerns, route config, request/response mapping, telemetry | `StateListEndpoint.cs`, `MasterInquirySearchEndpoint.cs` |
| **Services**         | Business logic, data orchestration, domain `Result<T>`           | `IMasterInquiryService`, `IBeneficiaryService`           |
| **Data**             | EF Core queries, entity mapping, persistence                     | `ProfitSharingDbContext`, Repositories                   |
| **Common.Contracts** | DTOs, domain errors, `Result<T>` type                            | `MemberDetails`, `Error.MemberNotFound`                  |

**Critical Rule:** Endpoints MUST NOT directly access `DbContext` or `IDbContextFactory`. All data operations go through services that return `Result<T>`.

---

## Project Structure

```
Demoulas.ProfitSharing.Endpoints/
├── Base/
│   ├── ProfitSharingEndpoint.cs          # Generic base classes with NavigationId
│   ├── EndpointWithCsvBase.cs            # CSV export base for reports
│   └── EndpointWithCsvTotalsBase.cs      # CSV with totals footer
├── Endpoints/
│   ├── Adjustments/                      # Profit detail merge operations
│   ├── Beneficiaries/                    # CRUD for beneficiaries
│   ├── BeneficiaryInquiry/               # Beneficiary search/lookup
│   ├── Distributions/                    # Distribution management
│   ├── ItOperations/                     # IT/DevOps tools (freeze, metadata)
│   ├── Lookups/                          # Reference data endpoints
│   ├── Master/                           # Master inquiry (employee/beneficiary search)
│   ├── Military/                         # Military contribution records
│   ├── Navigations/                      # Navigation tree and status
│   ├── ProfitDetails/                    # Profit detail reversals
│   ├── Reports/
│   │   ├── Adhoc/                        # Ad-hoc reports
│   │   ├── PayBen/                       # Pay-Ben report
│   │   └── YearEnd/                      # Year-end reports (eligibility, frozen, post-frozen, etc.)
│   ├── Validation/                       # Validation endpoints (checksums, transfers)
│   └── YearEnd/                          # Year-end operations (enrollment, final run)
├── Extensions/
│   └── TelemetryExtensions.cs            # MANDATORY telemetry patterns
├── Groups/
│   ├── LookupGroup.cs                    # Route: /lookup
│   ├── MasterInquiryGroup.cs             # Route: /master-inquiry
│   ├── BeneficiariesGroup.cs             # Route: /beneficiaries
│   ├── YearEndGroup.cs                   # Route: /year-end
│   └── ... (13+ groups total)
├── Processors/
│   └── TelemetryProcessor.cs             # Global pre/post processor for all endpoints
├── Validation/
│   └── IdsRequestValidator.cs            # FluentValidation example
├── HealthCheck/
│   └── EnvironmentHealthCheck.cs         # Health check endpoint
└── TypeConverters/
    └── YearMonthDayTypeConverter.cs      # CSV type converters
```

**Directory Conventions:**

- One endpoint per file, named `{Operation}{Entity}Endpoint.cs` (e.g., `CreateBeneficiaryEndpoint.cs`)
- Group endpoints by business domain, not HTTP verb
- Validators colocated in `/Validation` folder (shared) or inline as nested classes

---

## Base Endpoint Classes

All endpoints inherit from one of these base classes defined in `Base/ProfitSharingEndpoint.cs`. Each base class implements `IHasNavigationId` to support telemetry, authorization, and route tagging.

### IHasNavigationId

```csharp
/// <summary>
/// Common contract exposing a NavigationId for endpoints.
/// </summary>
public interface IHasNavigationId
{
    short NavigationId { get; }
}
```

Every endpoint must provide a `NavigationId` (from `Navigation.Constants`) to correlate with the navigation tree, enforce permissions, and tag telemetry.

### Base Class Variants

#### 1. ProfitSharingEndpoint<TRequest, TResponse>

For endpoints with both request and response.

```csharp
public abstract class ProfitSharingEndpoint<TRequest, TResponse>
    : Endpoint<TRequest, TResponse>, IHasNavigationId
    where TRequest : notnull
    where TResponse : notnull
{
    protected ProfitSharingEndpoint(short navigationId) { NavigationId = navigationId; }
    public short NavigationId { get; }
}
```

**Usage:**

```csharp
public class GetNavigationEndpoint : ProfitSharingEndpoint<NavigationRequestDto, NavigationResponseDto>
{
    public GetNavigationEndpoint(INavigationService service, ILogger<GetNavigationEndpoint> logger)
        : base(Navigation.Constants.Unknown)
    { ... }
}
```

#### 2. ProfitSharingRequestEndpoint<TRequest>

For endpoints with request but no response payload (commands returning 204 No Content or 200 OK with empty body).

```csharp
public abstract class ProfitSharingRequestEndpoint<TRequest>
    : Endpoint<TRequest>, IHasNavigationId
    where TRequest : notnull
```

**Usage:**

```csharp
public class YearEndSetEnrollmentEndpoint : ProfitSharingRequestEndpoint<ProfitYearRequest>
{
    public override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        await _service.UpdateEnrollmentId(req.ProfitYear, ct);
        await Send.NoContentAsync(ct);
    }
}
```

#### 3. ProfitSharingResponseEndpoint<TResponse>

For endpoints without request body but with response (GET with no parameters).

```csharp
public abstract class ProfitSharingResponseEndpoint<TResponse>
    : EndpointWithoutRequest<TResponse>, IHasNavigationId
    where TResponse : notnull
```

#### 4. ProfitSharingResultResponseEndpoint<TResponse>

For read-only endpoints returning typed results (recommended for lookups).

```csharp
public abstract class ProfitSharingResultResponseEndpoint<TResponse>
    : EndpointWithoutRequest<Results<Ok<TResponse>, NotFound, ProblemHttpResult>>, IHasNavigationId
    where TResponse : notnull
```

**Usage:**

```csharp
public sealed class StateListEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<StateListResponse>>
{
    public StateListEndpoint(ILogger<StateListEndpoint> logger)
        : base(Navigation.Constants.Inquiries) { }

    public override async Task<Results<Ok<...>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, new { }, async () =>
        {
            var states = GetStateList();
            return Result<ListResponseDto<StateListResponse>>.Success(new ListResponseDto<...> { Items = states });
        });
    }
}
```

#### 5. ProfitSharingEndpoint (no request/response)

For endpoints without request or response (rare).

```csharp
public abstract class ProfitSharingEndpoint : EndpointWithoutRequest, IHasNavigationId
```

### Choosing the Right Base Class

| Scenario                          | Base Class                                       | Example                      |
| --------------------------------- | ------------------------------------------------ | ---------------------------- |
| Query with request/response       | `ProfitSharingEndpoint<TRequest, TResponse>`     | MasterInquirySearchEndpoint  |
| Command with request, no response | `ProfitSharingRequestEndpoint<TRequest>`         | YearEndSetEnrollmentEndpoint |
| Simple lookup, no request         | `ProfitSharingResultResponseEndpoint<TResponse>` | StateListEndpoint            |
| Typed result with request         | `ProfitSharingEndpoint<TRequest, Results<...>>`  | CreateBeneficiaryEndpoint    |

---

## Endpoint Categories

### Lookups (13 endpoints)

**Group:** `LookupGroup` → `/lookup`

Reference data endpoints returning simple lists or single values:

- `StateListEndpoint` - State abbreviations and names
- `CalendarRecordEndpoint` - Profit year calendar records
- `DistributionStatusEndpoint` - Distribution status codes
- `TaxCodeEndpoint` - Tax code lookups
- `CommentTypeEndpoint` - Comment type reference data
- `PayClassificationLookupEndpoint` - Pay classification codes
- `MissiveLookupEndpoint` - Missive types
- `DuplicateSsnExistsEndpoint` - Check for duplicate SSNs

**Pattern:** Most are simple, cacheable endpoints returning `Result<ListResponseDto<T>>`. No sensitive data typically.

### Master Inquiry (5 endpoints)

**Group:** `MasterInquiryGroup` → `/master-inquiry`

Employee and beneficiary search with advanced filtering:

- `MasterInquirySearchEndpoint` - Main search with pagination
- `MasterInquiryMemberEndpoint` - Single member lookup
- `MasterInquiryMemberDetailsEndpoint` - Detailed member view
- `MasterInquiryFilteredDetailsEndpoint` - Filtered details
- `MasterInquiryDetailGroupingEndpoint` - Grouped detail view

**Pattern:** Complex queries with pagination, sorting, filtering. Use `RecordCountsProcessed` telemetry.

### Beneficiaries (7 endpoints)

**Group:** `BeneficiariesGroup` → `/beneficiaries`

CRUD operations for beneficiaries and contacts:

- `CreateBeneficiaryEndpoint` - Add new beneficiary
- `UpdateBeneficiaryEndpoint` - Update beneficiary
- `DeleteBeneficiaryEndpoint` - Delete beneficiary
- `CreateBeneficiaryContactEndpoint` - Add contact
- `UpdateBeneficiaryContactEndpoint` - Update contact
- `DeleteBeneficiaryContactEndpoint` - Delete contact
- `BeneficiaryDisbursementEndpoint` - Disbursement operations

**Pattern:** Mutation operations with validation. Return `CreateBeneficiaryResponse` with ID. Log creation/update events.

### Beneficiary Inquiry (5 endpoints)

**Group:** `BeneficiaryInquiry` (custom routes)

Beneficiary search and filtering:

- `BeneficiaryEndpoint` - Search beneficiaries
- `BeneficiaryDetailEndpoint` - Beneficiary details
- `BeneficiaryKindEndpoint` - Beneficiary kind lookup
- `BeneficiaryTypeEndpoint` - Beneficiary type lookup
- `BeneficiarySearchFilterEndpoint` - Advanced search

### Distributions (4 endpoints)

**Group:** `DistributionGroup` → `/distributions`

Distribution management (payments to members):

- `CreateDistributionEndpoint` - Create distribution
- `UpdateDistributionEndpoint` - Update distribution
- `DeleteDistributionEndpoint` - Delete distribution
- `DistributionSearchEndpoint` - Search distributions

**Pattern:** Commands use `Result<T>` with validation. Record `distribution-create`, `distribution-update` operations in telemetry.

### Year-End (3+ endpoints + 50+ report endpoints)

**Group:** `YearEndGroup` → `/year-end`

Year-end processing and reports:

**Operations:**

- `YearEndSetEnrollmentEndpoint` - Update enrollment IDs
- `YearEndProcessFinalRunEndpoint` - Execute final year-end run
- `CertificatesFileEndpoint` - Download certificate file

**Reports (nested under `/year-end/reports/`):**

- **Eligibility:** `GetEligibilityEndpoint`
- **Executive Hours:** `ExecutiveHoursAndDollarsEndpoint`, `SetExecutiveHoursAndDollarsEndpoint`
- **Frozen Reports:** `BalanceByAgeEndpoint`, `ContributionsByAgeEndpoint`, `ForfeituresAndPointsForYearEndpoint`, etc.
- **Post-Frozen Reports:** `CertificatesReportEndpoint`, `ProfitSharingLabelsEndpoint`, etc.
- **Cleanup Reports:** `GetDuplicateSSNsEndpoint`, `DuplicateNamesAndBirthdaysEndpoint`, etc.
- **Profit Master:** `ProfitMasterUpdateEndpoint`, `ProfitMasterRevertEndpoint`, `ProfitMasterStatusEndpoint`

**Pattern:** Most reports extend `EndpointWithCsvBase` for CSV export. Use archive support via `IAuditService.ArchiveCompletedReportAsync`.

### Navigation (3 endpoints)

**Group:** `NavigationGroup` → `/navigation`

Navigation tree and status management:

- `GetNavigationEndpoint` - Retrieve full navigation tree
- `GetNavigationStatusEndpoint` - Check navigation status
- `UpdateNavigationStatusEndpoint` - Update navigation status

**Pattern:** Uses distributed cache with version-based invalidation. See `NavigationService` for caching pattern.

### IT Operations (4 endpoints)

**Group:** `ItDevOpsGroup` / `ItDevOpsAllUsersGroup`

DevOps and admin tools:

- `FreezeDemographicsEndpoint` - Freeze demographics snapshot
- `GetFrozenDemographicsEndpoint` - List frozen snapshots
- `GetActiveFrozenDemographicEndpoint` - Active snapshot
- `GetTableMetadataEndpoint` - Database metadata

**Pattern:** Admin-only endpoints. Log all operations. Use `ItDevOpsGroup` for restricted access.

### Military (2 endpoints)

**Group:** `MilitaryGroup` → `/military`

Military contribution management:

- `CreateMilitaryContributionRecord` - Add military contribution
- `GetMilitaryContributionRecords` - Retrieve contributions

### Adjustments (1 endpoint)

**Group:** `AdjustmentGroup` → `/adjustments`

- `MergeProfitDetailsEndpoint` - Merge profit detail records

### Validation (3 endpoints)

**Group:** `ValidationGroup` → `/validation`

Data validation and integrity checks:

- `GetMasterUpdateValidationEndpoint` - Validate master updates
- `ValidateAllocTransfersEndpoint` - Validate allocation transfers
- `ValidateReportChecksumEndpoint` - Report checksum validation

### Ad-Hoc Reports (10+ endpoints)

**Group:** Nested under `/reports/adhoc`

Ad-hoc reporting endpoints:

- `AdhocBeneficiariesReportEndpoint`
- `TerminatedEmployeesReportEndpoint`
- `InactiveEmployeesBreakdownEndpoint`
- `RetiredEmployeesWithBalanceActivityEndpoint`
- etc.

**Pattern:** Extend `EndpointWithCsvBase`. Support CSV and JSON output based on `Accept` header.

---

## Telemetry Integration (MANDATORY)

**CRITICAL:** All endpoints MUST implement comprehensive telemetry using `TelemetryExtensions` patterns. This is a non-negotiable architectural requirement for production observability, security auditing, and performance monitoring.

### Why Telemetry is Mandatory

1. **Production Support:** Enables rapid diagnosis of issues in production via OpenTelemetry traces and metrics
2. **Security Auditing:** Tracks who accessed sensitive fields (SSN, OracleHcmId, etc.) for compliance
3. **Performance Monitoring:** Records endpoint duration, request/response sizes, error rates
4. **Business Intelligence:** Captures business operation metrics (year-end processing, report generation, etc.)
5. **User Activity Tracking:** Aggregates usage patterns by role for capacity planning

### Two Telemetry Patterns

#### Pattern 1: ExecuteWithTelemetry Wrapper (Recommended)

**Use this pattern for 90% of endpoints.** It provides automatic telemetry with minimal code.

```csharp
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Common.Telemetry;

public class MyEndpoint : ProfitSharingEndpoint<MyRequest, MyResponse>
{
    private readonly IMyService _service;
    private readonly ILogger<MyEndpoint> _logger; // REQUIRED for telemetry

    public MyEndpoint(IMyService service, ILogger<MyEndpoint> logger)
        : base(Navigation.Constants.SomeNavigationId)
    {
        _service = service;
        _logger = logger; // MUST inject logger
    }

    public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            // Your business logic here
            var result = await _service.ProcessAsync(req, ct);

            // Add business metrics (required for business operations)
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "employee-lookup"),
                new("endpoint", nameof(MyEndpoint)));

            // Record counts if processing collections
            if (result.Records?.Count > 0)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(result.Records.Count,
                    new("record_type", "employee"),
                    new("endpoint", nameof(MyEndpoint)));
            }

            return result;
        }, "Ssn", "OracleHcmId"); // ← CRITICAL: Declare all sensitive fields accessed
    }
}
```

**What `ExecuteWithTelemetry` Does:**

1. Starts an OpenTelemetry activity with endpoint name, navigation ID, user role, correlation ID
2. Records request metrics and sensitive field access
3. Executes your business logic
4. Records response metrics and large response warnings (>5MB)
5. Handles exceptions and records error metrics
6. Disposes activity

#### Pattern 2: Manual Telemetry (Advanced Control)

Use this when you need fine-grained control (e.g., HandleAsync override, custom error handling, file downloads).

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    using var activity = this.StartEndpointActivity(HttpContext);

    try
    {
        // 1. Record request metrics (declare sensitive fields)
        this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn", "OracleHcmId");

        // 2. Business logic
        var result = await _service.ProcessAsync(req, ct);

        // 3. Business metrics (required)
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "year-end-processing"),
            new("profit_year", req.ProfitYear.ToString()),
            new("endpoint", nameof(MyEndpoint)));

        // 4. Record response metrics
        this.RecordResponseMetrics(HttpContext, _logger, result);

        await Send.OkAsync(result, ct);
    }
    catch (Exception ex)
    {
        // 5. Record exception metrics
        this.RecordException(HttpContext, _logger, ex, activity);
        throw;
    }
}
```

### Logger Injection (REQUIRED)

**CRITICAL:** All endpoints MUST inject `ILogger<TEndpoint>` for telemetry correlation.

```csharp
public class MyEndpoint : ProfitSharingEndpoint<MyRequest, MyResponse>
{
    private readonly ILogger<MyEndpoint> _logger; // REQUIRED

    public MyEndpoint(IMyService service, ILogger<MyEndpoint> logger) // ← Inject logger
        : base(Navigation.Constants.SomeId)
    {
        _logger = logger;
    }
}
```

**Why?** The logger:

- Correlates structured logs with OpenTelemetry traces
- Enables sensitive field access logging
- Provides contextual information for debugging
- Supports log aggregation in production (Seq, Application Insights, etc.)

### Sensitive Field Declarations (CRITICAL)

When endpoints access sensitive data, **ALWAYS** declare the field names in telemetry calls:

```csharp
// Single sensitive field
this.ExecuteWithTelemetry(HttpContext, _logger, req, async () => { ... }, "Ssn");

// Multiple sensitive fields
this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn", "OracleHcmId", "Salary");

// No sensitive fields (lookup endpoints)
this.ExecuteWithTelemetry(HttpContext, _logger, req, async () => { ... });
```

**Common Sensitive Fields:**

- `"Ssn"` - Social Security Numbers
- `"OracleHcmId"` - Internal employee identifiers
- `"BadgeNumber"` - Employee badge numbers
- `"Salary"` - Salary information
- `"BeneficiaryInfo"` - Beneficiary details

**What Happens:**

1. `EndpointTelemetry.SensitiveFieldAccessTotal` counter is incremented
2. Structured log entry created: `"Sensitive field accessed: {Field} by user role {UserRole}"`
3. Security team can audit access patterns
4. Compliance requirements satisfied

### Business Metrics

Add business operation metrics appropriate to the endpoint category:

**Year-End Operations:**

```csharp
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "year-end-enrollment"),
    new("profit_year", profitYear.ToString()),
    new("endpoint", nameof(YearEndEnrollmentEndpoint)));
```

**Report Generation:**

```csharp
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "report-generation"),
    new("report_type", "profit-sharing"),
    new("endpoint", nameof(ProfitSharingReportEndpoint)));
```

**Employee Lookups:**

```csharp
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "employee-lookup"),
    new("lookup_type", "by-ssn"),
    new("endpoint", nameof(EmployeeLookupEndpoint)));
```

**Distribution Operations:**

```csharp
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "distribution-create"),
    new("endpoint", nameof(CreateDistributionEndpoint)));
```

**Record Counts:**

```csharp
EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
    new("record_type", "employee"),
    new("endpoint", nameof(MasterInquirySearchEndpoint)));
```

### TelemetryProcessor (Global)

In addition to endpoint-level telemetry, the `TelemetryProcessor` provides global pre/post-processing:

**Pre-Process (before endpoint execution):**

- Starts activity
- Stores in `HttpContext.Items` for retrieval

**Post-Process (after endpoint execution):**

- Calculates duration: `EndpointTelemetry.EndpointDurationMs.Record(...)`
- Records HTTP status-based errors
- Detects large responses (>5MB)
- Records user activity metrics by role

**Configuration:** Registered globally in FastEndpoints pipeline (see `Demoulas.Common.Api`).

### Telemetry Testing

**Unit tests MUST verify telemetry integration:**

```csharp
[Fact]
public async Task ExecuteAsync_RecordsTelemetry()
{
    // Arrange
    var endpoint = Factory.Create<MyEndpoint>();
    var req = new MyRequest { ... };

    // Act
    var response = await endpoint.ExecuteAsync(req, CancellationToken.None);

    // Assert
    response.Should().NotBeNull();
    // Verify business metrics were recorded (check metrics provider mock)
    // Verify activity was created (check ActivitySource mock)
    // Verify logger was called with expected messages
}
```

**Integration tests should verify:**

- Activity creation and completion
- Metrics recording (request, response, business operations)
- Sensitive field logging
- Error recording on exceptions

### Telemetry Documentation

For complete telemetry implementation details, see:

- **TELEMETRY_GUIDE.md** (`src/ui/public/docs/`) - Comprehensive 75+ page reference
- **TELEMETRY_QUICK_REFERENCE.md** (`src/ui/public/docs/`) - Developer cheat sheet
- **TELEMETRY_DEVOPS_GUIDE.md** (`src/ui/public/docs/`) - Production operations guide

---

## Result Pattern & HTTP Mapping

### Domain Result<T>

All service methods return `Result<T>` from `Demoulas.ProfitSharing.Common.Contracts`:

```csharp
public record Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }
    public ProblemDetails? ProblemDetails { get; }

    public static Result<T> Success(T value);
    public static Result<T> Failure(Error error);
    public static Result<T> ValidationFailure(string message, Dictionary<string, string[]> errors);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<ProblemDetails, TResult> onFailure);
}
```

### HTTP Result Mapping

Endpoints convert `Result<T>` to typed minimal API results using `ResultHttpExtensions`:

#### ToHttpResult Extension

```csharp
public static Results<Ok<T>, NotFound, ProblemHttpResult> ToHttpResult<T>(
    this Result<T> result,
    params Error[] notFoundErrors)
{
    return result.Match<Results<Ok<T>, NotFound, ProblemHttpResult>>(
        v => TypedResults.Ok(v),
        pd => notFoundErrors.Any(e => e.Description == pd.Detail)
            ? TypedResults.NotFound()
            : TypedResults.Problem(pd.Detail));
}
```

**Usage:**

```csharp
var result = await _service.GetMemberAsync(req.Id, ct);
return result.ToHttpResult(Error.MemberNotFound);
```

**Mapping Logic:**

- `Result.Success(value)` → `TypedResults.Ok(value)` (HTTP 200)
- `Result.Failure(Error.MemberNotFound)` → `TypedResults.NotFound()` (HTTP 404)
- `Result.Failure(otherError)` → `TypedResults.Problem(error.Description)` (HTTP 500 or custom status)

#### ToHttpResultWithValidation Extension

For endpoints that return validation errors:

```csharp
public static Results<Ok<T>, NotFound, BadRequest, ProblemHttpResult> ToHttpResultWithValidation<T>(
    this Result<T> result,
    params Error[] notFoundErrors)
```

**Usage:**

```csharp
var result = await _service.UpdateDistribution(req, ct);
return result.ToHttpResultWithValidation(Error.DistributionNotFound, Error.BadgeNumberNotFound);
```

**Mapping Logic:**

- `Result.Success(value)` → `Ok(value)` (HTTP 200)
- `Result.Failure(notFoundError)` → `NotFound()` (HTTP 404)
- `Result.ValidationFailure(...)` → `BadRequest()` (HTTP 400)
- Other failures → `Problem(...)` (HTTP 500)

#### ToResultOrNotFound Helper

Converts nullable values to `Result<T>`:

```csharp
public static Result<T> ToResultOrNotFound<T>(this T? value, Error notFoundError) where T : class
{
    return value is null ? Result<T>.Failure(notFoundError) : Result<T>.Success(value);
}
```

**Usage:**

```csharp
var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
return member.ToResultOrNotFound(Error.MemberNotFound).ToHttpResult(Error.MemberNotFound);
```

### Error Definition

Errors are defined in `Demoulas.ProfitSharing.Common.Contracts.Error`:

```csharp
public static class Error
{
    public static readonly Error MemberNotFound = new("Member.NotFound", "The requested member was not found.");
    public static readonly Error CalendarYearNotFound = new("CalendarYear.NotFound", "The requested calendar year was not found.");
    public static readonly Error DistributionNotFound = new("Distribution.NotFound", "Distribution record not found.");
    public static readonly Error BadgeNumberNotFound = new("BadgeNumber.NotFound", "Badge number not found.");
    // ... 50+ more errors
}
```

### Endpoint Return Type Patterns

| HTTP Scenario                       | FastEndpoints Return Type                                 | Example Endpoint             |
| ----------------------------------- | --------------------------------------------------------- | ---------------------------- |
| Query success or not found          | `Results<Ok<T>, NotFound, ProblemHttpResult>`             | StateListEndpoint            |
| Query with validation               | `Results<Ok<T>, NotFound, BadRequest, ProblemHttpResult>` | MasterInquirySearchEndpoint  |
| Command success or validation error | `Results<Ok<T>, BadRequest, ProblemHttpResult>`           | CreateBeneficiaryEndpoint    |
| Command with not found              | `Results<Ok<T>, NotFound, BadRequest, ProblemHttpResult>` | UpdateDistributionEndpoint   |
| No content command                  | `Results<NoContent, ProblemHttpResult>`                   | YearEndSetEnrollmentEndpoint |
| File download                       | `Task HandleAsync(...)` with `Send.StreamAsync`           | CertificatesFileEndpoint     |

### Example: Complete Result Flow

**Service Layer:**

```csharp
public async Task<Result<MemberDetails>> GetMemberAsync(int id, CancellationToken ct)
{
    await using var ctx = await _factory.CreateDbContextAsync(ct);
    ctx.UseReadOnlyContext();

    var member = await ctx.Members
        .TagWith($"GetMember-{id}")
        .FirstOrDefaultAsync(m => m.Id == id, ct);

    return member is null
        ? Result<MemberDetails>.Failure(Error.MemberNotFound)
        : Result<MemberDetails>.Success(_mapper.ToDto(member));
}
```

**Endpoint Layer:**

```csharp
public class GetMemberEndpoint : ProfitSharingEndpoint<GetMemberRequest, Results<Ok<MemberDetails>, NotFound, ProblemHttpResult>>
{
    private readonly IMasterInquiryService _service;
    private readonly ILogger<GetMemberEndpoint> _logger;

    public GetMemberEndpoint(IMasterInquiryService service, ILogger<GetMemberEndpoint> logger)
        : base(Navigation.Constants.MasterInquiry)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("member/{id}");
        Group<MasterInquiryGroup>();
    }

    public override async Task<Results<Ok<MemberDetails>, NotFound, ProblemHttpResult>> ExecuteAsync(GetMemberRequest req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.GetMemberAsync(req.Id, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "member-lookup"),
                new("endpoint", nameof(GetMemberEndpoint)));

            return result.ToHttpResult(Error.MemberNotFound);
        }, "Ssn", "OracleHcmId");
    }
}
```

**HTTP Outcomes:**

- Member found: `200 OK { "id": 1, "firstName": "John", ... }`
- Member not found: `404 Not Found`
- Database error: `500 Internal Server Error { "detail": "Database connection failed" }`

---

## Validation Patterns

### FluentValidation Integration

FastEndpoints integrates with **FluentValidation** for request DTO validation. Validators are automatically discovered and executed before `ExecuteAsync`.

#### Validator Example

```csharp
using FluentValidation;

public class IdsRequestValidator : AbstractValidator<IdsRequest>
{
    public IdsRequestValidator()
    {
        RuleFor(x => x.Ids)
            .NotNull()
            .WithMessage("Ids cannot be null.");

        RuleFor(x => x.Ids)
            .Must(ids => ids != null && ids.Length > 0)
            .WithMessage("At least one ID must be provided.");

        RuleFor(x => x.Ids)
            .Must(ids => ids == null || ids.Length <= 1000)
            .WithMessage("Cannot process more than 1000 IDs in a single request.");

        RuleFor(x => x.Ids)
            .Must(ids => ids == null || ids.All(id => id > 0))
            .WithMessage("All IDs must be positive integers.");

        RuleFor(x => x.Ids)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Length)
            .WithMessage("Duplicate IDs are not allowed.");
    }
}
```

**Registration:** FluentValidation validators are auto-discovered via `AddValidatorsFromAssemblyContaining<T>()` in DI setup.

#### Validation Response

When validation fails, FastEndpoints returns:

```json
{
  "errors": {
    "Ids": ["At least one ID must be provided."]
  },
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400
}
```

### Boundary Check Requirements

**MANDATORY:** All endpoints must validate:

1. **Numeric Ranges:** Min/max for integers, floats (page size, amounts, counts)
2. **String Lengths:** Min/max for text fields
3. **Collection Sizes:** Max items in arrays/lists
4. **Pagination Bounds:** Max page size (typically 1000), max offset
5. **Date Ranges:** Not before/after bounds, valid ranges
6. **Enum Validation:** Reject unknown or out-of-range enum values
7. **Required Fields:** Non-null, non-empty validation

#### Example: Pagination Validator

```csharp
public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
{
    public PaginationRequestValidator()
    {
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize must be between 1 and 1000.");

        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be >= 0.");

        RuleFor(x => x.SortBy)
            .MaximumLength(100)
            .When(x => x.SortBy != null)
            .WithMessage("SortBy must be <= 100 characters.");
    }
}
```

### Degenerate Query Guards

Prevent expensive queries by rejecting degenerate inputs:

```csharp
RuleFor(x => x.BadgeNumbers)
    .Must(badges => badges == null || !badges.All(b => b == 0))
    .WithMessage("All-zero badge numbers are not allowed (would scan entire table).");

RuleFor(x => x.Filters)
    .Must(filters => filters != null && filters.Any())
    .When(x => x.RequireFilters)
    .WithMessage("At least one filter is required for this query.");
```

### Validation Testing

**Unit tests MUST cover:**

- Happy path (valid input)
- Boundary cases (min, max, empty, null)
- Invalid inputs (negative numbers, oversized strings, invalid enums)
- Degenerate cases (all-zero IDs, empty filters)

```csharp
[Fact]
public void Validator_ShouldRejectEmptyIds()
{
    var validator = new IdsRequestValidator();
    var request = new IdsRequest { Ids = [] };

    var result = validator.Validate(request);

    result.IsValid.Should().BeFalse();
    result.Errors.Should().ContainSingle(e => e.ErrorMessage == "At least one ID must be provided.");
}
```

---

## Groups & Route Organization

### Group Definition

Groups provide logical route prefixing and shared configuration:

```csharp
public sealed class LookupGroup : GroupBase
{
    protected override string Route => "lookup";
    protected override string RouteName => "Lookup";
}
```

**Resulting Routes:**

- `GET /lookup/states` → StateListEndpoint
- `GET /lookup/tax-codes` → TaxCodeEndpoint

### All Groups

| Group                   | Route Prefix         | Purpose                       | Endpoints |
| ----------------------- | -------------------- | ----------------------------- | --------- |
| `LookupGroup`           | `/lookup`            | Reference data lookups        | 13        |
| `MasterInquiryGroup`    | `/master-inquiry`    | Employee/beneficiary search   | 5         |
| `BeneficiariesGroup`    | `/beneficiaries`     | Beneficiary CRUD              | 7         |
| `DistributionGroup`     | `/distributions`     | Distribution management       | 4         |
| `YearEndGroup`          | `/year-end`          | Year-end operations & reports | 50+       |
| `NavigationGroup`       | `/navigation`        | Navigation tree               | 3         |
| `MilitaryGroup`         | `/military`          | Military contributions        | 2         |
| `AdjustmentGroup`       | `/adjustments`       | Profit detail adjustments     | 1         |
| `ValidationGroup`       | `/validation`        | Data validation               | 3         |
| `ProfitDetailsGroup`    | `/profit-details`    | Profit detail operations      | 1         |
| `ItDevOpsGroup`         | `/it-operations`     | Admin tools (restricted)      | 4         |
| `ItDevOpsAllUsersGroup` | `/it-operations-all` | Admin tools (all users)       | -         |
| `JobsGroup`             | `/jobs`              | Background jobs               | -         |
| `BalanceGroup`          | `/balance`           | Balance inquiries             | -         |
| `BeneficiaryKindGroup`  | `/beneficiary-kind`  | Beneficiary kind lookups      | -         |
| `BeneficiaryTypeGroup`  | `/beneficiary-type`  | Beneficiary type lookups      | -         |

### Group Configuration

Groups can specify:

- Route prefix
- Shared authorization policies
- Versioning
- Common response examples

**Example with Authorization:**

```csharp
public sealed class ItDevOpsGroup : GroupBase
{
    protected override string Route => "it-operations";
    protected override string RouteName => "IT Operations";

    public override void Configure()
    {
        base.Configure();
        Policies(Policy.ITDEVOPS); // Require IT DevOps role
    }
}
```

---

## CSV Report Endpoints

### EndpointWithCsvBase

Specialized base class for reports that support both JSON and CSV output based on `Accept` header.

```csharp
public abstract class EndpointWithCsvBase<ReqType, RespType, MapType>
    : FastEndpoints.Endpoint<ReqType, ReportResponseBase<RespType>>, IHasNavigationId
    where ReqType : SortedPaginationRequestDto
    where RespType : class
    where MapType : ClassMap<RespType>
{
    protected EndpointWithCsvBase(short navigationId);

    public abstract Task<ReportResponseBase<RespType>> GetResponse(ReqType req, CancellationToken ct);
    public abstract string ReportFileName { get; }
}
```

### CSV Features

1. **Automatic Content Negotiation:**

   - `Accept: application/json` → JSON response
   - `Accept: text/csv` → CSV file download

2. **Pagination Override:**

   - CSV requests ignore pagination (set `Take = int.MaxValue`)

3. **CSV Mapping:**

   - Uses CsvHelper `ClassMap<T>` for column definitions

4. **Report Metadata:**

   - CSV includes report name and date header

5. **Output Caching:**
   - 5-minute cache for report responses (configurable)

### Example CSV Endpoint

```csharp
public class ExecutiveHoursAndDollarsEndpoint :
    EndpointWithCsvBase<ExecutiveHoursAndDollarsRequest, ExecutiveHoursAndDollarsResponse, ExecutiveHoursAndDollarsEndpoint.ExecutiveHoursAndDollarsMap>
{
    private readonly IExecutiveHoursAndDollarsService _reportService;
    private readonly IAuditService _auditService;
    private readonly ILogger<ExecutiveHoursAndDollarsEndpoint> _logger;

    public ExecutiveHoursAndDollarsEndpoint(IExecutiveHoursAndDollarsService reportService, IAuditService auditService, ILogger<ExecutiveHoursAndDollarsEndpoint> logger)
        : base(Navigation.Constants.ManageExecutiveHours)
    {
        _reportService = reportService;
        _auditService = auditService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("executive-hours-and-dollars");
        Group<YearEndGroup>();
        base.Configure(); // Important: calls parent Configure for CSV setup
    }

    public override string ReportFileName => "Executive Hours and Dollars";

    public override async Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetResponse(ExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _auditService.ArchiveCompletedReportAsync(ReportFileName,
                req.ProfitYear,
                req,
                (archiveReq, isArchiveRequest, cancellationToken) =>
                {
                    if (isArchiveRequest)
                    {
                        // Modify request for archiving (remove filters)
                        archiveReq = archiveReq with
                        {
                            BadgeNumber = null,
                            FullNameContains = null,
                            Ssn = null,
                            HasExecutiveHoursAndDollars = true,
                            IsMonthlyPayroll = true
                        };
                    }

                    return _reportService.GetExecutiveHoursAndDollarsReportAsync(archiveReq, cancellationToken);
                },
                ct);

            // Record year-end executive hours and dollars report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-executive-hours-dollars"),
                new("endpoint", "ExecutiveHoursAndDollarsEndpoint"),
                new("report_type", "executive-hours-dollars"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "executive-hours-dollars"),
                new("endpoint", "ExecutiveHoursAndDollarsEndpoint"));

            _logger.LogInformation("Year-end executive hours and dollars report generated for year {ProfitYear}, returned {Count} records (correlation: {CorrelationId})",
                req.ProfitYear, resultCount, HttpContext?.TraceIdentifier ?? "test-correlation");

            this.RecordResponseMetrics(HttpContext, _logger, result);
            return result ?? new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] }
            };
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

    public sealed class ExecutiveHoursAndDollarsMap : ClassMap<ExecutiveHoursAndDollarsResponse>
    {
        public ExecutiveHoursAndDollarsMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE");
            Map(m => m.FullName).Index(1).Name("NAME");
            Map(m => m.StoreNumber).Index(2).Name("STR");
            Map(m => m.HoursExecutive).Index(3).Name("EXEC HRS");
            Map(m => m.IncomeExecutive).Index(4).Name("EXEC DOLS");
            Map(m => m.CurrentHoursYear).Index(5).Name("ORA HRS CUR");
            Map(m => m.CurrentIncomeYear).Index(6).Name("ORA DOLS CUR");
            Map(m => m.PayFrequencyId).Index(7).Name("FREQ");
            Map(m => m.EmploymentStatusId).Index(8).Name("STATUS");
            Map(m => m.Ssn).Index(9).Name("SSN");
        }
    }
}
```

### CSV Output Example

```
Oct 23 2025 14:30
Executive Hours and Dollars
BADGE,NAME,STR,EXEC HRS,EXEC DOLS,ORA HRS CUR,ORA DOLS CUR,FREQ,STATUS,SSN
1001,"Doe, John",4,120.5,15000.00,2080,50000.00,1,1,XXX-XX-1234
1002,"Smith, Jane",5,80.0,10000.00,2080,45000.00,1,1,XXX-XX-5678
```

### Archive Support

Year-end reports use `IAuditService.ArchiveCompletedReportAsync` to:

1. Check if a completed report exists for the profit year
2. If exists, return archived data (fast)
3. If not, generate fresh report and archive it
4. Modify request for archiving (remove user-specific filters)

**Pattern:**

```csharp
var result = await _auditService.ArchiveCompletedReportAsync(
    reportName: "My Report",
    profitYear: req.ProfitYear,
    request: req,
    generateReportFunc: (modifiedReq, isArchive, ct) =>
    {
        if (isArchive)
        {
            // Strip user filters for canonical archive
            modifiedReq = modifiedReq with { BadgeNumber = null, FullNameContains = null };
        }
        return _reportService.GenerateReportAsync(modifiedReq, ct);
    },
    cancellationToken: ct);
```

---

## Authentication & Authorization

### Okta Integration

Authentication is handled via **Okta** (OAuth 2.0 / OpenID Connect). Users authenticate via the UI, which obtains a JWT token and includes it in API requests.

### Role-Based Authorization

Endpoints use role-based policies defined in `PolicyRoleMap`:

```csharp
public static class PolicyRoleMap
{
    public static readonly Dictionary<string, string[]> Map = new()
    {
        { Policy.ADMINISTRATOR, new[] { Role.ADMINISTRATOR } },
        { Policy.FINANCEMANAGER, new[] { Role.ADMINISTRATOR, Role.FINANCEMANAGER } },
        { Policy.AUDITOR, new[] { Role.ADMINISTRATOR, Role.AUDITOR } },
        { Policy.ITDEVOPS, new[] { Role.ADMINISTRATOR, Role.ITDEVOPS } },
        { Policy.READONLY, new[] { Role.ADMINISTRATOR, Role.FINANCEMANAGER, Role.AUDITOR, Role.READONLY } },
        // ... more policies
    };
}
```

### Endpoint Authorization

**Option 1: Group-Level (Preferred)**

```csharp
public sealed class ItDevOpsGroup : GroupBase
{
    public override void Configure()
    {
        base.Configure();
        Policies(Policy.ITDEVOPS); // All endpoints in this group require IT DevOps role
    }
}
```

**Option 2: Endpoint-Level**

```csharp
public override void Configure()
{
    Get("sensitive-data");
    Policies(Policy.ADMINISTRATOR); // Only administrators
    Group<MyGroup>();
}
```

### Read-Only Role Pattern

For read-only users, endpoints check `IReadOnlyRoleService`:

```csharp
var isReadOnly = await _readOnlyRoleService.IsReadOnlyAsync(ct);
if (isReadOnly)
{
    return TypedResults.Problem("Read-only users cannot modify data.", statusCode: 403);
}
```

**OR** use a pre-processor:

```csharp
public class ReadOnlyPreProcessor : IPreProcessor
{
    public async Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var readOnlyService = context.HttpContext.Resolve<IReadOnlyRoleService>();
        if (await readOnlyService.IsReadOnlyAsync(ct) && context.HttpContext.Request.Method != "GET")
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct);
        }
    }
}
```

### Impersonation Support

Admins can impersonate other users via the `x-impersonate-user` header. The authentication layer swaps the user principal.

**Swagger Support:** Custom `SwaggerImpersonationHeader` operation processor adds the header to Swagger UI.

---

## Creating New Endpoints

### Step-by-Step Guide

#### 1. Define Request/Response DTOs

**Request DTO:**

```csharp
namespace Demoulas.ProfitSharing.Common.Contracts.Request.MyFeature;

public sealed record MyFeatureRequest
{
    public int ProfitYear { get; init; }
    public int? BadgeNumber { get; init; }
    public string? FullNameContains { get; init; }

    public static MyFeatureRequest RequestExample() => new()
    {
        ProfitYear = 2024,
        BadgeNumber = 1001,
        FullNameContains = "Doe"
    };
}
```

**Response DTO:**

```csharp
namespace Demoulas.ProfitSharing.Common.Contracts.Response.MyFeature;

public sealed record MyFeatureResponse
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public decimal Amount { get; init; }

    public static MyFeatureResponse ResponseExample() => new()
    {
        Id = 1,
        FullName = "John Doe",
        Amount = 1000.00m
    };
}
```

#### 2. Create Validator (if needed)

```csharp
namespace Demoulas.ProfitSharing.Endpoints.Validation;

public sealed class MyFeatureRequestValidator : AbstractValidator<MyFeatureRequest>
{
    public MyFeatureRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .InclusiveBetween(2000, 2100)
            .WithMessage("ProfitYear must be between 2000 and 2100.");

        RuleFor(x => x.FullNameContains)
            .MaximumLength(100)
            .When(x => x.FullNameContains != null)
            .WithMessage("FullNameContains must be <= 100 characters.");
    }
}
```

#### 3. Create Endpoint Class

```csharp
using Demoulas.ProfitSharing.Common.Contracts.Request.MyFeature;
using Demoulas.ProfitSharing.Common.Contracts.Response.MyFeature;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.MyFeature;

public sealed class MyFeatureEndpoint : ProfitSharingEndpoint<MyFeatureRequest, Results<Ok<MyFeatureResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IMyFeatureService _service;
    private readonly ILogger<MyFeatureEndpoint> _logger;

    public MyFeatureEndpoint(IMyFeatureService service, ILogger<MyFeatureEndpoint> logger)
        : base(Navigation.Constants.MyFeatureNavigationId)
    {
        _service = service;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("my-feature/search");
        Summary(s =>
        {
            s.Summary = "Search for my feature data with filters.";
            s.Description = "Returns data matching the provided search criteria.";
            s.ExampleRequest = MyFeatureRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, MyFeatureResponse.ResponseExample() }
            };
            s.Responses[400] = "Bad Request. Invalid search parameters.";
            s.Responses[404] = "Not Found. No data matched the criteria.";
        });
        Group<MyFeatureGroup>();
    }

    public override async Task<Results<Ok<MyFeatureResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(MyFeatureRequest req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _service.GetDataAsync(req, ct);

            // Record business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "my-feature-search"),
                new("endpoint", nameof(MyFeatureEndpoint)));

            // Record counts if applicable
            if (result.Value != null)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(1,
                    new("record_type", "my-feature"),
                    new("endpoint", nameof(MyFeatureEndpoint)));
            }

            // Log operation (no sensitive data in log)
            _logger.LogInformation("My feature search completed for profit year {ProfitYear} (correlation: {CorrelationId})",
                req.ProfitYear, HttpContext.TraceIdentifier);

            return result.ToHttpResult(Error.MyFeatureNotFound);
        }, "Ssn", "BadgeNumber"); // ← Declare sensitive fields if accessed
    }
}
```

#### 4. Create or Update Group (if needed)

```csharp
namespace Demoulas.ProfitSharing.Endpoints.Groups;

public sealed class MyFeatureGroup : GroupBase
{
    protected override string Route => "my-feature";
    protected override string RouteName => "My Feature";
}
```

#### 5. Register Service

Ensure service is registered in DI (usually in `ServiceExtensions.cs`):

```csharp
services.AddScoped<IMyFeatureService, MyFeatureService>();
```

#### 6. Test Endpoint

**Unit Test:**

```csharp
[Fact]
[Description("PS-1234 : My feature endpoint returns data successfully")]
public async Task ExecuteAsync_ValidRequest_ReturnsData()
{
    // Arrange
    var factory = new EndpointFactory();
    var serviceMock = Substitute.For<IMyFeatureService>();
    serviceMock.GetDataAsync(Arg.Any<MyFeatureRequest>(), Arg.Any<CancellationToken>())
        .Returns(Result<MyFeatureResponse>.Success(new MyFeatureResponse { Id = 1, FullName = "Test", Amount = 100 }));

    var endpoint = factory.Create<MyFeatureEndpoint>(serviceMock);
    var request = new MyFeatureRequest { ProfitYear = 2024 };

    // Act
    var response = await endpoint.ExecuteAsync(request, CancellationToken.None);

    // Assert
    response.Should().NotBeNull();
    // Verify telemetry metrics were recorded
    // Verify logger was called
}
```

**Integration Test:**

```csharp
[Fact]
public async Task MyFeatureEndpoint_ReturnsOk()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new MyFeatureRequest { ProfitYear = 2024 };

    // Act
    var response = await client.PostAsJsonAsync("/my-feature/search", request);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var data = await response.Content.ReadFromJsonAsync<MyFeatureResponse>();
    data.Should().NotBeNull();
}
```

---

## Testing Endpoints

### Unit Testing Pattern

**Endpoint Factory:**

```csharp
public class EndpointFactory
{
    public TEndpoint Create<TEndpoint>(params object[] dependencies) where TEndpoint : class
    {
        // Use AutoFixture or manual construction
        return (TEndpoint)Activator.CreateInstance(typeof(TEndpoint), dependencies)!;
    }
}
```

**Test Example:**

```csharp
public class StateListEndpointTests
{
    [Fact]
    [Description("PS-1200 : State list endpoint returns hardcoded states")]
    public async Task ExecuteAsync_ReturnsStateList()
    {
        // Arrange
        var factory = new EndpointFactory();
        var loggerMock = Substitute.For<ILogger<StateListEndpoint>>();
        var endpoint = factory.Create<StateListEndpoint>(loggerMock);

        // Act
        var response = await endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        // Verify implicit conversion to Results<Ok<T>, NotFound, ProblemHttpResult>
        // Verify business metrics were recorded (if using real metrics)
        loggerMock.Received().LogInformation(Arg.Any<string>(), Arg.Any<object[]>());
    }
}
```

### Integration Testing Pattern

**WebApplicationFactory Setup:**

```csharp
public class ProfitSharingApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            // Replace DbContext with in-memory database
            services.RemoveAll<DbContextOptions<ProfitSharingDbContext>>();
            services.AddDbContext<ProfitSharingDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Mock authentication
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
        });
    }
}
```

**Integration Test:**

```csharp
public class MasterInquiryEndpointIntegrationTests : IClassFixture<ProfitSharingApiFactory>
{
    private readonly ProfitSharingApiFactory _factory;
    private readonly HttpClient _client;

    public MasterInquiryEndpointIntegrationTests(ProfitSharingApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task MasterInquirySearch_ValidRequest_ReturnsResults()
    {
        // Arrange
        var request = new MasterInquiryRequest
        {
            ProfitYear = 2024,
            PageSize = 10,
            Skip = 0
        };

        // Act
        var response = await _client.PostAsJsonAsync("/master-inquiry/search", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<MemberDetails>>();
        data.Should().NotBeNull();
        data!.Results.Should().NotBeNull();
    }
}
```

### Telemetry Testing

Verify telemetry integration:

```csharp
[Fact]
public async Task ExecuteAsync_RecordsTelemetryMetrics()
{
    // Arrange
    var meterListenerMock = new TestMeterListener();
    var endpoint = Factory.Create<MyEndpoint>();

    // Act
    await endpoint.ExecuteAsync(new MyRequest(), CancellationToken.None);

    // Assert
    meterListenerMock.Metrics.Should().Contain(m => m.Name == "endpoint.business_operations_total");
    meterListenerMock.Metrics.Should().Contain(m => m.Name == "endpoint.request_size_bytes");
}
```

---

## Common Patterns & Best Practices

### 1. Always Use Typed Results

**DO:**

```csharp
public override async Task<Results<Ok<MyResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(...)
{
    return result.ToHttpResult(Error.MyEntityNotFound);
}
```

**DON'T:**

```csharp
public override async Task<MyResponse> ExecuteAsync(...)
{
    return new MyResponse(); // No error handling
}
```

### 2. Inject Logger for Telemetry

**DO:**

```csharp
public MyEndpoint(IMyService service, ILogger<MyEndpoint> logger)
    : base(Navigation.Constants.MyId)
{
    _logger = logger;
}
```

**DON'T:**

```csharp
public MyEndpoint(IMyService service) // Missing logger
    : base(Navigation.Constants.MyId)
```

### 3. Declare Sensitive Fields

**DO:**

```csharp
return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () => { ... }, "Ssn", "OracleHcmId");
```

**DON'T:**

```csharp
return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () => { ... }); // Forgot to declare SSN access
```

### 4. Use Result<T> from Services

**DO:**

```csharp
var result = await _service.GetAsync(id, ct);
return result.ToHttpResult(Error.NotFound);
```

**DON'T:**

```csharp
var entity = await _service.GetAsync(id, ct);
return entity == null ? TypedResults.NotFound() : TypedResults.Ok(entity); // Inconsistent error handling
```

### 5. Record Business Metrics

**DO:**

```csharp
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "year-end-processing"),
    new("endpoint", nameof(MyEndpoint)));
```

**DON'T:**

```csharp
// No business metrics recorded
```

### 6. Validate All Inputs

**DO:**

```csharp
public class MyRequestValidator : AbstractValidator<MyRequest>
{
    public MyRequestValidator()
    {
        RuleFor(x => x.PageSize).InclusiveBetween(1, 1000);
        RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
    }
}
```

**DON'T:**

```csharp
// No validator - allows unbounded page sizes, invalid years
```

### 7. Use Groups for Route Organization

**DO:**

```csharp
public override void Configure()
{
    Get("my-endpoint");
    Group<MyFeatureGroup>(); // Results in /my-feature/my-endpoint
}
```

**DON'T:**

```csharp
public override void Configure()
{
    Get("my-feature/my-endpoint"); // Hardcoded prefix, no group
}
```

### 8. Provide Swagger Documentation

**DO:**

```csharp
Summary(s =>
{
    s.Summary = "Retrieve member details by ID";
    s.Description = "Returns detailed member information including demographics and balances.";
    s.ExampleRequest = new GetMemberRequest { Id = 1 };
    s.ResponseExamples = new Dictionary<int, object> { { 200, MemberDetails.Example() } };
    s.Responses[404] = "Member not found.";
});
```

**DON'T:**

```csharp
Summary(s => { s.Summary = "Get member"; }); // Minimal, unhelpful
```

### 9. Handle File Downloads Properly

**DO:**

```csharp
public override async Task HandleAsync(MyRequest req, CancellationToken ct)
{
    var stream = await _service.GenerateFileAsync(req, ct);
    HttpContext.Response.Headers.Append("Content-Disposition", "attachment; filename=\"file.csv\"");
    await Send.StreamAsync(stream, "file.csv", contentType: "text/csv", cancellation: ct);
}
```

**DON'T:**

```csharp
return await _service.GenerateFileAsync(req, ct); // No content disposition, wrong return type
```

### 10. Use Archive Support for Year-End Reports

**DO:**

```csharp
var result = await _auditService.ArchiveCompletedReportAsync(
    ReportFileName,
    req.ProfitYear,
    req,
    (archiveReq, isArchive, ct) => _service.GenerateAsync(archiveReq, ct),
    ct);
```

**DON'T:**

```csharp
var result = await _service.GenerateAsync(req, ct); // No caching, slow for repeated requests
```

---

## Reference Examples

### Simple Lookup Endpoint

```csharp
public sealed class StateListEndpoint : ProfitSharingResultResponseEndpoint<ListResponseDto<StateListResponse>>
{
    private readonly ILogger<StateListEndpoint> _logger;

    public StateListEndpoint(ILogger<StateListEndpoint> logger) : base(Navigation.Constants.Inquiries)
    {
        _logger = logger;
    }

    public override void Configure()
    {
        Get("states");
        Summary(s => { s.Summary = "Gets all available states"; });
        Group<LookupGroup>();
    }

    public override async Task<Results<Ok<ListResponseDto<StateListResponse>>, NotFound, ProblemHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, new { }, () =>
        {
            var states = GetStateList();
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "state-list-lookup"),
                new("endpoint", nameof(StateListEndpoint)));

            var response = new ListResponseDto<StateListResponse> { Items = states };
            return Task.FromResult(Result<ListResponseDto<StateListResponse>>.Success(response));
        });
    }

    private static List<StateListResponse> GetStateList() => new()
    {
        new StateListResponse { Abbreviation = "CT", Name = "Connecticut" },
        new StateListResponse { Abbreviation = "MA", Name = "Massachusetts" },
        // ... more states
    };
}
```

### Search Endpoint with Pagination

```csharp
public class MasterInquirySearchEndpoint : ProfitSharingEndpoint<MasterInquiryRequest, Results<Ok<PaginatedResponseDto<MemberDetails>>, NotFound, ProblemHttpResult>>
{
    private readonly IMasterInquiryService _masterInquiryService;
    private readonly ILogger<MasterInquirySearchEndpoint> _logger;

    public MasterInquirySearchEndpoint(IMasterInquiryService masterInquiryService, ILogger<MasterInquirySearchEndpoint> logger)
        : base(Navigation.Constants.MasterInquiry)
    {
        _masterInquiryService = masterInquiryService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("master-inquiry/search");
        Summary(s =>
        {
            s.Summary = "Search for profit sharing members with filters and pagination.";
            s.ExampleRequest = MasterInquiryRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, PaginatedResponseDto<MemberDetails>.Example() } };
        });
        Group<MasterInquiryGroup>();
    }

    public override Task<Results<Ok<PaginatedResponseDto<MemberDetails>>, NotFound, ProblemHttpResult>> ExecuteAsync(MasterInquiryRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var data = await _masterInquiryService.GetMembersAsync(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "search"),
                new("endpoint.category", "master-inquiry"));

            var resultCount = data.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("endpoint.name", nameof(MasterInquirySearchEndpoint)),
                new("operation", "search"));

            _logger.LogInformation("Master inquiry search completed: {ResultCount} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            return Result<PaginatedResponseDto<MemberDetails>>.Success(data).ToHttpResult();
        });
    }
}
```

### CRUD Endpoint (Create)

```csharp
public class CreateBeneficiaryEndpoint : ProfitSharingEndpoint<CreateBeneficiaryRequest, Results<Ok<CreateBeneficiaryResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IBeneficiaryService _beneficiaryService;
    private readonly ILogger<CreateBeneficiaryEndpoint> _logger;

    public CreateBeneficiaryEndpoint(IBeneficiaryService beneficiaryService, ILogger<CreateBeneficiaryEndpoint> logger)
        : base(Navigation.Constants.Beneficiaries)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/");
        Summary(s => { s.Summary = "Adds a new beneficiary"; });
        Group<BeneficiariesGroup>();
    }

    public override Task<Results<Ok<CreateBeneficiaryResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(CreateBeneficiaryRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var created = await _beneficiaryService.CreateBeneficiary(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "create-beneficiary"),
                new("endpoint.category", "beneficiaries"));

            _logger.LogInformation("Beneficiary created successfully, ID: {BeneficiaryId} (correlation: {CorrelationId})",
                created.BeneficiaryId, HttpContext.TraceIdentifier);

            return Result<CreateBeneficiaryResponse>.Success(created).ToHttpResult();
        });
    }
}
```

### CRUD Endpoint (Update with Validation)

```csharp
public sealed class UpdateDistributionEndpoint : ProfitSharingEndpoint<UpdateDistributionRequest, Results<Ok<CreateOrUpdateDistributionResponse>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<UpdateDistributionEndpoint> _logger;

    public UpdateDistributionEndpoint(IDistributionService distributionService, ILogger<UpdateDistributionEndpoint> logger)
        : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/");
        Group<DistributionGroup>();
        Summary(s => { s.Summary = "Updates an existing profit sharing distribution"; });
    }

    public override async Task<Results<Ok<CreateOrUpdateDistributionResponse>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(UpdateDistributionRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _distributionService.UpdateDistribution(req, ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-update"),
                new("endpoint", "UpdateDistributionEndpoint"));

            if (result.IsSuccess)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(1,
                    new("record_type", "distribution-updated"),
                    new("endpoint", "UpdateDistributionEndpoint"));

                _logger.LogInformation("Distribution updated for ID: {Id} (correlation: {CorrelationId})",
                    req.Id, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Distribution update failed for ID: {Id} - {Error} (correlation: {CorrelationId})",
                    req.Id, result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.DistributionNotFound, Error.BadgeNumberNotFound);
            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
```

### File Download Endpoint

```csharp
public sealed class CertificatesFileEndpoint : ProfitSharingEndpoint<CerficatePrintRequest, string>
{
    private readonly ICertificateService _certificateService;
    private readonly ILogger<CertificatesFileEndpoint> _logger;

    public CertificatesFileEndpoint(ICertificateService certificateService, ILogger<CertificatesFileEndpoint> logger)
        : base(Navigation.Constants.PrintProfitCerts)
    {
        _certificateService = certificateService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("post-frozen/certificates/download");
        Summary(s => { s.Summary = "Returns certificate file for pre-printed form letters"; });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(CerficatePrintRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var response = await _certificateService.GetCertificateFile(req, ct);
            var memoryStream = new MemoryStream();
            await using var writer = new StreamWriter(memoryStream);
            await writer.WriteAsync(response);
            await writer.FlushAsync(ct);

            memoryStream.Position = 0;
            var fileSize = memoryStream.Length;

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "certificate-file-download"),
                new("endpoint.category", "year-end"));

            EndpointTelemetry.RequestSizeBytes.Record(fileSize,
                new("direction", "response"),
                new("endpoint.category", "year-end"));

            _logger.LogInformation("Certificate file generated for profit year {ProfitYear}, size: {FileSize} bytes (correlation: {CorrelationId})",
                req.ProfitYear, fileSize, HttpContext.TraceIdentifier);

            HttpContext.Response.Headers.Append("Content-Disposition", "attachment; filename=\"PAYCERT.txt\"");
            await Send.StreamAsync(memoryStream, "PAYCERT.txt", contentType: "text/plain", cancellation: ct);

            this.RecordResponseMetrics(HttpContext, _logger, new { FileSize = fileSize, FileName = "PAYCERT.txt" }, isSuccess: true);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
```

### CSV Report Endpoint

```csharp
public class ExecutiveHoursAndDollarsEndpoint :
    EndpointWithCsvBase<ExecutiveHoursAndDollarsRequest, ExecutiveHoursAndDollarsResponse, ExecutiveHoursAndDollarsEndpoint.ExecutiveHoursAndDollarsMap>
{
    private readonly IExecutiveHoursAndDollarsService _reportService;
    private readonly IAuditService _auditService;
    private readonly ILogger<ExecutiveHoursAndDollarsEndpoint> _logger;

    public ExecutiveHoursAndDollarsEndpoint(IExecutiveHoursAndDollarsService reportService, IAuditService auditService, ILogger<ExecutiveHoursAndDollarsEndpoint> logger)
        : base(Navigation.Constants.ManageExecutiveHours)
    {
        _reportService = reportService;
        _auditService = auditService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("executive-hours-and-dollars");
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "Executive Hours and Dollars";

    public override async Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetResponse(ExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _auditService.ArchiveCompletedReportAsync(ReportFileName,
                req.ProfitYear,
                req,
                (archiveReq, isArchiveRequest, cancellationToken) =>
                {
                    if (isArchiveRequest)
                    {
                        archiveReq = archiveReq with
                        {
                            BadgeNumber = null,
                            FullNameContains = null,
                            Ssn = null,
                            HasExecutiveHoursAndDollars = true,
                            IsMonthlyPayroll = true
                        };
                    }
                    return _reportService.GetExecutiveHoursAndDollarsReportAsync(archiveReq, cancellationToken);
                },
                ct);

            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-executive-hours-dollars"),
                new("report_type", "executive-hours-dollars"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "executive-hours-dollars"),
                new("endpoint", "ExecutiveHoursAndDollarsEndpoint"));

            _logger.LogInformation("Report generated for year {ProfitYear}, {Count} records (correlation: {CorrelationId})",
                req.ProfitYear, resultCount, HttpContext?.TraceIdentifier ?? "test-correlation");

            this.RecordResponseMetrics(HttpContext, _logger, result);
            return result ?? new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] }
            };
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

    public sealed class ExecutiveHoursAndDollarsMap : ClassMap<ExecutiveHoursAndDollarsResponse>
    {
        public ExecutiveHoursAndDollarsMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE");
            Map(m => m.FullName).Index(1).Name("NAME");
            Map(m => m.StoreNumber).Index(2).Name("STR");
            Map(m => m.HoursExecutive).Index(3).Name("EXEC HRS");
            Map(m => m.IncomeExecutive).Index(4).Name("EXEC DOLS");
            Map(m => m.Ssn).Index(9).Name("SSN");
        }
    }
}
```

---
