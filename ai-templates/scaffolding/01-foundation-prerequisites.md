# Part 1: Foundation & Prerequisites

**Estimated Time:** 15-20 minutes  
**Prerequisites:** Windows/macOS/Linux with .NET SDK  
**Next:** [Part 2: Aspire Orchestration](./02-aspire-orchestration.md)

---

## 📋 Prerequisites Checklist

### Required Software

- **.NET SDK 10.0.100** or later
- **Visual Studio 2022 v17.13+** or **Rider 2024.3+** or **VS Code with C# Dev Kit**
- **.NET Aspire CLI** via `aspire install` (NOT the obsolete .NET Aspire workload)
- **Git** for version control
- **PowerShell 7+** for automation scripts

### Verification Commands

```powershell
# Verify .NET SDK version
dotnet --version
# Should output: 10.0.100 or higher

# Verify Aspire CLI
aspire --version
# Should output: Aspire CLI version info

# Verify PowerShell version
$PSVersionTable.PSVersion
# Should output: 7.0 or higher
```

### Required NuGet Package Sources

Ensure these NuGet sources are configured in `NuGet.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="Demoulas Private" value="[YOUR-PRIVATE-FEED-URL]" protocolVersion="3" />
  </packageSources>
  <packageSourceCredentials>
    <Demoulas_x0020_Private>
      <add key="Username" value="[USERNAME]" />
      <add key="ClearTextPassword" value="[PAT-TOKEN]" />
    </Demoulas_x0020_Private>
  </packageSourceCredentials>
</configuration>
```

**Note:** If you don't have access to the Demoulas private feed, you'll need to implement certain patterns manually (see "Without Demoulas.Common" sections in later parts).

---

## 🔑 Critical Demoulas.Common Library Dependency

This scaffolding assumes you have access to **Demoulas.Common** library (v1.0+) which provides:

| Package                         | Purpose            | Key Features                                                                  |
| ------------------------------- | ------------------ | ----------------------------------------------------------------------------- |
| `Demoulas.Common.Api`           | API infrastructure | Default endpoint configuration, middleware, telemetry, CORS, security headers |
| `Demoulas.Common.Data.Contexts` | Database patterns  | DbContext factory, connection management, health checks, retry policies       |
| `Demoulas.Common.Logging`       | Structured logging | Serilog setup, PII masking operators, correlation IDs                         |
| `Demoulas.Common.Contracts`     | Shared types       | Result<T>, Error types, DTOs                                                  |

### Without Demoulas.Common?

If you don't have access to Demoulas.Common, you'll need to implement:

- Custom `ConfigureDefaultEndpoints()` extension (see Part 3)
- Manual DbContext factory registration (see Part 4)
- Serilog setup with PII masking (see Part 3)
- Result<T> pattern implementation (see Part 5)

**Recommendation:** Request access to Demoulas.Common packages to save ~40 hours of infrastructure work.

---

## 🏗️ Solution Structure Template

### Recommended Project Organization

