# CLAUDE.md

AI Assistant Project Instructions for Claude Code (claude.ai/code) when working with this repository. Focus on THESE patterns; avoid generic advice.

## Architecture Overview

- Monorepo with two primary roots:
  - `src/services/` (.NET 9) multi-project solution `Demoulas.ProfitSharing.slnx` (FastEndpoints, EF Core 9 + Oracle, Aspire, Serilog, Feature Flags, RabbitMQ, Mapperly, Shouldly)
  - `src/ui/` (Vite + React + TypeScript + Tailwind + Redux Toolkit + internal `smart-ui-library`)
- Database: Oracle 19. EF Core migrations via `ProfitSharingDbContext`; CLI utility project `Demoulas.ProfitSharing.Data.Cli` performs schema ops & imports from legacy READY system
- Cross-cutting: Central package mgmt (`Directory.Packages.props`), shared build config (`Directory.Build.props`), global SDK pin (`global.json`)

## Key Backend Conventions

- Startup/entrypoint: run/debug `Demoulas.ProfitSharing.AppHost` (Aspire host). Avoid creating new ad-hoc hosts
- Use FastEndpoints; group endpoint files logically. Prefer minimal API style. Return typed results + proper status codes
- Mapping: Prefer `Mapperly` for DTO<->entity; follow existing mapper classes (see `*Mapper.cs`). Don't hand-write repetitive mapping unless customization is needed
- Data access: Use async EF Core patterns. Bulk maintenance uses `ExecuteUpdate/ExecuteDelete` where safe. For dynamic filters, build expressions (see `DemographicsService`). Avoid raw SQL unless performance justified—then parameterize
- Distributed Caching: Use `IDistributedCache` (NOT `IMemoryCache`). Serialize with `System.Text.Json`. For multi-key scenarios, use version-based invalidation (store version counter, increment on updates, include version in cache keys). Example: `navigation-tree-v{version}-{roles}`. Version bumps make old keys unreachable—no pattern deletion needed. Cache expiration: 30 min absolute + 15 min sliding. Unit tests: `MemoryDistributedCache`. Degrade gracefully on cache failures. See `NavigationService.GetNavigation()` for reference implementation. Read DISTRIBUTED_CACHING_PATTERNS.md (`.github/`) when needed for complete patterns
- Auditing & History: When updating mutable domain entities with historical tracking (example: `DemographicsService` creating `DemographicHistory` with `ValidFrom/ValidTo`), replicate the pattern: close current record (`ValidTo = now`), insert new history row. NEVER overwrite historical rows
- Identifiers: `OracleHcmId` is authoritative when present; fall back to composite `(Ssn,BadgeNumber)` only when Oracle id missing. Mirror guard logic (skip ambiguous BadgeNumber == 0 cases) if extending
- Entity updates: Keep helper methods like `UpdateEntityValues` cohesive; prefer adding fields there instead of scattering manual per-field assignments
- Validation errors & audits: Use `DemographicSyncAudit` pattern—batch add + save once. When adding new audit types, follow existing property naming

## Endpoint Results Pattern (MANDATORY)

All FastEndpoints MUST return typed minimal API union results AND internally use the domain `Result<T>` record (`Demoulas.ProfitSharing.Common.Contracts.Result<T>`) for service-layer outcomes.

Patterns:
- Service layer returns/constructs `Result<T>` (Success, Failure, ValidationFailure)
- Endpoint converts domain result via `Match` (or helper) to: `Results<Ok<T>, NotFound, ProblemHttpResult>` (queries) or `Results<Ok, ProblemHttpResult>` (commands). Include `NotFound` for resource-missing semantics; add `ValidationProblem` only if you propagate structured validation errors directly
- Helpers: Use `ResultHttpExtensions.ToResultOrNotFound()` + `ToHttpResult()` to reduce boilerplate (e.g., `dto.ToResultOrNotFound(Error.CalendarYearNotFound).ToHttpResult(Error.CalendarYearNotFound)`)
- Implicit: `Result<T>` has an implicit conversion to `Results<Ok<T>, NotFound, ProblemHttpResult>` but ONLY use it when you do not need to distinguish specific not-found errors (otherwise call `ToHttpResult(Error.SomeNotFound)`)
- Errors: Use specific not-found codes (e.g., `Error.CalendarYearNotFound`). Avoid reusing unrelated error descriptions to trigger NotFound
- Map: `Success => TypedResults.Ok(value)`, not-found error => `TypedResults.NotFound()`, other errors/validation => `TypedResults.Problem(problem.Detail)`
- Example (explicit mapping):
  ```csharp
  var result = await _svc.GetAsync(req.Id, ct);
  return result.ToHttpResult(Error.SomeEntityNotFound);
  ```
