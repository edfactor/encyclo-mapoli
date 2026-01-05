# .NET 10 Web API Infrastructure Scaffolding Guide

**Purpose:** Complete instructions for scaffolding a new .NET 10 Web API project with the same infrastructure as the Demoulas Profit Sharing solution. Can also be used to audit existing projects for alignment.

**Target Audience:** Development teams, AI coding assistants, technical leads

**Last Updated:** December 24, 2025

---

## 📚 Documentation Structure

This guide is organized into 11 focused sections:

### Foundation & Core Setup

#### [Part 1: Foundation & Prerequisites](./01-foundation-prerequisites.md) ⏱️ 15-20 min

- Required software and SDKs
- Solution structure template
- Central package management setup
- Directory.Build.props configuration
- Demoulas.Common library integration patterns

#### [Part 2: Aspire Orchestration](./02-aspire-orchestration.md) ⏱️ 15-20 min

- Complete AppHost Program.cs template (388 lines)
- **ResourceManager pattern** (NEW: API lifecycle management during DB operations)
- **CommandHelper pattern** (NEW: Database operations with user interaction)
- Service integration (Oracle, RabbitMQ, Redis)
- Playwright browser automation
- Aspire CLI commands with confirmation dialogs

#### [Part 3: API Bootstrap & Middleware](./03-api-bootstrap-middleware.md) ⏱️ 20-25 min

- Complete API Program.cs template (218 lines)
- CORS configuration (dev vs production)
- JSON serialization with PII masking
- Middleware ordering (CRITICAL)
- Endpoint instrumentation
- Health checks

#### [Part 4: Database & EF Core](./04-database-efcore.md) ⏱️ 15-20 min

- DatabaseServicesExtension pattern
- ContextFactoryRequest configuration
- Interceptor ordering (CRITICAL: HttpContextAccessor before Audit)
- Read-only contexts
- **NoOpServicePattern** (NEW: Graceful degradation for optional dependencies)
- Health checks
- Connection string management

### Service Extensions & Infrastructure

#### [Part 5a: Project Services Extension](./05a-project-services.md) ⏱️ 15 min

- AddProjectServices extension method
- Service lifetime management (Scoped, Singleton, Transient)
- Cache warmer registration
- Service organization patterns

#### [Part 5b: Security Services Extension](./05b-security-services.md) ⏱️ 15 min

- AddSecurityServices extension method
- Okta JWT authentication configuration
- **OktaEnable configuration toggle** (NEW: Runtime on/off switch)
- PolicyRoleMap pattern for authorization
- Read-only role service implementation
- UserContextService for claims extraction

#### [Part 5c: Telemetry & Middleware](./05c-telemetry-middleware.md) ⏱️ 20 min

- AddProfitSharingTelemetry extension method
- OpenTelemetry configuration (Tracing, Metrics, Logs)
- EndpointInstrumentationMiddleware implementation
- Session tracking with X-Session-ID
- Activity tagging and correlation

### Testing & Quality

#### [Part 6a: Health Checks](./06a-health-checks.md) ⏱️ 10 min

- EnvironmentHealthCheck implementation
- Database and cache health checks
- Custom health check patterns
- Health check endpoints and JSON responses

#### [Part 6b: Testing Patterns](./06b-testing-patterns.md) ⏱️ 15 min

- xUnit 3.0 test project setup
- Test collection definitions (parallel vs sequential)
- Test fixtures for shared setup
- [Description] attribute for Jira linking

#### [Part 6c: Architecture Tests](./06c-architecture-tests.md) ⏱️ 15 min

- ArchUnitNET setup and configuration
- Layer dependency tests
- Naming convention enforcement
- Immutability rules for DTOs

### Alignment & Migration

#### [Part 7: Alignment Checklist](./07-alignment-checklist.md) ⏱️ 30 min

- Package version audit and alignment
- Middleware ordering validation
- CORS configuration security review
- Interceptor ordering verification
- DbContext usage audit (endpoints vs services)
- Testing infrastructure compliance
- Priority matrix for migration work
- PR review checklist
- Validation scripts

---

## 🚀 Quick Start Paths

### Path A: New Project from Scratch

**Recommended order:** Parts 1 → 2 → 3 → 4 → 5 → 6 → 7

**Time Investment:** ~3-4 hours for full setup

**Steps:**

1. Read Part 1 (Foundation & Prerequisites)
2. Follow Parts 2-4 (Core Infrastructure)
3. Implement Parts 5a-5c (Service Extensions)
4. Set up Parts 6a-6c (Testing & Quality)
5. Validate with Part 7 checklist