```
MySolution/
├── src/
│   ├── MySolution.AppHost/                    # Aspire orchestrator
│   │   ├── Program.cs                         # Orchestration entry point (Part 2)
│   │   └── MySolution.AppHost.csproj
│   │
│   ├── MySolution.ServiceDefaults/            # Shared Aspire defaults (optional)
│   │   ├── Extensions.cs                      # Common telemetry/health setup
│   │   └── MySolution.ServiceDefaults.csproj
│   │
│   ├── MySolution.Api/                        # Web API project
│   │   ├── Program.cs                         # API bootstrap (Part 3)
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.QA.json
│   │   ├── appsettings.Production.json
│   │   ├── MySolution.Api.csproj
│   │   └── Serialization/                     # Source-generated JSON contexts
│   │       └── MyJsonSerializerContext.cs
│   │
│   ├── MySolution.Endpoints/                  # FastEndpoints definitions
│   │   ├── Base/                              # Base endpoint classes
│   │   │   └── MyEndpointBase.cs
│   │   ├── Endpoints/                         # Feature endpoints
│   │   │   ├── MyFeature/
│   │   │   │   ├── CreateEndpoint.cs
│   │   │   │   ├── UpdateEndpoint.cs
│   │   │   │   └── GetEndpoint.cs
│   │   ├── Extensions/                        # TelemetryExtensions, etc.
│   │   │   └── TelemetryExtensions.cs
│   │   ├── Groups/                            # Route groups
│   │   │   └── MyFeatureGroup.cs
│   │   ├── Processors/                        # Global processors
│   │   │   └── TelemetryProcessor.cs
│   │   ├── Validation/                        # FluentValidation validators
│   │   │   └── MyRequestValidator.cs
│   │   └── MySolution.Endpoints.csproj
│   │
│   ├── MySolution.Services/                   # Business logic
│   │   ├── Extensions/                        # Service registration (Part 5)
│   │   │   └── ServicesExtension.cs
│   │   ├── Implementations/                   # Service implementations
│   │   │   └── MyFeatureService.cs
│   │   ├── Interfaces/                        # Service contracts
│   │   │   └── IMyFeatureService.cs
│   │   ├── LogMasking/                        # Custom PII masking
│   │   │   └── MaskingOperators.cs
│   │   ├── Middleware/                        # Custom middleware (Part 5)
│   │   │   └── EndpointInstrumentationMiddleware.cs
│   │   └── MySolution.Services.csproj
│   │
│   ├── MySolution.Data/                       # EF Core data layer
│   │   ├── Contexts/                          # DbContext classes
│   │   │   ├── MyDbContext.cs
│   │   │   └── MyReadOnlyDbContext.cs
│   │   ├── Entities/                          # Entity models
│   │   │   └── MyEntity.cs
│   │   ├── Extensions/                        # Database service registration (Part 4)
│   │   │   └── DatabaseServicesExtension.cs
│   │   ├── Interceptors/                      # SaveChanges interceptors
│   │   │   └── AuditSaveChangesInterceptor.cs
│   │   ├── Migrations/                        # EF Core migrations
│   │   └── MySolution.Data.csproj
│   │
│   ├── MySolution.Common/                     # Shared contracts
│   │   ├── Contracts/                         # DTOs, Result<T>
│   │   │   ├── Request/
│   │   │   ├── Response/
│   │   │   └── Result.cs
│   │   ├── ActivitySources/                   # Custom OpenTelemetry sources
│   │   │   └── MyActivitySource.cs
│   │   ├── Metrics/                           # Custom metrics
│   │   │   └── MyMetrics.cs
│   │   └── MySolution.Common.csproj
│   │
│   └── MySolution.Security/                   # Auth/authz
│       ├── Extensions/                        # Security setup (Part 5)
│       │   └── SecurityExtension.cs
│       ├── Policies/                          # Authorization policies
│       │   └── PolicyRoleMap.cs
│       └── MySolution.Security.csproj
│
├── tests/
│   ├── MySolution.UnitTests/                  # Functional unit tests
│   │   ├── Services/
│   │   ├── Endpoints/
│   │   └── MySolution.UnitTests.csproj
│   │
│   ├── MySolution.UnitTests.Architecture/     # Architecture/analyzer tests (Part 6)
│   │   ├── ArchitectureTests.cs
│   │   └── MySolution.UnitTests.Architecture.csproj
│   │
│   └── MySolution.IntegrationTests/           # Integration tests
│       ├── Fixtures/
│       └── MySolution.IntegrationTests.csproj
│
├── MySolution.slnx                            # Solution file (XML format)
├── Directory.Build.props                      # Shared build properties (see below)
├── Directory.Packages.props                   # Central package management (see below)
├── global.json                                # SDK version pinning (see below)
├── NuGet.config                               # Package sources (see above)
└── .editorconfig                              # Code style rules
```

### Why This Structure?

1. **AppHost Orchestration** - Aspire manages all resources (databases, message queues, etc.)
2. **Clean Architecture** - Clear separation: Endpoints → Services → Data
3. **Testability** - Separate test projects for unit, integration, and architecture tests
4. **Shared Contracts** - Common project prevents circular dependencies
5. **Security Isolation** - Separate project for auth/authz concerns