- Avoid returning raw DTOs or nulls; always wrap service outcomes in `Result<T>` before translating to HTTP
- Catch unexpected exceptions and map to `TypedResults.Problem(ex.Message)` (logging appropriately) unless a global handler already standardizes this

## EF Core 9 Patterns & Best Practices (MANDATORY)

We use **EF Core 9** with Oracle provider. All DB access MUST follow these patterns.

**CRITICAL**: Use `context.UseReadOnlyContext()` for read-only ops—it auto-applies `.AsNoTracking()`. Do NOT add `.AsNoTracking()` when using `UseReadOnlyContext()`.

### Query Tagging (Recommended)
Tag queries for production traceability:
- `TagWith()`: Add business context (year, user, operation, ticket) - **Required for complex operations**

```csharp
// Business context tagging (required for year-end, reports, etc.)
var report = await _context.ProfitSharingRecords
    .TagWith($"YearEnd-{year}-Calc")
    .Where(r => r.ProfitYear == year)
    .ToListAsync(ct);

// Optional: Add call site for detailed tracing
var data = await _context.Employees
    .TagWith($"Report-{reportType}")
    .Where(e => e.IsActive)
    .ToListAsync(ct);
```

### Oracle-Specific Patterns
- **NO `??` in queries**: Oracle provider fails—use `x != null ? x : "default"` instead
- **String search**: Use `EF.Functions.Like(m.Name, "%search%")` for case-insensitive

### Bulk Operations (ExecuteUpdate/ExecuteDelete)
Use EF9 bulk ops—no entity loading, single SQL, efficient:
```csharp
await _context.Records
    .TagWith($"BulkUpdate-Status-{year}")
    .Where(r => r.Year == year)
    .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, newStatus), ct);
```

### Performance Patterns
**Read-only (preferred)**:
**Read-only (preferred)**:
```csharp
await using var ctx = await _factory.CreateDbContextAsync(ct);
ctx.UseReadOnlyContext(); // Auto AsNoTracking
var data = await ctx.Members.TagWith("GetMembers").ToListAsync(ct);
```

**Projection**: Select only needed columns for DTOs
**Lookups**: Pre-compute `ToDictionary`/`ToLookup` before loops
**Degenerate guard**: Validate inputs (e.g., prevent all-zero badge numbers)

### Critical Rules
- Services only—NO DbContext in endpoints
- Always async (`FirstOrDefaultAsync`, `ToListAsync`)
- Explicit `Include()`/`ThenInclude()`—NO lazy loading
- Validate inputs to prevent table scans

### Example Service
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

## Telemetry & Observability Patterns (MANDATORY)

All FastEndpoints MUST implement comprehensive telemetry using the established `TelemetryExtensions` patterns. Telemetry provides critical visibility into application usage, performance, security, and business operations for development, QA, and production support teams.

### Required Implementation (Choose One Pattern)

**Automatic Pattern (Recommended)**: Use the `ExecuteWithTelemetry` wrapper for comprehensive telemetry with minimal code:

```csharp
using Demoulas.ProfitSharing.Common.Telemetry;

public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
{
    return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
    {
        // Your business logic here
        var result = await _service.ProcessAsync(req, ct);
        
        // Add business metrics (required for business operations)
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "year-end-processing"),
            new("endpoint", nameof(MyEndpoint)));
            
        return result;
    }, "Ssn", "OracleHcmId"); // List all sensitive fields accessed
}
```

