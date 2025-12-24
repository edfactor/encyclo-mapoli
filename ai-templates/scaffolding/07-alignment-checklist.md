# Part 7: Alignment Checklist

**Estimated Time:** 30 minutes  
**Prerequisites:** [Parts 1-6 Complete](./README.md)  
**Purpose:** Audit existing projects for alignment with this infrastructure

---

## 🎯 Overview

This checklist helps identify inconsistencies in existing projects and provides migration strategies. Use this to bring legacy projects into alignment.

---

## ✅ Foundation Audit

### Package Versions

- [ ] **.NET SDK** version matches (`global.json`)
- [ ] **Central Package Management** enabled (`Directory.Packages.props`)
- [ ] **Package versions** aligned across solution
- [ ] **Obsolete packages** removed (e.g., `Swashbuckle` replaced with `FastEndpoints.Swagger`)

**Migration Strategy:**

```bash
# Check current SDK version
dotnet --version

# Update global.json if needed
dotnet new globaljson --sdk-version 10.0.100 --force

# Review Directory.Packages.props for version alignment
```

---

## ✅ Aspire Orchestration Audit

### AppHost Structure

- [ ] **ResourceManager** pattern used (not inline `.WithReference()` chains)
- [ ] **Aspire CLI** commands work (`aspire run`, `aspire update`)
- [ ] **ResourceManager registration** in DI
- [ ] **Port conflicts** resolved

**Migration Strategy:**

```csharp
// BEFORE: Inline resource chains
var api = builder.AddProject<Projects.MySolution_Api>("api")
    .WithReference(postgres)
    .WithReference(redis);

// AFTER: ResourceManager pattern
public class ResourceManager
{
    public IResourceBuilder<ProjectResource> AddApiProject(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<PostgresServerResource> postgres,
        IResourceBuilder<RedisResource> redis)
    {
        return builder.AddProject<Projects.MySolution_Api>("api")
            .WithReference(postgres)
            .WithReference(redis);
    }
}
```

---

## ✅ API Bootstrap Audit

### Middleware Ordering (CRITICAL)

- [ ] **PII Masking** registered FIRST
- [ ] **Health Checks** before authentication
- [ ] **Authentication** before authorization
- [ ] **Custom instrumentation** LAST

**Validation:**

```csharp
// CORRECT ORDER:
app.UseSmartPiiMasking();           // 1. PII masking FIRST
app.MapHealthChecks("/health");     // 2. Health checks
app.UseHttpsRedirection();          // 3. HTTPS
app.UseAuthentication();            // 4. Authentication BEFORE authorization
app.UseAuthorization();             // 5. Authorization AFTER authentication
app.MapFastEndpoints();             // 6. FastEndpoints
app.UseEndpointInstrumentation();   // 7. Custom middleware LAST
```

### CORS Configuration

- [ ] **No `AllowAnyOrigin()`** in production
- [ ] **Specific origins** configured per environment
- [ ] **Environment-specific** CORS policies

**Migration:**

```csharp
// ❌ WRONG - Insecure
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()  // SECURITY VIOLATION
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ✅ RIGHT - Environment-specific
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

services.AddCors(options =>
{
    options.AddPolicy("ApiCors", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});
```

---

## ✅ Database & EF Core Audit

### Interceptor Ordering (CRITICAL)

- [ ] **SoftDeleteInterceptor** registered FIRST
- [ ] **AuditInterceptor** registered AFTER soft delete
- [ ] **Custom interceptors** in correct order

**Validation:**

```csharp
// CORRECT ORDER:
services.AddDbContext<MyDbContext>(options =>
{
    options.UseOracle(connectionString);
    options.AddInterceptors(
        new SoftDeleteInterceptor(),      // 1. Soft delete FIRST
        new AuditInterceptor(),            // 2. Audit AFTER soft delete
        new CustomInterceptor());          // 3. Custom interceptors last
});
```

### ContextFactoryRequest Pattern

