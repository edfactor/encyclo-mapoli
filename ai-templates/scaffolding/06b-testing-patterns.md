# Part 6b: Testing Patterns

**Estimated Time:** 30 minutes  
**Prerequisites:** [Part 6a Complete](./06a-health-checks.md)  
**Next:** [Part 6c: Architecture Tests](./06c-architecture-tests.md)

---

## 🎯 Overview

Testing infrastructure follows a three-project structure:

1. **MySolution.UnitTests.Common** - Shared test infrastructure (base classes, mocks, fakers)
2. **MySolution.UnitTests** - Business logic tests (services, controllers, integration tests)
3. **MySolution.UnitTests.Architecture** - Architecture rules validation

**Note:** Older projects may use the naming pattern `MySolution.Api.Tests` - both are acceptable, but `UnitTests` is preferred for new projects.

**Testing Components:**

- **xUnit 3.0** - Modern test framework
- **Microsoft Testing Platform** - Modern test runner
- **MockDataContextFactory** - In-memory database with queryable DbSets
- **ApiTestBase** - Generic base class for API integration tests
- **Bogus** - Fake data generation
- **MockQueryable.Moq** - LINQ-enabled mocked DbSets

---

## 📦 Test Project Setup - Three Projects

### 1. MySolution.UnitTests.Common.csproj

Shared test infrastructure containing base classes, mocks, and fakers.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <!-- xUnit 3.0 + Microsoft Testing Platform -->
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="Microsoft.Testing.Platform" />

    <!-- Mocking and Fake Data -->
    <PackageReference Include="Bogus" />
    <PackageReference Include="MockQueryable.Moq" />
    <PackageReference Include="Moq" />

    <!-- ASP.NET Core Testing -->
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" />

    <!-- Assertion Library: Shouldly is MANDATORY for all business logic tests -->
    <PackageReference Include="Shouldly" />
  </ItemGroup>

  <ItemGroup>
    <!-- Reference to API and Data projects -->
    <ProjectReference Include="..\..\src\MySolution.Api\MySolution.Api.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Data\MySolution.Data.csproj" />
  </ItemGroup>
</Project>
```

**Folder Structure:**

```
MySolution.UnitTests.Common/
├── Base/
│   ├── ApiTestBase.cs
│   └── TestServiceOverrides.cs
├── Mocks/
│   └── MockDataContextFactory.cs
├── Fakes/
│   ├── EntityFaker1.cs
│   ├── EntityFaker2.cs
│   └── DataGenerator.cs
└── TestModuleInitializer.cs
```

### 2. MySolution.UnitTests.csproj

Business logic tests for services, controllers, and API endpoints.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <!-- xUnit 3.0 + Microsoft Testing Platform -->
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="Microsoft.Testing.Platform" />

    <!-- Assertion Library: Shouldly is MANDATORY for all business logic tests -->
    <PackageReference Include="Shouldly" />

    <!-- Test Coverage -->
    <PackageReference Include="coverlet.collector" />
  </ItemGroup>

  <ItemGroup>
    <!-- Reference Common test infrastructure -->
    <ProjectReference Include="..\MySolution.UnitTests.Common\MySolution.UnitTests.Common.csproj" />

    <!-- Reference Services if needed -->
    <ProjectReference Include="..\..\src\MySolution.Services\MySolution.Services.csproj" />
  </ItemGroup>
</Project>
```

### 3. MySolution.UnitTests.Architecture.csproj

Architecture validation tests (see [Part 6c](./06c-architecture-tests.md)).

---

## 🏗️ Test Infrastructure Components

## 🏗️ Test Infrastructure Components

### 1. TestModuleInitializer.cs

Sets the `ASPNETCORE_ENVIRONMENT` before any tests run. Required for xUnit v3 / Microsoft Testing Platform.

