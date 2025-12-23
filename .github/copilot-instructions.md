# AI Assistant Instructions — Smart Profit Sharing

This concise guide explains the must-know patterns, commands, and files for AI coding agents working in this repository.

**Quick summary:**

- Backend: .NET Aspire-hosted solution under `src/services/` (FastEndpoints + services + EF Core).
- Frontend: Vite + React + TypeScript under `src/ui/` (Redux Toolkit + RTK Query + smart-ui-library).
- Telemetry, PII masking, EF Core conventions, and grid/column factories are enforced repo-wide.

**Start here (authoritative references)**

- [.github/CODE_REVIEW_CHECKLIST.md](.github/CODE_REVIEW_CHECKLIST.md) — master patterns, security, auto-reject rules (age calc, PII).
- [.github/instructions/pages.instructions.md](.github/instructions/pages.instructions.md) — frontend page patterns and examples.
- [src/ui/public/docs/TELEMETRY_GUIDE.md](src/ui/public/docs/TELEMETRY_GUIDE.md) — telemetry + masking examples.

**Architecture & boundaries (big picture)**

- Endpoints (FastEndpoints) must call service layer methods; services own EF Core usage and return `Result<T>`.
- Frontend pages live in `src/ui/src/pages/*`. Use `Page`, `DSMAccordion`, and `DSMPaginatedGrid` (for paginated data) or `DSMGrid` (for non-paginated data) from `smart-ui-library`.
- Telemetry and security are cross-cutting: record sensitive-field accesses and mask PII before logging.

**Critical, project-specific rules (always follow)**

- Endpoints → Services only: do NOT use `DbContext` in endpoints. Move DB logic to services under `src/services/src/`.
- EF Core: Always use async APIs, call `UseReadOnlyContext()` for read-only queries, avoid `??` inside EF query expressions (Oracle provider), use `TagWith()` for complex ops.
- Bulk ops: use `ExecuteUpdateAsync` / `ExecuteDeleteAsync` instead of loading entities.
- Demographics keys: never use `Ssn` alone as a dictionary key — use composite `(Ssn, OracleHcmId)` or a `ToLookup()`.
- Financial rounding: use `MidpointRounding.AwayFromZero`.
- Age calculation: compute on backend only (frontend must not calculate age).

**Frontend conventions & examples**

- Grid columns: always use factory functions in `src/ui/src/utils/gridColumnFactory.ts` (badge/name columns are specialized).
- Server-side pagination: parent manages `pageNumber` (0-based UI → API 1-based), `pageSize`, `sortBy`, and `isSortDescending`; grids call parent handlers via props.
- Use RTK Query lazy hooks for user-initiated searches (e.g., `useLazySearchDistributionsQuery`).

**Common developer commands**

- Full app (API + UI):

```pwsh
aspire run
```

- Backend build & tests:

```pwsh
cd src/services
dotnet build Demoulas.ProfitSharing.slnx
dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj --no-build
```

- Frontend dev/build/tests:

```bash
cd src/ui
npm run dev
npm run build:qa
npm test
```

**Telemetry & logging**

- Prefer `ExecuteWithTelemetry` wrapper in endpoints; declare sensitive fields (e.g., `"Ssn"`) when recording metrics. See [src/ui/public/docs/TELEMETRY_GUIDE.md](src/ui/public/docs/TELEMETRY_GUIDE.md).

**AI agent rules (operational constraints)**

- Do not open or merge PRs automatically. Provide a suggested branch name, commit message, and exact `git` commands; stop before pushing.
- Make minimal/surgical changes. Update tests and docs if behavior or public APIs change.

**Files to inspect for pattern examples**

- Endpoints: `src/services/src/Demoulas.ProfitSharing.Endpoints/**`
- Services: `src/services/src/Demoulas.ProfitSharing.Services/**`
- Frontend pages: `src/ui/src/pages/**` (see `MasterInquiry`, `DistributionInquiry`)
- Grid factories: `src/ui/src/utils/gridColumnFactory.ts`
- Telemetry docs: `src/ui/public/docs/TELEMETRY_GUIDE.md`

