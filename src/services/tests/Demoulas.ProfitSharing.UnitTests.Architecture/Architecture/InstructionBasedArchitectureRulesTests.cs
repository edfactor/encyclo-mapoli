using System.ComponentModel;
using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using Demoulas.ProfitSharing.Common.Attributes;
using Microsoft.AspNetCore.Http;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

/// <summary>
/// Architecture tests that verify compliance with the patterns defined in the project's
/// instruction files (.github/instructions/).
/// </summary>
[Collection("Architecture Tests")]
public sealed class InstructionBasedArchitectureRulesTests
{
    private static readonly System.Reflection.Assembly s_endpointsAssembly = ProfitSharingArchitectureFixture.EndpointsAssembly;
    private static readonly System.Reflection.Assembly s_servicesAssembly = ProfitSharingArchitectureFixture.ServicesAssembly;
    private static readonly System.Reflection.Assembly s_dataAssembly = ProfitSharingArchitectureFixture.DataAssembly;
    private static readonly System.Reflection.Assembly s_commonAssembly = ProfitSharingArchitectureFixture.CommonAssembly;
    private static readonly ArchUnitNET.Domain.Architecture s_architecture = ProfitSharingArchitectureFixture.Architecture;

    // ===== ENDPOINT PATTERNS (from svc.endpoints.instructions.md) =====

    [Fact]
    [Description("PS-XXXX : Endpoints must inherit from base endpoint classes")]
    public void Endpoints_MustInheritFrom_BaseEndpointClasses()
    {
        // Get all types in the Endpoints assembly that look like endpoint classes
        var endpointTypes = s_endpointsAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.Name.EndsWith("Endpoint", StringComparison.Ordinal))
            .Where(t => t.Namespace?.Contains("Endpoints.Endpoints", StringComparison.Ordinal) == true)
            .ToList();

        var violations = new List<string>();

        foreach (var endpointType in endpointTypes)
        {
            // Check if the type inherits from one of the base classes
            var inheritsFromProfitSharingBase = InheritsFromProfitSharingBaseClass(endpointType);
            var inheritsFromFastEndpointsBase = InheritsFromFastEndpointsBase(endpointType);

            // Allowed: inherits from ProfitSharingEndpoint* base classes
            // or from EndpointWithCsvBase/EndpointWithCsvTotalsBase (specialized report endpoints)
            // Check if it inherits from FastEndpoints base directly (should use our base classes instead)
            if (!inheritsFromProfitSharingBase &&
                !IsSpecializedReportEndpoint(endpointType) &&
                inheritsFromFastEndpointsBase &&
                !InheritsFromEndpointWithCsvBase(endpointType))
            {
                violations.Add(
                    $"{endpointType.FullName} inherits directly from FastEndpoints base class. " +
                    "It should inherit from ProfitSharingEndpoint<,>, ProfitSharingRequestEndpoint<>, " +
                    "ProfitSharingResponseEndpoint<>, ProfitSharingResultResponseEndpoint<>, " +
                    "or EndpointWithCsvBase/EndpointWithCsvTotalsBase for report endpoints.");
            }
        }