```csharp
using System.Runtime.CompilerServices;

namespace MySolution.UnitTests.Common;

/// <summary>
///   Initializes the testing environment before any tests run.
///   Sets ASPNETCORE_ENVIRONMENT to "Testing" for WebApplicationFactory compatibility with xUnit v3.
/// </summary>
/// <remarks>
///   This module initializer is required for the Microsoft Testing Platform (xUnit v3).
///   It ensures the environment variable is set BEFORE the WebApplicationFactory creates the ASP.NET host,
///   preventing runtime configuration errors in integration tests.
/// </remarks>
internal static class TestModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }
}
```

### 2. ApiTestBase.cs

Generic base class for API integration tests using `WebApplicationFactory`. This implementation includes comprehensive service mocking to ensure test isolation.

```csharp
using MySolution.Data.Interfaces;
using MySolution.Common.Interfaces;
using MySolution.UnitTests.Common.Mocks;
using MySolution.UnitTests.Common.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MySolution.UnitTests.Common.Base;

/// <summary>
///   Base class for testing API endpoints with comprehensive service mocking.
///   NOTE: Each test CLASS (not method) gets its own factory instance.
///   Tests within the same [Collection] share the factory, so they may pollute each other's data.
///   Implements IAsyncDisposable to clean up ServiceProvider and HTTP clients after test completion.
/// </summary>
public class ApiTestBase<TStartup> : IAsyncDisposable where TStartup : class
{
    /// <summary>
    ///   Mock data context factory with pre-generated test data.
    ///   Each test class gets a fresh instance, but tests within a class share this factory.
    /// </summary>
    public MockDataContextFactory MockDbContextFactory { get; set; }

    /// <summary>
    ///   Mock cache service for domain-specific caching.
    /// </summary>
    protected readonly Mock<IMyCacheService> MockCacheService = new();

    /// <summary>
    ///   The HTTP client to invoke API endpoints.
    /// </summary>
    public HttpClient ApiClient { get; }

    /// <summary>
    ///   HTTP client configured for file downloads (CSV, etc.).
    /// </summary>
    public HttpClient DownloadClient { get; }

    /// <summary>
    ///   Service provider for accessing registered services in tests.
    /// </summary>
    public ServiceProvider? ServiceProvider { get; private set; }

    /// <summary>
    ///   Virtual method to allow derived classes to specify a custom HTTP client timeout.
    ///   Override this method to return a custom timeout value.
    /// </summary>
    /// <returns>The HTTP client timeout, or null to use the default 2-minute timeout.</returns>
    protected virtual TimeSpan? GetHttpClientTimeout() => null;

    /// <summary>
    ///   Initializes a new instance of <c>ApiTestBase</c> with all required service mocking.
    /// </summary>
    public ApiTestBase()
    {
        // Each test class gets its own FRESH factory with generated fake records
        MockDbContextFactory = MockDataContextFactory.InitializeForTesting();

        // Setup cache service with fake data
        SetupCacheService();

        // Note: ASPNETCORE_ENVIRONMENT is set via TestModuleInitializer to ensure
        // it's configured before any WebApplicationFactory or ASP.NET host is created.
        // This is required for xUnit v3 / Microsoft Testing Platform compatibility.

        WebApplicationFactory<TStartup> webApplicationFactory = new WebApplicationFactory<TStartup>();

        WebApplicationFactory<TStartup> builder = webApplicationFactory.WithWebHostBuilder(
            hostBuilder =>
            {
                // Set environment to Testing for test-specific configurations
                hostBuilder.UseEnvironment("Testing");

                // Provide mock connection string to prevent startup errors
                hostBuilder.UseConfiguration(GetMockConnectionStringConfiguration());

                hostBuilder.ConfigureTestServices(services =>
                {
                    // Replace logger with null logger to reduce test output noise
                    services.AddSingleton<ILoggerFactory, NullLoggerFactory>();

                    // Remove ALL hosted service registrations
                    var hostedServiceDescriptors = services.Where(d =>
                        d.ServiceType == typeof(IHostedService) ||
                        d.ImplementationType?.Name.Contains("HostedService") == true).ToList();
                    foreach (var descriptor in hostedServiceDescriptors)
                    {
                        services.Remove(descriptor);
                    }

                    // Remove any existing context factory registrations
                    var contextFactoryDescriptor = services.FirstOrDefault(d =>
                        d.ServiceType == typeof(IDbContextFactory<MyDbContext>));
                    if (contextFactoryDescriptor != null)
                    {
                        services.Remove(contextFactoryDescriptor);
                    }

                    // Register mock data context factory
                    services.AddSingleton<IMyDataContextFactory>((_) => MockDbContextFactory);
                    services.AddSingleton<IDbContextFactory<MyDbContext>>((_) => MockDbContextFactory);

                    // Register mock cache service
                    services.AddSingleton(MockCacheService.Object);

                    // Mock common data services that might be dependencies
                    var mockCommonService = new Mock<ICommonService>();
                    services.AddScoped<ICommonService>(sp => mockCommonService.Object);

                    // Add test authentication
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "TestScheme";
                        options.DefaultChallengeScheme = "TestScheme";
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TestScheme", options => { });

                    // Allow tests to provide additional service registrations
                    TestServiceOverrides.Hook?.Invoke(services);

                    ServiceProvider = services.BuildServiceProvider();
                });
            });

        ApiClient = builder.CreateClient();
        ApiClient.Timeout = GetHttpClientTimeout() ?? TimeSpan.FromMinutes(2);

        DownloadClient = builder.CreateClient();
        DownloadClient.Timeout = GetHttpClientTimeout() ?? TimeSpan.FromMinutes(2);
        DownloadClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/csv"));
    }

    /// <summary>
    ///   Setup cache service with fake data.
    /// </summary>
    private void SetupCacheService()
    {
        var cacheData = new MyCacheFaker().Generate(100);
        MockCacheService.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(cacheData);
        MockCacheService.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken ct) => cacheData.FirstOrDefault(x => x.Id == id));
    }

    /// <summary>
    ///   Provides mock connection string configuration to prevent startup errors.
    /// </summary>
    private static IConfiguration GetMockConnectionStringConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MyDatabase"] = "Server=localhost;Database=TestDb;User Id=test;Password=test;"
            })
            .Build();
    }

    /// <summary>
    ///   Disposes of resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        ApiClient?.Dispose();
        DownloadClient?.Dispose();
        if (ServiceProvider != null)
        {
            await ServiceProvider.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}
```