- [ ] **No direct `DbContext` in endpoints**
- [ ] **`IDbContextFactory<T>`** used in services
- [ ] **`UseReadOnlyContext()`** for queries

**Migration:**

```csharp
// ❌ BEFORE: Direct DbContext injection (wrong)
public class MemberEndpoint : Endpoint<MemberRequest>
{
    private readonly MyDbContext _context;  // WRONG - violates layer separation

    public MemberEndpoint(MyDbContext context)
    {
        _context = context;
    }
}

// ✅ AFTER: Factory pattern in service layer (correct)
public class MemberService : IMemberService
{
    private readonly IDbContextFactory<MyDbContext> _factory;

    public MemberService(IDbContextFactory<MyDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<MemberDto>> GetByIdAsync(int id, CancellationToken ct)
    {
        await using var context = await _factory.CreateDbContextAsync(ct);
        context.UseReadOnlyContext();  // Read-only optimization

        var member = await context.Members.FindAsync(id, ct);
        return member is null
            ? Result<MemberDto>.Failure(Error.NotFound)
            : Result<MemberDto>.Success(member.ToDto());
    }
}
```

---

## ✅ Security Services Audit

### JWT Authentication

- [ ] **Authority** configured correctly
- [ ] **Audience** validation enabled
- [ ] **RequireHttpsMetadata** set properly per environment
- [ ] **ClockSkew** set to 5 minutes

**Validation:**

```csharp
// Check appsettings.json
{
  "Okta": {
    "Domain": "https://your-domain.okta.com/oauth2/default",
    "Audience": "api://your-application",
    "RequireHttpsMetadata": true  // false only in Development
  }
}
```

### Policy-Based Authorization

- [ ] **PolicyRoleMap** pattern used
- [ ] **No hardcoded roles** in endpoints
- [ ] **Role inheritance** correctly configured

**Migration:**

```csharp
// ❌ BEFORE: Hardcoded roles (wrong)
[Authorize(Roles = "Admin,Manager")]
public class MyEndpoint : Endpoint<MyRequest>
{
}

// ✅ AFTER: Policy-based (correct)
public class MyEndpoint : Endpoint<MyRequest>
{
    public override void Configure()
    {
        Post("my-endpoint");
        Policies(Policy.FINANCEMANAGER);  // Uses PolicyRoleMap
    }
}

// PolicyRoleMap.cs
public static readonly Dictionary<string, string[]> Map = new()
{
    { Policy.FINANCEMANAGER, new[] { Role.ADMINISTRATOR, Role.FINANCEMANAGER } }
};
```

---

## ✅ Telemetry & Observability Audit

### OpenTelemetry Configuration

- [ ] **Custom ActivitySource** configured
- [ ] **Resource attributes** set (service name, environment)
- [ ] **Instrumentation** enabled (AspNetCore, HttpClient, EF Core)

### EndpointInstrumentationMiddleware

- [ ] **Session tracking** implemented (X-Session-ID)
- [ ] **Activity tags** include endpoint name, HTTP method, user ID
- [ ] **Logger scope** includes SessionId, TraceId, SpanId
- [ ] **Exception recording** with `activity.RecordException()`
- [ ] **Middleware registered LAST**

**Validation:**

```csharp
// Check middleware registration order
app.UseEndpointInstrumentation();  // MUST be LAST
```

---

## ✅ Health Checks Audit

- [ ] **EnvironmentHealthCheck** implemented
- [ ] **Database health check** registered (`AddDbContextCheck`)
- [ ] **Cache health check** registered
- [ ] **Custom ResponseWriter** for JSON output
- [ ] **HealthCheckPublisherOptions** configured (delay, period)

---

## ✅ Testing Patterns Audit

### Test Organization

- [ ] **Test collections** defined (parallel vs sequential)
- [ ] **[Description] attribute** used for Jira linking
- [ ] **Test fixtures** for shared setup
- [ ] **FluentAssertions/Shouldly** for assertions

### Architecture Tests

- [ ] **Layer dependency tests** implemented
- [ ] **Naming convention tests** implemented
- [ ] **Immutability tests** for DTOs
- [ ] **Custom rules** for project patterns

