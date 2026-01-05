# Part 6c: Architecture Tests

**Estimated Time:** 30 minutes  
**Prerequisites:** [Part 6b Complete](./06b-testing-patterns.md)  
**Next:** [Part 7: Alignment Checklist](./07-alignment-checklist.md)

---

## 🎯 Overview

Architecture tests enforce structural rules and organizational patterns across your codebase:

- **Contracts Organization** - Request/Response separation in Contracts folder
- **Validation Organization** - Validators centralized in Validation namespace
- **Layer Dependencies** - Services don't reference Endpoints/Api
- **Naming Conventions** - Consistent suffixes (Service, Endpoint, Validator, Dto)
- **Project Structure** - Domain-based organization, no Controllers
- **Immutability Rules** - DTOs are immutable with readonly properties

**Reference Implementation:** See `Demoulas.Handheld.UnitTests.Architecture` for complete examples (31 tests across 6 categories)

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
    <OutputType>Exe</OutputType>
    <EnableMicrosoftTestingPlatform>true</EnableMicrosoftTestingPlatform>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="TngTech.ArchUnitNET" />
    <PackageReference Include="TngTech.ArchUnitNET.xUnit" />
  </ItemGroup>

  <ItemGroup>
    <!-- Reference assemblies with actual types to analyze -->
    <ProjectReference Include="..\..\src\MySolution.Api\MySolution.Api.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Endpoints\MySolution.Endpoints.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Services\MySolution.Services.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Data\MySolution.Data.csproj" />
    <ProjectReference Include="..\..\src\MySolution.Common\MySolution.Common.csproj" />
  </ItemGroup>
</Project>
```

**Key Configuration Notes:**

- Use `TngTech.ArchUnitNET` packages (not just `ArchUnitNET`)
- Enable Microsoft Testing Platform for xUnit v3 compatibility
- Reference all assemblies you need to analyze (not just interfaces)

---

## 🏗️ Architecture Test Base Class

### ArchitectureTestBase.cs

```csharp
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MySolution.UnitTests.Architecture;

public abstract class ArchitectureTestBase
{
    // Build architecture from actual type references (not interface assemblies)
    protected static readonly Architecture Architecture =
        new ArchLoader()
            .LoadAssemblies(
                typeof(MySolution.Api.Program).Assembly,
                typeof(MySolution.Endpoints.Endpoints.GetItemsEndpoint).Assembly,
                typeof(MySolution.Services.Orders.OrderService).Assembly,
                typeof(MySolution.Common.Contracts.Request.GetOrderRequest).Assembly
            )
            .Build();
}
```

**Critical ArchUnitNET API Patterns:**

1. **ResideInNamespace()** - Takes single string parameter (NO boolean parameters)

   ```csharp
   // ✅ CORRECT
   .ResideInNamespace("MySolution.Common")

   // ❌ WRONG - no useRegularExpressions parameter
   .ResideInNamespace("MySolution.Common", useRegularExpressions: true)
   ```

2. **WithoutRequiringPositiveResults()** - Required for rules that may match zero types

   ```csharp
   // ✅ CORRECT - use when rule may have no matches
   .Should().NotDependOnAny(...)
   .Because("reason")
   .WithoutRequiringPositiveResults()
   .Check(Architecture);
   ```

3. **Method Chain Order** - Strict ordering required

   ```csharp
   // ✅ CORRECT ORDER
   Classes().That()...
       .Should()...
       .Because("reason")
       .WithoutRequiringPositiveResults()  // After Because()
       .Check(Architecture);

   // ❌ WRONG - WithoutRequiringPositiveResults before Because
   .Should()...
   .WithoutRequiringPositiveResults()
   .Because("reason")
   .Check(Architecture);
   ```

4. **Interfaces()** - Call .Should() directly, no .That() needed

   ```csharp
   // ✅ CORRECT
   Interfaces().Should().HaveNameStartingWith("I")

   // ❌ WRONG - unnecessary .That()
   Interfaces().That().AreInterfaces().Should()...
   ```

---

## � Test Category 1: Contracts Organization

### ContractsArchitectureTests.cs

Validates proper organization of Request/Response DTOs in the Contracts folder.

```csharp
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MySolution.UnitTests.Architecture;