**Manual Pattern (Advanced Control)**: Use individual telemetry methods for fine-grained control:

```csharp
using Demoulas.ProfitSharing.Common.Telemetry;

private readonly ILogger<MyEndpoint> _logger;

public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
{
    using var activity = this.StartEndpointActivity(HttpContext);
    
    try
    {
        // Record request metrics (required)
        this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn", "OracleHcmId");
        
        // Business logic
        var response = await _service.ProcessAsync(req, ct);
        
        // Business metrics (required for business operations)
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "employee-lookup"),
            new("endpoint", nameof(MyEndpoint)));
            
        // Record count metrics (when processing collections)
        if (response.Records?.Count > 0)
        {
            EndpointTelemetry.RecordCountsProcessed.Record(response.Records.Count,
                new("record_type", "employee"),
                new("endpoint", nameof(MyEndpoint)));
        }
        
        // Record response metrics (required)
        this.RecordResponseMetrics(HttpContext, _logger, response);
        
        return response;
    }
    catch (Exception ex)
    {
        // Record exception metrics (required)
        this.RecordException(HttpContext, _logger, ex, activity);
        throw;
    }
}
```

### Logger Injection (Required)

All endpoints MUST inject `ILogger<TEndpoint>` for telemetry correlation and structured logging:

```csharp
public class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    private readonly IMyService _service;
    private readonly ILogger<MyEndpoint> _logger; // Required for telemetry
    
    public MyEndpoint(IMyService service, ILogger<MyEndpoint> logger)
    {
        _service = service;
        _logger = logger;
    }
}
```

### Business Metrics (Context-Specific)

Add business operation metrics appropriate to the endpoint category:

**Year-End Operations**:
```csharp
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "year-end-enrollment"),
    new("profit_year", profitYear.ToString()),
    new("endpoint", nameof(YearEndEnrollmentEndpoint)));
```

**Report Generation**:
```csharp
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "report-generation"),
    new("report_type", "profit-sharing"),
    new("endpoint", nameof(ProfitSharingReportEndpoint)));
```

**Employee Lookups**:
```csharp
EndpointTelemetry.BusinessOperationsTotal.Add(1,
    new("operation", "employee-lookup"),
    new("lookup_type", "by-ssn"),
    new("endpoint", nameof(EmployeeLookupEndpoint)));
```

### Sensitive Field Guidelines (CRITICAL)

When endpoints access sensitive fields, ALWAYS list them in telemetry calls:

**Common Sensitive Fields**:
- `"Ssn"` - Social Security Numbers
- `"OracleHcmId"` - Internal employee identifiers  
- `"BadgeNumber"` - Employee badge numbers
- `"Salary"` - Salary information
- `"BeneficiaryInfo"` - Beneficiary details

**Examples**:
```csharp
// Single sensitive field
this.ExecuteWithTelemetry(HttpContext, _logger, req, async () => { ... }, "Ssn");

// Multiple sensitive fields
this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn", "OracleHcmId", "Salary");

// No sensitive fields (common for lookup endpoints)
this.ExecuteWithTelemetry(HttpContext, _logger, req, async () => { ... });
```

### Migration from Legacy Patterns

**Updating Existing Endpoints**:
- Replace ad-hoc OpenTelemetry activity creation with `StartEndpointActivity`
- Replace manual metrics with `TelemetryExtensions` methods
- Consolidate scattered telemetry logic using `ExecuteWithTelemetry` wrapper
- Add missing logger injection where absent
- Ensure sensitive field declarations are complete

### Documentation References

Complete implementation details and examples available in (read when needed):
- TELEMETRY_GUIDE.md (`src/ui/public/docs/`) - Comprehensive reference for developers, QA, and DevOps
- TELEMETRY_QUICK_REFERENCE.md (`src/ui/public/docs/`) - Developer cheat sheet with copy-paste examples  
- TELEMETRY_DEVOPS_GUIDE.md (`src/ui/public/docs/`) - Production monitoring and operations guide

## Backend Coding Style