### 3. TestAuthenticationHandler.cs

Test authentication handler that always succeeds for integration tests.

```csharp
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MySolution.UnitTests.Common.Base;

/// <summary>
/// Test authentication handler that always authenticates successfully.
/// </summary>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, "TestRole")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

### 4. TestServiceOverrides.cs

Hook for custom service registrations in tests.

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace MySolution.UnitTests.Common.Base;

/// <summary>
///   Static hook to allow individual test classes to register custom services
///   before the WebApplicationFactory builds the ServiceProvider.
/// </summary>
public static class TestServiceOverrides
{
    /// <summary>
    ///   Optional hook that allows test classes to modify the service collection
    ///   before the container is built. Reset to null after each test if needed.
    /// </summary>
    public static Action<IServiceCollection>? Hook { get; set; }
}
```

### 5. MockDataContextFactory.cs

In-memory mock implementation of `IMyDataContextFactory` with queryable DbSets.

```csharp
using MySolution.Data.Contexts;
using MySolution.Data.Entities;
using MySolution.Data.Interfaces;
using MySolution.UnitTests.Common.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using Moq;

namespace MySolution.UnitTests.Common.Mocks;

public sealed class MockDataContextFactory : IMyDataContextFactory
{
    private readonly Mock<MyDbContext> _myDbContext;
    private readonly Mock<MyReadOnlyDbContext> _myReadOnlyDbContext;

    // Backing data lists
    private readonly List<Entity1> _entity1s = [];
    private readonly List<Entity2> _entity2s = [];

    private MockDataContextFactory()
    {
        _myDbContext = new Mock<MyDbContext>();
        _myReadOnlyDbContext = new Mock<MyReadOnlyDbContext>();

        InitializeMockData();
        ConfigureDbContexts();
    }

    public static MockDataContextFactory InitializeForTesting()
    {
        return new MockDataContextFactory();
    }

    private void InitializeMockData()
    {
        // Generate test data using Bogus fakers
        _entity1s.AddRange(Entity1Faker.Generate(count: 100));
        _entity2s.AddRange(Entity2Faker.Generate(count: 200));
    }

    private void ConfigureDbContexts()
    {
        // Configure write context
        ConfigureContext(_myDbContext);

        // Configure read-only context
        ConfigureContext(_myReadOnlyDbContext);

        // Setup SaveChangesAsync for write context
        _myDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private void ConfigureContext<TContext>(Mock<TContext> mockContext) where TContext : class
    {
        // Setup Entity1s
        var mockEntity1s = BuildMockDbSetWithBackingList(_entity1s);
        SetupDbSetProperty(mockContext, "Entity1s", mockEntity1s);

        // Setup Entity2s
        var mockEntity2s = BuildMockDbSetWithBackingList(_entity2s);
        SetupDbSetProperty(mockContext, "Entity2s", mockEntity2s);
    }

    private static Mock<DbSet<T>> BuildMockDbSetWithBackingList<T>(List<T> data) where T : class
    {
        // Start with IQueryable-enabled DbSet backed by the list
        var mockSet = data.BuildMockDbSet();

        // Setup Add to modify backing list
        mockSet.Setup(m => m.Add(It.IsAny<T>())).Returns((T entity) =>
        {
            data.Add(entity);
            var mockEntry = new Mock<EntityEntry<T>>();
            mockEntry.Setup(e => e.Entity).Returns(entity);
            return mockEntry.Object;
        });

        // Setup AddAsync to modify backing list
        mockSet.Setup(m => m.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Returns((T entity, CancellationToken _) =>
            {
                data.Add(entity);
                var mockEntry = new Mock<EntityEntry<T>>();
                mockEntry.Setup(e => e.Entity).Returns(entity);
                return new ValueTask<EntityEntry<T>>(mockEntry.Object);
            });

        // Setup Remove to modify backing list
        mockSet.Setup(m => m.Remove(It.IsAny<T>())).Returns((T entity) =>
        {
            data.Remove(entity);
            var mockEntry = new Mock<EntityEntry<T>>();
            mockEntry.Setup(e => e.Entity).Returns(entity);
            return mockEntry.Object;
        });

        return mockSet;
    }

    private static void SetupDbSetProperty<TContext, TEntity>(
        Mock<TContext> mockContext,
        string propertyName,
        Mock<DbSet<TEntity>> mockDbSet)
        where TContext : class
        where TEntity : class
    {
        mockContext.Setup(ctx =>
            ((dynamic)ctx).GetType().GetProperty(propertyName).GetValue(ctx, null))
            .Returns(mockDbSet.Object);

        mockContext.Setup(ctx =>
            ((dynamic)ctx)[propertyName])
            .Returns(mockDbSet.Object);
    }

    // IMyDataContextFactory implementation
    public MyDbContext CreateDbContext() => _myDbContext.Object;
    public MyReadOnlyDbContext CreateReadOnlyDbContext() => _myReadOnlyDbContext.Object;

    public void Dispose() { }
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
```

