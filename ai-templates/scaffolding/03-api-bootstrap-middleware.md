# Part 3: API Bootstrap & Middleware

**Estimated Time:** 20-25 minutes  
**Prerequisites:** [Part 2 Complete](./02-aspire-orchestration.md)  
**Next:** [Part 4: Database & EF Core](./04-database-efcore.md)

---

## 🎯 Overview

The API `Program.cs` is the entry point for the web application. Critical aspects:

- **Middleware Ordering** - MUST follow exact sequence (see below)
- **CORS Configuration** - Dev vs production policies
- **JSON Serialization** - PII masking with source generation
- **Telemetry Integration** - OpenTelemetry + custom instrumentation
- **Health Checks** - Environment diagnostics

---

## 📦 API Project Setup

### 1. Create API Project

```powershell
cd src
dotnet new webapi -n MySolution.Api
```

### 2. API .csproj Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- Demoulas Common -->
    <PackageReference Include="Demoulas.Common.Api" />
    <PackageReference Include="Demoulas.Common.Logging" />

    <!-- FastEndpoints -->
    <PackageReference Include="FastEndpoints" />
    <PackageReference Include="FastEndpoints.Swagger" />

    <!-- Authentication -->
    <PackageReference Include="Okta.AspNetCore" />

    <!-- OpenTelemetry -->
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" />

    <!-- Swagger/OpenAPI -->
    <PackageReference Include="NSwag.AspNetCore" />
    <PackageReference Include="Scalar.AspNetCore" />

    <!-- PDF (optional) -->
    <PackageReference Include="QuestPDF" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MySolution.Endpoints\MySolution.Endpoints.csproj" />
    <ProjectReference Include="..\MySolution.Services\MySolution.Services.csproj" />
    <ProjectReference Include="..\MySolution.Security\MySolution.Security.csproj" />
    <ProjectReference Include="..\MySolution.Data\MySolution.Data.csproj" />
  </ItemGroup>
</Project>
```

---

## 🔧 Complete API Program.cs Template

```csharp
using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Contracts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Logging.Extensions;
using MySolution.Api.Serialization;
using MySolution.Common.ActivitySources;
using MySolution.Common.Metrics;
using MySolution.Data.Extensions;
using MySolution.Endpoints.HealthCheck;
using MySolution.Security;
using MySolution.Security.Extensions;
using MySolution.Services.Extensions;
using MySolution.Services.LogMasking;
using MySolution.Services.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using QuestPDF;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

// ========================================
// STEP 1: Kestrel Configuration (Dev Only)
// ========================================
// Production uses IIS/reverse proxy with web.config timeout settings
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // 3-minute timeout (buffer for 2-minute endpoint timeout)
        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(3);
        options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
    });
}

// ========================================
// STEP 2: Configuration Sources
// ========================================
if (!builder.Environment.IsTestEnvironment())
{
    builder.Configuration
        .AddJsonFile($"credSettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddUserSecrets<Program>();
}
else
{
    builder.Configuration
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}

// ========================================
// STEP 3: Logging with PII Masking
// ========================================
LoggingConfig logConfig = new();
builder.Configuration.Bind("SmartLogging", logConfig);

// CRITICAL: Add PII masking operators
logConfig.MaskingOperators = [
    new UnformattedSocialSecurityNumberMaskingOperator(),
    new SensitiveValueMaskingOperator()
];

builder.SetDefaultLoggerConfiguration(logConfig);

// ========================================
// STEP 4: QuestPDF License (if using PDF generation)
// ========================================
// For development: Community (free)
// For commercial use (revenue > $1M USD): Commercial license required
Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// ========================================
// STEP 5: Security Services (Okta + Policies)
// ========================================
builder.AddSecurityServices();

// ========================================
// STEP 6: CORS Configuration
// ========================================
string[] allowedOrigins = [
    "https://myapp.qa.example.com",
    "https://myapp.uat.example.com",
    "https://myapp.example.com"
];

string[] developmentOrigins = [
    "http://localhost:3100",
    "http://127.0.0.1:3100",
    "https://localhost:3100",
    "https://127.0.0.1:3100"
];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(pol =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // CRITICAL: Dev restricts to localhost ONLY (PS-2025)
            // Prevents MITM attacks on shared networks
            pol.WithOrigins(developmentOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Location", "x-custom-header");
        }
        else
        {
            // Production: Explicit whitelist only
            pol.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Location", "x-custom-header");
        }
    });
});

