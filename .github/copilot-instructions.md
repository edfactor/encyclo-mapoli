# AI Assistant Project Instructions

Concise, project-specific guidance for AI coding agents working in this repository. Focus on THESE patterns; avoid generic advice.

## Architecture Overview
- Monorepo with two primary roots:
  - `src/services/` (.NET 9) multi-project solution `Demoulas.ProfitSharing.slnx` (FastEndpoints, EF Core 9 + Oracle, Aspire, Serilog, Feature Flags, RabbitMQ, Mapperly, Shouldly).
  - `src/ui/` (Vite + React + TypeScript + Tailwind + Redux Toolkit + internal `smart-ui-library`).
- Database: Oracle 19. EF Core migrations via `ProfitSharingDbContext`; CLI utility project `Demoulas.ProfitSharing.Data.Cli` performs schema ops & imports from legacy READY system.
- Cross-cutting: Central package mgmt (`Directory.Packages.props`), shared build config (`Directory.Build.props`), global SDK pin (`global.json`).

## Key Backend Conventions
- Startup/entrypoint: run/debug `Demoulas.ProfitSharing.AppHost` (Aspire host). Avoid creating new ad-hoc hosts.
- Use FastEndpoints; group endpoint files logically. Prefer minimal API style. Return typed results + proper status codes.
- Mapping: Prefer `Mapperly` for DTO<->entity; follow existing mapper classes (see `*Mapper.cs`). Don't hand-write repetitive mapping unless customization is needed.
- Data access (service layer ONLY): All EF Core usage (DbContext/`IProfitSharingDataContextFactory`, LINQ queries, transactions) lives in dedicated domain/application services (e.g., `...Services` projects). Endpoints MUST NOT directly query the database nor reference DbSets. They invoke injected services that return `Result<T>` (or domain objects) and perform only request validation, orchestration, and HTTP result translation.
  - Bulk maintenance still uses `ExecuteUpdate/ExecuteDelete` inside services where safe.
  - For dynamic filters build expressions in services (see `DemographicsService`).
  - Avoid raw SQL; if performance requires it, encapsulate in the service layer, parameterize, and unit test.
- Auditing & History: When updating mutable domain entities with historical tracking (example: `DemographicsService` creating `DemographicHistory` with `ValidFrom/ValidTo`), replicate the pattern: close current record (`ValidTo = now`), insert new history row. NEVER overwrite historical rows.
- Identifiers: `OracleHcmId` is authoritative when present; fall back to composite `(Ssn,BadgeNumber)` only when Oracle id missing. Mirror guard logic (skip ambiguous BadgeNumber == 0 cases) if extending.
- Entity updates: Keep helper methods like `UpdateEntityValues` cohesive; prefer adding fields there instead of scattering manual per-field assignments.
- Validation errors & audits: Use `DemographicSyncAudit` pattern—batch add + save once. When adding new audit types, follow existing property naming.

## Endpoint Results Pattern (MANDATORY)
All FastEndpoints MUST return typed minimal API union results AND internally use the domain `Result<T>` record (`Demoulas.ProfitSharing.Common.Contracts.Result<T>`) for service-layer outcomes.

Patterns:
- Service layer returns/constructs `Result<T>` (Success, Failure, ValidationFailure).
- Endpoint converts domain result via `Match` (or helper) to: `Results<Ok<T>, NotFound, ProblemHttpResult>` (queries) or `Results<Ok, ProblemHttpResult>` (commands). Include `NotFound` for resource-missing semantics; add `ValidationProblem` only if you propagate structured validation errors directly.
- Helpers: Use `ResultHttpExtensions.ToResultOrNotFound()` + `ToHttpResult()` to reduce boilerplate (e.g., `dto.ToResultOrNotFound(Error.CalendarYearNotFound).ToHttpResult(Error.CalendarYearNotFound)`).
- Implicit: `Result<T>` has an implicit conversion to `Results<Ok<T>, NotFound, ProblemHttpResult>` but ONLY use it when you do not need to distinguish specific not-found errors (otherwise call `ToHttpResult(Error.SomeNotFound)`).
- Errors: Use specific not-found codes (e.g., `Error.CalendarYearNotFound`). Avoid reusing unrelated error descriptions to trigger NotFound.
- Map: `Success => TypedResults.Ok(value)`, not-found error => `TypedResults.NotFound()`, other errors/validation => `TypedResults.Problem(problem.Detail)`.
- Example (explicit mapping):
  ```csharp
  var result = await _svc.GetAsync(req.Id, ct);
  return result.ToHttpResult(Error.SomeEntityNotFound);
  ```
