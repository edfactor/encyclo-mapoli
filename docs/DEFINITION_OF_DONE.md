# Definition of Done (DoD)

**Project:** Smart Profit Sharing (Demoulas)

## Purpose

The Definition of Done (DoD) describes the **minimum, verifiable criteria** a change must meet to be considered complete, reviewable, and safe to release.

- Applies to **stories, tasks, and bug fixes**.
- Items may be marked **N/A** with a brief justification.
- If any **stop-ship** item is violated, the work is **not done**.

## Sources of truth (must-follow)

This DoD is grounded in the repository’s authoritative guidance:

- .github/CODE_REVIEW_CHECKLIST.md
- .github/instructions/all.security.instructions.md
- .github/instructions/all.code-review.instructions.md
- copilot-instructions.md
- src/ui/public/docs/TELEMETRY_GUIDE.md
- ai-templates/ (frontend patterns, testing, navigation, validation)

If this DoD conflicts with any of the above, **follow the sources of truth**.

---

## DoD Checklist

### 1) Requirements and Acceptance

- [ ] **Acceptance criteria met**: All ACs implemented; no partials.
- [ ] **Business behavior verified**: Expected outputs match requirements, including edge cases.
- [ ] **Stakeholder acceptance**: Product owner/stakeholders have validated the change when required.
- [ ] **Out-of-scope changes avoided**: Only required behavior changes were introduced.

### 2) Architecture and Code Quality

- [ ] **Repo patterns followed**: Conforms to established patterns and layering.
- [ ] **Minimal complexity**: Implementation is as simple as possible; avoids “clever” abstractions.
- [ ] **No breaking changes without intent**: Any breaking change is documented and coordinated.
- [ ] **Formatting/analyzers clean**: .editorconfig followed; analyzers/warnings treated as errors remain green.

#### Backend (Services / Endpoints)

- [ ] **No EF Core in endpoints**: Endpoints call the service layer; services own DB access.
- [ ] **Service results**: Services return `Result<T>` and endpoints map to typed HTTP results.
- [ ] **Async correctness**: Uses async EF APIs; avoids unnecessary `async/await` wrappers (AsyncFixer rules).
- [ ] **Oracle EF constraints**: Avoid `??` inside EF-translated query expressions; use explicit conditional logic.
- [ ] **Read-only queries**: Uses `UseReadOnlyContext()` for read-only operations.
- [ ] **Query performance**: Projects only needed columns; uses bulk ops (`ExecuteUpdateAsync`/`ExecuteDeleteAsync`) for bulk.
- [ ] **Degenerate guards**: Prevents accidental “scan the world” queries (e.g., all-zero badge input).
- [ ] **History/audit patterns preserved**: Never overwrites historical rows; follows “close current → insert new” patterns.

#### Frontend (UI)

- [ ] **Page + grid conventions**: Uses repo-standard components (`Page`, `DSMGrid` / `DSMPaginatedGrid`) and grid column factories.
- [ ] **No client-side role elevation**: UI never grants access via local storage or headers.
- [ ] **No frontend age calculation (STOP-SHIP)**: Age is computed server-side only.
- [ ] **Type safety**: Types updated in the appropriate location; no `any` escapes unless unavoidable and justified.

### 3) Security and Compliance (OWASP / FISMA-aligned)

- [ ] **Authorization cannot be bypassed**: Access control enforced server-side; never trusts client-provided roles/headers.
- [ ] **Input validation server-side**: Ranges, lengths, enums, collections validated; client validation is UX only.
- [ ] **Injection-safe**: Parameterized queries only; no string-concatenated SQL.
- [ ] **Secrets not in code**: No tokens/keys/connection strings committed.
- [ ] **No sensitive data leakage**: PII (SSN, names, email, phone, bank info) is not returned/logged inadvertently.
- [ ] **PII masking in logs**: Uses the repo-approved masking approach (e.g., SSN masking via shared extensions).
- [ ] **“FullName” pattern honored** (when person names are involved): Backend uses `FullName` + masking; frontend uses `fullName`; UI does not concatenate names.

### 4) Telemetry and Observability

- [ ] **Telemetry present for new endpoints**: Uses `ExecuteWithTelemetry` (recommended) or equivalent approved patterns.
- [ ] **Sensitive fields declared**: Any access to sensitive fields is declared to telemetry and masked appropriately.
- [ ] **Logs are useful, not noisy**: Correct severity; includes correlation context; no high-cardinality labels.
- [ ] **Errors are diagnosable**: Failures produce actionable logs/telemetry without leaking implementation details.

### 5) Testing and Verification

- [ ] **Focused automated tests added/updated** for changed behavior.
- [ ] **All relevant tests pass** (unit/integration/UI as applicable).
- [ ] **Negative/edge cases covered**: Boundaries, invalid inputs, authorization failures.
- [ ] **Regression risks addressed**: Either via tests, or explicitly documented manual verification steps.

#### Minimum evidence to attach in PR (typical)

Use the smallest relevant set; include outputs or a short summary in the PR description:

- Backend:
  - `cd src/services; dotnet build Demoulas.ProfitSharing.slnx`
  - `cd src/services; dotnet test --project tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj`
- Frontend:
  - `cd src/ui; npm run build:qa`
  - `cd src/ui; npm test` (when UI behavior changed)

### 6) Documentation

- [ ] **Docs updated** when behavior, processes, or operational expectations change.
- [ ] **User-facing docs** copied to `src/ui/public/docs/` when required.
- [ ] **Docs index updated**: If adding a new guide under `docs/`, add it to `docs/README.md`.

### 7) Release and Operational Readiness

- [ ] **Config/feature flags**: Any new settings are wired centrally, documented, and safe-by-default.
- [ ] **Database impact managed**: Migrations are reviewed and a safe rollout plan exists (if applicable).
- [ ] **Rollback plan**: For risky changes, rollback/disable steps are documented.
- [ ] **No environment-specific hacks**: No QA-only code paths or hardcoded hostnames introduced.

---

## Stop-Ship Findings (Not Done)

Any of the following blocks “Done” status until fixed:

- Frontend calculates age.
- Endpoint accesses EF Core / DbContext directly.
- New secret committed (API keys, tokens, `.env` content, connection strings).
- Raw SQL introduced without parameterization.
- Sensitive data logged unmasked (SSN, names, email, phone, bank info).
- Authorization can be bypassed (client-supplied roles/headers trusted).
- Builds/tests for impacted areas cannot be demonstrated.

---

## PR Review Expectations (Quick Links)

- Code review master checklist: .github/CODE_REVIEW_CHECKLIST.md
- Code review instructions: .github/instructions/all.code-review.instructions.md
- Security instructions: .github/instructions/all.security.instructions.md
- Telemetry reference: src/ui/public/docs/TELEMETRY_GUIDE.md
