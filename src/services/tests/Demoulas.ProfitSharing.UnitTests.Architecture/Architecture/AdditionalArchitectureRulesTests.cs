using System.ComponentModel;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using Microsoft.Extensions.Caching.Memory;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

[Collection("Architecture Tests")]
public sealed class AdditionalArchitectureRulesTests
{
    private static readonly System.Reflection.Assembly s_endpointsAssembly = ProfitSharingArchitectureFixture.EndpointsAssembly;
    private static readonly System.Reflection.Assembly s_servicesAssembly = ProfitSharingArchitectureFixture.ServicesAssembly;
    private static readonly System.Reflection.Assembly s_dataAssembly = ProfitSharingArchitectureFixture.DataAssembly;
    private static readonly ArchUnitNET.Domain.Architecture s_architecture = ProfitSharingArchitectureFixture.Architecture;

    [Fact]
    [Description("PS-2337 : Endpoints must not depend on Data project")]
    public void Endpoints_ShouldNotDependOn_DataProject()
    {
        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IObjectProvider<IType> data = Types().That()
            .ResideInAssembly(s_dataAssembly)
            .As("Data");

        IArchRule rule = Types().That().Are(endpoints)
            .Should().NotDependOnAny(data)
            .Because("Endpoints must not access persistence layer types directly; they should call services returning Result<T>.");

        ArchRuleAssertions.AssertNoArchitectureViolations(rule, s_architecture);
    }

    [Fact]
    [Description("PS-2337 : Data must not depend on Endpoints project")]
    public void Data_ShouldNotDependOn_EndpointsProject()
    {
        IObjectProvider<IType> data = Types().That()
            .ResideInAssembly(s_dataAssembly)
            .As("Data");

        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IArchRule rule = Types().That().Are(data)
            .Should().NotDependOnAny(endpoints)
            .Because("Data layer must be independent of HTTP/endpoint concerns.");

        ArchRuleAssertions.AssertNoArchitectureViolations(rule, s_architecture);
    }

    [Fact]
    [Description("PS-2337 : Services must not depend on FastEndpoints")]
    public void Services_ShouldNotDependOn_FastEndpoints()
    {
        IObjectProvider<IType> services = Types().That()
            .ResideInAssembly(s_servicesAssembly)
            .As("Services");

        IObjectProvider<IType> fastEndpoints = Types().That()
            .ResideInNamespace("FastEndpoints")
            .As("FastEndpoints");

        IArchRule rule = Types().That().Are(services)
            .Should().NotDependOnAny(fastEndpoints)
            .Because("Service layer should be HTTP-agnostic; endpoint framework types must stay in the Endpoints project.");

        ArchRuleAssertions.AssertNoArchitectureViolations(rule, s_architecture);
    }

    [Fact]
    [Description("PS-2337 : Endpoints/Services/Data must not use IMemoryCache")]
    public void ProductionAssemblies_ShouldNotDependOn_IMemoryCache()
    {
        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IObjectProvider<IType> services = Types().That()
            .ResideInAssembly(s_servicesAssembly)
            .As("Services");

        IObjectProvider<IType> data = Types().That()
            .ResideInAssembly(s_dataAssembly)
            .As("Data");

        IObjectProvider<IType> memoryCache = Types().That()
            .ResideInNamespace("Microsoft.Extensions.Caching.Memory")
            .As("IMemoryCache");

        IObjectProvider<IType> production = Types().That()
            .Are(endpoints)
            .Or().Are(services)
            .Or().Are(data)
            .As("Production Assemblies");

        IArchRule rule = Types().That().Are(production)
            .Should().NotDependOnAny(memoryCache)
            .Because("Application caching must use IDistributedCache, not IMemoryCache (per distributed caching requirements).");

        ArchRuleAssertions.AssertNoArchitectureViolations(rule, s_architecture);
    }

    [Fact]
    [Description("PS-XXXX : EmploymentStatusId must be typed as char or char? everywhere")]
    public void EmploymentStatusId_ShouldBeTypedAsChar_Everywhere()
    {
        // Get all assemblies to check
        var assembliesToCheck = new[]
        {
            s_endpointsAssembly,
            s_servicesAssembly,
            s_dataAssembly,
            ProfitSharingArchitectureFixture.CommonAssembly
        };

        var violations = new System.Collections.Generic.List<string>();

        foreach (var assembly in assembliesToCheck)
        {
            // Get all types in the assembly
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                // Skip compiler-generated anonymous types
                if (type.Name.StartsWith("<>f__AnonymousType", System.StringComparison.Ordinal) ||
                    type.Name.Contains("<>"))
                {
                    continue;
                }

                // Get all properties named EmploymentStatusId
                var properties = type.GetProperties(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance)
                    .Where(p => p.Name == "EmploymentStatusId");

                foreach (var property in properties)
                {
                    // Check if the property type is char or Nullable<char>
                    var propertyType = property.PropertyType;
                    var isChar = propertyType == typeof(char);
                    var isNullableChar = propertyType == typeof(char?);

                    if (!isChar && !isNullableChar)
                    {
                        violations.Add(
                            $"{type.FullName}.{property.Name} has type '{propertyType.FullName}' " +
                            $"but should be 'char' or 'char?' (nullable char)");
                    }
                }
            }
        }

        // Assert no violations found - output each violation separately for clarity
        if (violations.Any())
        {
            var message = "EmploymentStatusId type violations found:\n" +
                          string.Join("\n", violations);
            Assert.Fail(message);
        }
    }
}