### 5. Fakers - Using Bogus Library

Create realistic fake data for testing.

```csharp
using Bogus;
using MySolution.Data.Entities;

namespace MySolution.UnitTests.Common.Fakes;

public static class Entity1Faker
{
    private static readonly Faker<Entity1> _faker = new Faker<Entity1>()
        .RuleFor(e => e.Id, f => f.IndexFaker + 1)
        .RuleFor(e => e.Name, f => f.Company.CompanyName())
        .RuleFor(e => e.Code, f => f.Random.AlphaNumeric(10).ToUpper())
        .RuleFor(e => e.IsActive, f => f.Random.Bool(0.9f))
        .RuleFor(e => e.CreatedDate, f => f.Date.Past(1));

    public static List<Entity1> Generate(int count) => _faker.Generate(count);
}
```

---

## 🧪 Writing Tests

## 🧪 Writing Tests

### Example: Service Test with ApiTestBase

```csharp
using Shouldly;
using MySolution.Api;
using MySolution.UnitTests.Common.Base;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MySolution.UnitTests.Controllers;

/// <summary>
/// Tests for Entity1 API endpoints.
/// Each test class gets its own fresh MockDataContextFactory instance.
/// </summary>
[Collection("Entity1 Tests")]
public class Entity1ControllerTests : ApiTestBase<Program>
{
    [Fact]
    [Description("PS-1234 : GetAll returns list of entities")]
    public async Task GetAll_ReturnsEntities()
    {
        // Arrange - MockDbContextFactory already has fake data from Fakers

        // Act
        var response = await ApiClient.GetAsync("/api/entity1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var entities = await response.Content.ReadFromJsonAsync<List<Entity1Dto>>();
        entities.ShouldNotBeNull();
        entities.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    [Description("PS-1234 : GetById returns specific entity")]
    public async Task GetById_WhenEntityExists_ReturnsEntity()
    {
        // Arrange
        var existingEntity = MockDbContextFactory._entity1s.First();

        // Act
        var response = await ApiClient.GetAsync($"/api/entity1/{existingEntity.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var entity = await response.Content.ReadFromJsonAsync<Entity1Dto>();
        entity.ShouldNotBeNull();
        entity.Id.ShouldBe(existingEntity.Id);
    }

    [Fact]
    [Description("PS-1234 : Create adds new entity")]
    public async Task Create_ValidRequest_AddsEntity()
    {
        // Arrange
        var request = new CreateEntity1Request
        {
            Name = "Test Entity",
            Code = "TEST123"
        };

        // Act
        var response = await ApiClient.PostAsJsonAsync("/api/entity1", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Verify entity was added to mock backing list
        MockDbContextFactory._entity1s
            .ShouldContain(e => e.Name == "Test Entity");
    }
}
```