- Avoid returning raw DTOs or nulls; always wrap service outcomes in `Result<T>` before translating to HTTP.
- Catch unexpected exceptions and map to `TypedResults.Problem(ex.Message)` (logging appropriately) unless a global handler already standardizes this.

## Backend Coding Style (augmenting existing COPILOT_INSTRUCTIONS)
- File-scoped namespaces; one class per file; explicit access modifiers.
- Prefer explicit types unless initializer makes type obvious.
- Use `readonly` where applicable; private fields `_camelCase`; private static `s_` prefix; constants PascalCase.
- Always brace control blocks; favor null propagation `?.` and coalescing `??`.
  - IMPORTANT: Avoid using the null-coalescing operator `??` inside expressions that will be translated by Entity Framework Core into SQL (for example, inside LINQ-to-Entities projections or where clauses). The Oracle EF Core provider and some EF translations can fail or produce unexpected SQL when `??` is used in queries. Instead prefer one of the following safe patterns:
    - Use explicit conditional projection that EF can translate, e.g. `x.SomeNullableBool.HasValue ? x.SomeNullableBool.Value : fallback`.
    - Materialize the data to memory before using `??` by calling `.AsEnumerable()` or `.ToList()` if the dataset is small and it's safe to do so, then apply the `??` coalescing in LINQ-to-Objects.
    - Compute fallbacks or derived values in a service or computed property when possible (move complex logic out of the EF projection).
  - When adding this rule to new code or code reviews, add a brief comment explaining why `??` was avoided (e.g., "avoid EF translation issues with Oracle provider").
- XML doc comments for public & internal APIs.

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
- Prefer declarative validators (attributes or FluentValidation) in DTO classes for clarity and testability.
- Keep validation logic out of service/business methods; endpoints should validate and then call services with well-formed input.
- When trimming/normalizing input (for example, truncating an over-long string), document the behavior and return the normalized value or a validation error depending on severity.
- When limiting collection sizes, return `400 Bad Request` with a clear message when a client exceeds allowable limits.
- Add tests that assert the actual HTTP response code and message for invalid inputs.

Example (server-side DTO using data annotations):
```csharp
public class SearchRequest
{
    [Required]
    [Range(1, 1000)]
    public int PageSize { get; init; } = 50;

    [Range(0, 1000000)]
    public int Offset { get; init; }

    [StringLength(200, MinimumLength = 0)]
    public string? Query { get; init; }
}
```

Example (frontend TypeScript guard):
```ts
// ...existing code...
function validateSearchInput(input: SearchInput): string[] {
  const errors: string[] = [];
  if (input.pageSize < 1 || input.pageSize > 1000) errors.push('pageSize must be 1..1000');
  if ((input.query ?? '').length > 200) errors.push('query max length 200');
  return errors;
}
```

Quality gates (enforcement):
- Pull requests that add new endpoints or UI pages must include validation for all incoming inputs and at least one unit test covering an invalid boundary case.
- Code reviews should flag missing validators or reliance on client-side checks alone.

Security note:
- Proper boundary checking reduces risk of data exfiltration and denial-of-service via expensive queries or unbounded result sets. Combine validators with the telemetry and alerting guidance to detect suspicious or abusive request patterns.

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

## When Extending
- Add new endpoints through FastEndpoints with consistent foldering; register dependencies via DI in existing composition root.
- Share logic via interfaces in `Common` or specialized service projects; avoid cross-project circular refs.
- Update `CLAUDE.md` and this file if introducing a pervasive new pattern.

## Branching & Jira workflow (team conventions)

To keep branches and PRs consistent across the org, follow these conventions. Copilot assistants and developers should follow this policy for any work associated with a Jira ticket.

- Base branch: Always create feature / fix branches from `develop`. Do not branch from `main` or other long-lived branches unless explicitly instructed by a release manager.
- Branch naming: Use the Jira ticket key as a prefix and a short dash-separated description. Examples:
  - `fix/PS-1645-military-pre-hire-validation`
  - `feature/PS-1720-add-reporting-view`
- Ticket formats: Jira tickets may be referenced as a full URL or key. Normalize both forms to a key when creating branches or PRs:
  - `https://demoulas.atlassian.net/browse/PS-1645` → `PS-1645`
  - `PS-1645` → `PS-1645`

- Typical local workflow (PowerShell):
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

