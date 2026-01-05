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

### Extensions/SecurityExtension.cs

**CRITICAL:** Use `IHostApplicationBuilder` (not `IServiceCollection`) for modern .NET 10 pattern.

```csharp
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Security.Extensions;
using MySolution.Common.Configuration;
using MySolution.Common.Interfaces.Security;
using MySolution.Security.Certificate;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MySolution.Security.Extensions;

public static class SecurityExtension
{
    public static IHostApplicationBuilder AddSecurityServices(this WebApplicationBuilder builder)
    {
        // ========================================
        // STEP 1: Register JwtSettings using Options pattern
        // ========================================
        _ = builder.Services.AddOptions<JwtSettings>()
            .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register JwtSettings as singleton for direct injection (required by JwtTokenService)
        _ = builder.Services.AddSingleton(sp =>
            sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtSettings>>().Value);

        // ========================================
        // STEP 2: Register Certificate and JWT Services
        // ========================================
        _ = builder.Services.AddSingleton<ICertificateService, CertificateService>();
        _ = builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // ========================================
        // STEP 3: Bind Okta Configuration
        // ========================================
        _ = builder.Services.AddSingleton<OktaConfiguration>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            OktaConfiguration settings = new OktaConfiguration
            {
                OktaDomain = string.Empty,
                AuthorizationServerId = string.Empty,
                Audience = string.Empty,
                RolePrefix = string.Empty
            };
            config.Bind("Okta", settings);
            return settings;
        });

        // ========================================
        // STEP 4: Add Authentication (Okta or Testing)
        // ========================================
        if (!builder.Environment.IsTestEnvironment())
        {
            builder.Services.AddOktaSecurity(builder.Configuration);
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            builder.Services.AddTestingSecurity(builder.Configuration);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // ========================================
        // STEP 5: Configure Authorization Policies
        // ========================================
        _ = builder.ConfigureSecurityPolicies();

        return builder;
    }
}
```

---

## � PolicyExtensions (Internal)

**Pattern:** Separate policy configuration into internal extension method for clean separation.

### Extensions/PolicyExtensions.cs

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MySolution.Security.Extensions;

internal static class PolicyExtensions
{
    internal static WebApplicationBuilder ConfigureSecurityPolicies(this WebApplicationBuilder builder)
    {
        _ = builder.Services.AddAuthorization(options =>
        {
            foreach (var kvp in PolicyRoleMap.Map)
            {
                var policyName = kvp.Key;
                var roles = kvp.Value;
                options.AddPolicy(policyName, x => x.RequireRole(roles));
            }
        });

        return builder;
    }
}
```

**Key Points:**

- ✅ Internal visibility - policy configuration is implementation detail
- ✅ Extension on `WebApplicationBuilder` for consistent builder pattern
- ✅ Iterates PolicyRoleMap to create authorization policies
- ✅ Returns builder for method chaining

---

## �🗺️ PolicyRoleMap Pattern

### Security/PolicyRoleMap.cs

```csharp
namespace MySolution.Security;

/// <summary>
/// Centralized policy names used by [Authorize(Policy = ...)] to guard business actions.
/// </summary>
public static class Policy
{
    /// <summary>
    /// Policy for admin-level operations.
    /// </summary>
    public static readonly string ADMINISTRATOR = "ADMINISTRATOR";

    /// <summary>
    /// Policy for finance management operations.
    /// </summary>
    public static readonly string FINANCEMANAGER = "FINANCEMANAGER";

    /// <summary>
    /// Policy for read-only access.
    /// </summary>
    public static readonly string READONLY = "READONLY";
}

public static class Role
{
    public const string ADMINISTRATOR = "administrator";
    public const string FINANCEMANAGER = "finance-manager";
    public const string READONLY = "read-only";
    public const string AUDITOR = "auditor";
}

/// <summary>
/// Central map of authorization policies to allowed roles.
/// Used by both runtime registration and Swagger enrichment.
/// </summary>
public static class PolicyRoleMap
{
    public static readonly IReadOnlyDictionary<string, string[]> Map = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
    {
        // Admin can do everything
        [Policy.ADMINISTRATOR] = [Role.ADMINISTRATOR],

        // Finance managers can manage financial data
        [Policy.FINANCEMANAGER] = [Role.ADMINISTRATOR, Role.FINANCEMANAGER],

        // Read-only can view non-sensitive data
        [Policy.READONLY] = [Role.ADMINISTRATOR, Role.FINANCEMANAGER, Role.AUDITOR, Role.READONLY]
    };

    public static string[] GetRoles(string policyName) => Map.TryGetValue(policyName, out var roles) ? roles : [];
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

**NEW:** `OktaEnable` flag allows toggling authentication at runtime via configuration.

```json
{
  "Okta": {
    "OktaEnable": true, // NEW: Toggle authentication on/off
    "Domain": "https://your-okta-domain.okta.com/oauth2/default",
    "Audience": "api://default",
    "EnvironmentName": "Development",
    "RolePrefix": "MySolution"
  }
}
```

**Usage in Program.cs:**

```csharp
// Read OktaEnable flag from configuration
void OktaSettingsAction(OktaSwaggerConfiguration settings)
{
    builder.Configuration.Bind("Okta", settings);
}

// Pass useOktaSecurity parameter to enable/disable Okta at runtime
app.UseDefaultEndpoints(OktaSettingsAction, useOktaSecurity: true);
// Set to false to disable Okta authentication (useful for testing)
```

**Benefits:**

- ✅ Toggle authentication without code changes
- ✅ Useful for local development without Okta setup
- ✅ Simplifies testing scenarios
- ✅ Can be controlled per environment via appsettings.{Environment}.json

````

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
````

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