- File-scoped namespaces; one class per file; explicit access modifiers
- Prefer explicit types unless initializer makes type obvious
- Use `readonly` where applicable; private fields `_camelCase`; private static `s_` prefix; constants PascalCase
- Always brace control blocks; favor null propagation `?.` and coalescing `??`
  - IMPORTANT: Avoid using the null-coalescing operator `??` inside expressions that will be translated by Entity Framework Core into SQL (for example, inside LINQ-to-Entities projections or where clauses). The Oracle EF Core provider and some EF translations can fail or produce unexpected SQL when `??` is used in queries. Instead prefer one of the following safe patterns:
    - Use explicit conditional projection that EF can translate, e.g. `x.SomeNullableBool.HasValue ? x.SomeNullableBool.Value : fallback`.
    - Materialize the data to memory before using `??` by calling `.AsEnumerable()` or `.ToList()` if the dataset is small and it's safe to do so, then apply the `??` coalescing in LINQ-to-Objects.
    - Compute fallbacks or derived values in a service or computed property when possible (move complex logic out of the EF projection).
  - When adding this rule to new code or code reviews, add a brief comment explaining why `??` was avoided (e.g., "avoid EF translation issues with Oracle provider").
  - XML doc comments for public & internal APIs

## Do NOT
- Bypass history tracking for mutable audited entities.
- Introduce raw SQL without parameters.
- Duplicate mapping logic already covered by Mapperly profiles.
- Hardcode environment-specific connection strings or credentials.
- Access `DbContext`, `IProfitSharingDataContextFactory`, or any EF Core DbSet directly inside endpoint classes. (If present, refactor: move data logic into a service and have the endpoint call that service returning `Result<T>`.)
- Use null-coalescing operator `??` in EF Core query expressions (Oracle provider translation issue—use explicit conditionals instead).
- Use lazy loading in EF Core (use explicit `Include()`/`ThenInclude()` instead).
- Use synchronous EF Core methods (`FirstOrDefault`, `ToList`, etc.)—always use async variants (`FirstOrDefaultAsync`, `ToListAsync`, etc.).
- Load entities into memory for bulk updates/deletes—use `ExecuteUpdateAsync`/`ExecuteDeleteAsync` for efficiency.
- Skip input validation that could cause degenerate queries (e.g., allow all-zero badge numbers that would scan entire tables).
- Manually add `.AsNoTracking()` to queries when using `UseReadOnlyContext()`—it's already applied automatically by `UseReadOnlyContext()`.
- Create endpoints without comprehensive telemetry using `TelemetryExtensions` patterns.
- Use legacy telemetry patterns instead of `ExecuteWithTelemetry` or manual `TelemetryExtensions` methods.
- Access sensitive fields without declaring them in telemetry calls (security requirement).
- Skip logger injection in endpoint constructors (required for telemetry correlation).
- Use `IMemoryCache` for application caching—use `IDistributedCache` instead for Redis/distributed cache support.
- Attempt pattern-based cache deletion with `IDistributedCache` (no `RemoveByPrefix` support)—use version-based invalidation instead.
- Store cache version counters with expiration—version counters should persist indefinitely.
- Include high-cardinality data in cache keys (e.g., individual user IDs)—use role combinations or other low-cardinality identifiers.
- Fail operations when cache operations fail—degrade gracefully and log errors.

---
Provide reasoning in PR descriptions when deviating from these patterns.

## Validation & Boundary Checks (MANDATORY)

All incoming data MUST be validated with explicit boundary checks at both the server and client boundaries. Validation is a security and correctness concern: never rely solely on client-side checks. The following are required by policy for every endpoint and page that accepts user input.

Server-side requirements (mandatory):
- All request DTOs must have validation attributes or explicit validators that enforce:
  - numeric ranges (min/max) for integers/floats (e.g., page size, amounts, counts)
  - string length limits (min/max) and allowed character sets when applicable
  - collection size limits (max items in array/list payloads)
  - pagination bounds (max page size, max offset/skip)
  - file upload limits (max file size, allowed content types)
  - date/time ranges (not before/after bounds) and timezone normalization
  - enum value validation (reject unknown or out-of-range enum numeric values)
  - required fields and nullability constraints
