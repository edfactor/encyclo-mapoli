# AI Assistant Project Instructions

**Quick navigation guide for AI coding agents** working in this repository. Detailed patterns are in separate documents (see references below).

## Architecture Overview
- **Monorepo** with two primary roots:
  - `src/services/` - .NET 9 multi-project solution `Demoulas.ProfitSharing.slnx` (FastEndpoints, EF Core 9 + Oracle, Aspire, Serilog, RabbitMQ, Mapperly, Shouldly)
    - Hosted using **.NET Aspire** (`Demoulas.ProfitSharing.AppHost`) - do not create ad-hoc hosts
    - **Start app**: `aspire run` from project root
    - [Aspire docs](https://github.com/dotnet/docs-aspire/blob/main/docs/cli/overview.md)
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
- **Distributed Caching**: Use `IDistributedCache` (NOT `IMemoryCache`). See DISTRIBUTED_CACHING_PATTERNS.md (`.github/`) for complete patterns including version-based invalidation
- **Auditing & History**: Close current record (`ValidTo = now`), insert new history row. NEVER overwrite historical rows.
- **Identifiers**: `OracleHcmId` is authoritative; fall back to `(Ssn,BadgeNumber)` only when Oracle id missing
- **Entity updates**: Use helper methods like `UpdateEntityValues`; avoid scattered per-field assignments

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

## Telemetry & Observability (MANDATORY)

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
```

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

## Backend Coding Style (augmenting existing COPILOT_INSTRUCTIONS)
- File-scoped namespaces; one class per file; explicit access modifiers.
- Prefer explicit types unless initializer makes type obvious.
- Use `readonly` where applicable; private fields `_camelCase`; private static `s_` prefix; constants PascalCase.
- Always brace control blocks; favor null propagation `?.` and coalescing `??`.
  - IMPORTANT: Avoid using the null-coalescing operator `??` inside expressions that will be translated by Entity Framework Core into SQL. The Oracle EF Core provider can fail with `??` in queries. Use explicit conditional projection instead.
- XML doc comments for public & internal APIs.

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
- E2E: Playwright tests under `src/ui/e2e`; new tests should support `.playwright.env` driven creds (no hard-coded secrets).

## Testing & Quality
- Backend: xUnit + Shouldly. Place tests under `src/services/tests/` mirroring namespace structure. Use deterministic data builders (Bogus) where needed.
- All backend unit & service tests reside in the consolidated test project `Demoulas.ProfitSharing.UnitTests` (do NOT create stray ad-hoc test projects). Mirror source namespaces inside this project; prefer folder structure `Domain/`, `Services/`, `Endpoints/` for organization if adding new areas.
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
- Add new endpoints through FastEndpoints with consistent foldering; register dependencies via DI in existing composition root.
- ALL new endpoints MUST implement telemetry using `TelemetryExtensions` patterns (see Telemetry & Observability section).
- Include appropriate business metrics for the endpoint's domain (year-end, reports, lookups, etc.).
- Declare all sensitive fields accessed in telemetry calls for security auditing.
- Share logic via interfaces in `Common` or specialized service projects; avoid cross-project circular refs.
- Update `CLAUDE.md` and this file if introducing a pervasive new pattern.

## Branching & Workflow

See BRANCHING_AND_WORKFLOW.md (`.github/`) for complete Git, Jira, and PR conventions when needed.

**Quick Summary**:
- Always branch from `develop` (not `main`)
- Branch naming: `<type>/PS-####-short-description`
- Commit messages: Start with `PS-####:`
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

## Quick Commands (PowerShell)
```pwsh
# Start the entire application (API + UI) - RUN FROM PROJECT ROOT
aspire run
# Build services
cd src/services; dotnet build Demoulas.ProfitSharing.slnx
# Run tests (ONLY the consolidated UnitTests project; do not run entire solution test graph)
dotnet test src/services/tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj --no-build
```

## Do NOT
- Bypass history tracking for mutable audited entities.
- Introduce raw SQL without parameters.
- Duplicate mapping logic already covered by Mapperly profiles.
- Hardcode environment-specific connection strings or credentials.
- Access `DbContext`, `IProfitSharingDataContextFactory`, or any EF Core DbSet directly inside endpoint classes. (If present, refactor: move data logic into a service and have the endpoint call that service returning `Result<T>`.)
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