// ========================================
// STEP 7: JSON Serialization with PII Masking
// ========================================
builder.Services.ConfigureHttpJsonOptions(o =>
{
    // CRITICAL: Insert masking converter at index 0 for highest precedence
    o.SerializerOptions.Converters.Insert(0, new MaskingJsonConverterFactory(builder.Environment));

    // Source-generated JSON serializer for performance
    o.SerializerOptions.TypeInfoResolverChain.Insert(0, MyJsonSerializerContext.Default);
});

// ========================================
// STEP 8: Database Services (Part 4)
// ========================================
builder.AddDatabaseServices((services, factoryRequests) =>
{
    // See Part 4 for complete implementation
    factoryRequests.Add(ContextFactoryRequest.Initialize<MyDbContext>("MyDatabase",
        interceptorFactory: sp => [
            sp.GetRequiredService<AuditSaveChangesInterceptor>()
        ],
        denyCommitRoles: [Role.READONLY, Role.AUDITOR]));
});

// ========================================
// STEP 9: Application Services (Part 5)
// ========================================
builder.AddProjectServices();

// ========================================
// STEP 10: Swagger/OpenAPI Configuration
// ========================================
void OktaSettingsAction(OktaSwaggerConfiguration settings)
{
    builder.Configuration.Bind("Okta", settings);
}

void OktaDocumentSettings(AspNetCoreOpenApiDocumentGeneratorSettings settings)
{
    settings.OperationProcessors.Add(new SwaggerImpersonationHeader());
    settings.OperationProcessors.Add(new SwaggerAuthorizationDetails());
}

// ========================================
// STEP 11: Default Endpoints (Demoulas.Common)
// ========================================
builder.ConfigureDefaultEndpoints(
    meterNames: [],
    activitySourceNames: [MyActivitySource.Instance.Name, "MySolution.Endpoints"])
    .AddSwaggerOpenApi(oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings)
    .AddSwaggerOpenApi(version: 2, oktaSettingsAction: OktaSettingsAction, documentSettingsAction: OktaDocumentSettings);

// ========================================
// STEP 12: Custom Telemetry (Part 5)
// ========================================
builder.Services.AddMyTelemetry(builder.Configuration);

// ========================================
// STEP 13: Health Checks
// ========================================
builder.Services.AddHealthChecks()
    .AddCheck<EnvironmentHealthCheck>("Environment");

builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromMinutes(1);
    options.Period = TimeSpan.FromMinutes(15);
    options.Predicate = _ => true;
});

builder.Services.AddSingleton<IHealthCheckPublisher, HealthCheckResultLogger>();

// ========================================
// STEP 14: Service Provider Validation
// ========================================
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;  // Fail fast on DI configuration errors
});

// ========================================
// BUILD APPLICATION
// ========================================
WebApplication app = builder.Build();

// Initialize metrics (resolve version) then register gauges
GlobalMeter.InitializeFromServices(app.Services);
GlobalMeter.RegisterObservableGauges();
GlobalMeter.RecordDeploymentStartup();

// ========================================
// MIDDLEWARE PIPELINE (CRITICAL ORDER)
// ========================================

// 1. CORS (MUST be first)
app.UseCors();

// 2. No-cache headers for sensitive data
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.Expires = "0";
        return Task.CompletedTask;
    });
    await next();
});

// 3. Custom headers (demographic source tracking)
app.UseDemographicHeaders();

// 4. PII masking middleware
app.UseSensitiveValueMasking();