### Example: Service Test with Custom Overrides

```csharp
using MySolution.Services;
using MySolution.UnitTests.Common.Base;
using MySolution.UnitTests.Common.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MySolution.UnitTests.Services;

[Collection("Service Tests")]
public class CustomServiceTests : ApiTestBase<Program>
{
    public CustomServiceTests()
    {
        // Register custom service mock before tests run
        TestServiceOverrides.Hook = services =>
        {
            var mockExternalService = new Mock<IExternalService>();
            mockExternalService.Setup(x => x.GetDataAsync())
                .ReturnsAsync("Mocked Data");

            services.AddSingleton(mockExternalService.Object);
        };
    }

    [Fact]
    [Description("PS-1234 : Service uses custom mock")]
    public async Task ServiceMethod_UsesCustomMock()
    {
        // Arrange
        var service = ServiceProvider!.GetRequiredService<IMyService>();

        // Act
        var result = await service.ProcessDataAsync();

        // Assert
        result.Should().Contain("Mocked Data");
    }
}
```

---

## 🏗️ Test Collection Definitions

```csharp
namespace MySolution.UnitTests;

/// <summary>
/// Parallel test collections (independent endpoints).
/// Tests in different collections run in parallel.
/// </summary>
[CollectionDefinition("Member Tests", DisableParallelization = false)]
public class MemberTestCollection { }

[CollectionDefinition("Beneficiary Tests", DisableParallelization = false)]
public class BeneficiaryTestCollection { }

[CollectionDefinition("Distribution Tests", DisableParallelization = false)]
public class DistributionTestCollection { }

[CollectionDefinition("Lookup Tests", DisableParallelization = false)]
public class LookupTestCollection { }

/// <summary>
/// Sequential test collections (shared global state).
/// Tests in these collections run one at a time.
/// </summary>
[CollectionDefinition("Database Tests", DisableParallelization = true)]
public class DatabaseTestCollection { }

[CollectionDefinition("Year-End Tests", DisableParallelization = true)]
public class YearEndTestCollection { }

[CollectionDefinition("Integration Tests", DisableParallelization = true)]
public class IntegrationTestCollection { }
```

