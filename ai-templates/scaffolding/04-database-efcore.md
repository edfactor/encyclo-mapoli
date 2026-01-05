# Part 4: Database & EF Core

**Estimated Time:** 25-30 minutes  
**Prerequisites:** [Part 3 Complete](./03-api-bootstrap-middleware.md)  
**Next:** [Part 5: Extension Methods](./05-extension-methods.md)

---

## 🎯 Overview

Database configuration is handled by `DatabaseServicesExtension` with:

- **ContextFactoryRequest Pattern** - Multi-tenant context registration
- **Interceptor Ordering** - CRITICAL: HttpContextAccessor before AuditSaveChangesInterceptor
- **Read-Only Context** - Performance optimization for queries
- **Pagination Options** - Oracle window function optimization
- **Role-Based Commit Denial** - Prevent read-only users from writing

---

## 📦 Data Project Setup

### 1. Create Data Project

```powershell
cd src
dotnet new classlib -n MySolution.Data
```

### 2. Data .csproj Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- Demoulas Common -->
    <PackageReference Include="Demoulas.Common.Data" />

    <!-- EF Core -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
    <PackageReference Include="Oracle.EntityFrameworkCore" />

    <!-- Design-Time Tools -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Abstractions" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MySolution.Common\MySolution.Common.csproj" />
  </ItemGroup>
</Project>
```

---

## 🔧 NoOp Service Pattern for Optional Dependencies (NEW)

**Use Case:** When a service depends on an optional external resource (e.g., CommonDatabase for store validation), provide a no-op fallback implementation that allows graceful degradation.

### Example: NoOpStoreCacheService

```csharp
namespace MySolution.Data.Cli;

/// <summary>
/// No-op implementation of IStoreCacheService used when CommonDatabase connection is not configured.
/// Skips store validation during seeding operations.
/// </summary>
internal sealed class NoOpStoreCacheService : IStoreCacheService
{
    public Task<bool> CheckIfStoreExistsAsync(int storeNumber, CancellationToken cancellationToken = default)
    {
        // Always return true to skip validation
        return Task.FromResult(true);
    }

    public Task<ISet<StoreCacheObject>> GetAllStoresAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ISet<StoreCacheObject>>(new HashSet<StoreCacheObject>());
    }

    public Task ValidateStoreNumberAsync(int storeNumber, CancellationToken cancellationToken = default)
    {
        // No-op - skip validation
        return Task.CompletedTask;
    }

    public Task<string> GetDisplayNameAsync(int storeNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"Store {storeNumber}");
    }
}
```

### Usage in CLI/Service Registration

```csharp
// Get common database connection string (optional for some operations)
var commonConnectionString = builder.Configuration.GetConnectionString("CommonDatabase");
var hasCommonDatabase = !string.IsNullOrEmpty(commonConnectionString);