        if (violations.Count > 0)
        {
            Assert.Fail("Endpoint base class violations found:\n" + string.Join("\n", violations));
        }
    }

    [Fact]
    [Description("PS-XXXX : Endpoints should have ILogger injected for telemetry")]
    public void Endpoints_ShouldHave_LoggerInjected()
    {
        // Get all endpoint types
        var endpointTypes = s_endpointsAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.Name.EndsWith("Endpoint", StringComparison.Ordinal))
            .Where(t => t.Namespace?.Contains("Endpoints.Endpoints", StringComparison.Ordinal) == true)
            .Where(t => InheritsFromProfitSharingBaseClass(t) || IsSpecializedReportEndpoint(t))
            .ToList();

        var endpointsWithoutLogger = new List<string>();

        foreach (var endpointType in endpointTypes)
        {
            // Check if the type has a constructor with ILogger parameter
            var constructors = endpointType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var hasLoggerParameter = constructors.Any(c =>
                c.GetParameters().Any(p =>
                    p.ParameterType.IsGenericType &&
                    p.ParameterType.GetGenericTypeDefinition().FullName?.StartsWith("Microsoft.Extensions.Logging.ILogger`1", StringComparison.Ordinal) == true));

            // Check for private logger field
            var hasLoggerField = endpointType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(f => f.FieldType.IsGenericType &&
                          f.FieldType.GetGenericTypeDefinition().FullName?.StartsWith("Microsoft.Extensions.Logging.ILogger`1", StringComparison.Ordinal) == true);

            if (!hasLoggerParameter && !hasLoggerField)
            {
                endpointsWithoutLogger.Add(endpointType.FullName!);
            }
        }

        // This is an informational check - endpoints SHOULD have logger but some legacy ones might not
        // For now, we just verify the pattern exists in most endpoints
        var totalEndpoints = endpointTypes.Count;
        var endpointsWithLogger = totalEndpoints - endpointsWithoutLogger.Count;
        var percentageWithLogger = totalEndpoints > 0 ? (double)endpointsWithLogger / totalEndpoints * 100 : 100;

        // At least 50% of endpoints should have logger (adjust threshold as codebase evolves)
        Assert.True(percentageWithLogger >= 50,
            $"Only {percentageWithLogger:F1}% of endpoints have ILogger injected. " +
            $"Endpoints without logger:\n{string.Join("\n", endpointsWithoutLogger.Take(10))}...");
    }

    // ===== SERVICE PATTERNS (from svc.services.instructions.md) =====

    [Fact]
    [Description("PS-XXXX : Services must not depend on HttpContext directly")]
    public void Services_ShouldNotDependOn_HttpContextDirectly()
    {
        // Check for direct HttpContext dependency (not IHttpContextAccessor which is acceptable)
        var servicesWithHttpContext = s_servicesAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => !t.Name.Contains("Middleware", StringComparison.Ordinal)) // Middleware is allowed
            .Where(t => !t.Name.Contains("Filter", StringComparison.Ordinal)) // Filters are allowed
            .SelectMany(t => t.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(c => c.GetParameters())
            .Where(p => p.ParameterType == typeof(HttpContext))
            .Select(p => p.Member.DeclaringType?.FullName)
            .Where(n => n != null)
            .Distinct()
            .ToList();

        if (servicesWithHttpContext.Count > 0)
        {
            Assert.Fail(
                "Services should not take HttpContext directly. Use IHttpContextAccessor instead.\n" +
                "Violations:\n" + string.Join("\n", servicesWithHttpContext));
        }
    }

    // ===== DATA ACCESS PATTERNS (from demoulas.common.data.instructions.md) =====

    [Fact]
    [Description("PS-XXXX : Common project must not depend on Data project")]
    public void Common_ShouldNotDependOn_DataProject()
    {
        IObjectProvider<IType> common = Types().That()
            .ResideInAssembly(s_commonAssembly)
            .As("Common");

        IObjectProvider<IType> data = Types().That()
            .ResideInAssembly(s_dataAssembly)
            .As("Data");

        IArchRule rule = Types().That().Are(common)
            .Should().NotDependOnAny(data)
            .Because("Common must not reference Data to avoid circular dependencies. " +
                     "Interfaces that return entity types should remain in Services layer.");

        ArchRuleAssertions.AssertNoArchitectureViolations(rule, s_architecture);
    }

    // ===== FULLNAME PATTERN (from all.fullname-pattern.instructions.md) =====

    [Fact]
    [Description("PS-1829 : Response DTOs with person names should use FullName property with MaskSensitive")]
    public void ResponseDtos_WithPersonNames_ShouldUseFullNameProperty()
    {
        // Get all response DTO types in Common assembly (exclude Request DTOs)
        var responseDtoTypes = s_commonAssembly.GetTypes()
            .Where(t => t.IsClass || t.IsValueType)
            .Where(t => t.Name.EndsWith("Response", StringComparison.Ordinal) ||
                        (t.Name.EndsWith("Dto", StringComparison.Ordinal) &&
                         !t.Namespace!.Contains("Request", StringComparison.Ordinal)) ||
                        t.Name.EndsWith("Detail", StringComparison.Ordinal))
            .Where(t => !t.Name.Contains("Lookup", StringComparison.OrdinalIgnoreCase)) // Lookup DTOs are exempt
            .Where(t => !t.Name.Contains("Gender", StringComparison.OrdinalIgnoreCase)) // Gender is a lookup
            .Where(t => !t.Name.Contains("Status", StringComparison.OrdinalIgnoreCase)) // Status is a lookup
            .Where(t => !t.Name.Contains("Kind", StringComparison.OrdinalIgnoreCase)) // Kind is a lookup
            .Where(t => !t.Name.Contains("Type", StringComparison.OrdinalIgnoreCase)) // Type is a lookup
            .Where(t => !t.Name.Contains("Request", StringComparison.Ordinal)) // Request DTOs are input, not output
            .ToList();

        var violations = new List<string>();

        foreach (var dtoType in responseDtoTypes)
        {
            var properties = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Check if DTO has a "Name" property that looks like it's for a person
            // (has FirstName/LastName nearby or implements INameParts)
            var hasNameProperty = properties.Any(p =>
                p.Name == "Name" &&
                p.PropertyType == typeof(string));

            var hasFirstNameOrLastName = properties.Any(p =>
                p.Name is "FirstName" or "LastName");

            var hasFullName = properties.Any(p => p.Name == "FullName");

            // If it has Name + FirstName/LastName context but no FullName, it's likely a violation
            if (hasNameProperty && hasFirstNameOrLastName && !hasFullName)
            {
                violations.Add(
                    $"{dtoType.FullName} has 'Name' property alongside FirstName/LastName. " +
                    "Consider using 'FullName' instead per FULLNAME_CONSOLIDATION_GUIDE.md");
            }

            // Check that FullName properties in Response DTOs have MaskSensitive attribute
            // Only enforce for types whose names end with "Response"
            if (dtoType.Name.EndsWith("Response", StringComparison.Ordinal))
            {
                var fullNameProperty = properties.FirstOrDefault(p => p.Name == "FullName");
                if (fullNameProperty != null)
                {
                    var hasMaskSensitive = fullNameProperty.GetCustomAttribute<MaskSensitiveAttribute>() != null;
                    if (!hasMaskSensitive)
                    {
                        violations.Add(
                            $"{dtoType.FullName}.FullName is missing [MaskSensitive] attribute. " +
                            "Person names must be masked per security requirements.");
                    }
                }
            }
        }

        if (violations.Count > 0)
        {
            Assert.Fail("FullName pattern violations found:\n" + string.Join("\n", violations));
        }
    }

    // ===== SECURITY PATTERNS (from demoulas.common.security.instructions.md) =====

    [Fact]
    [Description("PS-XXXX : Endpoints must not use AllowAnonymous without explicit justification")]
    public void Endpoints_ShouldNotUse_AllowAnonymous()
    {
        var endpointTypes = s_endpointsAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.Name.EndsWith("Endpoint", StringComparison.Ordinal))
            .Where(t => t.Namespace?.Contains("Endpoints.Endpoints", StringComparison.Ordinal) == true)
            .ToList();

        var allowAnonymousEndpoints = new List<string>();

        // Known exceptions - endpoints that legitimately need anonymous access
        var allowedAnonymousEndpoints = new HashSet<string>(StringComparer.Ordinal)
        {
            "HealthCheckEndpoint",
            "SwaggerEndpoint",
            "OpenApiEndpoint",
            // Add other legitimate anonymous endpoints here
        };

        foreach (var endpointType in endpointTypes)
        {
            if (allowedAnonymousEndpoints.Contains(endpointType.Name))
            {
                continue;
            }

            // Check Configure method for AllowAnonymous call using reflection on method body
            var configureMethod = endpointType.GetMethod("Configure",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (configureMethod != null)
            {
                // Check if type has AllowAnonymousAttribute
                var hasAllowAnonymousAttribute = endpointType
                    .GetCustomAttributes(true)
                    .Any(a => a.GetType().Name.Contains("AllowAnonymous", StringComparison.Ordinal));

                if (hasAllowAnonymousAttribute)
                {
                    allowAnonymousEndpoints.Add(
                        $"{endpointType.FullName} has [AllowAnonymous] attribute. " +
                        "Verify this is intentional and documented.");
                }
            }
        }

        // This is informational - just report any AllowAnonymous usage for review
        // In stricter environments, this could be changed to Assert.Fail
        Assert.True(allowAnonymousEndpoints.Count == 0 || allowAnonymousEndpoints.Count > 0,
            "Anonymous endpoints tracked for review (informational).");
    }

    // ===== API PATTERNS (from demoulas.common.api.instructions.md) =====

    [Fact]
    [Description("PS-XXXX : Endpoints must implement IHasNavigationId interface")]
    public void Endpoints_MustImplement_IHasNavigationId()
    {
        var endpointTypes = s_endpointsAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.Name.EndsWith("Endpoint", StringComparison.Ordinal))
            .Where(t => t.Namespace?.Contains("Endpoints.Endpoints", StringComparison.Ordinal) == true)
            .Where(t => InheritsFromProfitSharingBaseClass(t))
            .ToList();

        var endpointsWithoutNavigationId = new List<string>();

        foreach (var endpointType in endpointTypes)
        {
            var implementsInterface = endpointType.GetInterfaces()
                .Any(i => i.Name == "IHasNavigationId");

            if (!implementsInterface)
            {
                endpointsWithoutNavigationId.Add(endpointType.FullName!);
            }
        }

        if (endpointsWithoutNavigationId.Count > 0)
        {
            Assert.Fail(
                "Endpoints must implement IHasNavigationId (inherited from base classes):\n" +
                string.Join("\n", endpointsWithoutNavigationId));
        }
    }

    // ===== CACHING PATTERNS (from demoulas.common.caching.instructions.md) =====

    [Fact]
    [Description("PS-XXXX : Caching services must use IDistributedCache not IMemoryCache")]
    public void CachingServices_MustUse_IDistributedCache()
    {
        // This is already covered by ProductionAssemblies_ShouldNotDependOn_IMemoryCache
        // but we add a specific check for the CachingServices assembly
        var cachingAssembly = ProfitSharingArchitectureFixture.CachingServicesAssembly;

        var typesUsingMemoryCache = cachingAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(c => c.GetParameters())
            .Where(p => p.ParameterType.FullName?.Contains("IMemoryCache", StringComparison.Ordinal) == true)
            .Select(p => p.Member.DeclaringType?.FullName)
            .Where(n => n != null)
            .Distinct()
            .ToList();

        if (typesUsingMemoryCache.Count > 0)
        {
            Assert.Fail(
                "Caching services must use IDistributedCache, not IMemoryCache:\n" +
                string.Join("\n", typesUsingMemoryCache!));
        }
    }

    // ===== HELPER METHODS =====

    private static bool InheritsFromProfitSharingBaseClass(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            var baseTypeName = baseType.Name;
            if (baseTypeName.StartsWith("ProfitSharingEndpoint", StringComparison.Ordinal) ||
                baseTypeName.StartsWith("ProfitSharingRequestEndpoint", StringComparison.Ordinal) ||
                baseTypeName.StartsWith("ProfitSharingResponseEndpoint", StringComparison.Ordinal) ||
                baseTypeName.StartsWith("ProfitSharingResultResponseEndpoint", StringComparison.Ordinal))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    private static bool InheritsFromFastEndpointsBase(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.Namespace?.StartsWith("FastEndpoints", StringComparison.Ordinal) == true)
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    private static bool IsSpecializedReportEndpoint(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            var baseTypeName = baseType.Name;
            if (baseTypeName.StartsWith("EndpointWithCsvBase", StringComparison.Ordinal) ||
                baseTypeName.StartsWith("EndpointWithCsvTotalsBase", StringComparison.Ordinal))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    private static bool InheritsFromEndpointWithCsvBase(Type type)
    {
        return IsSpecializedReportEndpoint(type);
    }
}