- Pull request guidance:
  - Open PR from your branch into `develop` (not `main`) unless the ticket or release manager instructs otherwise.
  - Title should start with the Jira key: `PS-1645: Prevent military contributions before hire date`.
  - Include the following in the PR body: summary of the change, which tests were run locally (and results), any migration or config changes, and QA steps to reproduce the fix.
  - When opening a PR for a Jira ticket, add a comment to the ticket with the PR link and a brief summary so reviewers and stakeholders are notified.
  - If the Jira ticket does not have story points set, assign story points using the Fibonacci-like sequence commonly used by the team: `1, 2, 3, 5, 8, 13`.

- Copilot assistant responsibilities:
  - When asked to create or reference a branch for a Jira ticket, normalize ticket input (URL or key) to the ticket key and produce the suggested branch name using the pattern above.
  - If a repository or the developer has an alternate branching policy for a particular ticket (e.g., release hotfix), mention the exception and prefer the explicit instruction.
  - When making code edits for a Jira ticket, create a local branch from `develop`, run unit tests (or the focused tests), commit, and push the branch to origin; report the push URL for PR creation.

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

These rules reduce merge conflicts, ensure CI runs the correct base, and make PRs easy to find by ticket key.

## Atlassian MCP & Confluence alignment

Use the Atlassian MCP for any Jira or Confluence interactions. Copilot assistants must use the organization's Atlassian MCP integration when creating or updating Jira issues, adding comments to tickets, or creating/updating Confluence pages. This ensures actions are auditable and follow the organization's access controls.

Align the workflow guidance in this document with the Confluence page "Agile Jira Workflow Development Best Practices":
https://demoulas.atlassian.net/wiki/spaces/PM/pages/339476525/Agile+Jira+Workflow+Development+Best+Practices

Key alignment points:
- Always start branches from `develop` for feature and bugfix work as described in Confluence.
- Use the ticket-key-first branch naming convention (e.g., `PS-####: short-desc`) and include the Jira key in PR titles.
- Follow the Confluence guidance for issue types, transitions, and story/acceptance criteria formatting when creating or updating tickets.
- When in doubt about branching or release strategy, follow the Confluence page or raise the question in the ticket comments and CC the release manager.

Copilot assistants should reference and obey this Confluence guidance when normalizing ticket keys, creating branch names, and preparing PR bodies. If the Confluence page changes, update this file to reflect the new team guidance.

## OpenTelemetry & Security Telemetry (recommended)

Additions in this section describe recommended packages, configuration keys, wiring patterns, metrics, logs and alerting that help IT Security detect abusive or unexpected data access patterns (for example: a user requesting large numbers of records, repeated downloads of sensitive fields, or unusually large response payloads).

Scope and goals:
- Provide low-risk, low-cardinality metrics that indicate aggregate access and volume trends.
- Provide higher-fidelity traces/logs (sampled) and exemplar linking to investigate suspicious users while avoiding high-cardinality metric explosion.
- Provide configuration toggles to opt-in/out of sensitive-field counting and PII-in-telemetry policies.

Recommended NuGet packages (add via centralized `Directory.Packages.props` when possible):
- `OpenTelemetry` (core)
- `OpenTelemetry.Extensions.Hosting` (host integration)
- `OpenTelemetry.Instrumentation.AspNetCore` (automatic HTTP/server traces)
- `OpenTelemetry.Instrumentation.Http` (outgoing HTTP client)
- `OpenTelemetry.Instrumentation.SqlClient` (DB-level spans/metrics as needed)
- `OpenTelemetry.Metrics` (metrics API)
- `OpenTelemetry.Exporter.Otlp` (OTLP exporter - preferred for production)
- `OpenTelemetry.Exporter.Console` (local/dev debugging only)

Where to wire it:
- Primary host: `src/services/src/Demoulas.ProfitSharing.AppHost/Program.cs` (Aspire host)
- API project: `src/services/src/Demoulas.ProfitSharing.Api/Program.cs`

Important repository note:
- The repository provides a centralized initial OpenTelemetry/logging setup via `Demoulas.Common.Logging.Extensions.AspireServiceDefaultExtensions`. Use this extension in your Aspire host (`AppHost`) and Api startup rather than duplicating base wiring. Extend or augment the telemetry after the extension has been applied (for example, add security-specific metrics, middleware, or additional exporters) instead of replacing or duplicating the base configuration.

  Rationale: the extension centralizes Serilog/OpenTelemetry bootstrapping, common resource attributes, and host-specific configuration; duplicating that wiring can lead to conflicting exporters, double-instrumentation, or inconsistent sampling/resource tags.