**Note on Project Naming:** Some projects may have directory names that differ from their `.csproj` file names due to refactoring. For example, `MySolution.Endpoints.Contracts/` directory might contain `MySolution.Common.csproj`. This is acceptable - the .csproj name takes precedence for project references.

---

## 📦 Central Package Management Setup

### Directory.Packages.props Template

Create at solution root to enable central package version management:

```xml
<Project>
  <PropertyGroup>
    <!-- CRITICAL: Enable central package management -->
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>
  </PropertyGroup>

  <ItemGroup>
    <!-- Aspire (orchestration and service discovery) -->
    <PackageVersion Include="Aspire.Hosting.AppHost" Version="13.1.0" />
    <PackageVersion Include="Aspire.Hosting.Oracle" Version="13.1.0" />
    <PackageVersion Include="Aspire.Hosting.RabbitMQ" Version="13.1.0" />
    <PackageVersion Include="Aspire.Hosting.Redis" Version="13.1.0" />
    <PackageVersion Include="Aspire.Oracle.EntityFrameworkCore" Version="13.1.0" />

    <!-- Demoulas Common (CRITICAL for standard infrastructure) -->
    <PackageVersion Include="Demoulas.Common.Api" Version="1.0.0" />
    <PackageVersion Include="Demoulas.Common.Contracts" Version="1.0.0" />
    <PackageVersion Include="Demoulas.Common.Data.Contexts" Version="1.0.0" />
    <PackageVersion Include="Demoulas.Common.Logging" Version="1.0.0" />

    <!-- FastEndpoints (lightweight API framework) -->
    <PackageVersion Include="FastEndpoints" Version="7.1.1" />
    <PackageVersion Include="FastEndpoints.Swagger" Version="7.1.1" />

    <!-- EF Core (database access) -->
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.1" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.1" />
    <PackageVersion Include="Oracle.EntityFrameworkCore" Version="10.1.10" />

    <!-- Authentication -->
    <PackageVersion Include="Okta.AspNetCore" Version="5.0.0" />

    <!-- OpenTelemetry (observability) -->
    <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.10.0" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.10.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.10.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.10.0" />

    <!-- Caching -->
    <PackageVersion Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="10.0.0" />

    <!-- Testing -->
    <PackageVersion Include="xunit" Version="3.0.0" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="3.0.0" />
    <PackageVersion Include="Microsoft.Testing.Platform.MSBuild" Version="1.6.1" />
    <PackageVersion Include="Shouldly" Version="4.2.1" />
    <PackageVersion Include="NSubstitute" Version="5.1.0" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />

    <!-- Architecture Testing -->
    <PackageVersion Include="TngTech.ArchUnitNET" Version="0.13.1" />
    <PackageVersion Include="TngTech.ArchUnitNET.xUnit" Version="0.13.1" />

    <!-- Security -->
    <PackageVersion Include="SecurityCodeScan.VS2019" Version="5.6.7" />

    <!-- Validation -->
    <PackageVersion Include="FluentValidation" Version="11.11.0" />
    <PackageVersion Include="FluentValidation.DependencyInjectionExtensions" Version="11.11.0" />

    <!-- CSV Export -->
    <PackageVersion Include="CsvHelper" Version="33.0.1" />

    <!-- Swagger/OpenAPI -->
    <PackageVersion Include="NSwag.AspNetCore" Version="14.2.0" />
    <PackageVersion Include="Scalar.AspNetCore" Version="1.2.49" />

    <!-- PDF Generation (if needed) -->
    <PackageVersion Include="QuestPDF" Version="2024.12.3" />
  </ItemGroup>
</Project>
```

**Critical Notes:**

- All projects use `<PackageReference Include="..." />` WITHOUT version attributes
- Versions defined ONCE in `Directory.Packages.props`
- `CentralPackageTransitivePinningEnabled=true` prevents transitive dependency conflicts
- Update versions here to cascade to all projects

**Example Project File (.csproj):**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <!-- NO VERSION SPECIFIED - comes from Directory.Packages.props -->
    <PackageReference Include="FastEndpoints" />
    <PackageReference Include="FluentValidation" />
  </ItemGroup>