// 5. Default endpoints (auth, telemetry, compression, etc.)
app.UseDefaultEndpoints(OktaSettingsAction)
    .UseReDoc(settings =>
    {
        settings.Path = "/redoc";
        settings.DocumentPath = "/swagger/Release 1.0/swagger.json";
    });

// 6. CRITICAL: Global endpoint instrumentation (MUST be last before MapEndpoints)
app.UseEndpointInstrumentation();

// 7. Scalar API documentation (alternative to Swagger UI)
OktaSwaggerConfiguration oktaSwaggerConfiguration = OktaSwaggerConfiguration.Empty();
OktaSettingsAction(oktaSwaggerConfiguration);
app.MapScalarApiReference(options =>
{
    options.OpenApiRoutePattern = "/swagger/Release 1.0/swagger.json";
    options.Theme = ScalarTheme.DeepSpace;
});

// ========================================
// RUN APPLICATION
// ========================================
await app.RunAsync();

// Expose Program class for testing
namespace MySolution.Api
{
    public partial class Program { }
}
```

---

## 🚨 CRITICAL: Middleware Ordering

**THIS ORDER IS MANDATORY**. Changing it will break authentication, telemetry, or security.

```
1. UseCors()
   ↓
2. no-cache headers middleware
   ↓
3. UseDemographicHeaders() (custom middleware)
   ↓
4. UseSensitiveValueMasking() (custom middleware)
   ↓
5. UseDefaultEndpoints() (Demoulas.Common - includes auth, telemetry, compression)
   ↓
6. UseEndpointInstrumentation() (custom telemetry middleware - MUST BE LAST)
```

**Why This Order:**

- **CORS first** - CORS headers must be set before any other processing
- **No-cache** - Prevents sensitive data caching before authentication
- **Custom headers** - Track data sources before business logic
- **PII masking** - Mask sensitive data before logging/telemetry
- **Default endpoints** - Includes authentication (must be before instrumentation)
- **Instrumentation last** - Wraps entire request lifecycle for telemetry

---

## 🔐 CORS Configuration Patterns

### Development (Localhost Only)

```csharp
if (builder.Environment.IsDevelopment())
{
    pol.WithOrigins([
        "http://localhost:3100",
        "http://127.0.0.1:3100",
        "https://localhost:3100",
        "https://127.0.0.1:3100"
    ])
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
}
```

**Why Restrict to Localhost:**

- Prevents MITM attacks on shared networks (coffee shops, airports, etc.)
- See ticket PS-2025 for security rationale

### Production (Explicit Whitelist)

```csharp
else
{
    pol.WithOrigins([
        "https://myapp.qa.example.com",
        "https://myapp.uat.example.com",
        "https://myapp.example.com"
    ])
    .AllowAnyMethod()
    .AllowAnyHeader();
}
```

**❌ NEVER USE:**

```csharp
pol.AllowAnyOrigin()  // SECURITY VULNERABILITY - allows any website to call your API
```

---

## 📝 JSON Serialization with PII Masking

### Source-Generated JSON Context

Create `Serialization/MyJsonSerializerContext.cs`:

```csharp
using System.Text.Json.Serialization;
using MySolution.Common.Contracts.Request;
using MySolution.Common.Contracts.Response;

namespace MySolution.Api.Serialization;