If you want, I can also produce a suggested PR body and branch name for this change — tell me whether to prepare that next.

# AI Assistant Project Instructions

**Quick navigation guide for AI coding agents** working in this repository. Detailed patterns are in separate documents (see references below).

## Architecture Overview

- **Monorepo** with two primary roots:
  - `src/services/` - .NET 10 multi-project solution `Demoulas.ProfitSharing.slnx` (FastEndpoints, EF Core 10 + Oracle, Aspire, Serilog, RabbitMQ, Mapperly, Shouldly)
    - Hosted using **.NET Aspire** (`Demoulas.ProfitSharing.AppHost`) - do not create ad-hoc hosts
    - **Start app**: `aspire run` from project root
  - Aspire docs: https://github.com/dotnet/docs-aspire/blob/main/docs/cli/overview.md
  - `src/ui/` - Vite + React + TypeScript + Tailwind + Redux Toolkit + `smart-ui-library`
- **Database**: Oracle 19. EF Core migrations via `ProfitSharingDbContext`; CLI utility `Demoulas.ProfitSharing.Data.Cli` for schema ops
- **Cross-cutting**: Central package mgmt (`Directory.Packages.props`), shared build config (`Directory.Build.props`), global SDK pin (`global.json`)

## Key Backend Conventions

- **Startup**: Run/debug `Demoulas.ProfitSharing.AppHost` (Aspire host) - avoid ad-hoc hosts
- **Endpoints**: FastEndpoints; group logically, use minimal API style, return typed results
- **Mapping**: Prefer `Mapperly` for DTO<->entity (see `*Mapper.cs`)
- **Data Access** (service layer ONLY): All EF Core usage lives in services (e.g., `...Services` projects). Endpoints invoke services returning `Result<T>`—NO direct DbContext/DbSet access in endpoints.
  - Bulk maintenance uses `ExecuteUpdate/ExecuteDelete` inside services
  - Dynamic filters build expressions in services (see `DemographicsService`)
  - Avoid raw SQL; if needed, encapsulate in service layer