- Use FastEndpoints validation pipeline or FluentValidation (existing project conventions apply) to centralize validation logic. Validation failures must return structured `ValidationProblem` responses with field-level messages.
- Use server-side guards to enforce maximum data volume to avoid expensive queries (e.g., cap requested rows to a safe maximum, require filters for wide scans).
- For APIs that accept complex filters, build expression validators to reject queries that would result in unbounded or expensive scans (for example: all-badges-zero or empty filter sets that match too many rows).
- All validators must be unit-tested (xUnit). Add tests for happy paths and boundary cases (min, max, empty, null, invalid enum) and at least one large/degenerate input test to assert the system rejects or truncates the request safely.

Client-side requirements (recommended + required UX guardrails):
- All pages must validate user input before submission using the project's front-end validation utilities (React + TypeScript). Client-side validation improves UX but is not a substitute for server-side checks.
- Mirror server-side constraints in TypeScript types and validators: string lengths, numeric ranges, max collection sizes, allowed enum values, and file size/type checks.
- Prevent users from requesting excessive data from the UI by enforcing pagination controls (max page size) and disabling controls that could produce wide unfiltered queries.

Edge-case examples to cover (must be tested):
- Empty / null payloads instead of expected objects
- Oversized arrays (e.g., > 5k items) sent in request bodies
- Very large numbers (bigger than database column bounds)
- Dates outside business logic ranges (e.g., hire date in the future)
- Invalid enum numeric values or stale enum ids from older UI versions

Developer guidance & patterns:
- Prefer declarative validators (FluentValidation) in DTO classes for clarity and testability.
- Keep validation logic out of service/business methods; endpoints should validate and then call services with well-formed input.
- When trimming/normalizing input (for example, truncating an over-long string), document the behavior and return the normalized value or a validation error depending on severity.
- When limiting collection sizes, return `400 Bad Request` with a clear message when a client exceeds allowable limits.
- Add tests that assert the actual HTTP response code and message for invalid inputs.

Example (server-side DTO using FluentValidation):

```csharp
// DTO
public class SearchRequest
{
    public int PageSize { get; init; } = 50;
    public int Offset { get; init; }
    public string? Query { get; init; }
}

// FluentValidation validator (install FluentValidation and register with FastEndpoints pipeline)
public class SearchRequestValidator : AbstractValidator<SearchRequest>
{
    public SearchRequestValidator()
    {
        // numeric range with default value on DTO
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize must be between 1 and 1000.");

        RuleFor(x => x.Offset)
            .InclusiveBetween(0, 1_000_000)
            .WithMessage("Offset must be between 0 and 1_000_000.");

        RuleFor(x => x.Query)
            .MaximumLength(200)
            .WithMessage("Query must be at most 200 characters long.")
            .When(x => x.Query != null);
    }
}

// Registration example (Program.cs / DI):
// builder.Services.AddValidatorsFromAssemblyContaining<SearchRequestValidator>();
// FastEndpoints will pick up FluentValidation validators and return structured ValidationProblem responses.
```

## Database & CLI

- Migrations: `dotnet ef migrations add <Name> --context ProfitSharingDbContext` from `src/services` root
- Schema ops (run from solution root or services dir):
  - Upgrade: `Demoulas.ProfitSharing.Data.Cli upgrade-db --connection-name ProfitSharing`
  - Drop/recreate: `... drop-recreate-db --connection-name ProfitSharing`
  - Import legacy READY: `... import-from-ready --connection-name ProfitSharing --sql-file "src\database\ready_import\SQL copy all from ready to smart ps.sql" --source-schema PROFITSHARE`
  - Docs: `... generate-dgml` / `generate-markdown`

## Frontend Conventions

