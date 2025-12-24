# Part 6b: Testing Patterns

**Estimated Time:** 15 minutes  
**Prerequisites:** [Part 6a Complete](./06a-health-checks.md)  
**Next:** [Part 6c: Architecture Tests](./06c-architecture-tests.md)

---

## 🎯 Overview

Testing infrastructure includes:

- **xUnit 3.0** - Modern test framework
- **Collection Definitions** - Parallel vs sequential execution
- **Test Fixtures** - Shared setup/teardown
- **Microsoft Testing Platform** - Modern test runner

---

## 📦 Test Project Setup

### MySolution.UnitTests.csproj

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

    <!-- Assertion Libraries -->
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="Shouldly" />

    <!-- Mocking -->
    <PackageReference Include="NSubstitute" />
    <PackageReference Include="Moq" />

    <!-- Test Coverage -->
    <PackageReference Include="coverlet.collector" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\MySolution.Api\MySolution.Api.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Services\MySolution.Services.csproj" />
  </ItemGroup>
</Project>
```

---

## 🏗️ Test Collection Definitions

### TestCollectionDefinitions.cs

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
using FluentAssertions;
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
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(memberId);
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
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be("Member.NotFound");
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

## ✅ Validation Checklist - Part 6b

- [ ] **Test project** created with xUnit 3.0
- [ ] **TestCollectionDefinitions.cs** created
- [ ] **Parallel collections** for independent tests
- [ ] **Sequential collections** for shared state tests
- [ ] **Test fixtures** for database/integration tests
- [ ] **[Description] attribute** used for ticket linking
- [ ] **FluentAssertions/Shouldly** for readable assertions

---

## 🎓 Key Takeaways - Part 6b

1. **Collection Definitions** - Control parallel/sequential execution
2. **Test Fixtures** - Share setup across multiple tests
3. **Description Attribute** - Link tests to Jira tickets
4. **xUnit 3.0** - Modern test framework with MTP

---

**Next:** [Part 6c: Architecture Tests](./06c-architecture-tests.md)