### Path B: Audit Existing Project

**Recommended order:** Part 7 (checklist) → Parts 1-6c (as needed for fixes)

**Time Investment:** ~1-2 hours for audit, varies for remediation

**Steps:**

1. Start with Part 7 alignment checklist
2. Identify gaps in current implementation
3. Reference relevant Parts (1-6c) for fix patterns
4. Prioritize security issues (CORS, PII, JWT)
5. Re-validate with checklist

### Path C: AI-Assisted Scaffolding

**For AI coding assistants (like GitHub Copilot, Claude, etc.)**

**Prompt Template:**

```
I need to scaffold a new .NET 10 Web API project following the patterns in this scaffolding guide.

Context: [Describe your project domain]

Please implement:
1. [Specify which parts you need, e.g., "Part 5b: Security services with Okta JWT"]
2. [Any specific customizations, e.g., "Replace Okta with Azure AD B2C"]

Follow the templates exactly from the scaffolding guide, preserving:
- Middleware ordering (Part 3)
- Interceptor ordering (Part 4)
- Session tracking (Part 5c)
- Security configurations (Part 5b)
```

---

## 🎯 Key Patterns Summary

### CRITICAL Patterns (Must Follow Exactly)

1. **Middleware Ordering** (Part 3)

   ```
   PII Masking → Health Checks → HTTPS → Authentication →
   Authorization → FastEndpoints → Instrumentation (LAST)
   ```

2. **Interceptor Ordering** (Part 4)

   ```
   HttpContextAccessor MUST be registered BEFORE AuditSaveChangesInterceptor
   Order: [HttpContextAccessor, Audit, Domain-specific interceptors]
   ```

3. **Central Package Management** (Part 1)

   - ALL versions in Directory.Packages.props
   - NO versions in individual .csproj files
   - `CentralPackageTransitivePinningEnabled=true`

4. **Performance Flags** (Part 1)

   ```xml
   <ServerGarbageCollection>true</ServerGarbageCollection>
   <PublishReadyToRun>true</PublishReadyToRun>
   <TieredCompilation>true</TieredCompilation>
   ```

5. **CORS by Environment** (Part 3)
   - Dev: localhost:3100 ONLY (no AllowAnyOrigin)
   - Prod: Explicit whitelist of UI origins

### Common Patterns (Strongly Recommended)

- **Result<T> Pattern** - Service methods return Result<T>, not throw exceptions
- **Read-Only Contexts** - Use `UseReadOnlyContext()` for queries
- **Telemetry** - Custom ActivitySource + business metrics
- **PII Masking** - MaskingJsonConverterFactory + MaskingOperators
- **Health Checks** - EnvironmentHealthCheck with 12 diagnostic fields

---

## 📖 How to Use This Guide

### For Human Developers

1. Read sections in order for new projects
2. Use as reference for specific patterns
3. Bookmark for PR reviews against standards

### For AI Coding Assistants

1. Load relevant sections as context
2. Follow templates exactly
3. Preserve ordering and configurations
4. Flag deviations in implementation notes

### For Technical Leads

1. Use Part 7 for architecture reviews
2. Enforce checklist in PR templates
3. Share with teams as onboarding material

---

## 🔗 Related Documentation

- [Main Project Instructions](../../copilot-instructions.md) - Core architecture and patterns
- [Code Review Checklist](../../.github/CODE_REVIEW_CHECKLIST.md) - PR review standards
- [Telemetry Guide](../../src/ui/public/docs/TELEMETRY_GUIDE.md) - Observability patterns
- [Security Instructions](../../.github/instructions/security.instructions.md) - OWASP Top 10 compliance

---

## 📝 Document Conventions

### Code Blocks

- `✅ RIGHT` - Correct pattern to follow
- `❌ WRONG` - Anti-pattern to avoid
- `// CRITICAL` - Must-follow comment
- `// Note` - Important context

### Sections

- 📋 Checklist items
- 🎯 Key patterns
- 🔧 Configuration
- 📦 Packages
- ⚠️ Warnings

---

## 🤝 Contributing

To update this scaffolding guide:

1. Make changes to relevant part files
2. Update this README if structure changes
3. Update "Last Updated" date
4. Test changes with actual scaffolding
5. Submit PR with validation results

---

## ⚖️ License

This documentation is part of the Demoulas Smart Profit Sharing project and follows the same license terms.

---

**Questions or Issues?** Contact the Architecture Team or open a discussion in the project repository.
