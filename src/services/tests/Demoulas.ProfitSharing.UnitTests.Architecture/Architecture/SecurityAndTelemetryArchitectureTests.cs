using System.ComponentModel;
using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

/// <summary>
/// Architecture tests that verify security and telemetry patterns defined in the instruction files.
/// Based on demoulas.common.security.instructions.md and TELEMETRY_GUIDE.md.
/// </summary>
[Collection("Architecture Tests")]
public sealed class SecurityAndTelemetryArchitectureTests
{
    private static readonly System.Reflection.Assembly s_endpointsAssembly = ProfitSharingArchitectureFixture.EndpointsAssembly;
    private static readonly System.Reflection.Assembly s_servicesAssembly = ProfitSharingArchitectureFixture.ServicesAssembly;
    private static readonly System.Reflection.Assembly s_securityAssembly = ProfitSharingArchitectureFixture.SecurityAssembly;
    private static readonly ArchUnitNET.Domain.Architecture s_architecture = ProfitSharingArchitectureFixture.Architecture;

    // ===== SECURITY PATTERNS (from demoulas.common.security.instructions.md) =====

    [Fact]
    [Description("PS-XXXX : Security project must not depend on Endpoints or Services")]
    public void Security_ShouldNotDependOn_EndpointsOrServices()
    {
        IObjectProvider<IType> security = Types().That()
            .ResideInAssembly(s_securityAssembly)
            .As("Security");

        IObjectProvider<IType> endpoints = Types().That()
            .ResideInAssembly(s_endpointsAssembly)
            .As("Endpoints");

        IObjectProvider<IType> services = Types().That()
            .ResideInAssembly(s_servicesAssembly)
            .As("Services");

        IArchRule securityShouldNotDependOnEndpoints = Types().That().Are(security)
            .Should().NotDependOnAny(endpoints)
            .Because("Security project must be independent of application layers.");

        IArchRule securityShouldNotDependOnServices = Types().That().Are(security)
            .Should().NotDependOnAny(services)
            .Because("Security project must be independent of application layers.");

        ArchRuleAssertions.AssertNoArchitectureViolations(securityShouldNotDependOnEndpoints, s_architecture);
        ArchRuleAssertions.AssertNoArchitectureViolations(securityShouldNotDependOnServices, s_architecture);
    }

    [Fact]
    [Description("PS-XXXX : PolicyRoleMap must be centralized in Security project")]
    public void PolicyRoleMap_MustBe_InSecurityProject()
    {
        // Check that PolicyRoleMap exists in Security assembly
        var policyRoleMapTypes = s_securityAssembly.GetTypes()
            .Where(t => t.Name.Contains("PolicyRoleMap", StringComparison.Ordinal) ||
                        t.Name.Contains("PolicyRole", StringComparison.Ordinal))
            .ToList();

        Assert.True(policyRoleMapTypes.Count > 0,
            "PolicyRoleMap or policy-role mapping class must exist in Security project " +
            "for centralized authorization management.");

        // Check that no other assembly defines PolicyRoleMap
        var endpointsPolicyTypes = s_endpointsAssembly.GetTypes()
            .Where(t => t.Name.Contains("PolicyRoleMap", StringComparison.Ordinal))
            .ToList();

        Assert.True(endpointsPolicyTypes.Count == 0,
            "PolicyRoleMap must not be defined in Endpoints project. " +
            "Use the centralized version in Security project.");

        var servicesPolicyTypes = s_servicesAssembly.GetTypes()
            .Where(t => t.Name.Contains("PolicyRoleMap", StringComparison.Ordinal))
            .ToList();

        Assert.True(servicesPolicyTypes.Count == 0,
            "PolicyRoleMap must not be defined in Services project. " +
            "Use the centralized version in Security project.");
    }

    // ===== TELEMETRY PATTERNS (from TELEMETRY_GUIDE.md) =====

    [Fact]
    [Description("PS-XXXX : TelemetryExtensions must be in Endpoints project")]
    public void TelemetryExtensions_MustBe_InEndpointsProject()
    {
        var telemetryTypes = s_endpointsAssembly.GetTypes()
            .Where(t => t.Name.Contains("Telemetry", StringComparison.Ordinal) &&
                        t.Name.Contains("Extension", StringComparison.Ordinal))
            .ToList();

        Assert.True(telemetryTypes.Count > 0,
            "TelemetryExtensions class must exist in Endpoints project " +
            "to provide endpoint-specific telemetry methods.");
    }

    [Fact]
    [Description("PS-XXXX : EndpointTelemetry metrics class must be centralized")]
    public void EndpointTelemetry_MustBe_Centralized()
    {
        // EndpointTelemetry should be in Common.Telemetry
        var commonAssembly = ProfitSharingArchitectureFixture.CommonAssembly;

        var telemetryMetricsTypes = commonAssembly.GetTypes()
            .Where(t => t.Name == "EndpointTelemetry")
            .ToList();

        Assert.True(telemetryMetricsTypes.Count > 0,
            "EndpointTelemetry class must exist in Common project " +
            "to provide centralized metric definitions.");

        // Should have static meter and counters
        var telemetryType = telemetryMetricsTypes[0];
        var staticFields = telemetryType.GetFields(BindingFlags.Public | BindingFlags.Static);

        Assert.True(staticFields.Length > 0,
            "EndpointTelemetry must define static metric instruments " +
            "(Counter, Histogram, etc.).");
    }