Configuration (appsettings / environment variable keys)
- `OpenTelemetry:Enabled` (bool)
- `OpenTelemetry:Otlp:Endpoint` (string) - OTLP collector endpoint (include protocol `http://` or `https://`)
- `OpenTelemetry:Exporter:Console:Enabled` (bool) - dev only
- `OpenTelemetry:Tracing:Sampler` (`always_on`|`parentbased_always_on`|`traceidratio`) and `OpenTelemetry:Tracing:SamplerRate` (0.01..1.0)
- `OpenTelemetry:Metrics:Enabled` (bool)
- `SecurityTelemetry:EnableSensitiveFieldCounting` (bool) - default false
- `SecurityTelemetry:SensitiveFields` (array) - e.g., `['Ssn','Salary','BankAccount']`
- `SecurityTelemetry:HighCardinalityTagging:Enabled` (bool) - default false (only enable with care)

Wiring note (important):

- Do NOT call `builder.Services.AddOpenTelemetry()` in library or host `Program.cs` — the repository uses a centralized bootstrap provided by `Demoulas.Common.Logging.Extensions.AspireServiceDefaultExtensions`. That extension centralizes Serilog/OpenTelemetry bootstrapping, resource attributes, exporter configuration, and sampling.
- Instead, ensure your host calls the shared Aspire extension (the AppHost and Api templates already do this). After the shared extension has been applied, extend telemetry by registering middleware/services that emit security-specific metrics or logs (do not replace or duplicate the base wiring).

How to extend safely (conceptual guidance):

- Register a small service or middleware responsible for security telemetry (response sizes, sensitive-field access counts, bulk-export counts). This component should use the runtime telemetry APIs (for example `System.Diagnostics.ActivitySource` for traces and `System.Diagnostics.Metrics.Meter` for metrics) to emit events and counters. These APIs integrate with the pre-configured OpenTelemetry pipeline from the shared extension.

Conceptual (non-compiling) example showing the pattern:

```csharp
// ...existing code that applies the shared Aspire extension (do not duplicate AddOpenTelemetry)
// e.g. Demoulas.Common.Logging.Extensions.AspireServiceDefaultExtensions.ApplyDefaults(builder, configuration);

// Register your security telemetry components
builder.Services.AddSingleton<SecurityTelemetryMiddleware>();
builder.Services.AddSingleton<SecurityTelemetryService>();

app.UseMiddleware<SecurityTelemetryMiddleware>();

// Inside SecurityTelemetryService or middleware you can use standard runtime telemetry APIs:
// private static readonly ActivitySource s_activity = new("Demoulas.ProfitSharing.Security");
// private static readonly Meter s_meter = new("Demoulas.ProfitSharing.Metrics");
// private static readonly Counter<long> s_sensitiveFieldCounter = s_meter.CreateCounter<long>("ps_sensitive_field_access_total");
// s_sensitiveFieldCounter.Add(1, new("field", "Ssn"), new("endpoint", "/api/employee"));
```

Notes:
- Keep metric labels low-cardinality (service, environment, endpoint_category). For per-user investigations emit a log or trace exemplar with `user_id` rather than attaching `user_id` to every metric label.
- Guard sensitive counters behind a configuration toggle (e.g., `SecurityTelemetry:EnableSensitiveFieldCounting`) and ensure the default is off for production until approved.

Implementation guidance and cautions:
- Avoid high-cardinality metric labels (e.g., using raw `user_id` on every metric). High-cardinality labels hurt metric backends and performance.
- Use low-cardinality dimensions on metrics (e.g., `user_role`, `org_id`, `endpoint_category`) and emit per-user details to logs/traces when thresholds are exceeded. Link logs/traces back to exemplars when supported.
- For per-user investigations, use traces (sampled with higher rate for suspicious requests) and structured logs containing `user_id`, `badgeNumber`, `OracleHcmId` (mask or hash values when necessary) and a telemetry correlation id.
- Provide toggles for `SecurityTelemetry:EnableSensitiveFieldCounting` and ensure it defaults to `false` in production unless IT has approved telemetry PII handling and storage.
- When counting access to sensitive fields, the implementation should report counts to a metric called e.g. `ps_sensitive_field_access_total{field="Ssn", endpoint="/api/employee"}` — but do not include `user_id` as a cardinality label in metric series. Instead, emit an associated log or trace stamped with the `user_id` and correlation id for detailed investigation.