---

## 🧪 Using Test Collections

### MemberServiceTests.cs

```csharp
using Shouldly;
using MySolution.Services;
using Xunit;

namespace MySolution.UnitTests.Services;

[Collection("Member Tests")]  // ← Runs in parallel with other collections
public class MemberServiceTests
{
    [Fact]
    [Description("PS-1234 : GetMemberById returns member when found")]
    public async Task GetMemberById_WhenMemberExists_ReturnsMember()
    {
        // Arrange
        var service = CreateMemberService();
        var memberId = 1;

        // Act
        var result = await service.GetByIdAsync(memberId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(memberId);
    }

    [Fact]
    [Description("PS-1234 : GetMemberById returns error when not found")]
    public async Task GetMemberById_WhenMemberNotFound_ReturnsError()
    {
        // Arrange
        var service = CreateMemberService();
        var memberId = 99999;

        // Act
        var result = await service.GetByIdAsync(memberId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe("Member.NotFound");
    }

    private IMemberService CreateMemberService()
    {
        // Factory method for service creation with mocked dependencies
        var mockFactory = Substitute.For<IMyDataContextFactory>();
        // ... setup mocks
        return new MemberService(mockFactory);
    }
}
```

---

## 🏭 Test Fixtures

### Database Test Fixture

```csharp
using Microsoft.EntityFrameworkCore;
using MySolution.Data.Contexts;

namespace MySolution.UnitTests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    public MyDbContext Context { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new MyDbContext(options);
        await Context.Database.EnsureCreatedAsync();

        // Seed test data
        await SeedTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
    }

    private async Task SeedTestDataAsync()
    {
        Context.Members.AddRange(
            new Member { Id = 1, FirstName = "John", LastName = "Doe", Ssn = "123456789" },
            new Member { Id = 2, FirstName = "Jane", LastName = "Smith", Ssn = "987654321" }
        );
        await Context.SaveChangesAsync();
    }
}

/// <summary>
/// Collection definition for database tests using fixture.
/// </summary>
[CollectionDefinition("Database Fixture Collection")]
public class DatabaseFixtureCollection : ICollectionFixture<DatabaseFixture> { }
```

### Using Test Fixture

```csharp
[Collection("Database Fixture Collection")]
public class MemberRepositoryTests
{
    private readonly DatabaseFixture _fixture;

    public MemberRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetMembers_ReturnsSeededData()
    {
        // Arrange
        var context = _fixture.Context;

        // Act
        var members = await context.Members.ToListAsync();

        // Assert
        members.Should().HaveCount(2);
    }
}
```

---

## 📝 [Description] Attribute Pattern

```csharp
using System.ComponentModel;

[Fact]
[Description("PS-1234 : Validates SSN format")]
public void ValidateSSN_ValidFormat_ReturnsTrue()
{
    // Test implementation
}
```

**Why:** Links tests to Jira tickets for traceability

---

## 🔐 Testing Authentication (SecurityExtensions)

### Extensions/SecurityExtensions.cs

Create JWT tokens for testing authenticated API calls.

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace MySolution.UnitTests.Common.Extensions;

