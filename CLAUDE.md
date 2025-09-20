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

## Backend Coding Style

- File-scoped namespaces; one class per file; explicit access modifiers
- Prefer explicit types unless initializer makes type obvious
- Use `readonly` where applicable; private fields `_camelCase`; private static `s_` prefix; constants PascalCase
- Always brace control blocks; favor null propagation `?.` and coalescing `??`
- XML doc comments for public & internal APIs

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

## When Extending

- Add new endpoints through FastEndpoints with consistent foldering; register dependencies via DI in existing composition root
- Share logic via interfaces in `Common` or specialized service projects; avoid cross-project circular refs
- Update `COPILOT_INSTRUCTIONS.md` and this file if introducing a pervasive new pattern

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
- Create files unless they're absolutely necessary for achieving your goal
- Proactively create documentation files (*.md) or README files unless explicitly requested

## Important Notes

- Always verify parent directories before creating new files/folders
- Quote file paths with spaces in bash commands
- Run lint/typecheck before committing changes
- Frontend dev server runs on port 3100
- Use existing patterns and libraries rather than introducing new ones
- Provide reasoning in PR descriptions when deviating from these patterns