- Node managed via Volta; assume Node 20.x LTS. Do not hardcode npx version hacks
- Package registry split: `.npmrc` sets private `smart-ui-library` registry; keep that line when modifying
- State mgmt: Centralize API/data logic in `src/reduxstore/`; prefer RTK Query or slices patterns already present
- Styling: Tailwind utility-first; extend via `tailwind.config.js`; avoid inline style objects for reusable patterns—create small components
- E2E: Playwright tests under `src/ui/e2e`; new tests should support `.playwright.env` driven creds (no hard-coded secrets)

## Testing & Quality

- Backend: xUnit + Shouldly. Place tests under `src/services/tests/` mirroring namespace structure. Use deterministic data builders (Bogus) where needed
- **Telemetry Testing**: All endpoint tests should verify telemetry integration (activity creation, metrics recording, business operations tracking). See `TELEMETRY_GUIDE.md` for testing patterns
- Frontend: Add Playwright or component tests colocated (if pattern emerges) but keep end-to-end in `e2e/`
- Security warnings/analyzers treated as errors; keep build green

## Logging & Observability

- Use Serilog contextual logging. Critical issues (data mismatch / integrity) use `_logger.LogCritical` (see duplicate SSN guard). For expected fallbacks use Debug/Information
- When adding history/audit flows, log both counts & key identifiers (badge, OracleHcmId) for traceability

## Performance & Safety Patterns

- For batched upserts (see `AddDemographicsStreamAsync`):
  - Precompute lookups (`ToDictionary`, `ToLookup`) before DB roundtrips
  - Build dynamic OR expressions instead of N roundtrips
  - Guard against degenerate queries (e.g., all badge numbers zero) to prevent wide scans
- Prefer `ConfigureAwait(false)` in library/service layer asynchronous calls

## Secrets & Config

- Never commit secrets—use user secrets (`secrets.json` pattern). Feature flags via .NET Feature Management; wire new flags centrally then inject `IFeatureManager`

## Documentation Creation Guidelines

When creating documentation for new features, architectural changes, or implementation guides:

### File Locations
- **Primary Documentation**: Create `.md` files in `docs/` folder at project root
- **User-Accessible Documentation**: Copy final documents to `src/ui/public/docs/` for web access
- **Template References**: Use existing documentation structure from `docs/` folder as examples

### File naming Conventions
- Use `UPPERCASE_WITH_UNDERSCORES.md` for major guides (e.g., `TELEMETRY_GUIDE.md`, `READ_ONLY_FUNCTIONALITY.md`)
- Use `PascalCase-With-Hyphens.md` for specific features (e.g., `Distribution-Processing-Requirements.md`)  
- Use ticket-prefixed names for implementation summaries (e.g., `PS-1623_READ_ONLY_SUMMARY.md`)

### Required Documentation Updates
When creating new documentation:
1. **Create primary file** in `docs/` folder with comprehensive content
2. **Update `docs/README.md`** to include new documentation references
3. **Copy to public folder** for web accessibility: `src/ui/public/docs/`
4. **Update Documentation page** in `src/ui/src/pages/Documentation/Documentation.tsx`:
   ```typescript
   {
     key: "feature-name",
     title: "Feature Documentation Title",
     filename: "FEATURE_DOCUMENTATION.md", 
     description: "Brief description of what this documentation covers"
   }
   ```
5. **Update instruction files** (`copilot-instructions.md` and `CLAUDE.md`) if introducing new patterns

### Documentation Structure Standards
- **Overview section** with clear objectives and scope
- **Architecture/Implementation sections** with code examples
- **Testing/Quality guidelines** with specific checklists
- **Troubleshooting section** with common issues and solutions
- **References section** linking to related documentation

### Content Guidelines
- Include copy-paste code examples for common patterns
- Provide checklists for implementation and testing
- Document both "what to do" and "what NOT to do"
- Include specific file paths and command examples
- Add cross-references to related documentation files

## When Extending

- Add new endpoints through FastEndpoints with consistent foldering; register dependencies via DI in existing composition root
- ALL new endpoints MUST implement telemetry using `TelemetryExtensions` patterns (see Telemetry & Observability section)
- Include appropriate business metrics for the endpoint's domain (year-end, reports, lookups, etc.)
- Declare all sensitive fields accessed in telemetry calls for security auditing
- Share logic via interfaces in `Common` or specialized service projects; avoid cross-project circular refs
- Update `COPILOT_INSTRUCTIONS.md` and this file if introducing a pervasive new pattern