if (hasCommonDatabase)
{
    // Register full service with database
    builder.Services.AddDbContext<IDemoulasCommonWarehouseContext, DemoulasCommonWarehouseContext>(options =>
    {
        options.UseOracle(commonConnectionString!);
        options.UseExceptionProcessor();
    });

    builder.Services.AddSingleton<IStoreService, StoreService>();
    builder.Services.AddSingleton<IStoreCacheService, StoreCacheHostedService>();
}
else
{
    // Register no-op fallback
    builder.Services.AddSingleton<IStoreCacheService>(new NoOpStoreCacheService());

    Console.WriteLine("⚠️  WARNING: CommonDatabase connection string not found.");
    Console.WriteLine("    Store validation will be skipped during seeding.");
    Console.WriteLine("    Provide ConnectionStrings:CommonDatabase to enable validation.");
}
```

**Benefits:**

- ✅ Graceful degradation when optional services unavailable
- ✅ Clear warning messages for missing configuration
- ✅ Prevents null reference exceptions
- ✅ Allows development without all dependencies configured
- ✅ Follows null object pattern for cleaner code

---

## 🗂️ Data Project Structure

```
MySolution.Data/
├── Contexts/
│   ├── MyDbContext.cs                      # Main EF Core DbContext
│   ├── MyReadOnlyDbContext.cs              # Read-only context for queries
│   └── EntityMapping/                       # Fluent API configurations
│       ├── MemberMap.cs
│       ├── BeneficiaryMap.cs
│       └── ... (one per entity)
├── Entities/
│   ├── Member.cs
│   ├── Beneficiary.cs
│   ├── Distribution.cs
│   └── ... (all entity classes)
├── Configuration/
│   ├── DataConfig.cs                       # Data configuration settings
│   └── CommitGuardOverride.cs              # Commit guard implementation
├── Extensions/
│   ├── DatabaseServicesExtension.cs        # CRITICAL: Context registration
│   └── ContextExtensions.cs                # Model configuration helper
├── Factories/
│   ├── DataContextFactory.cs               # Context factory implementation
│   └── DesignTimeDbContextFactory.cs       # For migrations (design-time)
├── Interfaces/
│   ├── IMyDataContextFactory.cs            # Context factory interface
│   └── IMyDbContext.cs                     # DbContext interface
└── Migrations/
    └── ... (auto-generated by EF Core)
```

---

## 🔧 Complete DatabaseServicesExtension.cs Template

```csharp
using System.Diagnostics;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.Common.Data.Contexts.Configuration;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Contexts.Interceptor;
using Demoulas.Common.Data.Contexts.Interfaces;
using MySolution.Data.Configuration;
using MySolution.Data.Factories;
using MySolution.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MySolution.Data.Extensions;

public static class DatabaseServicesExtension
{
    /// <summary>
    /// Adds database services to the specified <see cref="IHostApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder"/> to which the database services will be added.
    /// </param>
    /// <param name="contextFactoryRequests">
    /// A collection of <see cref="ContextFactoryRequest"/> objects used to configure the database context factories.
    /// </param>
    /// <returns>
    /// The updated <see cref="IHostApplicationBuilder"/> with the database services registered.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a service of type <see cref="IMyDataContextFactory"/> is already registered in the service collection.
    /// </exception>
    public static IHostApplicationBuilder AddDatabaseServices(this IHostApplicationBuilder builder, Action<IServiceCollection,
        List<ContextFactoryRequest>>? contextFactoryRequests)
    {
        if (builder.Services.Any(s => s.ServiceType == typeof(IMyDataContextFactory)))
        {
            throw new InvalidOperationException($"Service type {typeof(IMyDataContextFactory).FullName} is already registered.");
        }

        _ = builder.Services.AddSingleton<DataConfig>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            DataConfig dataConfig = new DataConfig();
            config.Bind("DataConfig", dataConfig);
            return dataConfig;
        });

        // Must be registered BEFORE AuditSaveChangesInterceptor (which depends on it)
        _ = builder.Services.AddHttpContextAccessor();

        _ = builder.Services.AddScoped<AuditSaveChangesInterceptor>(provider =>
        {
            var user = provider.GetService<IAppUser>();
            var dataConfig = provider.GetRequiredService<DataConfig>();
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            return new AuditSaveChangesInterceptor(dataConfig, user, httpContextAccessor);
        });

        _ = builder.Services.AddSingleton<ICommitGuardOverride, CommitGuardOverride>();

        List<ContextFactoryRequest> factoryRequests = new();
        contextFactoryRequests?.Invoke(builder.Services, factoryRequests);
        IMyDataContextFactory factory = DataContextFactory.Initialize(builder, factoryRequests);

        _ = builder.Services.AddSingleton(factory);

        builder.Services.AddSingleton(new PaginationOptions
        {
            EnableOracleWindowFunctionOptimization = true,
            MaxComplexityScoreForOptimization = 6,
            FallbackOnError = !Debugger.IsAttached
        });

        return builder;
    }
}
```

---

## 🏗️ ContextFactoryRequest Pattern

### Usage in API Program.cs

```csharp
using MySolution.Common.Interceptors;
using MySolution.Security;