Recommended metrics (low-cardinality) to emit
- app_requests_total{service,environment,endpoint_category,method,response_status}
- app_request_duration_seconds_bucket{service,environment,endpoint_category}
- app_response_bytes_sum / app_response_bytes_count (histogram) by {service,environment,endpoint_category}
- app_user_request_rate{service,environment,user_role} - aggregated per role (not per id)
- ps_sensitive_field_access_total{service,environment,field,endpoint_category} - counts of sensitive field reads (no user_id tag by default)
- ps_large_downloads_total{service,environment,endpoint_category} - increments when response size > configured threshold (e.g., 5 MB)
- ps_bulk_export_operations_total{service,environment,export_type} - background job/export counts

Suggested logs/traces for enrichment
- Log structured event when `ps_sensitive_field_access_total` is incremented with: timestamp, correlation_id, endpoint, field, user_role, masked_or_hashed_user_id, record_count, request_query_parameters (redacted)
- On thresholds (rate or volume) exceeded for a single user, increase trace sampling for that user's requests for a limited window (e.g., 10 minutes) to collect more context.

Sample alerting queries and rules
- High cardinality caution: avoid queries that iterate over all `user_id` values in metric systems that do not support it. Prefer aggregation by `user_role` or by `org_id`.

- PromQL (alert when a single account performs excessive requests in 5m):
  sum by (user_id) (increase(app_requests_total[5m]))
  > 500
  # Note: only use if your metric backend and retention supports per-user label cardinality.

- PromQL (safer, role-based):
  sum by (user_role) (increase(app_requests_total[5m]))
  > 10000

- Large download detection (PromQL):
  increase(ps_large_downloads_total[10m]) > 5

- Sensitive field spike detection (PromQL):
  increase(ps_sensitive_field_access_total{field="Ssn"}[1h]) > 100

- SIEM / Log search (ELK / Splunk style):
  `event.type: "sensitive_field_access" AND field: "Ssn" AND user_id: "X" | stats count() by user_id | where count > 100`

Operational recommendations
- Start with metrics enabled but sensitive-field counting disabled. Use role-based and endpoint-based alerts first to tune thresholds.
- If enabling per-user metrics, put strict quotas on cardinality and retention and obtain approval from IT and privacy teams.
- Ensure telemetry collectors and storage (OTLP/Prometheus/Splunk/Elastic) have appropriate access controls, encryption at rest, and retention/archival policies.
- Document who can query per-user telemetry and under what process (audit trail for who accessed telemetry data).

Privacy, compliance & security considerations
- Mask or hash direct identifiers (SSN, OracleHcmId) before sending them to telemetry systems when possible. Keep raw identifiers in the primary database only.
- Use feature flags and secure environment variables to control whether telemetry includes PII or not.
- Add explicit documentation and an approval workflow before enabling sensitive telemetry for production.

Developer checklist for implementing security telemetry
1. Add OpenTelemetry packages to `Directory.Packages.props` and restore.
2. Wire basic tracing & metrics to OTLP in AppHost and Api `Program.cs` with configuration toggles.
3. Implement middleware that:
   - extracts `user_id`, `user_role`, `correlation_id` from the request/security principal;
   - measures response size and increments `ps_large_downloads_total` when thresholds are exceeded;
   - increments `ps_sensitive_field_access_total{field=...}` when application code reads sensitive fields (guarded behind `SecurityTelemetry:EnableSensitiveFieldCounting`).
4. Emit structured logs for sensitive-field reads (masked/hashes) with correlation ids.
5. Add Prometheus/OTel collection and alert rules in infra repo / runbook.

Next steps & references
- When ready to implement code, update `src/services/src/Demoulas.ProfitSharing.AppHost/Program.cs` and `src/services/src/Demoulas.ProfitSharing.Api/Program.cs` following code snippet patterns above and register a short-lived feature flag to toggle sensitive counting.
- Keep the default production posture conservative: metrics ON, PII counting OFF.

## Quick Commands (PowerShell)
```pwsh
# Build services
cd src/services; dotnet build Demoulas.ProfitSharing.slnx
# Run tests (ONLY the consolidated UnitTests project; do not run entire solution test graph)
dotnet test src/services/tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj --no-build
# Start UI
cd src/ui; npm run dev
```

## Do NOT
- Bypass history tracking for mutable audited entities.
- Introduce raw SQL without parameters.
- Duplicate mapping logic already covered by Mapperly profiles.
- Hardcode environment-specific connection strings or credentials.
- Access `DbContext`, `IProfitSharingDataContextFactory`, or any EF Core DbSet directly inside endpoint classes. (If present, refactor: move data logic into a service and have the endpoint call that service returning `Result<T>`.)

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
