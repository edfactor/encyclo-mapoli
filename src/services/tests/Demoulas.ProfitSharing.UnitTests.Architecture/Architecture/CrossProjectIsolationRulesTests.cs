using System.ComponentModel;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using Microsoft.Extensions.Caching.Memory;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

[Collection("Architecture Tests")]
public sealed class CrossProjectIsolationRulesTests
{
    private static readonly System.Reflection.Assembly s_endpointsAssembly = ProfitSharingArchitectureFixture.EndpointsAssembly;

    private static readonly System.Reflection.Assembly s_servicesAssembly = ProfitSharingArchitectureFixture.ServicesAssembly;
    private static readonly System.Reflection.Assembly s_dataAssembly = ProfitSharingArchitectureFixture.DataAssembly;
    private static readonly System.Reflection.Assembly s_commonAssembly = ProfitSharingArchitectureFixture.CommonAssembly;
    private static readonly System.Reflection.Assembly s_securityAssembly = ProfitSharingArchitectureFixture.SecurityAssembly;
    private static readonly System.Reflection.Assembly s_reportingAssembly = ProfitSharingArchitectureFixture.ReportingAssembly;
    private static readonly System.Reflection.Assembly s_cachingServicesAssembly = ProfitSharingArchitectureFixture.CachingServicesAssembly;

    private static readonly ArchUnitNET.Domain.Architecture s_architecture = ProfitSharingArchitectureFixture.Architecture;

    [Fact]
    [Description("PS-2337 : Core backend assemblies must not depend on Endpoints")]
    public void CoreAssemblies_ShouldNotDependOn_EndpointsProject()
    {
        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IObjectProvider<IType> core = Types().That()
            .ResideInAssembly(s_servicesAssembly)
            .Or().ResideInAssembly(s_dataAssembly)
            .Or().ResideInAssembly(s_commonAssembly)
            .Or().ResideInAssembly(s_securityAssembly)
            .Or().ResideInAssembly(s_reportingAssembly)
            .Or().ResideInAssembly(s_cachingServicesAssembly)
            .As("Core Backend Assemblies");

        IArchRule rule = Types().That().Are(core)
            .Should().NotDependOnAny(endpoints)
            .Because("Only the Endpoints project should depend on other layers; core libraries must not depend on endpoint implementations.");

        ArchRuleAssertions.AssertNoArchitectureViolations(rule, s_architecture);
    }

    [Fact]
    [Description("PS-2337 : Backend assemblies must not depend on IMemoryCache")]
    public void BackendAssemblies_ShouldNotDependOn_IMemoryCache()
    {
        IObjectProvider<IType> memoryCache = Types().That()
            .ResideInNamespace("Microsoft.Extensions.Caching.Memory")
            .As("IMemoryCache");

        IObjectProvider<IType> backend = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .Or().ResideInAssembly(s_servicesAssembly)
            .Or().ResideInAssembly(s_dataAssembly)
            .Or().ResideInAssembly(s_commonAssembly)
            .Or().ResideInAssembly(s_securityAssembly)
            .Or().ResideInAssembly(s_reportingAssembly)
            .Or().ResideInAssembly(s_cachingServicesAssembly)
            .As("Backend Assemblies");

        IArchRule rule = Types().That().Are(backend)
            .Should().NotDependOnAny(memoryCache)
            .Because("Caching must use IDistributedCache; IMemoryCache is not allowed per distributed caching requirements.");

        ArchRuleAssertions.AssertNoArchitectureViolations(rule, s_architecture);
    }
}