- **EF Core 10 Best Practices**: See detailed EF Core patterns in the section below. References:
  - [EF Core 10.0 What's New](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew)
  - [Oracle Entity Framework Core 10 Features](https://docs.oracle.com/en/database/oracle/oracle-database/26/odpnt/EFCore10features.html)
  - [Oracle .NET Database Samples](https://github.com/oracle/dotnet-db-samples)
- **Distributed Caching**: Use `IDistributedCache` (NOT `IMemoryCache`). See DISTRIBUTED_CACHING_PATTERNS.md (`.github/`) for complete patterns including version-based invalidation
- **Auditing & History**: Close current record (`ValidTo = now`), insert new history row. NEVER overwrite historical rows.
- **Identifiers**: `OracleHcmId` is authoritative; fall back to `(Ssn,BadgeNumber)` only when Oracle id missing
- **Entity updates**: Use helper methods like `UpdateEntityValues`; avoid scattered per-field assignments

## Security Requirements (MANDATORY - OWASP Top 10 2021/2025 Aligned)

**ALL NEW CODE must follow security patterns. See [security.instructions.md](.github/instructions/security.instructions.md) for complete guidance.**

**Critical security concepts covered:**

- **5 Security Pillars**: Application Security, Infrastructure, IAM, Data Security, Detection & Response
- **OWASP Top 10**: Broken Access Control, Cryptographic Failures, Injection, Insecure Design, etc.
- **STRIDE Threat Modeling**: Spoofing, Tampering, Repudiation, Information Disclosure, DoS, Elevation of Privilege
- **Secure-by-Default Principles**: Least privilege, fail closed, defense in depth

**Quick security checklist** (see security.instructions.md for details):

- [ ] **Server-side validation ALWAYS**: Never trust client-provided roles/headers
- [ ] **Parameterized queries ONLY**: No SQL string concatenation
- [ ] **PII masked in logs**: SSN, email, phone, names must be masked
- [ ] **HTTPS enforced**: Security headers present (HSTS, CSP, X-Frame-Options)
- [ ] **Input validation**: Ranges, lengths, degenerate query guards
- [ ] **Generic error messages**: No stack traces, SQL, or paths in responses
- [ ] **Dependency security**: Monthly audits, critical CVEs within 48 hours
      See `VALIDATION_PATTERNS.md` (`.github/`) for complete patterns

**For detailed security guidance, see [.github/instructions/security.instructions.md](.github/instructions/security.instructions.md)**

## Endpoint Results Pattern (MANDATORY)

All FastEndpoints MUST return typed minimal API union results AND use domain `Result<T>` record (`Demoulas.ProfitSharing.Common.Contracts.Result<T>`) for service outcomes.

**Quick Pattern**:

- Service returns `Result<T>` (Success, Failure, ValidationFailure)
- Endpoint converts via `Match` or helpers to: `Results<Ok<T>, NotFound, ProblemHttpResult>`
- Use `ResultHttpExtensions.ToResultOrNotFound()` + `ToHttpResult()` to reduce boilerplate
- Specific error codes for NotFound (e.g., `Error.CalendarYearNotFound`)

**Example**:

```csharp
var result = await _svc.GetAsync(req.Id, ct);
return result.ToHttpResult(Error.SomeEntityNotFound);
```

## EF Core 10 Patterns & Best Practices (MANDATORY)

We use **EF Core 10** with Oracle provider. All DB access MUST follow these patterns.

**References**:

- [EF Core 10.0 What's New](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew)
- [Oracle Entity Framework Core 10 Features](https://docs.oracle.com/en/database/oracle/oracle-database/26/odpnt/EFCore10features.html)
- [Oracle .NET Database Samples](https://github.com/oracle/dotnet-db-samples)

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

### Dictionary Keys with Demographic (CRITICAL)

**SSN is NOT unique** in the `Demographic` entity. When building dictionaries/lookups from Demographics:

- **WRONG**: `demographics.ToDictionary(d => d.Ssn)` — will throw duplicate key exception
- **CORRECT**: Use a composite key combining SSN with a unique identifier:
  - `(d.Ssn, d.OracleHcmId)` — **preferred** (most reliable across systems)
  - `(d.Ssn, d.BadgeNumber)` — when OracleHcmId unavailable
  - `(d.Ssn, d.Id)` — when badge/HCM unavailable (database ID)

**Examples**:

```csharp
// Best: Use OracleHcmId
var demographicsByKey = demographics
    .ToDictionary(d => (d.Ssn, d.OracleHcmId), d => d);

// Fallback: Use badge when HCM ID missing
var demographicsByKey = demographics
    .Where(d => d.OracleHcmId != 0) // Guard against missing identifiers
    .ToDictionary(d => (d.Ssn, d.OracleHcmId), d => d);

// For lookups (one-to-many): Use ToLookup to avoid duplicates entirely
var demographicsByKey = demographics
    .ToLookup(d => (d.Ssn, d.OracleHcmId));

// Access: var matches = demographicsByKey[(ssn, hcmId)];
```

**Why**: Historical data, data feeds, and migrations can create SSN duplicates (same employee with multiple records). Always pair SSN with a unique identifier when creating dictionaries.

### Critical Rules

- Services only—NO DbContext in endpoints
- Always async (`FirstOrDefaultAsync`, `ToListAsync`)
- Explicit `Include()`/`ThenInclude()`—NO lazy loading
- Validate inputs to prevent table scans

### Example Service

````csharp
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

All FastEndpoints MUST implement comprehensive telemetry. **See [TELEMETRY_GUIDE.md](../src/ui/public/docs/TELEMETRY_GUIDE.md)** for complete reference.

**Quick Pattern** (recommended):
```csharp
public override async Task<MyResponse> ExecuteAsync(MyRequest req, CancellationToken ct)
{
    return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
    {
        var result = await _service.ProcessAsync(req, ct);

        // Add business metrics
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "year-end-processing"),
            new("endpoint", nameof(MyEndpoint)));

        return result;
    }, "Ssn", "OracleHcmId"); // List sensitive fields accessed
}
````

**Required**:

- Inject `ILogger<TEndpoint>` for correlation
- Use `TelemetryExtensions` patterns (`ExecuteWithTelemetry` or manual methods)
- Include business metrics appropriate to endpoint category
- Declare sensitive fields in telemetry calls

**Documentation** (read these files when needed, not loaded by default):

- TELEMETRY_GUIDE.md (`src/ui/public/docs/`) - Comprehensive 75+ page reference for developers, QA, DevOps
- TELEMETRY_QUICK_REFERENCE.md (`src/ui/public/docs/`) - Developer cheat sheet with copy-paste examples
- TELEMETRY_DEVOPS_GUIDE.md (`src/ui/public/docs/`) - Production operations guide
- SECURITY_TELEMETRY_SETUP.md (`.github/`) - Advanced security monitoring setup

## Validation & Boundary Checks (MANDATORY)

All incoming data MUST be validated at server and client boundaries. See VALIDATION_PATTERNS.md (`.github/`) for complete reference when needed.

**Quick Pattern**:

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

**Required**:

- Numeric ranges, string lengths, collection sizes
- Enum validation, date ranges, required fields
- Unit tests covering boundary cases
- Client-side validation mirrors server constraints

### Backend Coding Style (augmenting existing COPILOT_INSTRUCTIONS)

- File-scoped namespaces; one class per file; explicit access modifiers.
- Prefer explicit types unless initializer makes type obvious.
- Use `readonly` where applicable; private fields `_camelCase` (including `private static readonly`); constants PascalCase.
- Always brace control blocks; favor null propagation `?.` and coalescing `??`.
  - IMPORTANT: Avoid using the null-coalescing operator `??` inside expressions that will be translated by Entity Framework Core into SQL. The Oracle EF Core provider can fail with `??` in queries. Use explicit conditional projection instead.
- XML doc comments for public & internal APIs.

### Decimal Rounding (CRITICAL for Financial Calculations)

**Always use `MidpointRounding.AwayFromZero` for monetary/financial calculations** to match COBOL behavior.

```csharp
// ✅ RIGHT: Use AwayFromZero for financial calculations (matches COBOL)
var roundedAmount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

// ❌ WRONG: Default rounding uses ToEven (banker's rounding)
var roundedAmount = Math.Round(amount, 2);  // Uses MidpointRounding.ToEven
```

**Why**: COBOL uses traditional rounding (0.5 rounds up), while .NET defaults to banker's rounding (0.5 rounds to nearest even). This can cause penny differences in financial reports.

**When to use**:

- Tax calculations (federal, state)
- Distribution amounts
- Forfeiture amounts
- Any monetary aggregation or reporting
- Profit sharing calculations

### Async/Await Patterns (AsyncFixer01 - Critical)

**AsyncFixer analyzer (https://github.com/semihokur/AsyncFixer) enforces these patterns. Violations are build errors.**

- **Return Task directly when await is the last statement** (AsyncFixer01 - REQUIRED):

  ```csharp
  // ❌ WRONG: Unnecessary async/await wrapper
  public async Task<Result<T>> GetAsync(int id, CancellationToken ct)
  {
      return await _service.FetchAsync(id, ct);  // AsyncFixer01 error
  }

  // ✅ RIGHT: Return Task directly, caller awaits
  public Task<Result<T>> GetAsync(int id, CancellationToken ct)
  {
      return _service.FetchAsync(id, ct);
  }
  ```

- **Use `async` keyword only when needed** (multiple awaits, try-catch, using statements):

  ```csharp
  // ✅ RIGHT: Multiple awaits require async
  public async Task<Result<T>> ProcessAsync(int id, CancellationToken ct)
  {
      var data = await _service.FetchAsync(id, ct);
      var processed = await _processor.ProcessAsync(data, ct);
      return processed;
  }

  // ✅ RIGHT: Error handling requires async
  public async Task<Result<T>> GetWithErrorHandlingAsync(int id, CancellationToken ct)
  {
      try
      {
          return await _service.FetchAsync(id, ct);
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Error fetching");
          throw;
      }
  }

  // ✅ RIGHT: Resource cleanup requires async
  public async Task DoWorkAsync(CancellationToken ct)
  {
      await using var resource = await _factory.CreateAsync();
      await resource.DoSomethingAsync(ct);
  }
  ```

- **Never wrap single awaits unnecessarily**:

  ```csharp
  // ❌ WRONG: AsyncFixer01 violation
  public async Task FooAsync() => await BarAsync();

  // ✅ RIGHT: Return Task directly
  public Task FooAsync() => BarAsync();
  ```

- **Tests must be `async Task`** (exception to the rule above - test frameworks require it):
  ```csharp
  // ✅ RIGHT: Test methods are async Task, even with single await
  [Fact]
  public async Task MyTest_ShouldDoSomething()
  {
      var result = await _service.GetAsync(1, CancellationToken.None);
      result.ShouldNotBeNull();
  }
  ```

### Project-specific Conventions

These conventions are important project-wide rules. Follow them in addition to the coding style above:

- Public methods use PascalCase naming
- Private fields start with underscore (\_)
- Use Task/Task<T> or ValueTask/ValueTask<T> for asynchronous operations
- Dependencies are injected through constructor parameters (constructor injection)
- Use Result<T> pattern for error handling instead of throwing exceptions directly
- Use DTOs/ViewModels for data transfer between layers

## Database & CLI

- Add a migration (run from repo root PowerShell):
  ```pwsh
  pwsh -NoLogo -Command "cd src/services/src/Demoulas.ProfitSharing.Data; dotnet ef migrations add <MigrationName> --context ProfitSharingDbContext"
  ```
  (Use singular imperative names, e.g., `AddMemberIndex`, `RenameColumnXToY`).
- Apply schema changes: Run `Demoulas.ProfitSharing.Data.Cli` with the `upgrade-db` launch setting (or `Demoulas.ProfitSharing.Data.Cli upgrade-db --connection-name ProfitSharing`).
- Other ops:
  - Drop/recreate: `... drop-recreate-db --connection-name ProfitSharing`
  - Import legacy READY: `... import-from-ready --connection-name ProfitSharing --sql-file "src\database\ready_import\SQL copy all from ready to smart ps.sql" --source-schema PROFITSHARE`
  - Docs: `... generate-dgml` / `generate-markdown`.

## Frontend Conventions

- Node managed via Volta; assume Node 20.x LTS. Do not hardcode npx version hacks.
- Package registry split: `.npmrc` sets private `smart-ui-library` registry; keep that line when modifying.
- State mgmt: Centralize API/data logic in `src/reduxstore/`; prefer RTK Query or slices patterns already present.
- Styling: Tailwind utility-first; extend via `tailwind.config.js`; avoid inline style objects for reusable patterns—create small components.
- Numeric inputs: Do NOT rely on native browser up/down spinners. Prefer text inputs with `inputMode="numeric"` + validation for badge/SSN-style fields. Spinners are disabled globally in `src/ui/src/styles/index.css`.
- E2E: Playwright tests under `src/ui/e2e`; new tests should support `.playwright.env` driven creds (no hard-coded secrets).

### Build & Deployment (Vite)

- **Build system**: Vite with mode-based configuration. See `src/ui/package.json` scripts:
  - `npm run dev` - Development server on port 3100 (Vite dev server)
  - `npm run build:prod` - Production build: runs `tsc -b` then `vite build --mode production`
  - `npm run build:qa` - QA build: `vite build --mode qa`
  - `npm run build:uat` - UAT build: `vite build --mode uat`
- **TypeScript compilation**: Production builds run `tsc -b` BEFORE vite build. TypeScript errors will block the build.
- **Code splitting**: Vite automatically code-splits routes using `React.lazy()`. Each lazy route creates a separate chunk file in `build/static/js/` (or `dist/`).
- **Output locations**:
  - Development: Run Vite dev server directly
  - Build output: `build/` directory with subdirectories `static/js/`, `static/css/`, etc.
- **Build verification**: After code changes affecting routes or components:
  1. Run `npm run lint` - verify 0 errors, 0 warnings
  2. Run `npm run build:qa` - verify build succeeds, check chunk file creation
  3. Verify chunk files exist: `build/static/js/ComponentName-[hash].js` for each lazy route

## Testing & Quality

- Backend: xUnit + Shouldly. Place tests under `src/services/tests/` mirroring namespace structure. Use deterministic data builders (Bogus) where needed.
- Backend test project split (do NOT create additional ad-hoc test projects):
  - **Functional unit/service tests** live in `Demoulas.ProfitSharing.UnitTests`.
  - **Architecture, analyzer, and infrastructure tests** live in `Demoulas.ProfitSharing.UnitTests.Architecture`.
  - Integration tests remain in `Demoulas.ProfitSharing.IntegrationTests`.
  - Do not run tests against stale binaries (e.g., via `--no-build`) unless a successful build has already been verified.
- **Telemetry Testing**: All endpoint tests should verify telemetry integration (activity creation, metrics recording, business operations tracking). See `TELEMETRY_GUIDE.md` for testing patterns.
- Frontend: Add Playwright or component tests colocated (if pattern emerges) but keep end-to-end in `e2e/`.
- Security warnings/analyzers treated as errors; keep build green.

## Logging & Observability

- Use Serilog contextual logging. Critical issues (data mismatch / integrity) use `_logger.LogCritical` (see duplicate SSN guard). For expected fallbacks use Debug/Information.
- When adding history/audit flows, log both counts & key identifiers (badge, OracleHcmId) for traceability.

## Performance & Safety Patterns

- For batched upserts (see `AddDemographicsStreamAsync`):
  - Precompute lookups (`ToDictionary`, `ToLookup`) before DB roundtrips.
  - Build dynamic OR expressions instead of N roundtrips.
  - Guard against degenerate queries (e.g., all badge numbers zero) to prevent wide scans.
- Prefer `ConfigureAwait(false)` in library/service layer asynchronous calls.

## Secrets & Config

- Never commit secrets—use user secrets (`secrets.json` pattern). Feature flags via .NET Feature Management; wire new flags centrally then inject `IFeatureManager`.

## Documentation Creation Guidelines

When creating documentation for new features, architectural changes, or implementation guides:

### File Locations

- **User-Accessible Documentation**: Copy final documents to `src/ui/public/docs/` for web access
- **Template References**: Use existing documentation structure from `src/ui/public/docs/` folder as examples

### File naming Conventions

- Use `UPPERCASE_WITH_UNDERSCORES.md` for major guides (e.g., `TELEMETRY_GUIDE.md`, `READ_ONLY_FUNCTIONALITY.md`)
- Use `PascalCase-With-Hyphens.md` for specific features (e.g., `Distribution-Processing-Requirements.md`)
- Use ticket-prefixed names for implementation summaries (e.g., `PS-1623_READ_ONLY_SUMMARY.md`)

### Required Documentation Updates

When creating new documentation:

1. **Create primary file** in `src/ui/public/docs/` folder with comprehensive content
2. **Update `README.md`** to include new documentation references
3. **Update Documentation page** in `src/ui/src/pages/Documentation/Documentation.tsx`:
   ```typescript
   {
     key: "feature-name",
     title: "Feature Documentation Title",
     filename: "FEATURE_DOCUMENTATION.md",
     description: "Brief description of what this documentation covers"
   }
   ```
4. **Update instruction files** (`copilot-instructions.md` and `CLAUDE.md`) if introducing new patterns

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

- Add new endpoints through FastEndpoints with consistent foldering; register dependencies via DI in existing composition root.
- **CODE REVIEW CHECKLIST**: Use the [Master Code Review Checklist](CODE_REVIEW_CHECKLIST.md) for all PRs - includes comprehensive security, architecture, frontend, backend, and telemetry guidance. **CRITICAL**: Frontend section includes auto-reject blockers like age calculation.
- **NEW ENDPOINT CHECKLIST**: Use the [RESTful API Guidelines Instructions](instructions/restful-api-guidelines.instructions.md) (see “New endpoint checklist”) to verify design, implementation, documentation, and security before submitting PR
- ALL new endpoints MUST implement telemetry using `TelemetryExtensions` patterns (see Telemetry & Observability section).
- Include appropriate business metrics for the endpoint's domain (year-end, reports, lookups, etc.).
- Declare all sensitive fields accessed in telemetry calls for security auditing.
- Share logic via interfaces in `Common` or specialized service projects; avoid cross-project circular refs.
- Update `CLAUDE.md` and this file if introducing a pervasive new pattern.

## Branching & Workflow

See BRANCHING_AND_WORKFLOW.md (`.github/`) for complete Git, Jira, and PR conventions when needed.

**Quick Summary**:

- Always branch from `develop` (not `main`)
- Branching and commit message examples:

```text
Branch example: feature/PS-1720-add-reporting-view
Commit example: PS-1720: Add reporting view
```

- PR title: Start with Jira key
- Use Atlassian MCP for all Jira/Confluence interactions
- AI assistants: Do NOT auto-create or auto-merge PRs (human review required)

**Typical workflow**:

```pwsh
git checkout develop && git pull origin develop
git checkout -b feature/PS-1720-add-reporting-view
# Make changes, commit, test
git push -u origin feature/PS-1720-add-reporting-view
# Open PR manually after review
```

## Detailed Pattern References

For comprehensive implementation details, see these dedicated guides:

### Core Patterns (read when needed)

- RESTFUL_API_GUIDELINES.INSTRUCTIONS.md (`.github/instructions/`) - Zalando RESTful API guidelines, endpoint design, HTTP semantics, URL patterns, pagination
- DISTRIBUTED_CACHING_PATTERNS.md (`.github/`) - IDistributedCache patterns, version-based invalidation, unit testing
- VALIDATION_PATTERNS.md (`.github/`) - Server & client validation, FluentValidation examples, boundary checks
- BRANCHING_AND_WORKFLOW.md (`.github/`) - Git branching, Jira workflow, PR guidelines, deny list

### Telemetry & Observability (read when needed)

- TELEMETRY_GUIDE.md (`src/ui/public/docs/`) - Comprehensive 75+ page reference for developers, QA, DevOps
- TELEMETRY_QUICK_REFERENCE.md (`src/ui/public/docs/`) - Developer cheat sheet with copy-paste examples
- TELEMETRY_DEVOPS_GUIDE.md (`src/ui/public/docs/`) - Production operations, monitoring, alerting
- SECURITY_TELEMETRY_SETUP.md (`.github/`) - Advanced security monitoring patterns

### Feature-Specific Guides (read when needed)

- READ_ONLY_FUNCTIONALITY.md (`src/ui/public/docs/`) - Read-only role implementation
- READ_ONLY_QUICK_REFERENCE.md (`src/ui/public/docs/`) - Read-only patterns cheat sheet
- Distribution-Processing-Requirements.md (`src/ui/public/docs/`) - Distribution processing flows
- Year-End-Testability-And-Acceptance-Criteria.md (`src/ui/public/docs/`) - Year-end processing tests

### Front End Tests

- fe-unit-tests.md ('ai-templates/front-end/') Guide to writing tests for react tests

## Quick Commands (PowerShell)

```pwsh
# Start the entire application (API + UI) - RUN FROM PROJECT ROOT
aspire run
# Build services
cd src/services; dotnet build Demoulas.ProfitSharing.slnx
# Run tests (ONLY the consolidated UnitTests project; do not run entire solution test graph)
# NOTE: Tests use xUnit v3 + Microsoft Testing Platform (MTP).
# Run from src/services so global.json test runner settings are applied.
cd src/services
# Always build first (do not run tests if the solution doesn't compile)
dotnet build Demoulas.ProfitSharing.slnx

# Then run tests against the current source
dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj

# Optional fast loop (ONLY after a successful build has been verified)
dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj --no-build
# Filter examples (MTP/xUnit options; NOT VSTest `--filter`):
# dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj --filter-class *OracleHcmDiagnostics*
# dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj --filter-class *OracleHcmDiagnostics* --no-build
```

## Do NOT

### Security - CRITICAL

- **Trust client-provided roles/headers for authorization**: Always re-validate server-side. Remove `localStorage` role elevation code (PS-2021). Never bypass `PolicyRoleMap.cs` validation.
- **Store secrets in code**: Use Azure Key Vault or environment variables. Never hardcode connection strings, API keys, or credentials.
- **Expose PII in logs/responses**: SSN, email, phone, bank accounts must be masked. Use `GetMaskedUserId()` pattern. Never log unmasked identity claims.
- **Skip input validation**: All user inputs must be validated server-side (ranges, lengths, enum values, degenerate cases). Client validation is UX only.
- **Build SQL strings by concatenation**: Always use EF Core parameterized queries. NEVER string-concatenate SQL.
- **Return sensitive data in error messages**: Never expose stack traces, SQL queries, file paths, or PII in HTTP error responses.
- **Implement authentication client-side only**: Use Okta OIDC server-side flow. Never rely on localStorage for auth state.
- **Deploy without security headers**: All endpoints must return HSTS, CSP, X-Frame-Options, etc. (See PS-2023, PS-2024 tickets).
- **Use `AllowAnyOrigin()` in production CORS**: Restrict to specific domains. In dev, allow only `localhost:3100` (See PS-2025).
- **Skip telemetry on sensitive operations**: Year-end processing, member termination, report generation MUST have telemetry with sensitive fields declared (See PS-2026).
- **Skip re-validation when handling impersonation**: Always re-check that authenticated user is allowed to assume requested roles (PS-2022).

### Architecture & Data Access

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

### Observability & Quality

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

## Formatting & EditorConfig (additional guidance)

- Follow the repository `.editorconfig` for formatting and analyzer rules. This file is the authoritative source for whitespace, naming, and stylistic rules; update it only when you have a clear, repo-wide reason and include rationale in the PR.
- Preferred style highlights:
  - File-scoped namespaces and one class per file.
  - Use explicit access modifiers for all types and members.
  - Favor `is null` / `is not null` for null checks and `nameof(...)` for member names.
  - Prefer pattern matching and switch expressions where they improve clarity.
  - Avoid inserting ad-hoc formatting changes that conflict with `.editorconfig`.
- If you need to add or change formatting rules, update `.editorconfig` and include a short explanation in the PR body so reviewers can assess scope and impact.

## AI Assistant Operational Rules (Repository-specific)

- Do NOT create or open Pull Requests automatically. AI assistants may prepare branch names, commit messages, and a suggested PR title/body, and provide the exact `git` commands to push the branch, but must stop short of actually creating or opening the PR in the remote hosting service. PR creation is a manual step for a human reviewer to perform.

- When adding new unit tests, include a `Description` attribute on the test method with the Jira ticket number and a terse description in the following format:

  ```csharp
  [Description("PS-1721 : Duplicate detection by contribution year")]
  public async Task MyNewTest() { ... }
  ```

  This attribute helps link tests to tickets and provides a terse description for test explorers and reviewers.

---

Provide reasoning in PR descriptions when deviating from these patterns.
