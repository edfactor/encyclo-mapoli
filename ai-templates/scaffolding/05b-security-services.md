# Part 5b: Security Services Extension

**Estimated Time:** 15 minutes  
**Prerequisites:** [Part 5a Complete](./05a-project-services.md)  
**Next:** [Part 5c: Telemetry & Middleware](./05c-telemetry-middleware.md)

---

## 🎯 Overview

Security services handle:

- **Okta JWT Authentication** - OAuth 2.0 bearer tokens
- **Policy-Based Authorization** - Role mappings
- **Impersonation Support** - Admin context switching
- **Read-Only Role Service** - Prevent unauthorized writes

---

## 🔐 AddSecurityServices Extension

### Extensions/SecurityServicesExtension.cs

```csharp
using Demoulas.Common.Contracts.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MySolution.Security;

namespace MySolution.Api.Extensions;

public static class SecurityServicesExtension
{
    public static IServiceCollection AddSecurityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ========================================
        // STEP 1: Bind Okta Configuration
        // ========================================
        var oktaConfig = new OktaConfiguration();
        configuration.Bind("Okta", oktaConfig);
        services.AddSingleton(oktaConfig);

        // ========================================
        // STEP 2: JWT Authentication
        // ========================================
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = oktaConfig.Domain;
                options.Audience = oktaConfig.Audience;
                options.RequireHttpsMetadata = true;  // Enforce HTTPS

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5)  // Allow 5min clock drift
                };
            });

        // ========================================
        // STEP 3: Policy-Based Authorization
        // ========================================
        services.AddAuthorization(options =>
        {
            foreach (var (policy, roles) in PolicyRoleMap.Map)
            {
                options.AddPolicy(policy, policyBuilder =>
                    policyBuilder.RequireRole(roles));
            }
        });

        // ========================================
        // STEP 4: Custom Security Services
        // ========================================
        services.AddScoped<IReadOnlyRoleService, ReadOnlyRoleService>();
        services.AddScoped<IUserContextService, UserContextService>();

        return services;
    }
}
```

---

## 🗺️ PolicyRoleMap Pattern

### Security/PolicyRoleMap.cs

```csharp
namespace MySolution.Security;

public static class Policy
{
    public const string ADMINISTRATOR = nameof(ADMINISTRATOR);
    public const string FINANCEMANAGER = nameof(FINANCEMANAGER);
    public const string READONLY = nameof(READONLY);
    public const string AUDITOR = nameof(AUDITOR);
    public const string ITDEVOPS = nameof(ITDEVOPS);
}

public static class Role
{
    public const string ADMINISTRATOR = "ADMINISTRATOR";
    public const string FINANCEMANAGER = "FINANCEMANAGER";
    public const string READONLY = "READONLY";
    public const string AUDITOR = "AUDITOR";
    public const string ITDEVOPS = "ITDEVOPS";
}

public static class PolicyRoleMap
{
    public static readonly Dictionary<string, string[]> Map = new()
    {
        // Admin can do everything
        { Policy.ADMINISTRATOR, new[] { Role.ADMINISTRATOR } },

        // Finance managers can manage financial data
        { Policy.FINANCEMANAGER, new[] { Role.ADMINISTRATOR, Role.FINANCEMANAGER } },

        // Auditors can view everything but not modify
        { Policy.AUDITOR, new[] { Role.ADMINISTRATOR, Role.AUDITOR } },

        // IT DevOps can manage technical operations
        { Policy.ITDEVOPS, new[] { Role.ADMINISTRATOR, Role.ITDEVOPS } },

        // Read-only can view non-sensitive data
        { Policy.READONLY, new[] { Role.ADMINISTRATOR, Role.FINANCEMANAGER, Role.AUDITOR, Role.READONLY } }
    };
}
```

---

## 👤 Read-Only Role Service

### Interfaces/IReadOnlyRoleService.cs

```csharp
namespace MySolution.Common.Interfaces;

public interface IReadOnlyRoleService
{
    Task<bool> IsReadOnlyAsync(CancellationToken ct = default);
}
```

### Services/ReadOnlyRoleService.cs

```csharp
using Microsoft.AspNetCore.Http;
using MySolution.Common.Interfaces;
using MySolution.Security;
using System.Security.Claims;

namespace MySolution.Services;

public class ReadOnlyRoleService : IReadOnlyRoleService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReadOnlyRoleService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<bool> IsReadOnlyAsync(CancellationToken ct = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user is null) return Task.FromResult(false);

        var roles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        // User is read-only if they ONLY have READONLY or AUDITOR role
        bool hasReadOnlyRole = roles.Contains(Role.READONLY);
        bool hasAuditorRole = roles.Contains(Role.AUDITOR);
        bool hasWriteRole = roles.Contains(Role.ADMINISTRATOR) || roles.Contains(Role.FINANCEMANAGER);

        return Task.FromResult((hasReadOnlyRole || hasAuditorRole) && !hasWriteRole);
    }
}
```

---

## 🔄 User Context Service

### Interfaces/IUserContextService.cs

```csharp
namespace MySolution.Common.Interfaces;

public interface IUserContextService
{
    string? GetUserId();
    string[] GetUserRoles();
    string? GetUserEmail();
}
```

### Services/UserContextService.cs

```csharp
using Microsoft.AspNetCore.Http;
using MySolution.Common.Interfaces;
using System.Security.Claims;

namespace MySolution.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string[] GetUserRoles()
    {
        return _httpContextAccessor.HttpContext?.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray() ?? [];
    }

    public string? GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.Email)?.Value;
    }
}
```

---

## 🔧 Okta Configuration

### appsettings.json

```json
{
  "Okta": {
    "Domain": "https://your-okta-domain.okta.com/oauth2/default",
    "Audience": "api://default",
    "EnvironmentName": "Development",
    "RolePrefix": "MySolution"
  }
}
```

### Configuration Class

```csharp
namespace Demoulas.Common.Contracts.Configuration;

public class OktaConfiguration
{
    public string Domain { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string? EnvironmentName { get; set; }
    public string RolePrefix { get; set; } = string.Empty;
}
```

---

## ✅ Validation Checklist - Part 5b

- [ ] **SecurityServicesExtension.cs** created
- [ ] **Okta JWT authentication** configured
- [ ] **PolicyRoleMap** defined with all roles
- [ ] **IReadOnlyRoleService** implemented
- [ ] **IUserContextService** implemented
- [ ] **Okta configuration** in appsettings.json
- [ ] **Policies applied** to endpoint groups

---

## 🎓 Key Takeaways - Part 5b

1. **JWT Authentication** - Okta validates bearer tokens
2. **Policy-Based Authorization** - Clean role mappings
3. **Read-Only Detection** - Prevent unauthorized writes
4. **User Context Service** - Access current user info

---

**Next:** [Part 5c: Telemetry & Middleware Extension](./05c-telemetry-middleware.md)
