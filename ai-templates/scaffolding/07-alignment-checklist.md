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

- [ ] **UseCors()** called FIRST (before all other middleware)
- [ ] **UseCommonMiddleware()** included (via UseDefaultEndpoints - adds server timing, versioning headers)
- [ ] **Authentication** before authorization
- [ ] **No custom endpoint instrumentation middleware** (telemetry automatic via DemoulasEndpoint)

**Validation:**

```csharp
// CORRECT ORDER (post-Common.Api adoption):
app.UseCors();                      // 1. CORS FIRST (CRITICAL)
// ... custom middleware (no-cache, PII masking)
app.UseDefaultEndpoints();          // Includes UseCommonMiddleware, authentication, authorization
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

### ConfigureDefaultEndpoints (Demoulas.Common.Api)

- [ ] **ConfigureDefaultEndpoints()** called with custom meter names
- [ ] **Meter names** follow naming pattern: `"{Company}.{Project}.{Layer}"`
- [ ] **EF Core instrumentation** enabled with `SetDbStatementForText` and `SetDbStatementForStoredProcedure`
- [ ] **UseCommonMiddleware()** included (via UseDefaultEndpoints)

**Validation:**

```csharp
// In Program.cs - Check builder configuration
builder.ConfigureDefaultEndpoints(
    meterNames: [
        "Demoulas.Smart.StoreOrdering.Endpoints",
        "Demoulas.Smart.StoreOrdering.Services",
        "Demoulas.Smart.StoreOrdering.Data"
    ]);

// Check EF Core instrumentation registration
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        // ... existing meters
        metrics.AddMeter("Microsoft.EntityFrameworkCore");
    })
    .WithTracing(tracing =>
    {
        // ... existing tracing
        tracing.AddEntityFrameworkCoreInstrumentation(options =>
        {
            options.SetDbStatementForText = true;
            options.SetDbStatementForStoredProcedure = true;
            options.EnrichWithIDbCommand = (activity, command) =>
            {
                activity.DisplayName = command.CommandText;
            };
        });
    });
```

### DemoulasEndpoint Base Classes

- [ ] **All endpoints** inherit from `DemoulasEndpoint<TRequest, TResponse>` (or variant)
- [ ] **ExecuteAsync renamed** to `HandleRequestAsync` (protected abstract)
- [ ] **GetSensitiveFields()** overridden for PII tracking (SSN, DOB, email)
- [ ] **AddCustomTelemetryTags()** overridden for domain metadata (store_id, user_id, order_id)
- [ ] **BusinessOperationsTotal** metrics added for operational insights

**Validation:**

```csharp
// Check endpoint inheritance
public sealed class GetStoresEndpoint : DemoulasEndpoint<GetStoresRequest, StoreListResponse>
{
    // Constructor MUST inject ILogger<T>
    public GetStoresEndpoint(
        IStoreService service,
        ILogger<GetStoresEndpoint> logger)  // Required for telemetry
        : base(navigationId)
    {
        _service = service;
        _logger = logger;
    }

    // MUST implement HandleRequestAsync (not ExecuteAsync)
    protected override async Task<StoreListResponse> HandleRequestAsync(
        GetStoresRequest req, CancellationToken ct)
    {
        // Business logic
        var result = await _service.GetStoresAsync(req, ct);

        // Business metrics (required)
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "store-list"),
            new("endpoint", nameof(GetStoresEndpoint)));

        return result;
    }

    // Override for PII tracking (compliance)
    protected override string[] GetSensitiveFields(GetStoresRequest request) =>
        new[] { "Ssn", "DateOfBirth", "Email" };  // If applicable

    // Override for domain-specific telemetry
    protected override void AddCustomTelemetryTags(Activity activity, GetStoresRequest request)
    {
        activity.SetTag("store.region_id", request.RegionId);
        activity.SetTag("store.include_inactive", request.IncludeInactive);
    }
}
```

**Migration Strategy:**

```csharp
// BEFORE: Standard FastEndpoints pattern (no telemetry)
public class GetStoresEndpoint : Endpoint<GetStoresRequest, StoreListResponse>
{
    public override async Task<StoreListResponse> ExecuteAsync(
        GetStoresRequest req, CancellationToken ct)
    {
        var stores = await _service.GetStoresAsync(req, ct);
        return new StoreListResponse { Stores = stores };
    }
}

