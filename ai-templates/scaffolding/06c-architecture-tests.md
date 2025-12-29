# Part 6c: Architecture Tests

**Estimated Time:** 15 minutes  
**Prerequisites:** [Part 6b Complete](./06b-testing-patterns.md)  
**Next:** [Part 7: Alignment Checklist](./07-alignment-checklist.md)

---

## 🎯 Overview

Architecture tests enforce:

- **Layer Dependencies** - Services don't reference Endpoints
- **Immutability Rules** - DTOs are immutable records
- **Naming Conventions** - Interfaces start with I
- **Project Structure** - Proper namespace organization

---

## 📦 Architecture Test Project Setup

### MySolution.UnitTests.Architecture.csproj

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
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="ArchUnitNET" />
    <PackageReference Include="ArchUnitNET.xUnit" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\MySolution.Api\MySolution.Api.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Services\MySolution.Services.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Data\MySolution.Data.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Common\MySolution.Common.csproj" />
  </ItemGroup>
</Project>
```

---

## 🏗️ Architecture Test Base Class

### ArchitectureTestBase.cs

```csharp
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.Fluent;

namespace MySolution.UnitTests.Architecture;

public abstract class ArchitectureTestBase
{
    protected static readonly Architecture Architecture =
        new ArchLoader()
            .LoadAssemblies(
                typeof(MySolution.Api.Program).Assembly,
                typeof(MySolution.Services.MemberService).Assembly,
                typeof(MySolution.Data.Contexts.MyDbContext).Assembly,
                typeof(MySolution.Common.Interfaces.IMemberService).Assembly)
            .Build();

    protected static IObjectProvider<IType> ApiLayer =>
        ArchRuleDefinition.Types()
            .That().ResideInNamespace("MySolution.Api.*", useRegularExpressions: true)
            .As("API Layer");

    protected static IObjectProvider<IType> ServiceLayer =>
        ArchRuleDefinition.Types()
            .That().ResideInNamespace("MySolution.Services.*", useRegularExpressions: true)
            .As("Service Layer");

    protected static IObjectProvider<IType> DataLayer =>
        ArchRuleDefinition.Types()
            .That().ResideInNamespace("MySolution.Data.*", useRegularExpressions: true)
            .As("Data Layer");

    protected static IObjectProvider<IType> CommonLayer =>
        ArchRuleDefinition.Types()
            .That().ResideInNamespace("MySolution.Common.*", useRegularExpressions: true)
            .As("Common Layer");
}
```

---

## 🔒 Layer Dependency Tests

### LayerDependencyTests.cs

```csharp
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;

namespace MySolution.UnitTests.Architecture;

public class LayerDependencyTests : ArchitectureTestBase
{
    [Fact]
    [Description("Services should not depend on API endpoints")]
    public void Services_ShouldNot_DependOn_Endpoints()
    {
        var rule = ArchRuleDefinition.Types()
            .That().Are(ServiceLayer)
            .Should().NotDependOnAny(ApiLayer)
            .Because("Service layer should be independent of API layer");

        rule.Check(Architecture);
    }

    [Fact]
    [Description("Data layer should not depend on services")]
    public void Data_ShouldNot_DependOn_Services()
    {
        var rule = ArchRuleDefinition.Types()
            .That().Are(DataLayer)
            .Should().NotDependOnAny(ServiceLayer)
            .Because("Data layer should not know about business logic");

        rule.Check(Architecture);
    }

    [Fact]
    [Description("Common should not depend on any other layer")]
    public void Common_ShouldNot_DependOn_AnyLayer()
    {
        var rule = ArchRuleDefinition.Types()
            .That().Are(CommonLayer)
            .Should().NotDependOnAny(ApiLayer)
            .AndShould().NotDependOnAny(ServiceLayer)
            .AndShould().NotDependOnAny(DataLayer)
            .Because("Common layer should be self-contained");

        rule.Check(Architecture);
    }
}
```

---

## 📐 Naming Convention Tests

### NamingConventionTests.cs

```csharp
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;

namespace MySolution.UnitTests.Architecture;

public class NamingConventionTests : ArchitectureTestBase
{
    [Fact]
    [Description("Interfaces should start with I")]
    public void Interfaces_ShouldStartWith_I()
    {
        var rule = ArchRuleDefinition.Interfaces()
            .That().AreInterfaces()
            .Should().HaveNameStartingWith("I")
            .Because("Interface naming convention");

        rule.Check(Architecture);
    }