</Project>
```

---

## 🔧 Directory.Build.props Template

Create at solution root for shared build configuration:

```xml
<Project>
  <PropertyGroup>
    <!-- Target Framework -->
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>

    <!-- Performance Optimizations (CRITICAL for production) -->
    <ServerGarbageCollection>true</ServerGarbageCollection>
    <ConcurrentGarbageCollection>true</ConcurrentGarbageCollection>
    <TieredCompilation>true</TieredCompilation>
    <TieredCompilationQuickJit>true</TieredCompilationQuickJit>
    <PublishReadyToRun>true</PublishReadyToRun>
    <PublishReadyToRunShowWarnings>true</PublishReadyToRunShowWarnings>
    <IlcOptimizationPreference>Speed</IlcOptimizationPreference>

    <!-- Code Analysis -->
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisMode>AllEnabledByDefault</AnalysisMode>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>

    <!-- Security -->
    <EnableDefaultSecurityFrameworkAnalyzers>true</EnableDefaultSecurityFrameworkAnalyzers>

    <!-- Reproducible Builds -->
    <Deterministic>true</Deterministic>
    <ContinuousIntegrationBuild Condition="'$(CI)' == 'true'">true</ContinuousIntegrationBuild>
  </PropertyGroup>

  <!-- Suppressed Warnings (adjust project-specific needs) -->
  <PropertyGroup>
    <!--
      CA1014: Mark assemblies with CLSCompliant (not needed for internal APIs)
      CA1062: Validate arguments of public methods (FastEndpoints handles this)
      CA1307: Specify StringComparison for clarity (covered by analyzers)
      CA1812: Avoid uninstantiated internal classes (DI handles this)
      CA1848: Use LoggerMessage delegates (performance, not critical)
      CA2007: Consider calling ConfigureAwait on awaited task (not needed in ASP.NET Core)
      CA2227: Collection properties should be read only (DTOs need setters)
      CS1591: Missing XML comment for publicly visible type or member (optional)
    -->
    <NoWarn>$(NoWarn);CA1014;CA1062;CA1307;CA1812;CA1848;CA2007;CA2227;CS1591</NoWarn>
  </PropertyGroup>
</Project>
```

**Key Settings Explained:**

| Setting                   | Value | Why                                                              |
| ------------------------- | ----- | ---------------------------------------------------------------- |
| `ServerGarbageCollection` | true  | Uses server GC (better for web apps with sustained throughput)   |
| `PublishReadyToRun`       | true  | AOT compilation for faster startup (critical for Docker)         |
| `TreatWarningsAsErrors`   | true  | Zero-tolerance for warnings (CRITICAL for security/quality)      |
| `TieredCompilation`       | true  | JIT optimization for hot paths (better steady-state performance) |
| `EnableNETAnalyzers`      | true  | Enables all .NET code quality analyzers                          |
| `Deterministic`           | true  | Reproducible builds (same input = same output)                   |

---

## 🎯 global.json - SDK Version Pinning

Create at solution root to ensure consistent SDK across team:

```json
{
  "$schema": "https://json.schemastore.org/global.json",
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  },
  "msbuild-sdks": {
    "Aspire.AppHost.Sdk": "13.1.0"
  }
}
```

**Why Pin SDK Version:**

- Prevents "works on my machine" issues
- Ensures consistent build behavior in CI/CD
- Allows controlled SDK upgrades
- `rollForward: latestFeature` allows patch updates but not major/minor changes

---

## 🔗 Demoulas.Common Integration Patterns

### Required Common Library Methods

If using Demoulas.Common, these extension methods are available:

#### 1. ConfigureDefaultEndpoints (from Demoulas.Common.Api)

```csharp
// In Program.cs (Part 3)
builder.ConfigureDefaultEndpoints(
    meterNames: ["MyApp.Metrics"],
    activitySourceNames: ["MyApp.ActivitySource"]
);
```

**What ConfigureDefaultEndpoints Provides:**

- ✅ Kestrel HTTP/HTTPS configuration
- ✅ Okta JWT authentication
- ✅ OpenTelemetry (metrics + traces)
- ✅ Serilog structured logging
- ✅ HSTS headers (non-dev environments)
- ✅ Security headers via NetEscapades (X-Frame-Options, CSP, etc.)
- ✅ Request/response compression
- ✅ API versioning support
- ✅ FastEndpoints registration with Swagger

**Method Signature:**

```csharp
public static WebApplicationBuilder ConfigureDefaultEndpoints(
    this WebApplicationBuilder builder,
    bool addOktaSecurity = true,
    string[]? meterNames = null,
    string[]? activitySourceNames = null,
    IRolePermissionService? rolePermissionService = null,
    JsonConverter[]? jsonConverters = null)