---

## 🚨 Common Migration Issues

### Issue 1: Middleware Ordering

**Symptom:** Authentication not working, PII leaking in logs

**Fix:**

```csharp
// Move PII masking to FIRST
app.UseSmartPiiMasking();  // Must be first
// ... other middleware
app.UseEndpointInstrumentation();  // Must be last
```

### Issue 2: DbContext in Endpoints

**Symptom:** Violates layer separation, can't test endpoints

**Fix:**

```csharp
// Move all data access to services
// Endpoints call services returning Result<T>
var result = await _service.GetAsync(id, ct);
return result.ToHttpResult(Error.NotFound);
```

### Issue 3: Hardcoded CORS Origins

**Symptom:** Security risk, production allows all origins

**Fix:**

```csharp
// Use environment-specific configuration
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();
```

### Issue 4: Missing Interceptor Ordering

**Symptom:** Audit logs missing, soft delete not working

**Fix:**

```csharp
// Order matters! SoftDelete BEFORE Audit
options.AddInterceptors(
    new SoftDeleteInterceptor(),  // FIRST
    new AuditInterceptor());       // AFTER
```

### Issue 5: No Session Tracking

**Symptom:** Can't correlate logs across requests

**Fix:**

```csharp
// Add EndpointInstrumentationMiddleware with X-Session-ID
public class EndpointInstrumentationMiddleware
{
    // ... implementation from Part 5c
}
```

---

## 📋 PR Review Checklist

Use this checklist when reviewing PRs:

**Security:**

- [ ] No hardcoded secrets
- [ ] PII masked in logs
- [ ] No `AllowAnyOrigin()` in CORS
- [ ] Server-side authorization checks

**Architecture:**

- [ ] No `DbContext` in endpoints
- [ ] Services return `Result<T>`
- [ ] DTOs are immutable records
- [ ] Proper namespace organization

**Telemetry:**

- [ ] Activity created with endpoint name
- [ ] Session tracking implemented
- [ ] Exception recording with `activity.RecordException()`
- [ ] Logger scope includes correlation IDs

**Testing:**

- [ ] Unit tests for new services
- [ ] [Description] attribute on test methods
- [ ] Architecture tests updated if needed
- [ ] Integration tests for new endpoints

---

## 🎯 Alignment Priority Matrix

**Priority 1 (Security - Fix Immediately):**

1. CORS `AllowAnyOrigin()` removal
2. PII masking implementation
3. JWT authentication configuration
4. Authorization policy enforcement

**Priority 2 (Architecture - Fix in Next Sprint):**

1. `DbContext` removal from endpoints
2. `Result<T>` pattern adoption
3. Middleware ordering fixes
4. Interceptor ordering fixes

**Priority 3 (Observability - Plan for Q2):**

1. OpenTelemetry instrumentation
2. Session tracking implementation
3. Health check enhancements
4. Architecture tests implementation

---

## ✅ Final Validation

After alignment, verify:

```bash
# 1. Build succeeds
dotnet build

# 2. Tests pass
dotnet test

# 3. Architecture tests pass
dotnet test --filter "FullyQualifiedName~Architecture"

# 4. Health checks work
curl https://localhost:7000/health

# 5. Aspire runs
aspire run
```

---

## 🎓 Key Takeaways - Part 7

1. **Security First** - CORS, PII, JWT validation are critical
2. **Layer Separation** - Endpoints → Services → Data
3. **Middleware Ordering** - PII first, instrumentation last
4. **Interceptor Ordering** - SoftDelete before Audit
5. **Testing** - Architecture tests prevent regression

---

**Complete! 🎉** You now have:

- Comprehensive scaffolding instructions
- Migration strategies for legacy code
- PR review checklist
- Priority matrix for alignment work

**Next Steps:**

1. Share with team for feedback
2. Create Jira tickets for alignment work
3. Schedule architecture review sessions
4. Update team documentation

---

[Return to Index](./README.md)