builder.AddDatabaseServices((services, factoryRequests) =>
{
    // Primary database context
    factoryRequests.Add(ContextFactoryRequest.Initialize<MyDbContext>(
        connectionStringName: "MyDatabase",
        interceptorFactory: sp => [
            sp.GetRequiredService<AuditSaveChangesInterceptor>(),
            sp.GetRequiredService<TenantIdInterceptor>()  // If multi-tenant
        ],
        denyCommitRoles: [Role.READONLY, Role.AUDITOR]));

    // Warehouse context (different database)
    factoryRequests.Add(ContextFactoryRequest.Initialize<WarehouseDbContext>(
        connectionStringName: "Warehouse",
        interceptorFactory: sp => [
            sp.GetRequiredService<AuditSaveChangesInterceptor>()
        ],
        denyCommitRoles: [Role.READONLY]));
});
```

### ContextFactoryRequest.Initialize Parameters

```csharp
public static ContextFactoryRequest Initialize<TContext>(
    string connectionStringName,
    Func<IServiceProvider, IInterceptor[]>? interceptorFactory = null,
    string[]? denyCommitRoles = null)
    where TContext : DbContext
{
    return new ContextFactoryRequest
    {
        ContextType = typeof(TContext),
        ConnectionStringName = connectionStringName,
        InterceptorFactory = interceptorFactory,
        DenyCommitRoles = denyCommitRoles ?? []
    };
}
```

---

## 🔧 DbContext Implementation

### MyDbContext.cs (Write Context)

```csharp
using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.Smart.MyApp.Data.Entities;
using Demoulas.Smart.MyApp.Data.Extensions;
using Demoulas.Smart.MyApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.Smart.MyApp.Data.Contexts;

public class MyDbContext : OracleDbContext<MyDbContext>, IMyDbContext
{
    public MyDbContext()
    {
        // Used for Unit testing/Mocking only
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    // DbSets
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
    public virtual DbSet<Distribution> Distributions { get; set; }
    // ... more DbSets

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyModelConfiguration();
    }
}
```

### MyReadOnlyDbContext.cs (Read-Only Context)

```csharp
using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.Smart.MyApp.Data.Entities;
using Demoulas.Smart.MyApp.Data.Extensions;
using Demoulas.Smart.MyApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.Smart.MyApp.Data.Contexts;

public class MyReadOnlyDbContext : ReadOnlyOracleDbContext<MyReadOnlyDbContext>, IMyDbContext
{
    public MyReadOnlyDbContext()
    {
        // Used for Unit testing/Mocking only
    }

    public MyReadOnlyDbContext(DbContextOptions<MyReadOnlyDbContext> options)
        : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    // DbSets (same as write context)
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
    public virtual DbSet<Distribution> Distributions { get; set; }
    // ... more DbSets

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyModelConfiguration();
    }
}
```

---

## 🏭 Context Factory Interface

### IMyDbContext.cs (Interface)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySolution.Data.Entities;

namespace MySolution.Data.Interfaces;

public interface IMyDbContext
{
    DbSet<Member> Members { get; set; }
    DbSet<Beneficiary> Beneficiaries { get; set; }
    DbSet<Distribution> Distributions { get; set; }
    // ... more DbSets

    DatabaseFacade Database { get; }
}
```

### IMyDataContextFactory.cs

```csharp
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Entities.Contexts;
using MySolution.Data.Contexts;

namespace MySolution.Data.Interfaces;

public interface IMyDataContextFactory : IDataContextFactory<MyDbContext, MyReadOnlyDbContext>
{
    Task<T> UseWarehouseContext<T>(Func<DemoulasCommonWarehouseContext, Task<T>> func);
}
```

### DataContextFactory.cs (Implementation)