public static class SecurityExtensions
{
    public static void CreateAndAssignTokenForClient(this HttpClient client, params string[] roles)
    {
#pragma warning disable S6781 // JWT secret keys should not be disclosed
        var securityKey = new SymmetricSecurityKey("abcdefghijklmnopqrstuvwxyz123456"u8.ToArray());
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
#pragma warning restore S6781 // JWT secret keys should not be disclosed

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Unit Test User"),
            new Claim(ClaimTypes.Email, "testuser@company.com")
        };

        foreach (var role in roles)
        {
            // Add as both "groups" claim (for claims transformation) and direct role claim (for authorization)
            claims.Add(new Claim("groups", $"MySolution-Testing-{role}"));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: "Unit Test Issuer",
            audience: "Unit Test Audience",
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(60),
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(token);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
```

**Usage in Tests:**

```csharp
[Fact]
public async Task GetStores_AsAdministrator_ReturnsStores()
{
    // Arrange
    using var client = Factory.CreateClient();
    client.CreateAndAssignTokenForClient("ADMINISTRATOR");

    // Act
    var response = await client.GetAsync("/stores");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
}

[Fact]
public async Task GetStores_AsStoreManager_ReturnsForbidden()
{
    // Arrange
    using var client = Factory.CreateClient();
    client.CreateAndAssignTokenForClient("STORE_MANAGER");

    // Act
    var response = await client.GetAsync("/admin/stores");

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
}
```

**Key Points:**

- ✅ Creates valid JWT tokens with roles
- ✅ Adds both "groups" claim (for transformation) and ClaimTypes.Role
- ✅ Token expires in 60 seconds (prevents token reuse)
- ✅ Uses consistent naming convention: `{ProjectName}-Testing-{Role}`
- ✅ Suppresses S6781 warning (test-only secret is acceptable)

---

## ✅ Validation Checklist - Part 6b

**Three-Project Structure:**

- [ ] **UnitTests.Common** project created with Base/, Mocks/, Fakes/ folders
- [ ] **UnitTests** project created referencing Common
- [ ] **UnitTests.Architecture** project created (see Part 6c)

**Test Infrastructure:**

- [ ] **TestModuleInitializer.cs** sets ASPNETCORE_ENVIRONMENT
- [ ] **ApiTestBase<TStartup>** generic base class created
- [ ] **TestServiceOverrides** hook for custom services
- [ ] **MockDataContextFactory** with backing lists and queryable DbSets
- [ ] **Fakers** using Bogus library for realistic test data

**Test Organization:**

- [ ] **TestCollectionDefinitions.cs** with parallel/sequential collections
- [ ] **[Description] attribute** used for Jira ticket linking
- [ ] **Shouldly** for readable assertions (FluentAssertions is FORBIDDEN)

**Dependencies:**

- [ ] **xUnit 3.0** with Microsoft Testing Platform
- [ ] **Bogus** for fake data generation
- [ ] **MockQueryable.Moq** for LINQ-enabled mocked DbSets
- [ ] **Microsoft.AspNetCore.Mvc.Testing** for integration tests
- [ ] **Shouldly** for assertions (FluentAssertions is FORBIDDEN)

> **⚠️ CRITICAL:** FluentAssertions is **FORBIDDEN** for all business logic tests. Use **Shouldly** exclusively.
> FluentAssertions may only appear as a transitive dependency of ArchUnitNET in architecture test projects.

---

## 🎓 Key Takeaways - Part 6b

1. **Three-Project Structure** - Separates shared infrastructure (Common), business logic tests (UnitTests), and architecture rules (Architecture)
2. **MockDataContextFactory** - In-memory testing with queryable DbSets and backing lists for Add/Remove operations
3. **ApiTestBase<TStartup>** - Generic base class with WebApplicationFactory for API integration tests
4. **Bogus Fakers** - Generate realistic fake data for robust testing
5. **TestModuleInitializer** - Required for xUnit v3 / Microsoft Testing Platform compatibility
6. **Collection Definitions** - Control parallel/sequential execution for optimal test performance

---

**Next:** [Part 6c: Architecture Tests](./06c-architecture-tests.md)
