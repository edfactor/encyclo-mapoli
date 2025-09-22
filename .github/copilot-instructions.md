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

## AI Assistant Operational Rules (Repository-specific)

- Do NOT create or open Pull Requests automatically. AI assistants may prepare branch names, commit messages, and a suggested PR title/body, and provide the exact `git` commands to push the branch, but must stop short of actually creating or opening the PR in the remote hosting service. PR creation is a manual step for a human reviewer to perform.

- When adding new unit tests, include a `Description` attribute on the test method with the Jira ticket number and a terse description in the following format:

  ```csharp
  [Description("PS-1721 : Duplicate detection by contribution year")]
  public async Task MyNewTest() { ... }
  ```

  This attribute helps link tests to tickets and provides a terse description for test explorers and reviewers.