```csharp
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Contexts.Factory;
using Demoulas.Common.Data.Services.Entities.Contexts;
using MySolution.Data.Contexts;
using MySolution.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MySolution.Data.Factories;

public sealed class DataContextFactory : DataContextFactoryBase<MyDbContext, MyReadOnlyDbContext>, IMyDataContextFactory
{
    private DataContextFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    public static IMyDataContextFactory Initialize(IHostApplicationBuilder builder, IEnumerable<ContextFactoryRequest> contextFactoryRequests)
    {
        var serviceCollection = InitializeContexts(builder, contextFactoryRequests);

        return new DataContextFactory(serviceCollection.BuildServiceProvider());
    }

    /// <summary>
    /// Context to access Warehouse related data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<T> UseWarehouseContext<T>(Func<DemoulasCommonWarehouseContext, Task<T>> func)
    {
        using (Logger.BeginScope("Warehouse DB Operation"))
        {
            await using AsyncServiceScope scope = ServiceProvider.CreateAsyncScope();
            DemoulasCommonWarehouseContext dbContext = scope.ServiceProvider.GetRequiredService<DemoulasCommonWarehouseContext>();
            return await func(dbContext);
        }
    }
}
```

---

## 🔍 Read-Only Context Pattern (CRITICAL)

### Usage in Services

```csharp
public class MemberService : IMemberService
{
    private readonly IMyDataContextFactory _factory;

    public MemberService(IMyDataContextFactory factory)
    {
        _factory = factory;
    }

    // Query operation - use read-only context
    public async Task<Result<MemberDto>> GetMemberAsync(int id, CancellationToken ct)
    {
        var member = await _factory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.Members
                .TagWith($"GetMember-{id}")
                .FirstOrDefaultAsync(m => m.Id == id, ct);
        });

        return member is null
            ? Result<MemberDto>.Failure(Error.MemberNotFound)
            : Result<MemberDto>.Success(member.ToDto());
    }

    // Write operation - use write context
    public async Task<Result<int>> CreateMemberAsync(CreateMemberRequest request, CancellationToken ct)
    {
        return await _factory.UseContext(async ctx =>
        {
            var member = new Member
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                // ... more properties
            };

            ctx.Members.Add(member);
            await ctx.SaveChangesAsync(ct);

            return Result<int>.Success(member.Id);
        });
    }
}
```

**Why UseReadOnlyContext():**

- 30-40% faster queries (no change tracking overhead)
- Prevents accidental modifications
- Auto-applies `AsNoTracking()` globally
- **DO NOT** add `.AsNoTracking()` manually when using `UseReadOnlyContext()` (redundant)

---

## 🚨 CRITICAL: Interceptor Ordering

### Wrong Order (FAILS)

```csharp
// ❌ WRONG: AuditSaveChangesInterceptor registered BEFORE HttpContextAccessor
services.AddScoped<AuditSaveChangesInterceptor>();
services.AddHttpContextAccessor();  // TOO LATE - interceptor already tries to use it
```

**Error:**

```
System.InvalidOperationException: Unable to resolve service for type 'Microsoft.AspNetCore.Http.IHttpContextAccessor'
```

### Correct Order (WORKS)

```csharp
// ✅ RIGHT: HttpContextAccessor registered BEFORE interceptor
services.AddHttpContextAccessor();
services.AddScoped<AuditSaveChangesInterceptor>();
```

**Why:** `AuditSaveChangesInterceptor` constructor injects `IHttpContextAccessor` to get user info. If `IHttpContextAccessor` isn't registered first, DI fails.

---

## ⚙️ Pagination Options for Oracle

```csharp
services.Configure<PaginationOptions>(options =>
{
    // Enable Oracle window function optimization
    // Uses ROW_NUMBER() OVER (ORDER BY ...) for efficient pagination
    options.EnableOracleWindowFunctionOptimization = true;

    // Complexity threshold (0-10 scale)
    // Queries with complexity > 6 use standard Skip/Take
    options.MaxComplexityScoreForOptimization = 6;

    // Fail fast vs graceful degradation
    // In dev: false (throw exception to alert developers)
    // In prod: true (fall back to Skip/Take if window function fails)
    options.FallbackOnError = !Debugger.IsAttached;
});
```

