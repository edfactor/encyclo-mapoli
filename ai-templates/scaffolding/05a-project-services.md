# Part 5a: Project Services Extension

**Estimated Time:** 15 minutes  
**Prerequisites:** [Part 4 Complete](./04-database-efcore.md)  
**Next:** [Part 5b: Security Services](./05b-security-services.md)

---

## 🎯 Overview

`AddProjectServices` registers all business services with proper lifetimes:

- **Scoped Services** - Per-request isolation (default for most services)
- **Singleton Services** - Application-wide shared state (caches, hosted services)
- **Transient Services** - New instance per injection (rarely used)

---

## 📦 Extension Method Template

### Extensions/ServicesExtension.cs

**CRITICAL:** Use `IHostApplicationBuilder` (not `IServiceCollection`) for modern .NET 10 pattern.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySolution.Common.Interfaces;
using MySolution.Services;
using MySolution.Services.Caching.Extensions;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.Common.Data.Services.Service;

namespace MySolution.Services.Extensions;

/// <summary>
/// Provides helper methods for configuring project services.
/// </summary>
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectServices(this IHostApplicationBuilder builder)
    {
        // ========================================
        // Business Services (Scoped - per request)
        // ========================================
        builder.Services.AddScoped<IMemberService, MemberService>();
        builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();
        builder.Services.AddScoped<IDistributionService, DistributionService>();
        builder.Services.AddScoped<IReportService, ReportService>();
        // ... add all business services

        // ========================================
        // Common Data Services
        // ========================================
        builder.Services.AddSingleton<IStoreService, StoreService>();
        builder.Services.AddScoped<IDepartmentService, DepartmentService>();
        builder.Services.AddScoped<IAccountingPeriodsService, AccountingPeriodsService>();

        // ========================================
        // Cache Services (via extension)
        // ========================================
        builder.AddProjectCachingServices();

        return builder;
    }
}
```

---

## 🔄 Service Lifetime Guidelines

### Scoped (Default for Business Logic)

```csharp
builder.Services.AddScoped<IMemberService, MemberService>();
```

**When to use:**

- Per-request database operations
- Services that use DbContext
- Services that hold request-specific state
- **Most business services should be Scoped**

### Singleton (Shared State)

```csharp
builder.Services.AddSingleton<IStateTaxCache, StateTaxCache>();
```

**When to use:**

- Caches (no per-request state)
- Configuration wrappers
- Thread-safe shared services
- Hosted services

**⚠️ Warning:** Singletons cannot inject Scoped services!

### Transient (Rarely Used)

```csharp
builder.Services.AddTransient<IEmailSender, EmailSender>();
```

**When to use:**

- Stateless, lightweight services
- Services used once per operation
- **Generally avoid - use Scoped instead**

---

## 📋 Complete Service Registration Example

```csharp
public static IServiceCollection AddProjectServices(this IServiceCollection services)
{
    // Core Services
    services.AddScoped<IMasterInquiryService, MasterInquiryService>();
    services.AddScoped<IBeneficiaryService, BeneficiaryService>();
    services.AddScoped<IDistributionService, DistributionService>();
    services.AddScoped<IYearEndService, YearEndService>();
    services.AddScoped<IReportService, ReportService>();
    services.AddScoped<IAuditService, AuditService>();

    // Lookup Services
    services.AddScoped<IStateLookupService, StateLookupService>();
    services.AddScoped<ITaxCodeLookupService, TaxCodeLookupService>();

    // Calculation Services
    services.AddScoped<IProfitCalculationService, ProfitCalculationService>();
    services.AddScoped<ITaxCalculationService, TaxCalculationService>();

    // Validation Services
    services.AddScoped<IValidationService, ValidationService>();
    services.AddScoped<IChecksumValidationService, ChecksumValidationService>();

    // Cache Services (Singleton)
    services.AddSingleton<INavigationCache, NavigationCache>();
    services.AddSingleton<IStateTaxCache, StateTaxCache>();

    // Hosted Services
    services.AddHostedService<StateTaxCacheWarmerHostedService>();
    services.AddHostedService<NavigationCacheWarmerHostedService>();

    return services;
}
```

---

## 🏭 Cache Warmer Pattern

### Hosted Service Implementation

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MySolution.Services.HostedServices;

public class StateTaxCacheWarmerHostedService : BackgroundService
{
    private readonly IStateTaxCache _cache;
    private readonly ILogger<StateTaxCacheWarmerHostedService> _logger;

    public StateTaxCacheWarmerHostedService(
        IStateTaxCache cache,
        ILogger<StateTaxCacheWarmerHostedService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait 5 seconds for app to initialize
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        try
        {
            _logger.LogInformation("Warming up state tax cache...");
            await _cache.WarmupAsync(stoppingToken);
            _logger.LogInformation("State tax cache warmed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm state tax cache");
        }
    }
}
```

### Cache Interface

```csharp
public interface IStateTaxCache
{
    Task<StateTaxInfo?> GetAsync(string state, CancellationToken ct);
    Task WarmupAsync(CancellationToken ct);
}
```

---

## 🧪 Service Interface Pattern

### IMemberService.cs

```csharp
using MySolution.Common.Contracts;
using MySolution.Common.DTOs;

namespace MySolution.Common.Interfaces;

public interface IMemberService
{
    Task<Result<MemberDto>> GetByIdAsync(int id, CancellationToken ct);
    Task<Result<PaginatedResponseDto<MemberDto>>> SearchAsync(
        MemberSearchRequest request, CancellationToken ct);
    Task<Result<int>> CreateAsync(CreateMemberRequest request, CancellationToken ct);
    Task<Result<bool>> UpdateAsync(UpdateMemberRequest request, CancellationToken ct);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct);
}
```

---

## ✅ Validation Checklist - Part 5a

- [ ] **ServicesExtension.cs** created with AddProjectServices
- [ ] **All business services** registered as Scoped
- [ ] **Cache services** registered as Singleton
- [ ] **Hosted services** registered if needed
- [ ] **Service interfaces** defined in Common project
- [ ] **Service implementations** in Services project
- [ ] **No Scoped services injected into Singletons**

---

## 🎓 Key Takeaways - Part 5a

1. **Default to Scoped** - Most services should be per-request
2. **Singleton for Caches** - Shared state across requests
3. **Hosted Services** - Background tasks for cache warming
4. **Interface Pattern** - All services expose interfaces

---

**Next:** [Part 5b: Security Services Extension](./05b-security-services.md)