[JsonSerializable(typeof(MyRequest))]
[JsonSerializable(typeof(MyResponse))]
[JsonSerializable(typeof(List<MyResponse>))]
// Add all DTOs used in your API
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class MyJsonSerializerContext : JsonSerializerContext
{
}
```

**Why Source Generation:**

- 30-40% faster serialization
- Reduced startup time
- AOT (ahead-of-time) compilation support

### PII Masking Converter

Create `Services/LogMasking/MaskingJsonConverterFactory.cs`:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;

namespace MySolution.Services.LogMasking;

public class MaskingJsonConverterFactory : JsonConverterFactory
{
    private readonly IHostEnvironment _environment;

    public MaskingJsonConverterFactory(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        // Check if type has [MaskSensitive] attribute
        return typeToConvert.GetCustomAttributes(typeof(MaskSensitiveAttribute), true).Any();
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return (JsonConverter)Activator.CreateInstance(
            typeof(MaskingJsonConverter<>).MakeGenericType(typeToConvert),
            _environment)!;
    }
}

public class MaskingJsonConverter<T> : JsonConverter<T>
{
    private readonly IHostEnvironment _environment;

    public MaskingJsonConverter(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Deserialize normally
        return JsonSerializer.Deserialize<T>(ref reader, options)!;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        // In test environment, don't mask (for easier debugging)
        if (_environment.EnvironmentName == "Test")
        {
            JsonSerializer.Serialize(writer, value, options);
            return;
        }

        // Mask PII fields before serialization
        var maskedValue = MaskPiiFields(value);
        JsonSerializer.Serialize(writer, maskedValue, options);
    }

    private T MaskPiiFields(T value)
    {
        // Reflection-based PII masking
        // Check for [MaskSensitive] attribute on properties
        var properties = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(MaskSensitiveAttribute), true).Any());

        foreach (var prop in properties)
        {
            if (prop.PropertyType == typeof(string))
            {
                var currentValue = (string?)prop.GetValue(value);
                if (!string.IsNullOrEmpty(currentValue))
                {
                    var maskedValue = MaskValue(currentValue, prop.Name);
                    prop.SetValue(value, maskedValue);
                }
            }
        }

        return value;
    }

    private static string MaskValue(string value, string fieldName)
    {
        // SSN: 123-45-6789 -> ***-**-6789
        if (fieldName.Contains("Ssn", StringComparison.OrdinalIgnoreCase))
        {
            return value.Length >= 4 ? "***-**-" + value.Substring(value.Length - 4) : "***";
        }

        // Email: user@domain.com -> u***@d***.com
        if (fieldName.Contains("Email", StringComparison.OrdinalIgnoreCase))
        {
            var parts = value.Split('@');
            if (parts.Length == 2)
            {
                var domainParts = parts[1].Split('.');
                return $"{parts[0][0]}***@{domainParts[0][0]}***.{string.Join(".", domainParts.Skip(1))}";
            }
        }

        // Default: show first/last char only
        return value.Length > 2 ? $"{value[0]}***{value[^1]}" : "***";
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class MaskSensitiveAttribute : Attribute { }
```

**Usage in DTOs:**

```csharp
[MaskSensitive]  // Mask entire class
public class MemberDetails
{
    public int Id { get; set; }

    [MaskSensitive]  // Mask individual property
    public string Ssn { get; set; }

    [MaskSensitive]
    public string Email { get; set; }

    public string FirstName { get; set; }  // Not masked
}
```

---

## ✅ Validation Checklist - Part 3

- [ ] **API project created** with correct packages
- [ ] **Program.cs** follows complete template
- [ ] **Middleware ordering** matches CRITICAL sequence
- [ ] **CORS** configured (localhost dev, whitelist prod)
- [ ] **JSON serialization** includes PII masking converter
- [ ] **Source-generated JSON context** created
- [ ] **Logging** configured with PII masking operators
- [ ] **Health checks** registered
- [ ] **Kestrel timeouts** configured (dev only)
- [ ] **aspire run** starts API successfully
- [ ] **Swagger UI** accessible at /swagger

---

## 🎓 Key Takeaways - Part 3

1. **Middleware Order** - MUST follow exact sequence for security
2. **CORS Restrictions** - No AllowAnyOrigin(), explicit whitelists only
3. **PII Masking** - Automatic masking via JSON converter
4. **Source Generation** - Performance optimization for JSON serialization
5. **Demoulas.Common** - ConfigureDefaultEndpoints provides 80% of setup

---

**Next:** [Part 4: Database & EF Core](./04-database-efcore.md) - DatabaseServicesExtension, interceptors, ContextFactoryRequest pattern