**Why Oracle Optimization:**

- Oracle performs poorly with `OFFSET/FETCH` on large datasets
- Window functions (`ROW_NUMBER()`) are significantly faster
- Complexity score prevents over-optimization on complex queries

---

## 🔐 Role-Based Commit Denial

### Configuration

```csharp
factoryRequests.Add(ContextFactoryRequest.Initialize<MyDbContext>(
    connectionStringName: "MyDatabase",
    interceptorFactory: sp => [sp.GetRequiredService<AuditSaveChangesInterceptor>()],
    denyCommitRoles: [Role.READONLY, Role.AUDITOR]));  // ← Deny commits for these roles
```

### How It Works

When user with READONLY role tries to save:

```csharp
await ctx.SaveChangesAsync(ct);  // Throws exception
```

**Exception:**

```
System.UnauthorizedAccessException: User with role 'READONLY' is not authorized to commit changes.
```

**Implementation** (in Demoulas.Common):

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var userRoles = _httpContextAccessor.HttpContext?.User.Claims
        .Where(c => c.Type == ClaimTypes.Role)
        .Select(c => c.Value)
        .ToArray() ?? [];

    if (_denyCommitRoles.Any(deniedRole => userRoles.Contains(deniedRole)))
    {
        throw new UnauthorizedAccessException($"User with role(s) '{string.Join(", ", userRoles)}' is not authorized to commit changes.");
    }

    return await base.SaveChangesAsync(cancellationToken);
}
```

---

## �️ ContextExtensions (Model Configuration)

### ContextExtensions.cs

```csharp
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MySolution.Data.Extensions;

internal static class ContextExtensions
{
    public static ModelBuilder ApplyModelConfiguration(this ModelBuilder modelBuilder)
    {
        // Apply all entity configurations from the assembly
        Assembly assemblyWithConfigurations = typeof(ContextExtensions).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);

        // Force table names to be upper case for consistency with all existing DSM projects
        foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
        {
            // Set table name to upper case
            entity.SetTableName(entity.GetTableName()?.ToUpper());
        }

        // Set the global delete behavior to NoAction for all relationships
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        return modelBuilder;
    }
}
```

---

## 🛠️ Design-Time DbContext Factory (for Migrations)

### DesignTimeDbContextFactory.cs

```csharp
using MySolution.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

//When running migrations or using design-time tools, Entity Framework will need to be able to create an instance of this DbContext without going through the normal startup process
namespace MySolution.Data.Factories;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<MyDbContext> optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        _ = optionsBuilder.UseOracle(builder =>
        {
            builder.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
        });
#if DEBUG
        _ = optionsBuilder.EnableDetailedErrors();
#endif
        _ = optionsBuilder.UseUpperCaseNamingConvention();

        return new MyDbContext(optionsBuilder.Options);
    }
}
```

---

## 📝 Configuration Classes

### DataConfig.cs

```csharp
namespace MySolution.Data.Configuration;

public class DataConfig
{
    public bool EnableAuditLogging { get; set; } = true;
    public string? AuditTableName { get; set; }
}
```

### CommitGuardOverride.cs

```csharp
using Demoulas.Common.Data.Contexts.Interfaces;

namespace MySolution.Data.Configuration;

public class CommitGuardOverride : ICommitGuardOverride
{
    public bool ShouldAllowCommit()
    {
        return true;
    }
}
```

---

## 🗄️ Entity Configuration Example

### MemberMap.cs (Fluent API Configuration)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Data.Entities;

namespace MySolution.Data.Contexts.EntityMapping;

internal sealed class MemberMap : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("MEMBERS");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.FirstName)
            .HasColumnName("FIRST_NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.LastName)
            .HasColumnName("LAST_NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Ssn)
            .HasColumnName("SSN")
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(m => m.BadgeNumber)
            .HasColumnName("BADGE_NUMBER")
            .IsRequired();

        // Indexes
        builder.HasIndex(m => m.Ssn).HasDatabaseName("IX_MEMBERS_SSN");
        builder.HasIndex(m => m.BadgeNumber).HasDatabaseName("IX_MEMBERS_BADGE_NUMBER");

        // Relationships
        builder.HasMany(m => m.Distributions)
            .WithOne(d => d.Member)
            .HasForeignKey(d => d.MemberId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
```

