# Code Review Instructions (Demoulas.Common)

This document defines **how to review changes** in this repository and what checks are required to meet baseline engineering quality, **OWASP Top 10**, and **FISMA Moderate** expectations.

## Sources of truth (must-follow)

These repo-specific documents take precedence and should be referenced during review:

- `.github/COPILOT_INSTRUCTIONS.md` (architecture, multi-targeting, CPM, backend conventions, endpoint patterns, operational rules)
- `.github/instructions/services.instructions.md` (extension method catalog + usage guidance)
- `MULTI-TARGETING-GUIDE.md` (net8/net9/net10 strategy and Oracle EF constraint)
- `Directory.Build.props` + `Directory.Packages.props` (multi-targeting + central package management)

If this document conflicts with any of the above, **follow the sources of truth**.

---

## Review scope and expectations

A reviewer is responsible for confirming:

1. **Correctness**: behavior matches requirements; edge cases handled.
2. **Maintainability**: code aligns with repo patterns; minimal complexity.
3. **Compatibility**: multi-targeting and packaging rules are honored.
4. **Security**: OWASP Top 10 risks are addressed for relevant changes.
5. **Compliance posture**: changes do not regress FISMA Moderate-aligned controls.
6. **Evidence**: author provides build/test/logging/security evidence as applicable.

### Minimum required evidence (for most PRs)

- Build succeeds for all target frameworks used by the touched projects (see multi-targeting rules).
- Tests: at least the focused test projects for the touched code.
- No secrets committed; no credential material added to repo.
- If Snyk MCP is available: run `mcp_snyk_snyk_code_scan` and `mcp_snyk_snyk_sca_scan` at `medium+` severity and attach the summarized output (and `mcp_snyk_snyk_iac_scan` where applicable).

---

## Repository-specific engineering checks

### Multi-targeting (CRITICAL)

- Projects must continue to support **all target frameworks defined by** `Directory.Build.props`.
- Any new API usage is compatible across target frameworks (or is behind conditional compilation, if already used by the repo).
- If the change touches EF Core / Oracle EF Core:
  - Follow `MULTI-TARGETING-GUIDE.md` for the Oracle EF constraint.

### Central Package Management (CRITICAL)

- No `<PackageReference ... Version="..." />` in `.csproj` files.
- Package versions are only defined in `Directory.Packages.props`.
- If a package is framework-specific, it appears in the correct conditional `ItemGroup` blocks.

### Backend architecture rules

Validate alignment with `.github/COPILOT_INSTRUCTIONS.md`:

- FastEndpoints endpoints:
  - Endpoints do **not** query EF Core directly and do not access `DbContext`/DbSets.
  - Endpoints translate service outcomes using the required typed result + `Result<T>` patterns.
- Service layer:
  - EF Core usage is isolated to services.
  - Raw SQL is avoided; if unavoidable it is parameterized and tested.
- EF Core + Oracle provider:
  - Avoid `??` null-coalescing **inside EF-translated queries** when it can cause translation issues.
- Mapping:
  - Prefer Mapperly where used; avoid repetitive hand-written mapping.
- Auditing/history:
  - Follow established history patterns; do not overwrite historical rows.

### Logging and observability

- Logging is appropriate for severity (expected vs unexpected vs integrity-critical).
- Sensitive data is not logged.

### Extension methods (services)

When PRs add or modify extension methods (see `.github/instructions/services.instructions.md`):

- API shape is consistent (namespace/class naming, overload patterns).
- Parameter validation behavior matches existing conventions (e.g., null/empty handling).
- Exceptions are intentional and documented (XML docs).
- Performance considerations are addressed (avoid unnecessary allocations; prefer idiomatic .NET patterns used in the repo).
- Unit tests exist for new behaviors and edge cases.

---

## Security review checklist (OWASP Top 10)

Use this as a **reviewer checklist**. Not every item applies to every PR; reviewers should explicitly mark “N/A” where appropriate.

### A01: Broken Access Control

- Authorization is enforced at the correct layer (endpoint/service) and cannot be bypassed.
- Access checks are based on trusted identifiers (no client-supplied role/permission claims unless validated).
- Multi-tenant or scope boundaries (if any) are enforced consistently.

### A02: Cryptographic Failures

- Sensitive data is protected in transit (TLS assumed) and at rest where applicable.
- No custom crypto; use platform-provided primitives.
- Secrets/keys are not hard-coded; configuration uses secure stores/secret management.
- Avoid reversible “encoding” (Base64) being used as “encryption” for sensitive data.