    [Fact]
    [Description("Services should end with Service")]
    public void Services_ShouldEndWith_Service()
    {
        var rule = ArchRuleDefinition.Classes()
            .That().ResideInNamespace("MySolution.Services", useRegularExpressions: false)
            .And().AreNotAbstract()
            .Should().HaveNameEndingWith("Service")
            .Because("Service naming convention");

        rule.Check(Architecture);
    }

    [Fact]
    [Description("Endpoints should end with Endpoint")]
    public void Endpoints_ShouldEndWith_Endpoint()
    {
        var rule = ArchRuleDefinition.Classes()
            .That().ResideInNamespace("MySolution.Api.Endpoints.*", useRegularExpressions: true)
            .And().AreNotAbstract()
            .Should().HaveNameEndingWith("Endpoint")
            .Because("Endpoint naming convention");

        rule.Check(Architecture);
    }
}
```

---

## 🔐 Immutability Tests

### ImmutabilityTests.cs

```csharp
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;

namespace MySolution.UnitTests.Architecture;

public class ImmutabilityTests : ArchitectureTestBase
{
    [Fact]
    [Description("DTOs should be immutable records")]
    public void DTOs_ShouldBe_Records()
    {
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespace("MySolution.Common.Contracts.*", useRegularExpressions: true)
            .And().HaveNameEndingWith("Dto")
            .Should().BeRecords()
            .Because("DTOs should be immutable");

        rule.Check(Architecture);
    }

    [Fact]
    [Description("Request DTOs should be immutable")]
    public void RequestDTOs_ShouldBe_Immutable()
    {
        var rule = ArchRuleDefinition.Types()
            .That().ResideInNamespace("MySolution.Common.Contracts.Request.*", useRegularExpressions: true)
            .Should().BeRecords()
            .Because("Request objects should not be modified after creation");

        rule.Check(Architecture);
    }
}
```

---

## 🎯 Custom Architecture Rules

### CustomArchitectureRules.cs

```csharp
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;

namespace MySolution.UnitTests.Architecture;

public class CustomArchitectureRules : ArchitectureTestBase
{
    [Fact]
    [Description("Controllers should not exist (use FastEndpoints)")]
    public void Controllers_ShouldNot_Exist()
    {
        var rule = ArchRuleDefinition.Classes()
            .That().HaveNameEndingWith("Controller")
            .Should().NotExist()
            .Because("Use FastEndpoints instead of controllers");

        rule.Check(Architecture);
    }

    [Fact]
    [Description("DbContext should only be used in Data layer")]
    public void DbContext_ShouldOnlyBeUsed_InDataLayer()
    {
        var rule = ArchRuleDefinition.Classes()
            .That().AreAssignableTo("Microsoft.EntityFrameworkCore.DbContext")
            .Should().ResideInNamespace("MySolution.Data.*", useRegularExpressions: true)
            .Because("DbContext should only be in Data layer");

        rule.Check(Architecture);
    }

    [Fact]
    [Description("Services should return Result<T>")]
    public void Services_ShouldReturn_ResultT()
    {
        var rule = ArchRuleDefinition.Methods()
            .That().AreDeclaredIn(ServiceLayer)
            .And().ArePublic()
            .And().DoNotHaveReturnType(typeof(void))
            .Should().HaveReturnType("MySolution.Common.Contracts.Result`1")
            .Or().HaveReturnType("System.Threading.Tasks.Task`1")
            .Because("Services should use Result<T> pattern");

        // Note: This rule requires more sophisticated pattern matching
        // May need custom ArchUnit extension
    }
}
```

---

## ✅ Validation Checklist - Part 6c

- [ ] **Architecture test project** created
- [ ] **ArchUnitNET** package installed
- [ ] **Layer dependency tests** implemented
- [ ] **Naming convention tests** implemented
- [ ] **Immutability tests** for DTOs
- [ ] **Custom rules** for project-specific patterns
- [ ] **Tests run** in CI/CD pipeline

---

## 🎓 Key Takeaways - Part 6c

1. **Automated Enforcement** - Architecture rules enforced by tests
2. **Layer Independence** - Prevent circular dependencies
3. **Naming Conventions** - Consistent naming across codebase
4. **Immutability** - DTOs are immutable records

---

**Next:** [Part 7: Alignment Checklist](./07-alignment-checklist.md) - Audit existing projects
