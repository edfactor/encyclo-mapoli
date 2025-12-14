# Code Review Checklist

**Project:** Smart Profit Sharing - Demoulas  
**Purpose:** Quick reference for PR reviews and code consistency audits  
**Last Updated:** November 20, 2025

This checklist consolidates all architectural patterns, security requirements, and coding standards from project instructions. Use it to review PRs and audit code for consistency.

---

## Quick Review Guide

### For Small PRs (< 5 files changed)

Focus on: **Security**, **Coding Style**, **Testing**

### For Medium PRs (5-20 files)

Add: **Architecture**, **Telemetry**, **Validation**

### For Large PRs (20+ files or new features)

Review: **All sections** including documentation and branching

---

## Table of Contents

1. [Security (MANDATORY - OWASP Top 10)](#1-security-mandatory---owasp-top-10)
2. [Architecture & Data Access](#2-architecture--data-access)
3. [Backend - Endpoints](#3-backend---endpoints)
4. [Backend - Services](#4-backend---services)
5. [Backend - EF Core & Database](#5-backend---ef-core--database)
6. [Backend - Coding Style](#6-backend---coding-style)
7. [Frontend - React/TypeScript](#7-frontend---reacttypescript)
8. [Telemetry & Observability](#8-telemetry--observability)
9. [Validation & Error Handling](#9-validation--error-handling)
10. [Testing](#10-testing)
11. [Documentation](#11-documentation)
12. [Branching & Git Workflow](#12-branching--git-workflow)
13. [Performance & Safety](#13-performance--safety)

---

## 1. Security (MANDATORY - OWASP Top 10)

**All security items are CRITICAL. Deviations require security review.**

### Authentication & Authorization (A01/A07)

- [ ] **Server-side role validation**: Always re-validate roles server-side, never trust client headers

  ```csharp
  // ❌ WRONG: Trust header blindly
  var roles = req.Headers["x-impersonating-roles"];

  // ✅ RIGHT: Re-validate against authenticated user
  var allowedRoles = GetUserAllowedImpersonationRoles(userId);
  if (!requestedRoles.All(r => allowedRoles.Contains(r)))
      throw new UnauthorizedAccessException();
  ```

- [ ] **No localStorage for auth state**: Never use `localStorage` for roles/tokens that determine access
- [ ] **Centralized role validation**: Use `PolicyRoleMap.cs` for all authorization decisions
- [ ] **Principle of least privilege**: Users get minimum required roles
- [ ] **No client-side auth bypass**: Never implement authentication client-side only

**Related Tickets:** PS-2021, PS-2022

### Input Validation & SQL Injection (A03/A09)

- [ ] **All inputs validated server-side**: Client validation is UX only, never security
- [ ] **Parameterized queries only**: EF Core auto-parameterizes; NEVER construct SQL strings manually
- [ ] **Boundary checks present**: Numeric ranges, string lengths, collection sizes validated
  ```csharp
  if (pageSize < 1 || pageSize > 1000)
      throw new ValidationException("PageSize must be 1-1000");
  ```
- [ ] **Degenerate query guards**: Prevent queries that scan entire tables
  ```csharp
  if (badge == 0) throw new ValidationException("Badge cannot be zero");
  ```
- [ ] **Enum validation**: All enum values validated against allowed set
- [ ] **No raw SQL**: If needed, encapsulated in service layer with parameterization

### PII Protection & Data Exposure (A01/A09)

- [x] **PII masked in logs**: SSN, email, phone, bank accounts automatically masked ✅ (Implemented via shared common library with custom masking operators)

  ```csharp
  // Automatic masking via LoggingConfig in Program.cs
  LoggingConfig logConfig = new();
  builder.Configuration.Bind("SmartLogging", logConfig);

  logConfig.MaskingOperators = [
      new UnformattedSocialSecurityNumberMaskingOperator(),
      new SensitiveValueMaskingOperator()
  ];

  // Custom masking operators can be added as needed
  // Telemetry still uses MaskSensitiveValue() for explicit masking
  ```

- [ ] **Names masked in logs/telemetry**: First name, last name, full name MUST be masked when logged

  ```csharp
  // ❌ WRONG: Logging unmasked names
  _logger.LogInformation("Processing employee: {Name}", employee.FullName);

  // ✅ RIGHT: Mask names before logging
  var maskedName = TelemetryExtensions.MaskSensitiveValue(employee.FullName, "FullName");
  _logger.LogInformation("Processing employee: {MaskedName}", maskedName);
  ```

  **CRITICAL: ALL service methods with PII access MUST include masking in logs. No unmasked names, SSN, email, phone, or addresses should ever appear in logs.**

- [ ] **Standardized name properties in DTOs**: Use consistent naming conventions for name fields

  ```csharp
  // ✅ RIGHT: Standard property names
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string FullName { get; set; }  // Computed: FirstName + LastName

  // ❌ WRONG: Custom variations
  public string Name { get; set; }           // Too generic
  public string EmployeeName { get; set; }   // Non-standard
  public string MemberName { get; set; }     // Non-standard
  public string DisplayName { get; set; }    // Ambiguous
  ```

  **CRITICAL: ONLY use FullName. NEVER fallback to manual name concatenation like `LastName, FirstName` in DTOs or services. Always use the pre-computed FullName property.**

- [ ] **Minimal claims extraction**: Only extract 'sub' (subject) from Okta JWT
- [ ] **Read-only contexts used**: Use `UseReadOnlyContext()` for query-only operations
- [ ] **No SSN-only composite keys**: Use `(Ssn, OracleHcmId)` not just `Ssn`
- [ ] **No sensitive data in error messages**: Never expose PII in HTTP responses

### 🚨 CRITICAL: Age NEVER Calculated in Frontend (Auto-Reject)

**This is a BLOCKING issue. Reject immediately if found.**

- [ ] **Age calculation backend-only**: Age MUST be calculated by backend API, never in frontend JavaScript/React

  ```typescript
  // ❌ WRONG - AUTO-REJECT: Frontend age calculation
  const age = Math.floor(
    (Date.now() - new Date(dateOfBirth).getTime()) /
      (1000 * 60 * 60 * 24 * 365.25)
  );
  const dobDisplay = `${mmDDYYFormat(dateOfBirth)} (${age})`;

  // ✅ RIGHT: Display DOB only, let backend provide age if needed
  const dobDisplay = dateOfBirth ? mmDDYYFormat(dateOfBirth) : "N/A";
  ```

  **Why this is critical:**

  1. **Backend inconsistency**: Frontend calculation diverges from backend due to timezone and timing differences, causing data inconsistency
  2. **Sensitive data masking violation**: Age is sensitive data that must be masked for unprivileged users—frontend can't enforce masking rules
  3. **Access control breach**: Backend determines what fields are visible per user role; frontend calculation bypasses this control

  **Rejection template:**

  ```
  CRITICAL - AUTO-REJECT: Age must be calculated backend-only.

  • Remove frontend age calculation
  • Display DOB only (no age display)
  • If age required: request from backend API response

  Impact: Frontend age calculation causes data inconsistency, breaks sensitive data masking, bypasses access control.
  Reference: CODE_REVIEW_CHECKLIST.md - Security section
  ```

**Related Ticket:** PS-XXXX (Age calculation security fix)

**Related Ticket:** ~~PS-2026~~ (✅ PII masking via shared library)

### Transport Security (A02/A05)

- [x] **HTTPS enforcement**: HTTPS handled at load balancer; `UseHsts()` enabled via shared common library ✅ (PS-2024)
- [x] **Security headers present** ✅ (Implemented via shared common library):
  - `X-Frame-Options: DENY`
  - `X-Content-Type-Options: nosniff`
  - `Content-Security-Policy: default-src 'self'`
  - `Strict-Transport-Security: max-age=31536000`
- [x] **CORS restrictions**: Dev allows only `localhost:3100`; prod has specific domains only ✅ (PS-2025 completed)
- [x] **No `AllowAnyOrigin()`**: Never used in CORS configuration ✅ (PS-2025 completed)

**Related Tickets:** ~~PS-2025~~ (✅ Completed), ~~PS-2023~~ (✅ Headers via shared library), ~~PS-2024~~ (✅ HSTS via shared library, HTTPS at load balancer)

### Compliance Baseline (FISMA Moderate)

- [ ] **FISMA Moderate baseline**: Security-related changes MUST maintain alignment with a FISMA Moderate baseline (access control, auditability, crypto/transport protections, incident readiness, and PII protections). If a change impacts authN/authZ, logging/telemetry, masking, or data exposure, include explicit security validation steps in the PR.

### Error Handling & Secrets

- [ ] **No sensitive data in errors**: Stack traces, SQL queries, PII never in HTTP responses
- [ ] **Structured error codes**: Use domain errors (e.g., `Error.CalendarYearNotFound`)
- [ ] **Consistent error responses**: `ProblemHttpResult` with standard structure
- [ ] **Correlation IDs present**: All errors tied to audit trails
- [ ] **No hardcoded secrets**: Use Azure Key Vault or environment variables
- [ ] **Secrets in user-secrets**: Never commit to `.appsettings.json` or code

---

## 2. Architecture & Data Access

### Layer Separation

- [ ] **No DbContext in endpoints**: All EF Core access through services only
- [ ] **Services return `Result<T>`**: Endpoints convert to HTTP results
- [ ] **No direct DbSet access**: Endpoints call service methods, not repositories
- [ ] **Dependency injection used**: Constructor injection for all dependencies
- [ ] **No circular references**: Shared logic in `Common` or specialized projects

### Data Access Patterns

- [ ] **History tracking maintained**: Close current record (`ValidTo = now`), insert new row
- [ ] **Never overwrite history**: Historical rows are immutable
- [ ] **`OracleHcmId` preferred**: Fall back to `(Ssn,BadgeNumber)` only when missing
- [ ] **Helper methods used**: `UpdateEntityValues` used instead of scattered field assignments
- [ ] **No ad-hoc hosts**: Use `Demoulas.ProfitSharing.AppHost` (Aspire host) only

### Mapping & DTOs

- [ ] **Mapperly used**: Prefer Mapperly for DTO<->entity mapping
- [ ] **No duplicate mapping logic**: Use existing `*Mapper.cs` profiles
- [ ] **DTOs for data transfer**: All layers communicate via DTOs/ViewModels

---

## 3. Backend - Endpoints

### FastEndpoints Structure

- [ ] **Typed results returned**: `Results<Ok<T>, NotFound, ProblemHttpResult>` pattern
- [ ] **Domain `Result<T>` used**: Services return `Result<T>`, converted via `ToHttpResult()`
- [ ] **ResultHttpExtensions used**: `ToResultOrNotFound()` + `ToHttpResult()` for boilerplate reduction
- [ ] **Specific error codes**: `Error.SomeEntityNotFound` not generic messages
- [ ] **Example pattern followed**:
  ```csharp
  var result = await _svc.GetAsync(req.Id, ct);
  return result.ToHttpResult(Error.SomeEntityNotFound);
  ```

### URL & Path Design (RESTful Guidelines)

- [ ] **Resource-oriented URLs**: Nouns not verbs (e.g., `/members` not `/get-members`)
- [ ] **Kebab-case paths**: `/profit-year-distributions` not `/ProfitYearDistributions`
- [ ] **Plural resource names**: `/distributions` not `/distribution`
- [ ] **No trailing slashes**: `/distributions` not `/distributions/`
- [ ] **Domain-specific names**: `/profit-year-distributions` not just `/distributions`
- [ ] **HTTP methods correct**: GET (read), POST (create), PUT (update), DELETE (remove)

### Endpoint Registration

- [ ] **Grouped logically**: Use FastEndpoints groups for route organization
- [ ] **Consistent foldering**: Follows existing structure in `Endpoints/` project
- [ ] **DI registration**: Dependencies registered in composition root
- [ ] **Navigation integration**: Navigation constants updated if UI-accessible

### OpenAPI/Swagger

- [ ] **Endpoint summary present**: Clear description of what endpoint does
- [ ] **Request/response examples**: Sample payloads documented
- [ ] **Error responses documented**: 404, 400, 401, etc. with examples

**Reference:** `.github/instructions/restful-api-guidelines.instructions.md`

---

## 4. Backend - Services

### Service Layer Patterns

- [ ] **Async operations**: All methods use `async`/`await` with `CancellationToken`
- [ ] **Result pattern used**: Return `Result<T>`, not raw entities or throw exceptions
- [ ] **Validation in services**: Business rule validation before persistence
- [ ] **Transaction management**: Use transactions for multi-step operations
- [ ] **No endpoint concerns**: Services don't know about HTTP, JSON, status codes

### Service Examples

```csharp
public async Task<Result<MemberDto>> GetByIdAsync(int id, CancellationToken ct)
{
    await using var ctx = await _factory.CreateDbContextAsync(ct);
    ctx.UseReadOnlyContext();

    var member = await ctx.Members
        .TagWith($"GetMember-{id}")
        .FirstOrDefaultAsync(m => m.Id == id, ct);

    return member is null
        ? Result<MemberDto>.Failure(Error.MemberNotFound)
        : Result<MemberDto>.Success(member.ToDto());
}
```

**Reference:** `.github/instructions/services.instructions.md`

---

## 5. Backend - EF Core & Database

### EF Core 9 Patterns (MANDATORY)

- [ ] **`UseReadOnlyContext()` used**: For read-only operations (auto-applies `AsNoTracking`)
- [ ] **No manual `AsNoTracking()`**: When using `UseReadOnlyContext()` (redundant)
- [ ] **Query tagging present**: `TagWith()` for business context (year, operation, ticket)
  ```csharp
  var report = await _context.ProfitSharingRecords
      .TagWith($"YearEnd-{year}-Calc")
      .Where(r => r.ProfitYear == year)
      .ToListAsync(ct);
  ```
- [ ] **Async methods used**: `FirstOrDefaultAsync`, `ToListAsync` not synchronous variants
- [ ] **Explicit includes**: `Include()`/`ThenInclude()` used (NO lazy loading)
- [ ] **Bulk operations**: `ExecuteUpdateAsync`/`ExecuteDeleteAsync` for efficiency
  ```csharp
  await _context.Records
      .TagWith($"BulkUpdate-Status-{year}")
      .Where(r => r.Year == year)
      .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, newStatus), ct);
  ```

### Oracle-Specific Patterns

- [ ] **No `??` in queries**: Oracle provider fails—use `x != null ? x : "default"` instead
- [ ] **Case-insensitive search**: Use `EF.Functions.Like(m.Name, "%search%")`
- [ ] **No raw SQL**: If needed, parameterized and in service layer only

### Dictionary Keys with Demographics (CRITICAL)

- [ ] **Composite keys used**: `(d.Ssn, d.OracleHcmId)` not just `d.Ssn`

  ```csharp
  // ✅ RIGHT: Composite key
  var demographicsByKey = demographics
      .ToDictionary(d => (d.Ssn, d.OracleHcmId), d => d);

  // ❌ WRONG: SSN only (will throw duplicate key exception at runtime)
  var demographicsByKey = demographics
      .ToDictionary(d => d.Ssn, d => d);  // CRASH if duplicate SSNs exist

  // ❌ ALSO WRONG: Async variant with SSN only - CAUSES RUNTIME CRASH
  var totalBalances = await _totalService.GetTotalBalanceSet(ctx, request.ProfitYear)
      .Where(tb => employeeSsns.Contains(tb.Ssn))
      .ToDictionaryAsync(tb => tb.Ssn, cancellationToken);  // CRASH if duplicate SSNs
  ```

  **CRITICAL: Using SSN alone as dictionary key WILL crash at runtime if duplicates exist. ALWAYS use composite key `(Ssn, Id)` or use `ToLookup()` for one-to-many relationships.**

- [ ] **Guards present**: Check `OracleHcmId != 0` before dictionary operations
- [ ] **`ToLookup` for one-to-many**: Use when duplicates expected (preferred for TotalBalance queries)

### Performance Patterns

- [ ] **Projection used**: Select only needed columns for DTOs
- [ ] **Lookups pre-computed**: `ToDictionary`/`ToLookup` before loops
- [ ] **Degenerate guards**: Input validation prevents table scans
- [ ] **`ConfigureAwait(false)`**: Used in library/service layer async calls

### Migrations

- [ ] **Migration naming**: Singular imperative (e.g., `AddMemberIndex`, `RenameColumnXToY`)
- [ ] **Migration tested**: Applied to dev database before PR
- [ ] **CLI utility used**: `Demoulas.ProfitSharing.Data.Cli` for schema operations

---

## 6. Backend - Coding Style

### Naming & Formatting

- [ ] **File-scoped namespaces**: `namespace Demoulas.ProfitSharing.Services;` not `{ }`
- [ ] **One class per file**: Each type in separate file
- [ ] **Explicit access modifiers**: All types and members have modifiers
- [ ] **PascalCase public methods**: Public APIs use PascalCase
- [ ] **`_camelCase` private fields**: Private fields start with underscore
- [ ] **`s_` static prefix**: Private static fields use `s_` prefix
- [ ] **PascalCase constants**: Constants use PascalCase (not UPPER_SNAKE)
- [ ] **Explicit types**: Unless initializer makes type obvious
- [ ] **`readonly` where applicable**: Immutable fields marked readonly
- [ ] **Braces on control blocks**: Always use braces (even single-line)

### Language Features

- [ ] **Null propagation**: Use `?.` and `??` (except in EF queries)
- [ ] **`is null` / `is not null`**: Preferred over `== null`
- [ ] **`nameof(...)` used**: For member/type references
- [ ] **Pattern matching**: Use where it improves clarity
- [ ] **Switch expressions**: Preferred over traditional switch statements
- [ ] **XML doc comments**: Public & internal APIs documented

### AsyncFixer01 Compliance (CRITICAL)

**AsyncFixer analyzer enforces async/await patterns. Violations are build errors.**

- [ ] **Single await returns Task directly** (AsyncFixer01 - MANDATORY):

  ```csharp
  // ❌ WRONG: Unnecessary async wrapper
  public async Task<Result<T>> GetAsync(int id, CancellationToken ct)
  {
      return await _service.FetchAsync(id, ct);  // AsyncFixer01 error
  }

  // ✅ RIGHT: Return Task directly
  public Task<Result<T>> GetAsync(int id, CancellationToken ct)
  {
      return _service.FetchAsync(id, ct);
  }
  ```

- [ ] **`async` keyword only when needed**: Multiple awaits, error handling, resource cleanup
- [ ] **Never wrap single awaits**: Remove unnecessary `async/await` wrappers
- [ ] **Exception: Test methods are `async Task`**: Test frameworks require it
- [ ] **No AsyncFixer01 violations in build**: All violations fixed before PR

### Project-Specific Conventions

- [ ] **Constructor injection**: Dependencies injected through constructor
- [ ] **`Result<T>` pattern**: For error handling instead of exceptions
- [ ] **DTOs for transfer**: Between layers

**Reference:** `.editorconfig`

### Decimal Rounding (CRITICAL for Financial Calculations)

**Always use `MidpointRounding.AwayFromZero` for monetary/financial calculations** to match COBOL behavior (traditional rounding, not banker's rounding).

- [ ] **`MidpointRounding.AwayFromZero` used**: For all financial calculations

  ```csharp
  // ✅ RIGHT: Use AwayFromZero for financial calculations (matches COBOL)
  var roundedAmount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

  // ❌ WRONG: Default rounding uses ToEven (banker's rounding)
  var roundedAmount = Math.Round(amount, 2);  // Uses MidpointRounding.ToEven - INCORRECT
  ```

- [ ] **When to use**: Tax calculations (federal, state), distribution amounts, forfeiture amounts, any monetary aggregation or reporting, profit sharing calculations
- [ ] **Why critical**: COBOL uses traditional rounding (0.5 rounds up), while .NET defaults to banker's rounding (0.5 rounds to nearest even). This causes penny differences in financial reports that don't match READY data.
- [ ] **Validation**: Financial calculation unit tests verify rounded values match READY production values "to the penny"

**Related Tickets:** PS-2275 (QPAY129 YTD totals rounding validation)

---

## 7. Frontend - React/TypeScript

### Component Structure

- [ ] **Functional components**: Use hooks, not class components
- [ ] **TypeScript types**: All props/state typed (no `any`)
- [ ] **Type files**: Complex types in separate `types.ts` files
- [ ] **Hooks extracted**: Custom hooks in `hooks/` folder
- [ ] **Component colocated**: Grid definitions, filters, etc. near parent component

### State Management

- [ ] **Redux Toolkit**: API/data logic centralized in `src/reduxstore/`
- [ ] **RTK Query**: Preferred for API calls
- [ ] **Slice patterns**: Follow existing slice structure
- [ ] **No prop drilling**: Use Redux for deeply nested state

### Styling

- [ ] **Tailwind utilities**: Utility-first approach
- [ ] **No inline styles**: For reusable patterns (create components instead)
- [ ] **`tailwind.config.js`**: Extended for custom utilities
- [ ] **Consistent spacing**: Use Tailwind spacing scale

### AG Grid Configuration (CRITICAL)

- [ ] **Grid filter disabled by default**: All column definitions must have `filter: false`

  ```typescript
  // ❌ CRITICAL WRONG: Enables all columns to be filterable
  const cols: ColDef[] = Object.keys(sampleData).map((key) => ({
    headerName: key.replace(/([A-Z])/g, " $1").trim(),
    field: key,
    sortable: true,
    filter: true, // WRONG - enables filtering on every column
    resizable: true,
  }));

  // ✅ RIGHT: Filters disabled, only sortable
  const cols: ColDef[] = Object.keys(sampleData).map((key) => ({
    headerName: key.replace(/([A-Z])/g, " $1").trim(),
    field: key,
    sortable: true,
    filter: false, // CORRECT - no filtering by default
    resizable: true,
  }));
  ```

  **CRITICAL: AG Grid filters must be disabled by default on all columns. Always set `filter: false` unless there's a specific business requirement to enable filtering.**

### Validation

- [ ] **Client-side mirrors server**: Same validation rules
- [ ] **Client validation is UX only**: Never rely on it for security
- [ ] **Error messages consistent**: Match backend error format

### Testing

- [ ] **Playwright tests**: E2E tests in `src/ui/e2e`
- [ ] **`.playwright.env`**: Credentials from env file (no hardcoded secrets)
- [ ] **Component tests**: Follow patterns if colocated

### 🚨 Frontend Security Critical Violations (Auto-Reject)

**These are blocking issues that must be rejected immediately:**

- [ ] **Age NEVER calculated in frontend**: See Security section above (auto-reject violation)
- [ ] **No unmasked PII in logs/console**: SSN, email, phone, bank accounts must be masked
- [ ] **No client-side role elevation**: Never use `localStorage` to set roles that determine access
- [ ] **No direct fetch/axios**: Use RTK Query hooks instead
- [ ] **No stack traces in error messages**: Never expose technical details to users

**Reference:** Security section for age calculation details

**Reference:** `.github/instructions/pages.instructions.md`

---

## 8. Telemetry & Observability

### Telemetry Implementation (MANDATORY)

**All endpoints MUST implement comprehensive telemetry.**

- [ ] **`ExecuteWithTelemetry` used**: Wrapper pattern for automatic telemetry (recommended)

  ```csharp
  return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
  {
      var result = await _service.ProcessAsync(req, ct);

      EndpointTelemetry.BusinessOperationsTotal.Add(1,
          new("operation", "year-end-processing"),
          new("endpoint", nameof(MyEndpoint)));

      return result;
  }, "Ssn", "OracleHcmId", "FirstName", "LastName"); // ALL sensitive fields declared
  ```

- [ ] **Logger injected**: `ILogger<TEndpoint>` in constructor
- [ ] **Using statements**: `using Demoulas.ProfitSharing.Endpoints.Extensions;`
- [ ] **Activity started**: `StartEndpointActivity()` called if manual telemetry
- [ ] **Request metrics recorded**: `RecordRequestMetrics()` called
- [ ] **Response metrics recorded**: `RecordResponseMetrics()` called
- [ ] **Exceptions recorded**: `RecordException()` with correlation ID
- [ ] **Sensitive fields declared**: List ALL PII fields accessed including names (e.g., `"Ssn", "Email", "FirstName", "LastName", "FullName"`)

### Business Metrics

- [ ] **Business operations tracked**: `BusinessOperationsTotal.Add()` called
  ```csharp
  EndpointTelemetry.BusinessOperationsTotal.Add(1,
      new("operation", "year-end-profit-calculation"),
      new("endpoint", "YearEndProcessEndpoint"),
      new("profit_year", "2025"));
  ```
- [ ] **Record counts tracked**: `RecordCountsProcessed.Record()` for data volumes
  ```csharp
  EndpointTelemetry.RecordCountsProcessed.Record(employeeCount,
      new("record_type", "employees-processed"),
      new("endpoint", "EmployeeProcessingEndpoint"));
  ```
- [ ] **Operation labels correct**: Use standard operation names (see telemetry guide)

### PII Protection in Telemetry

- [ ] **PII masked**: Actual SSN/email/phone/name values NEVER logged
- [ ] **Masking function used**: `MaskSensitiveValue()` for any PII logging (SSN, email, names)
- [ ] **Correlation IDs used**: For debugging without exposing PII
- [ ] **Sensitive field tracking**: Declared in telemetry calls (security requirement) - include `FirstName`, `LastName`, `FullName` when accessed

### Telemetry Testing

- [ ] **Unit tests verify telemetry**: Tests check activity creation, metrics recording
- [ ] **Mock logger verified**: Test assertions on logger calls

**Reference:** `TELEMETRY_GUIDE.md` (src/ui/public/docs/)

### OpenTelemetry Configuration (NEW ENVIRONMENTS or Config Changes)

**Applies to:** New environment setup, Dynatrace migration, infrastructure changes

- [ ] **Temporality preference set to "delta"**: `OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE: "delta"` (CRITICAL - not the OTLP SDK default)
- [ ] **OTEL endpoint configured**: `OTEL_EXPORTER_OTLP_ENDPOINT` points to correct Dynatrace instance
- [ ] **OTEL protocol set correctly**: `OTEL_EXPORTER_OTLP_PROTOCOL: "http/protobuf"` (binary format, not gRPC)
- [ ] **Service name follows convention**: `OTEL_SERVICE_NAME: "ProfitSharing-{Environment}"` (e.g., ProfitSharing-QA)
- [ ] **API token secured**: `OTEL_EXPORTER_OTLP_HEADERS` uses environment variable or Key Vault, NOT hardcoded
- [ ] **HTTP/1.1 forced**: `OTEL_EXPORTER_OTLP_FORCE_HTTP11: true` (Dynatrace prefers HTTP/1.1)
- [ ] **SSL verification production-ready**: `OTEL_EXPORTER_OTLP_IGNORE_SSL: false` in production (true only for dev self-signed certs)
- [ ] **Resource attributes populated**: `OTEL_RESOURCE_ATTRIBUTES` includes service name, environment, version for Dynatrace grouping
- [ ] **All environments have same settings**: Development, Testing, QA, UAT appsettings include temporality preference for consistency

**Why This Matters:**

- Dynatrace doesn't support "cumulative" temporality (the OTLP SDK default) - metrics will silently fail to ingest
- Configuration drift (different settings per environment) causes debugging nightmares when metrics appear in some envs but not others
- Missing API token permissions causes silent failures (no error message, just no metrics)
- `http/protobuf` is required (Dynatrace doesn't support gRPC)

**Reference:** `.github/OTEL_CONFIGURATION_QUICK_REFERENCE.md`, `.github/TELEMETRY_CONFIGURATION_REVIEW.md`

---

## 9. Validation & Error Handling

### Server-Side Validation (MANDATORY)

- [ ] **FluentValidation used**: All request DTOs have validators
  ```csharp
  public class SearchRequestValidator : AbstractValidator<SearchRequest>
  {
      public SearchRequestValidator()
      {
          RuleFor(x => x.PageSize)
              .InclusiveBetween(1, 1000)
              .WithMessage("PageSize must be between 1 and 1000.");
      }
  }
  ```
- [ ] **Numeric ranges**: Min/max bounds validated
- [ ] **String lengths**: Max length constraints
- [ ] **Collection sizes**: Prevent unbounded collections
- [ ] **Enum validation**: All enum values validated
- [ ] **Date ranges**: Start/end date validation
- [ ] **Required fields**: Null checks on required properties

### Client-Side Validation

- [ ] **Mirrors server rules**: Same constraints as backend
- [ ] **UX only**: Never security-critical
- [ ] **Error messages**: User-friendly, consistent with backend

### Error Handling

- [ ] **Domain errors used**: `Error.MemberNotFound` not generic exceptions
- [ ] **`Result<T>` pattern**: Services return results, not throw
- [ ] **HTTP mapping correct**: `Result<T>` mapped to appropriate status codes
- [ ] **Problem JSON format**: Errors follow RFC 7807 structure
- [ ] **No sensitive data**: Stack traces, SQL, PII never exposed
- [ ] **Correlation IDs**: All errors include correlation for tracing

**Reference:** `VALIDATION_PATTERNS.md` (.github/)

---

## 10. Testing

### Backend Tests

- [ ] **Consolidated test project**: Tests in `Demoulas.ProfitSharing.UnitTests` only
- [ ] **xUnit + Shouldly**: Test framework and assertions
- [ ] **Namespace mirroring**: Test namespaces match source structure
- [ ] **`Description` attribute**: Tests tagged with Jira ticket
  ```csharp
  [Description("PS-1721 : Duplicate detection by contribution year")]
  public async Task MyNewTest() { ... }
  ```
- [ ] **Async tests**: Use `async Task` for all async operations
- [ ] **Deterministic data**: Use Bogus builders for test data
- [ ] **Telemetry verified**: Tests check telemetry integration
- [ ] **Boundary cases covered**: Edge cases, nulls, empty collections tested

### Frontend Tests

- [ ] **Playwright for E2E**: End-to-end tests in `src/ui/e2e`
- [ ] **No hardcoded secrets**: Credentials from `.playwright.env`
- [ ] **Component tests**: Follow established patterns

### Test Quality

- [ ] **Meaningful names**: Test names describe what's tested
- [ ] **Arrange-Act-Assert**: Clear test structure
- [ ] **One assertion per test**: Or related assertions only
- [ ] **No test interdependencies**: Tests run in any order

**Reference:** `ai-templates/front-end/fe-unit-tests.md`

---

## 11. Documentation

### Code Documentation

- [ ] **XML doc comments**: Public/internal APIs documented
- [ ] **Complex logic explained**: Non-obvious code has comments
- [ ] **TODO/HACK/FIXME**: Tracked with ticket numbers

### Feature Documentation

- [ ] **User-accessible docs**: In `src/ui/public/docs/` folder
- [ ] **Naming convention**: `UPPERCASE_WITH_UNDERSCORES.md` for major guides
- [ ] **README updated**: New docs added to README
- [ ] **Documentation page updated**: `Documentation.tsx` includes new docs
- [ ] **Structure standards**:
  - Overview section with objectives
  - Architecture/Implementation sections with code examples
  - Testing/Quality guidelines with checklists
  - Troubleshooting section
  - References section with links

### Documentation for New Patterns

- [ ] **Instructions updated**: `copilot-instructions.md` and `CLAUDE.md` updated
- [ ] **Examples provided**: Copy-paste code examples
- [ ] **Checklists included**: Implementation and testing checklists
- [ ] **Do/Don't examples**: Clear guidance on patterns

---

## 12. Branching & Git Workflow

### Branch Naming

- [ ] **Branched from `develop`**: Never from `main`
- [ ] **Naming pattern**: `feature/PS-1720-add-reporting-view`
  - Type: `feature`, `bugfix`, `hotfix`, `chore`
  - Jira key: `PS-1234`
  - Description: kebab-case, 3-5 words

### Commit Messages

- [ ] **Jira key prefix**: `PS-1720: Add reporting view`
- [ ] **Imperative mood**: "Add feature" not "Added feature"
- [ ] **Concise summary**: < 72 characters
- [ ] **Body if needed**: Wrapped at 72 characters

### Pull Requests

- [ ] **PR title starts with Jira key**: `PS-1720: Add reporting view`
- [ ] **Description present**: What changed, why, testing done
- [ ] **Deviations explained**: Reasoning for any pattern deviations
- [ ] **Links to tickets**: Jira ticket linked
- [ ] **No auto-merge**: Human review required
- [ ] **Build passes**: All tests green
- [ ] **No security warnings**: Analyzer warnings treated as errors

### Git Practices

- [ ] **No force push**: To shared branches
- [ ] **Clean history**: Logical commits, rebased if needed
- [ ] **No merge commits**: Use rebase workflow
- [ ] **`.gitignore` respected**: No ignored files committed

**Reference:** `BRANCHING_AND_WORKFLOW.md` (.github/)

---

## 13. Performance & Safety

### Performance Patterns

- [ ] **Lookups pre-computed**: `ToDictionary`/`ToLookup` before loops
- [ ] **Dynamic expressions**: Built for filters (not N roundtrips)
- [ ] **Degenerate query guards**: Input validation prevents table scans
- [ ] **Bulk operations**: `ExecuteUpdate`/`ExecuteDelete` for efficiency
- [ ] **Projection used**: Select only needed columns
- [ ] **Read-only contexts**: `UseReadOnlyContext()` for queries

### Caching

- [ ] **`IDistributedCache` used**: NOT `IMemoryCache`
- [ ] **Version-based invalidation**: Not pattern-based deletion
- [ ] **Version counters persist**: No expiration on version counters
- [ ] **Low-cardinality keys**: No user IDs in cache keys
- [ ] **Graceful degradation**: Operations don't fail on cache errors

### Batched Operations

- [ ] **Precomputed lookups**: Before DB roundtrips
- [ ] **Dynamic OR expressions**: Instead of N queries
- [ ] **Guard against degenerate cases**: E.g., all-zero badge numbers

**Reference:** `DISTRIBUTED_CACHING_PATTERNS.md` (.github/)

---

## PR Review Workflow

### Before Submitting PR

1. [ ] Self-review against this checklist
2. [ ] Run all tests locally (backend + frontend)
3. [ ] Build passes with no warnings
4. [ ] Code formatted per `.editorconfig`
5. [ ] Documentation updated if needed
6. [ ] Secrets not committed
7. [ ] Branch up-to-date with `develop`

### Reviewer Checklist

1. [ ] PR title/description clear and complete
2. [ ] Security section reviewed (MANDATORY)
3. [ ] Telemetry present on all new endpoints
4. [ ] Tests added/updated
5. [ ] Architecture patterns followed
6. [ ] No deviations without justification
7. [ ] Documentation updated if needed
8. [ ] Build and tests pass

### Common Issues

- **Missing telemetry**: All endpoints require comprehensive telemetry
- **DbContext in endpoints**: Move to service layer
- **Client-side auth**: Re-validate server-side
- **PII in logs**: Use masking functions for SSN, email, phone, names
- **Non-standard DTO property names**: Use `FirstName`, `LastName`, `FullName` not `Name`, `EmployeeName`, etc.
- **Synchronous EF methods**: Use async variants
- **Hardcoded secrets**: Move to user secrets
- **Missing validation**: Server-side validation required
- **`??` in EF queries**: Use explicit conditionals

---

## Quick Commands

### Run Tests

```pwsh
# Backend tests
# NOTE: Tests use xUnit v3 + Microsoft Testing Platform (MTP).
# Run from src/services so global.json test runner settings are applied.
cd src/services
dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj

# Frontend tests
cd src/ui
npm test
```

### Check for Issues

```pwsh
# Build with warnings as errors
dotnet build Demoulas.ProfitSharing.slnx

# Check for outdated packages
dotnet list package --outdated

# Frontend lint
cd src/ui
npm run lint
```

### Start Application

```pwsh
# From project root
aspire run
```

---

## Resources

### Primary References

- **copilot-instructions.md** - Core architecture and patterns
- **TELEMETRY_GUIDE.md** - Comprehensive telemetry reference
- **VALIDATION_PATTERNS.md** - Validation guidelines
- **DISTRIBUTED_CACHING_PATTERNS.md** - Caching patterns
- **BRANCHING_AND_WORKFLOW.md** - Git workflow

### Instruction Files

- `.github/instructions/restful-api-guidelines.instructions.md`
- `.github/instructions/endpoints.instructions.md`
- `.github/instructions/services.instructions.md`
- `.github/instructions/pages.instructions.md`
- `.github/instructions/redux.instructions.md`

### Security Tickets

- **PS-2021**: Remove localStorage impersonation
- **PS-2022**: Server-side role validation
- **PS-2023**: ✅ Security headers middleware (Implemented via shared common library)
- **PS-2024**: ✅ HTTPS + HSTS enforcement (HSTS via shared library, HTTPS at load balancer)
- **PS-2025**: ✅ CORS restrictions (Completed - localhost-only in dev)
- **PS-2026**: ✅ Telemetry PII masking (Implemented via shared library with custom masking operators)

---

## Version History

| Version | Date       | Changes                         |
| ------- | ---------- | ------------------------------- |
| 1.0     | 2025-11-20 | Initial comprehensive checklist |

---

**For questions or clarifications, contact the development team or see referenced documentation.**