    // ===== LOGGING PATTERNS (from demoulas.common.logging.instructions.md) =====

    [Fact]
    [Description("PS-XXXX : Services should use ILogger injection pattern")]
    public void Services_ShouldUse_ILoggerInjection()
    {
        var serviceTypes = s_servicesAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic)
            .Where(t => t.Name.EndsWith("Service", StringComparison.Ordinal))
            .Where(t => !t.Name.Contains("Extension", StringComparison.Ordinal))
            .ToList();

        var servicesWithLogger = 0;
        var servicesWithoutLogger = new List<string>();

        foreach (var serviceType in serviceTypes)
        {
            // Check for ILogger<T> field or constructor parameter
            var hasLoggerField = serviceType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(f => f.FieldType.IsGenericType &&
                          f.FieldType.GetGenericTypeDefinition().FullName?.StartsWith(
                              "Microsoft.Extensions.Logging.ILogger`1", StringComparison.Ordinal) == true);

            var hasLoggerParam = serviceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .Any(c => c.GetParameters().Any(p =>
                    p.ParameterType.IsGenericType &&
                    p.ParameterType.GetGenericTypeDefinition().FullName?.StartsWith(
                        "Microsoft.Extensions.Logging.ILogger`1", StringComparison.Ordinal) == true));

            if (hasLoggerField || hasLoggerParam)
            {
                servicesWithLogger++;
            }
            else
            {
                servicesWithoutLogger.Add(serviceType.Name);
            }
        }

        var totalServices = serviceTypes.Count;
        var percentageWithLogger = totalServices > 0 ? (double)servicesWithLogger / totalServices * 100 : 100;

        // At least 35% of services should have logger (tracking adoption - threshold will increase)
        Assert.True(percentageWithLogger >= 35,
            $"Only {percentageWithLogger:F1}% of services have ILogger. " +
            $"Services without logger:\n{string.Join("\n", servicesWithoutLogger.Take(10))}...");
    }

    // ===== SSN MASKING PATTERNS (from copilot-instructions.md) =====

    [Fact]
    [Description("PS-XXXX : SsnExtensions must be in Common.Extensions")]
    public void SsnExtensions_MustBe_InCommonExtensions()
    {
        var commonAssembly = ProfitSharingArchitectureFixture.CommonAssembly;

        var ssnExtensionTypes = commonAssembly.GetTypes()
            .Where(t => t.Name.Contains("Ssn", StringComparison.OrdinalIgnoreCase) &&
                        t.Name.Contains("Extension", StringComparison.Ordinal))
            .ToList();

        Assert.True(ssnExtensionTypes.Count > 0,
            "SsnExtensions class must exist in Common project " +
            "for centralized SSN masking. Use SsnExtensions.MaskSsn() everywhere.");

        // Verify no duplicate SSN masking in other projects
        var endpointsSsnTypes = s_endpointsAssembly.GetTypes()
            .Where(t => t.Name.Contains("SsnMask", StringComparison.OrdinalIgnoreCase) ||
                        (t.Name.Contains("Ssn", StringComparison.OrdinalIgnoreCase) &&
                         t.Name.Contains("Helper", StringComparison.Ordinal)))
            .ToList();

        Assert.True(endpointsSsnTypes.Count == 0,
            "SSN masking utilities should not be duplicated in Endpoints. " +
            "Use SsnExtensions from Common project.");
    }

    // ===== DISTRIBUTED TRACING PATTERNS =====

    [Fact]
    [Description("PS-XXXX : OpenTelemetry ActivitySource must be centralized")]
    public void ActivitySource_MustBe_Centralized()
    {
        var commonAssembly = ProfitSharingArchitectureFixture.CommonAssembly;

        // Look for ActivitySource definition
        var activitySourceTypes = commonAssembly.GetTypes()
            .Where(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
                .Any(f => f.FieldType.FullName?.Contains("System.Diagnostics.ActivitySource", StringComparison.Ordinal) == true))
            .ToList();

        // At least one type should define ActivitySource in Common
        Assert.True(activitySourceTypes.Count > 0,
            "At least one type in Common should define an ActivitySource " +
            "for distributed tracing coordination.");
    }

    // ===== MIDDLEWARE PATTERNS =====

    [Fact]
    [Description("PS-XXXX : Middleware classes must be in appropriate namespaces")]
    public void Middleware_MustBe_InAppropriateNamespaces()
    {
        var middlewareTypes = s_endpointsAssembly.GetTypes()
            .Concat(s_servicesAssembly.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.Name.EndsWith("Middleware", StringComparison.Ordinal))
            .ToList();

        var misplacedMiddleware = new List<string>();

        foreach (var middlewareType in middlewareTypes)
        {
            var ns = middlewareType.Namespace ?? string.Empty;

            // Middleware should be in a Middleware namespace
            if (!ns.Contains("Middleware", StringComparison.Ordinal) &&
                !ns.Contains("Serialization", StringComparison.Ordinal)) // Serialization middleware is okay elsewhere
            {
                misplacedMiddleware.Add(
                    $"{middlewareType.FullName} should be in a 'Middleware' namespace for discoverability.");
            }
        }

        if (misplacedMiddleware.Count > 0)
        {
            Assert.Fail(
                "Middleware classes should be in appropriate namespaces:\n" +
                string.Join("\n", misplacedMiddleware));
        }
    }
}