### A03: Injection

- All SQL is parameterized (prefer EF Core; raw SQL only when necessary).
- User-controlled input is validated/normalized before being used in:
  - queries, filters, dynamic sorting, or expression building
  - file paths
  - command execution
  - structured logging templates

### A04: Insecure Design

- New features have clear abuse-case considerations (what could a malicious user do?).
- Security controls are part of the design (not bolted on afterward).
- Public APIs have safe defaults.

### A05: Security Misconfiguration

- Secure defaults are preserved (no debug-only settings leaking into production behaviors).
- No permissive CORS / auth bypass / “temporary” flags added without safeguards.
- Feature flags (when used) are wired centrally and documented.

### A06: Vulnerable and Outdated Components

- Dependency updates follow repo CPM rules.
- If adding a new package:
  - It is justified and minimal.
  - Its license and maintenance status are acceptable.
  - Vulnerability scan results (prefer Snyk MCP when available) are provided when risk is non-trivial.

### A07: Identification and Authentication Failures

- Authn/authz changes do not weaken existing controls.
- Tokens/credentials are never logged.
- Session/token validation uses standard libraries; no homegrown parsing.

### A08: Software and Data Integrity Failures

- Inputs that affect persistence are validated.
- Data migration/import utilities have safeguards (dry-run, idempotency, audit logging) where appropriate.
- Critical writes are auditable and tamper-evident to the extent supported by existing patterns.

### A09: Security Logging and Monitoring Failures

- Security-relevant events are logged (authz failures, data integrity failures, suspicious activity) without exposing secrets.
- Logs include enough context to investigate (correlation identifiers where available).

### A10: Server-Side Request Forgery (SSRF)

If the change introduces outbound HTTP calls:

- Destination URLs are not fully user-controlled.
- Allow-listing or strict validation is used for host/port/scheme.
- Internal network targets are not reachable via user input.

---

## Compliance-oriented checks (FISMA Baseline: MODERATE)

FISMA Moderate implementations vary by system boundary, but reviewers can enforce a practical, code-focused baseline aligned with common NIST SP 800-53 control families.

Reviewers should ensure PRs do not regress these areas:

### Access Control (AC)

- Authorization decisions are explicit, consistently applied, and testable.
- Least privilege: new roles/permissions are not broader than required.

### Identification & Authentication (IA)

- Authentication flows rely on validated identities (e.g., Okta / token validation as used by consuming systems).
- Secrets are stored outside source control.

### Audit & Accountability (AU)

- Security-relevant actions are logged with sufficient context for investigation.
- Logs do not contain sensitive information (tokens, passwords, SSNs, secrets).

### Configuration Management (CM)

- Changes preserve deterministic builds and central package/version management.
- Configuration settings are documented when new knobs are introduced.

### System & Communications Protection (SC)

- Crypto usage is via approved platform libraries.
- Network communications (HTTP clients) validate endpoints and do not introduce SSRF paths.

### System & Information Integrity (SI)

- Input validation and error handling prevent injection and data corruption.
- Dependencies are kept current and assessed for known vulnerabilities.

### Contingency/Resilience (CP) – code-level perspective

- Changes do not prevent recovery (e.g., migrations are safe, reversible where possible, and documented).
- Long-running jobs/services have sensible retry/cancellation behavior.

### Evidence expectations (what reviewers should ask for)

Depending on the change:

- **Build evidence**: builds across target frameworks for impacted projects.
- **Test evidence**: focused unit/integration tests; new tests for new behaviors.
- **Security evidence**: dependency scan output for material dependency changes; justification for exceptions.
- **Logging evidence**: examples of log events for security-relevant paths (with sensitive data redacted).

---

## Reviewer “stop-ship” findings

Any of the following should block approval until resolved:

- Package versions specified in project files instead of `Directory.Packages.props`.
- Single-targeting or dropping target frameworks without an explicit versioning/compatibility plan.
- Endpoints directly using EF Core/DbContext instead of going through services.
- New secrets committed (API keys, tokens, connection strings, `.env` contents, etc.).
- Raw SQL introduced without parameterization.
- Authz/authn regressions, or ability to access data outside intended scope.
- Logging of sensitive data.

---

## Quick reviewer prompts

- “Which target frameworks did you build and test?”
- “Which test project(s) did you run?”
- “Does this introduce any new external HTTP calls or URL inputs?”
- “Where is authorization enforced for this operation?”
- “If a dependency was added/updated, what’s the vulnerability/maintenance posture?”