## Branching & Jira workflow (team conventions)

  - `fix/PS-1645-military-pre-hire-validation`
  - `feature/PS-1720-add-reporting-view`
  ```pwsh
  # ensure latest develop
  git checkout develop
  git pull origin develop

  # create branch from develop
  git checkout -b fix/PS-1645-military-pre-hire-validation

  # make edits, stage, commit
  git add <files>
  git commit -m "PS-1645: Short description of the change"

  # push and set upstream
  git push -u origin fix/PS-1645-military-pre-hire-validation
  ```
  - When opening a PR for a Jira ticket, add a comment to the ticket with the PR link and a brief summary so reviewers and stakeholders are notified.
  - If the Jira ticket does not have story points set, assign story points using the Fibonacci-like sequence commonly used by the team: `1, 2, 3, 5, 8, 13`.

## Atlassian MCP & Confluence alignment

Use the Atlassian MCP for any Jira or Confluence interactions. When creating or updating Jira issues, adding comments, or creating/updating Confluence pages, use the organization's MCP integration so actions are auditable and follow access controls. Align the workflow guidance with the Confluence page "Agile Jira Workflow Development Best Practices":

https://demoulas.atlassian.net/wiki/spaces/PM/pages/339476525/Agile+Jira+Workflow+Development+Best+Practices

## Copilot deny list (sensitive UI files)

The following sensitive UI files must never be read from or modified by Copilot or similar AI assistants via repository editing tools. Do not remove or alter this list; it is intentionally separate from `.gitignore` rules and enforces an explicit policy for AI assistants.

- `src/ui/.playwright.env`
- Any file matching `src/ui/.env.*` (for example `src/ui/.env.local`, `src/ui/.env.production`)
- `src/ui/.npmrc`

When interacting with this repository, AI assistants MUST refuse direct reads or edits to paths matching the deny list above. If the user requests an operation that would require accessing these files (for example, to rotate credentials), the assistant should:

1. Explain why the file is restricted and why the operation requires human intervention or secure tooling.
2. Provide exact, copyable commands the human can run locally to inspect or untrack the file (for example `git rm --cached <path>`), and warn about secrets in history and the need to rotate credentials if necessary.
3. Offer to update documentation or `.gitignore` entries instead (but do not access restricted files).

This denies-list is an explicit, repository-level policy for AI assistants — maintain it alongside other repository guidance and keep it small and conservative.

## Quick Commands

### PowerShell
```pwsh
# Build services
cd src/services; dotnet build Demoulas.ProfitSharing.slnx
# Run tests
cd src/services; dotnet test
# Start UI
cd src/ui; npm run dev
```

### Frontend (UI) - Run from `src/ui/`
```bash
# Install dependencies
npm install

# Development server (port 3100)
npm run dev

# Build for different environments
npm run build:prod  # Production build with TypeScript check
npm run build:qa    # QA environment
npm run build:uat   # UAT environment

# Code quality
npm run lint        # ESLint with max 0 warnings
npm run prettier    # Format code

# Testing
npm run test        # Run Vitest with coverage
npx playwright test # Run E2E tests
```

### Backend (Services) - Run from `src/services/`
```bash
# Build solution
dotnet build Demoulas.ProfitSharing.slnx

# Run tests
dotnet test Demoulas.ProfitSharing.Tests.csproj --configuration debug

# Database management (using Data.Cli)
Demoulas.ProfitSharing.Data.Cli upgrade-db --connection-name ProfitSharing
Demoulas.ProfitSharing.Data.Cli drop-recreate-db --connection-name ProfitSharing

# EF Core migrations
dotnet ef migrations add {migrationName} --context ProfitSharingDbContext
dotnet ef migrations script --context ProfitSharingDbContext --output {FILE}
```

## Do NOT