```

#### 2. AddDatabaseServices (from Demoulas.Common.Data.Contexts)

```csharp
// In Program.cs (Part 4)
builder.AddDatabaseServices((services, contextRequests) =>
{
    contextRequests.Add(ContextFactoryRequest.Initialize<MyDbContext>(
        connectionName: "MyConnection",
        interceptorFactory: sp => [
            sp.GetRequiredService<AuditSaveChangesInterceptor>()
        ],
        denyCommitRoles: ["READONLY", "AUDITOR"]
    ));
});
```

**What AddDatabaseServices Provides:**

- ✅ `IDbContextFactory<T>` registration for each context
- ✅ Connection string management from configuration
- ✅ Health checks for database connectivity
- ✅ Retry policies for transient failures
- ✅ Interceptor pipeline management
- ✅ Role-based commit denial

**Method Signature:**

```csharp
public static WebApplicationBuilder AddDatabaseServices(
    this WebApplicationBuilder builder,
    Action<IServiceCollection, List<ContextFactoryRequest>> configure)
```

#### 3. SetDefaultLoggerConfiguration (from Demoulas.Common.Logging)

```csharp
// In Program.cs (Part 3)
LoggingConfig logConfig = new();
builder.Configuration.Bind("SmartLogging", logConfig);

logConfig.MaskingOperators = [
    new UnformattedSocialSecurityNumberMaskingOperator(),
    new SensitiveValueMaskingOperator()
];

builder.SetDefaultLoggerConfiguration(logConfig);
```

**What SetDefaultLoggerConfiguration Provides:**

- ✅ Serilog with structured logging
- ✅ Console sink (development)
- ✅ File sink (production)
- ✅ PII masking operators
- ✅ Log enrichment with correlation IDs, user info, etc.
- ✅ Configurable log levels per environment

**appsettings.json Configuration:**

```json
{
  "SmartLogging": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

---

## ✅ Validation Checklist - Part 1

Before proceeding to Part 2, verify:

- [ ] **.NET SDK 10.0.100+** installed (`dotnet --version`)
- [ ] **Aspire CLI** installed (`aspire --version`)
- [ ] **PowerShell 7+** installed (`$PSVersionTable.PSVersion`)
- [ ] **NuGet.config** has Demoulas private feed configured (or plan to implement patterns manually)
- [ ] **Directory.Packages.props** created at solution root with all package versions
- [ ] **Directory.Build.props** created with performance flags and analysis settings
- [ ] **global.json** pins SDK to 10.0.100 with Aspire.AppHost.Sdk
- [ ] **Demoulas.Common** packages accessible via NuGet (or manual implementation planned)
- [ ] **Solution structure** documented and directory layout planned
- [ ] **Central package management** enabled (`ManagePackageVersionsCentrally=true`)
- [ ] **Team notified** of new infrastructure standards

---

## 🎓 Key Takeaways - Part 1

1. **Central Package Management** - Single source of truth for all package versions prevents version conflicts
2. **Performance by Default** - ServerGC, ReadyToRun, TieredCompilation in Directory.Build.props
3. **Security by Default** - Warnings as errors, security analyzers enabled, PII masking built-in
4. **Demoulas.Common Integration** - Provides 80% of infrastructure out-of-box with battle-tested patterns
5. **Consistent SDK** - global.json prevents version drift across team and CI/CD

---

## 📚 Additional Resources

- [.NET 10.0 Performance Improvements](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-10/)
- [Central Package Management Docs](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [.NET Aspire Overview](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)

---

**Next:** [Part 2: Aspire Orchestration](./02-aspire-orchestration.md) - Complete AppHost setup with Oracle, RabbitMQ, Redis, and Playwright integration