---

## �🗄️ Entity Configuration Example

### MemberConfiguration.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySolution.Data.Entities;

namespace MySolution.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("MEMBERS", "MYAPP");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.FirstName)
            .HasColumnName("FIRST_NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.LastName)
            .HasColumnName("LAST_NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Ssn)
            .HasColumnName("SSN")
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(m => m.BadgeNumber)
            .HasColumnName("BADGE_NUMBER")
            .IsRequired();

        // Indexes
        builder.HasIndex(m => m.Ssn).HasDatabaseName("IX_MEMBERS_SSN");
        builder.HasIndex(m => m.BadgeNumber).HasDatabaseName("IX_MEMBERS_BADGE_NUMBER");

        // Relationships
        builder.HasMany(m => m.Distributions)
            .WithOne(d => d.Member)
            .HasForeignKey(d => d.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

## 🛠️ Design-Time DbContext Factory (for Migrations)

### DesignTimeDbContextFactory.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MySolution.Data.Contexts;

namespace MySolution.Data;

/// <summary>
/// Used by EF Core tooling (dotnet ef migrations add/update).
/// Allows migrations without running the full application.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false)
            .AddUserSecrets<DesignTimeDbContextFactory>()  // Load connection string from user secrets
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        var connectionString = configuration.GetConnectionString("MyDatabase")
            ?? throw new InvalidOperationException("Connection string 'MyDatabase' not found");

        optionsBuilder.UseOracle(connectionString);

        return new MyDbContext(optionsBuilder.Options);
    }
}
```

---

## 🧪 Migration Commands

```powershell
# Add a new migration
cd src/MySolution.Data
dotnet ef migrations add AddMemberTable --context MyDbContext

# Update database to latest migration
dotnet ef database update --context MyDbContext

# Revert to specific migration
dotnet ef database update PreviousMigrationName --context MyDbContext

# Generate SQL script for migration (for DBA review)
dotnet ef migrations script --context MyDbContext --output migration.sql

# Remove last migration (if not applied to database)
dotnet ef migrations remove --context MyDbContext
```

---

## ✅ Validation Checklist - Part 4

- [ ] **Data project created** with EF Core packages
- [ ] **DatabaseServicesExtension.cs** implemented with CRITICAL ordering
- [ ] **HttpContextAccessor** registered BEFORE interceptors
- [ ] **DbContext** inherits from `DeemDbContext`
- [ ] **ContextFactoryRequest.Initialize** used in API Program.cs
- [ ] **Read-only context pattern** understood (`UseReadOnlyContext()`)
- [ ] **Entity configurations** created (one per entity)
- [ ] **PaginationOptions** configured for Oracle optimization
- [ ] **Role-based commit denial** configured
- [ ] **Design-time factory** created for migrations
- [ ] **First migration** created and applied successfully

---

## 🎓 Key Takeaways - Part 4

1. **Interceptor Ordering** - HttpContextAccessor MUST be registered before interceptors that use it
2. **Read-Only Context** - Use `UseReadOnlyContext()` for 30-40% faster queries
3. **ContextFactoryRequest Pattern** - Clean registration of multiple contexts
4. **Oracle Optimization** - Window functions for efficient pagination
5. **Role-Based Security** - Prevent read-only users from committing changes

---

**Next:** [Part 5: Extension Methods](./05-extension-methods.md) - AddProjectServices, AddSecurityServices, AddProfitSharingTelemetry patterns