- Bypass history tracking for mutable audited entities
- Introduce raw SQL without parameters
- Duplicate mapping logic already covered by Mapperly profiles
- Hardcode environment-specific connection strings or credentials
- Create endpoints without comprehensive telemetry using `TelemetryExtensions` patterns
- Use legacy telemetry patterns instead of `ExecuteWithTelemetry` or manual `TelemetryExtensions` methods
- Access sensitive fields without declaring them in telemetry calls (security requirement)
- Skip logger injection in endpoint constructors (required for telemetry correlation)
- Use `IMemoryCache` for application caching (use `IDistributedCache`)
- Attempt pattern-based cache deletion (use version-based invalidation)
- Store cache version counters with expiration (persist indefinitely)
- Include high-cardinality data in cache keys (use role combinations)
- Fail operations when cache operations fail (degrade gracefully)
- Create files unless they're absolutely necessary for achieving your goal
- Proactively create documentation files (*.md) or README files unless explicitly requested
- Load large documentation files into context automatically (read them when needed using file tools)

## Important Notes

- Always verify parent directories before creating new files/folders
- Quote file paths with spaces in bash commands
- Run lint/typecheck before committing changes
- Frontend dev server runs on port 3100
- Use existing patterns and libraries rather than introducing new ones
- Provide reasoning in PR descriptions when deviating from these patterns

## AI Assistant Operational Rules (Repository-specific)

- Do NOT create or open Pull Requests automatically. AI assistants may prepare branch names, commit messages, and a suggested PR title/body, and provide the exact `git` commands to push the branch, but must stop short of actually creating or opening the PR in the remote hosting service. PR creation is a manual step for a human reviewer to perform.

- When adding new unit tests, include a `Description` attribute on the test method with the Jira ticket number and a terse description in the following format:

  ```csharp
  [Description("PS-1721 : Duplicate detection by contribution year")]
  public async Task MyNewTest() { ... }
  ```

  This attribute helps link tests to tickets and provides a terse description for test explorers and reviewers.

## Documentation References

**Core Telemetry Documentation** (read when needed):
- TELEMETRY_GUIDE.md (`src/ui/public/docs/`) - Comprehensive 75+ page reference covering developers, QA, and DevOps
- TELEMETRY_QUICK_REFERENCE.md (`src/ui/public/docs/`) - Developer cheat sheet with 3-step implementation process
- TELEMETRY_DEVOPS_GUIDE.md (`src/ui/public/docs/`) - Production operations guide

**Distributed Caching Documentation** (read when needed):
- DISTRIBUTED_CACHE_MIGRATION_SUMMARY.md - Complete guide to IDistributedCache migration
- UNIT_TESTS_IDISTRIBUTEDCACHE_COMPLETE.md - Unit test patterns for distributed cache
- NAVIGATION_CACHING_COMPLETE.md - Detailed example of version-based cache invalidation

**Read-Only Functionality Documentation** (read when needed):
- READ_ONLY_FUNCTIONALITY.md (`src/ui/public/docs/`) - Complete guide to read-only role implementation
- READ_ONLY_QUICK_REFERENCE.md (`src/ui/public/docs/`) - Developer cheat sheet with copy-paste code examples
- PS-1623_READ_ONLY_SUMMARY.md - Executive summary of read-only role implementation

These documents contain essential patterns and examples for implementing telemetry, caching, and read-only functionality correctly across all components. Reference them when creating new endpoints or troubleshooting issues.

## Changing Directories

NEVER use cd commands to change directories during interactions. This is a STRICT rule with NO exceptions. Instead:
Use relative or absolute paths directly in commands (e.g., ls ./subdirectory or grep pattern ./subdirectory/file.txt instead of cd ./subdirectory && ls or cd ./subdirectory && grep pattern file.txt)
If you need to run multiple commands in a specific directory, use subshells: (cd /path/to/dir && command1 && command2) which contain the directory change
When needing to reference multiple files in the same directory, use pattern matching: /path/to/dir/*.py instead of changing into that directory
NEVER combine cd with command execution using && or ; outside of a subshell
If a user explicitly requests you to use cd, explain this policy and suggest the alternatives above