public class ContractsArchitectureTests : ArchitectureTestBase
{
    [Fact]
    public void RequestClasses_Should_Be_In_Contracts_Request_Namespace()
    {
        Classes().That().HaveNameEndingWith("Request")
            .And().ResideInNamespace("MySolution.Common")
            .Should().ResideInNamespace("MySolution.Common.Contracts.Request")
            .Because("request classes should be centralized in Contracts.Request folder")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void ResponseClasses_Should_Be_In_Contracts_Response_Namespace()
    {
        Classes().That().HaveNameEndingWith("Response")
            .And().ResideInNamespace("MySolution.Common")
            .Should().ResideInNamespace("MySolution.Common.Contracts.Response")
            .Because("response DTOs should be centralized in Contracts.Response folder")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Contracts_Request_Should_Not_Depend_On_Response()
    {
        Types().That().ResideInNamespace("MySolution.Common.Contracts.Request")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MySolution.Common.Contracts.Response"))
            .Because("request contracts should not depend on response contracts")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Contracts_Should_Not_Depend_On_Services()
    {
        Types().That().ResideInNamespace("MySolution.Common.Contracts")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MySolution.Services"))
            .Because("contracts should be independent of service implementations")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Contracts_Should_Be_Immutable_Data_Objects()
    {
        // Verify all contracts have readonly properties (init-only setters)
        Classes().That().ResideInNamespace("MySolution.Common.Contracts")
            .And().AreNotAbstract()
            .Should().HavePropertyMemberWithSetter(false) // No setters, only init
            .Because("contract DTOs should be immutable")
            .Check(Architecture);
    }
}
```

**Key Tests:**

- Request/Response folder separation
- No circular dependencies between Request and Response
- Contracts independent of Services/Endpoints
- Immutability enforcement

---

## ✅ Test Category 2: Validation Organization

### ValidationArchitectureTests.cs

Validates FluentValidation validator organization and dependencies.

```csharp
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MySolution.UnitTests.Architecture;

public class ValidationArchitectureTests : ArchitectureTestBase
{
    [Fact]
    public void Validators_Should_Be_In_Validation_Namespace()
    {
        Classes().That().HaveNameEndingWith("Validator")
            .And().ResideInNamespace("MySolution.Common")
            .Should().ResideInNamespace("MySolution.Common.Validation")
            .Because("validators should be centralized in Validation namespace")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Validators_Should_End_With_Validator_Suffix()
    {
        Classes().That().ResideInNamespace("MySolution.Common.Validation")
            .And().AreNotAbstract()
            .Should().HaveNameEndingWith("Validator")
            .Because("validation classes should follow naming convention")
            .Check(Architecture);
    }

    [Fact]
    public void Request_Validators_Should_End_With_RequestValidator()
    {
        // Validators for request objects should have RequestValidator suffix
        Classes().That().ResideInNamespace("MySolution.Common.Validation")
            .And().HaveNameEndingWith("RequestValidator")
            .Should().Exist()
            .Because("request validators should be clearly identified")
            .Check(Architecture);
    }

    [Fact]
    public void Validators_Should_Not_Depend_On_Services()
    {
        Types().That().ResideInNamespace("MySolution.Common.Validation")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MySolution.Services"))
            .Because("validation layer should not depend on service implementations")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }
}
```

**Key Tests:**

- Validators centralized in Validation namespace
- Naming conventions enforced (*Validator, *RequestValidator)
- Validators independent of Services/Endpoints

---

## 🏗️ Test Category 3: Layer Dependencies

### LayerDependencyTests.cs

Enforces proper dependency flow between architectural layers.

```csharp
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MySolution.UnitTests.Architecture;

public class LayerDependencyTests : ArchitectureTestBase
{
    [Fact]
    public void Services_Should_Not_Depend_On_Endpoints()
    {
        Types().That().ResideInNamespace("MySolution.Services")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MySolution.Endpoints"))
            .Because("service layer should be independent of API/endpoint layer")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Services_Should_Not_Depend_On_Api()
    {
        Types().That().ResideInNamespace("MySolution.Services")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MySolution.Api"))
            .Because("service layer should not depend on API hosting layer")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Common_Should_Not_Depend_On_Services()
    {
        Types().That().ResideInNamespace("MySolution.Common")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MySolution.Services"))
            .Because("common layer (contracts, DTOs) should not depend on service implementations")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Common_Should_Not_Depend_On_Endpoints()
    {
        Types().That().ResideInNamespace("MySolution.Common")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MySolution.Endpoints"))
            .Because("common layer should not depend on API layer")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Common_Should_Not_Depend_On_Api()
    {
        Types().That().ResideInNamespace("MySolution.Common")
            .Should().NotDependOnAny(Types().That().ResideInNamespace("MySolution.Api"))
            .Because("common layer should be self-contained")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }
}
```

**Enforced Dependency Flow:**

```
Api → Endpoints → Services → Data
         ↓           ↓
       Common ← ← ← ← ←
```

---

## 📐 Test Category 4: Naming Conventions

### NamingConventionTests.cs

Enforces consistent naming patterns across the codebase.

```csharp
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MySolution.UnitTests.Architecture;

public class NamingConventionTests : ArchitectureTestBase
{
    [Fact]
    public void Interfaces_Should_Start_With_I()
    {
        // Note: Interfaces() returns a type that can call .Should() directly
        Interfaces().Should().HaveNameStartingWith("I")
            .Because("interface naming convention requires I prefix")
            .Check(Architecture);
    }

    [Fact]
    public void Services_Should_End_With_Service()
    {
        Classes().That().ResideInNamespace("MySolution.Services")
            .And().AreNotAbstract()
            .Should().HaveNameEndingWith("Service")
            .Because("service classes should follow naming convention")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Endpoints_Should_End_With_Endpoint()
    {
        Classes().That().ResideInNamespace("MySolution.Endpoints")
            .And().AreNotAbstract()
            .And().DoNotHaveName("EndpointBase") // Exclude base classes
            .Should().HaveNameEndingWith("Endpoint")
            .Because("endpoint classes should follow FastEndpoints naming convention")
            .Check(Architecture);
    }

    [Fact]
    public void Dtos_Should_End_With_Dto()
    {
        Classes().That().HaveNameEndingWith("Dto")
            .Should().ResideInNamespace("MySolution.Common")
            .Because("DTOs should be in Common layer")
            .Check(Architecture);
    }

    [Fact]
    public void Response_Classes_Should_Have_Consistent_Naming()
    {
        Classes().That().ResideInNamespace("MySolution.Common.Contracts.Response")
            .Should().HaveNameEndingWith("Response")
            .Or().HaveNameEndingWith("ResponseDto")
            .Because("response classes should have consistent naming")
            .Check(Architecture);
    }
}
```

**Naming Patterns:**

- `I*` - Interfaces
- `*Service` - Service implementations
- `*Endpoint` - FastEndpoints endpoints
- `*Dto` - Data transfer objects
- `*Request` - Request objects
- `*Response` / `*ResponseDto` - Response objects
- `*Validator` - FluentValidation validators

---

## 🏛️ Test Category 5: Project Structure

### ProjectStructureTests.cs

Validates overall project organization and domain patterns.

```csharp
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MySolution.UnitTests.Architecture;

public class ProjectStructureTests : ArchitectureTestBase
{
    [Fact]
    public void Endpoints_Should_Be_Organized_In_Endpoints_Namespace()
    {
        Classes().That().HaveNameEndingWith("Endpoint")
            .And().AreNotAbstract()
            .Should().ResideInNamespace("MySolution.Endpoints")
            .Because("endpoints should be organized in Endpoints project")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Services_Should_Be_Organized_By_Domain()
    {
        // Services should be in domain-specific namespaces (e.g., MySolution.Services.Orders)
        Classes().That().ResideInNamespace("MySolution.Services")
            .And().HaveNameEndingWith("Service")
            .Should().NotResideInNamespace("MySolution.Services") // Not in root
            .Because("services should be organized by domain (e.g., Services.Orders, Services.Items)")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void Service_Interfaces_Should_Be_In_Interfaces_Namespace()
    {
        Interfaces().That().HaveNameEndingWith("Service")
            .Should().ResideInNamespace("MySolution.Common.Interfaces")
            .Because("service interfaces should be in Common.Interfaces")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }

    [Fact]
    public void No_Controllers_Should_Exist()
    {
        Classes().That().HaveNameEndingWith("Controller")
            .Should().NotExist()
            .Because("FastEndpoints should be used instead of controllers")
            .Check(Architecture);
    }

    [Fact]
    public void All_Validation_Should_Be_In_Validation_Namespace()
    {
        Classes().That().ImplementInterface("FluentValidation.IValidator")
            .Should().ResideInNamespace("MySolution.Common.Validation")
            .Because("all validators should be centralized in Validation namespace")
            .WithoutRequiringPositiveResults()
            .Check(Architecture);
    }
}
```

**Structure Patterns:**

- Endpoints in `*.Endpoints.Endpoints.*`
- Services in `*.Services.{Domain}.*` (e.g., `Services.Orders`)
- Interfaces in `*.Common.Interfaces`
- Contracts in `*.Common.Contracts.{Request|Response}`
- Validation in `*.Common.Validation`
- No MVC Controllers (FastEndpoints only)

---

## 🎯 Test Category 6: Endpoint-Specific Tests

### EndpointArchitectureTests.cs

Validates FastEndpoints-specific patterns.

```csharp
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MySolution.UnitTests.Architecture;

public class EndpointArchitectureTests : ArchitectureTestBase
{
    [Fact]
    public void Endpoints_Should_Be_In_Endpoints_Namespace()
    {
        Classes().That().HaveNameEndingWith("Endpoint")
            .And().AreNotAbstract()
            .And().DoNotHaveFullName("MySolution.Endpoints.Base.EndpointBase")
            .Should().ResideInNamespace("MySolution.Endpoints.Endpoints")
            .Because("concrete endpoints should be in Endpoints.Endpoints namespace")
            .Check(Architecture);
    }

    [Fact]
    public void Endpoints_Should_Not_Reference_DbContext()
    {
        Types().That().ResideInNamespace("MySolution.Endpoints")
            .Should().NotDependOnAny(Types().That().HaveNameEndingWith("DbContext"))
            .Because("endpoints should not directly access database (use services)")
            .Check(Architecture);
    }
}
```

---

## 🧪 Running Architecture Tests

### Command Line

```bash
# Run all architecture tests
cd tests/MySolution.UnitTests.Architecture
dotnet test

# Run specific test category
dotnet test --filter "FullyQualifiedName~ContractsArchitectureTests"
dotnet test --filter "FullyQualifiedName~LayerDependencyTests"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Visual Studio / Rider

- Open Test Explorer
- Filter by project: `MySolution.UnitTests.Architecture`
- Run all or specific test categories

### CI/CD Integration

```yaml
# Example: Azure Pipelines
- task: DotNetCoreCLI@2
  displayName: "Run Architecture Tests"
  inputs:
    command: "test"
    projects: "**/MySolution.UnitTests.Architecture.csproj"
    arguments: "--configuration $(buildConfiguration) --logger trx"
```

---

## 🚨 Common Test Failures & Fixes

### 1. "Requires positive evaluation, not just absence of violations"

**Problem:** Rule expects to find matching types but finds none.

**Fix:** Add `.WithoutRequiringPositiveResults()` after `.Because()`

```csharp
// ❌ FAILS when no matching types
.Should().ResideInNamespace("...")
.Because("reason")
.Check(Architecture);

// ✅ CORRECT - allows zero matches
.Should().ResideInNamespace("...")
.Because("reason")
.WithoutRequiringPositiveResults()
.Check(Architecture);
```

### 2. "does not contain a definition for 'Because'"

**Problem:** `.WithoutRequiringPositiveResults()` called before `.Because()`

**Fix:** Correct method chain order

```csharp
// ❌ WRONG ORDER
.Should()...
.WithoutRequiringPositiveResults()  // Too early
.Because("reason")
.Check(Architecture);

// ✅ CORRECT ORDER
.Should()...
.Because("reason")
.WithoutRequiringPositiveResults()  // After Because
.Check(Architecture);
```

### 3. "ResideInNamespace does not contain a definition for 'useRegularExpressions'"

**Problem:** Using boolean parameters that don't exist in ArchUnitNET API

**Fix:** Remove all boolean parameters

```csharp
// ❌ WRONG - no boolean parameters exist
.ResideInNamespace("MySolution.Services", useRegularExpressions: true)
.DoNotResideInNamespace("MySolution.Api", false)

// ✅ CORRECT - single string parameter only
.ResideInNamespace("MySolution.Services")
.DoNotResideInNamespace("MySolution.Api")
```

### 4. Assembly Not Loaded

**Problem:** Architecture test can't find types from a project

**Fix:** Reference actual implementation assembly (not interface-only assembly)

```csharp
// ❌ WRONG - interface assembly has no implementations
typeof(MySolution.Common.Interfaces.IOrderService).Assembly

// ✅ CORRECT - implementation assembly with actual types
typeof(MySolution.Services.Orders.OrderService).Assembly
```

---

## 📊 Test Organization Summary

| Test Class                  | Tests  | Purpose                                   |
| --------------------------- | ------ | ----------------------------------------- |
| ContractsArchitectureTests  | 9      | Request/Response organization             |
| ValidationArchitectureTests | 5      | Validator organization & patterns         |
| LayerDependencyTests        | 5      | Layer boundary enforcement                |
| NamingConventionTests       | 5      | Consistent naming across codebase         |
| ProjectStructureTests       | 7      | Domain organization, no Controllers       |
| EndpointArchitectureTests   | 2      | FastEndpoints patterns                    |
| **Total**                   | **33** | **Comprehensive architecture validation** |

---

## ✅ Validation Checklist - Part 6c

- [ ] **Architecture test project** created with correct packages
  - `TngTech.ArchUnitNET` and `TngTech.ArchUnitNET.xUnit`
  - `EnableMicrosoftTestingPlatform=true` for xUnit v3
- [ ] **All assemblies referenced** in ArchLoader (implementation assemblies)
- [ ] **Contracts organization tests** (9 tests)
  - Request/Response separation
  - No circular dependencies
  - Immutability checks
- [ ] **Validation organization tests** (5 tests)
  - Validators centralized
  - Naming conventions
  - No service dependencies
- [ ] **Layer dependency tests** (5 tests)
  - Services independent of Endpoints/Api
  - Common self-contained
- [ ] **Naming convention tests** (5 tests)
  - Interfaces, Services, Endpoints, DTOs
- [ ] **Project structure tests** (7 tests)
  - Domain organization
  - No Controllers
- [ ] **Endpoint-specific tests** (2+ tests)
  - FastEndpoints patterns
  - No direct DbContext access
- [ ] **All tests pass** with `dotnet test`
- [ ] **Tests integrated** into CI/CD pipeline
- [ ] **Documentation created** (README.md in test project)

---

## 🎓 Key Takeaways - Part 6c

1. **Automated Enforcement** - Architecture rules automatically enforced by tests
2. **Organization Patterns** - Contracts/Validation folders properly structured
3. **Layer Independence** - Prevent circular dependencies, enforce uni-directional flow
4. **Naming Consistency** - Consistent suffixes (Service, Endpoint, Validator, Dto)
5. **FastEndpoints Pattern** - No Controllers, endpoints don't access DbContext directly
6. **ArchUnitNET Quirks** - API differs from documentation; use trial/error or working examples
7. **WithoutRequiringPositiveResults** - Essential for organizational rules that may have zero matches

---

**Next:** [Part 7: Alignment Checklist](./07-alignment-checklist.md) - Audit existing projects
