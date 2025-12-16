using System.ComponentModel;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Services;
using Microsoft.EntityFrameworkCore;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

[Collection("Architecture Tests")]
public sealed class ArchitectureRulesTests
{
    private static readonly System.Reflection.Assembly s_endpointsAssembly = ProfitSharingArchitectureFixture.EndpointsAssembly;
    private static readonly System.Reflection.Assembly s_servicesAssembly = ProfitSharingArchitectureFixture.ServicesAssembly;
    private static readonly System.Reflection.Assembly s_dataAssembly = ProfitSharingArchitectureFixture.DataAssembly;
    private static readonly ArchUnitNET.Domain.Architecture s_architecture = ProfitSharingArchitectureFixture.Architecture;

    [Fact]
    [Description("PS-2337 : Endpoints must not depend on EF Core or DbContexts")]
    public void Endpoints_ShouldNotDependOn_EfCore_Or_DataContexts()
    {
        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IObjectProvider<IType> efCore = Types().That()
            .ResideInNamespace("Microsoft.EntityFrameworkCore")
            .As("EF Core");

        IObjectProvider<IType> dataContexts = Types().That()
            .ResideInNamespace("Demoulas.ProfitSharing.Data.Contexts")
            .As("Data Contexts");

        IObjectProvider<IType> dataContextFactoryInterfaces = Types().That()
            .ResideInNamespace("Demoulas.ProfitSharing.Data.Interfaces")
            .As("Data Context Factory Interfaces");

        IArchRule endpointsShouldNotDependOnEfOrDb = Types().That().Are(endpoints)
            .Should().NotDependOnAny(efCore)
            .AndShould().NotDependOnAny(dataContexts)
            .AndShould().NotDependOnAny(dataContextFactoryInterfaces)
            .Because("Endpoints must not access EF Core/DbContexts directly; they should call services returning Result<T>.");

        ArchRuleAssertions.AssertNoArchitectureViolations(endpointsShouldNotDependOnEfOrDb, s_architecture);
    }

    [Fact]
    [Description("PS-2337 : Services must not depend on Endpoints")]
    public void Services_ShouldNotDependOn_Endpoints()
    {
        IObjectProvider<IType> services = Types().That()
            .ResideInAssembly(s_servicesAssembly)
            .As("Services");

        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IArchRule servicesShouldNotDependOnEndpoints = Types().That().Are(services)
            .Should().NotDependOnAny(endpoints)
            .Because("Service layer should be HTTP-agnostic and must not depend on endpoint implementations.");

        ArchRuleAssertions.AssertNoArchitectureViolations(servicesShouldNotDependOnEndpoints, s_architecture);
    }
}