// AFTER: DemoulasEndpoint with automatic telemetry
public class GetStoresEndpoint : DemoulasEndpoint<GetStoresRequest, StoreListResponse>
{
    private readonly ILogger<GetStoresEndpoint> _logger;  // REQUIRED

    public GetStoresEndpoint(
        IStoreService service,
        ILogger<GetStoresEndpoint> logger)  // Inject logger
        : base(navigationId)
    {
        _service = service;
        _logger = logger;
    }

    // Renamed ExecuteAsync → HandleRequestAsync
    protected override async Task<StoreListResponse> HandleRequestAsync(
        GetStoresRequest req, CancellationToken ct)
    {
        var stores = await _service.GetStoresAsync(req, ct);

        // Add business metrics
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "store-list"),
            new("endpoint", nameof(GetStoresEndpoint)));

        return new StoreListResponse { Stores = stores };
    }

    // Add domain tags
    protected override void AddCustomTelemetryTags(Activity activity, GetStoresRequest request)
    {
        if (request.RegionId.HasValue)
            activity.SetTag("store.region_id", request.RegionId.Value);
    }
}
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

**Symptom:** CORS headers missing, authentication not working

**Fix:**

```csharp
// Move UseCors() to FIRST position (CRITICAL - security vulnerability)
app.UseCors();  // Must be first for preflight requests
// ... other middleware
app.UseDefaultEndpoints();  // Includes UseCommonMiddleware
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

### Issue 5: Endpoints Not Using DemoulasEndpoint

**Symptom:** No telemetry, missing PII tracking, no correlation IDs

**Fix:**

```csharp
// Migrate from Endpoint<T> to DemoulasEndpoint<T>
// See "DemoulasEndpoint Base Classes" section above for full pattern
public class MyEndpoint : DemoulasEndpoint<MyRequest, MyResponse>
{
    private readonly ILogger<MyEndpoint> _logger;  // REQUIRED

    public MyEndpoint(IMyService service, ILogger<MyEndpoint> logger)
        : base(navigationId)
    {
        _service = service;
        _logger = logger;  // Must inject for telemetry
    }

    // Rename ExecuteAsync → HandleRequestAsync
    protected override async Task<MyResponse> HandleRequestAsync(
        MyRequest req, CancellationToken ct)
    {
        // Business logic
        var result = await _service.ProcessAsync(req, ct);

        // Add business metrics
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "my-operation"),
            new("endpoint", nameof(MyEndpoint)));

        return result;
    }
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

- [ ] Endpoints inherit from `DemoulasEndpoint<TRequest, TResponse>`
- [ ] `ILogger<TEndpoint>` injected in constructor
- [ ] `ExecuteAsync` renamed to `HandleRequestAsync`
- [ ] `GetSensitiveFields()` overridden for PII access
- [ ] `AddCustomTelemetryTags()` overridden for domain metadata
- [ ] `BusinessOperationsTotal` metrics added
- [ ] ConfigureDefaultEndpoints called with custom meters

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

1. DemoulasEndpoint migration for all endpoints
2. GetSensitiveFields() overrides for PII tracking
3. AddCustomTelemetryTags() for domain metadata
4. BusinessOperationsTotal metrics implementation
5. Health check enhancements
6. Architecture tests implementation

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

1. **Security First** - CORS (FIRST in pipeline), PII, JWT validation are critical
2. **Layer Separation** - Endpoints → Services → Data (no DbContext in endpoints)
3. **Middleware Ordering** - CORS first, UseCommonMiddleware via UseDefaultEndpoints
4. **Interceptor Ordering** - SoftDelete before Audit
5. **Telemetry via Base Classes** - DemoulasEndpoint provides automatic instrumentation
6. **Testing** - Architecture tests prevent regression

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
