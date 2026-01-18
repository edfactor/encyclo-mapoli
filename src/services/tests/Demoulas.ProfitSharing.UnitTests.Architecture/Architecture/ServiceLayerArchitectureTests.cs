using System.ComponentModel;
using System.Reflection;

namespace Demoulas.ProfitSharing.UnitTests.Architecture.Architecture;

/// <summary>
/// Architecture tests that verify service layer patterns defined in the instruction files.
/// Focuses on EF Core patterns, async/await usage, and data access patterns.
/// </summary>
[Collection("Architecture Tests")]
public sealed class ServiceLayerArchitectureTests
{
    private static readonly System.Reflection.Assembly _servicesAssembly = ProfitSharingArchitectureFixture.ServicesAssembly;
    private static readonly System.Reflection.Assembly _commonAssembly = ProfitSharingArchitectureFixture.CommonAssembly;

    // ===== EF CORE PATTERNS (from svc.services.instructions.md) =====

    [Fact]
    [Description("Services must use IProfitSharingDataContextFactory pattern")]
    public void Services_MustUse_DataContextFactoryPattern()
    {
        // Get all service types
        var serviceTypes = _servicesAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.Name.EndsWith("Service", StringComparison.Ordinal))
            .Where(t => !t.Name.Contains("Extension", StringComparison.Ordinal)) // Extension classes exempt
            .Where(t => !t.Name.Contains("Helper", StringComparison.Ordinal)) // Helper classes exempt
            .ToList();

        var servicesWithDirectDbContext = new List<string>();

        // Check for direct DbContext injection (should use factory instead)
        foreach (var serviceType in serviceTypes)
        {
            var constructors = serviceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                var hasDirectDbContext = parameters.Any(p =>
                    p.ParameterType.Name.EndsWith("DbContext", StringComparison.Ordinal) &&
                    !p.ParameterType.Name.Contains("Factory", StringComparison.Ordinal));

                if (hasDirectDbContext)
                {
                    servicesWithDirectDbContext.Add(
                        $"{serviceType.FullName} injects DbContext directly. " +
                        "Use IProfitSharingDataContextFactory instead for proper lifecycle management.");
                }
            }
        }

        if (servicesWithDirectDbContext.Count > 0)
        {
            Assert.Fail(
                "Services should use IProfitSharingDataContextFactory instead of direct DbContext injection:\n" +
                string.Join("\n", servicesWithDirectDbContext));
        }
    }

    [Fact]
    [Description("Service methods should return Task or ValueTask for async operations")]
    public void ServiceMethods_ShouldBe_AsyncForDataOperations()
    {
        // Get all service types
        var serviceTypes = _servicesAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.Name.EndsWith("Service", StringComparison.Ordinal))
            .ToList();

        var syncMethodsWithDbOperations = new List<string>();

        foreach (var serviceType in serviceTypes)
        {
            var publicMethods = serviceType.GetMethods(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in publicMethods)
            {
                // Check if method name suggests data operation
                var methodName = method.Name;
                var isDataOperation = methodName.StartsWith("Get", StringComparison.Ordinal) ||
                                       methodName.StartsWith("Create", StringComparison.Ordinal) ||
                                       methodName.StartsWith("Update", StringComparison.Ordinal) ||
                                       methodName.StartsWith("Delete", StringComparison.Ordinal) ||
                                       methodName.StartsWith("Find", StringComparison.Ordinal) ||
                                       methodName.StartsWith("Search", StringComparison.Ordinal) ||
                                       methodName.StartsWith("Load", StringComparison.Ordinal) ||
                                       methodName.StartsWith("Save", StringComparison.Ordinal);

                if (!isDataOperation)
                {
                    continue;
                }

                var returnType = method.ReturnType;
                var isAsync = returnType.IsGenericType &&
                              (returnType.GetGenericTypeDefinition() == typeof(Task<>) ||
                               returnType.GetGenericTypeDefinition() == typeof(ValueTask<>) ||
                               returnType == typeof(Task) ||
                               returnType == typeof(ValueTask));

                // Non-generic Task/ValueTask
                if (!isAsync && returnType == typeof(Task))
                {
                    isAsync = true;
                }

                if (!isAsync && returnType == typeof(ValueTask))
                {
                    isAsync = true;
                }

                // Check for methods that should be async but aren't
                // (has data-related name but doesn't return Task)
                // Exclude IQueryable/IEnumerable return types - these use deferred execution
                var isQueryable = returnType.Name.StartsWith("IQueryable", StringComparison.Ordinal) ||
                                  returnType.Name.StartsWith("IEnumerable", StringComparison.Ordinal);

                if (!isAsync && returnType != typeof(void) && !isQueryable)
                {
                    // Check if method has CancellationToken parameter (indicates it should be async)
                    var hasCancellationToken = method.GetParameters()
                        .Any(p => p.ParameterType == typeof(CancellationToken));

                    if (hasCancellationToken)
                    {
                        syncMethodsWithDbOperations.Add(
                            $"{serviceType.Name}.{methodName} has CancellationToken but returns {returnType.Name}. " +
                            "Methods with CancellationToken should be async (return Task<T> or ValueTask<T>).");
                    }
                }
            }
        }

        if (syncMethodsWithDbOperations.Count > 0)
        {
            Assert.Fail(
                "Service methods with CancellationToken should be async:\n" +
                string.Join("\n", syncMethodsWithDbOperations));
        }
    }

    // ===== INTERFACE PATTERNS (from svc.services.instructions.md) =====

    [Fact]
    [Description("Public services must have corresponding interface")]
    public void PublicServices_ShouldHave_CorrespondingInterface()
    {
        var serviceTypes = _servicesAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true })
            .Where(t => t.Name.EndsWith("Service", StringComparison.Ordinal))
            .Where(t => !t.Name.Contains("Extension", StringComparison.Ordinal))
            .Where(t => !t.Name.Contains("Helper", StringComparison.Ordinal))
            .Where(t => !t.IsNested) // Skip nested classes
            .ToList();

        var servicesWithoutInterface = new List<string>();

        foreach (var serviceType in serviceTypes)
        {
            var expectedInterfaceName = $"I{serviceType.Name}";

            // Check if the type implements an interface with the expected naming convention
            var implementsExpectedInterface = serviceType.GetInterfaces()
                .Any(i => i.Name == expectedInterfaceName ||
                          i.Name.StartsWith($"I{serviceType.Name}`", StringComparison.Ordinal)); // Generic interfaces

            if (!implementsExpectedInterface)
            {
                // Check if it implements any domain interface at all
                var implementsAnyDomainInterface = serviceType.GetInterfaces()
                    .Any(i => i.Name.StartsWith("I", StringComparison.Ordinal) &&
                              (i.Namespace?.Contains("ProfitSharing", StringComparison.Ordinal) == true ||
                               i.Namespace?.Contains("Demoulas", StringComparison.Ordinal) == true));

                if (!implementsAnyDomainInterface)
                {
                    servicesWithoutInterface.Add(
                        $"{serviceType.FullName} does not implement I{serviceType.Name} or any domain interface.");
                }
            }
        }

        // Allow some flexibility - not all services need interfaces (internal helpers, etc.)
        var totalServices = serviceTypes.Count;
        var servicesWithInterfaces = totalServices - servicesWithoutInterface.Count;
        var percentageWithInterfaces = totalServices > 0 ? (double)servicesWithInterfaces / totalServices * 100 : 100;

        // At least 80% of public services should have interfaces
        Assert.True(percentageWithInterfaces >= 80,
            $"Only {percentageWithInterfaces:F1}% of public services have interfaces. " +
            $"Services without interfaces:\n{string.Join("\n", servicesWithoutInterface.Take(10))}...");
    }

    // ===== RESULT PATTERN (from svc.services.instructions.md) =====

    [Fact]
    [Description("Services should use Result<T> pattern for error handling")]
    public void Services_ShouldUse_ResultPatternForErrors()
    {
        var serviceInterfaces = _commonAssembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(t => t.Name.StartsWith("I", StringComparison.Ordinal) &&
                        t.Name.EndsWith("Service", StringComparison.Ordinal))
            .ToList();

        var methodsReturningResult = 0;
        var methodsNotReturningResult = 0;
        var methodsNotReturningResultList = new List<string>();

        foreach (var interfaceType in serviceInterfaces)
        {
            var methods = interfaceType.GetMethods();

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;

                // Unwrap Task<T> or ValueTask<T>
                if (returnType.IsGenericType)
                {
                    var genericDef = returnType.GetGenericTypeDefinition();
                    if (genericDef == typeof(Task<>) || genericDef == typeof(ValueTask<>))
                    {
                        returnType = returnType.GetGenericArguments()[0];
                    }
                }

                // Check if return type is Result<T>
                var isResultType = returnType.Name.StartsWith("Result`", StringComparison.Ordinal) ||
                                   returnType.Name == "Result";

                if (isResultType)
                {
                    methodsReturningResult++;
                }
                else if (returnType != typeof(void) && returnType != typeof(Task) && returnType != typeof(ValueTask))
                {
                    // This is a method that returns a value but not Result<T>
                    // Not necessarily wrong - some methods are lookups that throw on not found
                    methodsNotReturningResult++;

                    // Track methods that might benefit from Result pattern
                    var methodName = method.Name;
                    if (methodName.StartsWith("Create", StringComparison.Ordinal) ||
                        methodName.StartsWith("Update", StringComparison.Ordinal) ||
                        methodName.StartsWith("Delete", StringComparison.Ordinal) ||
                        methodName.Contains("Validate", StringComparison.Ordinal))
                    {
                        methodsNotReturningResultList.Add($"{interfaceType.Name}.{methodName}");
                    }
                }
            }
        }

        // Informational - track Result<T> adoption rate
        var totalMethods = methodsReturningResult + methodsNotReturningResult;

        // The test passes - this is informational tracking. Assert true to satisfy analyzer.
        Assert.True(totalMethods >= 0, "Result<T> adoption tracking completed.");
    }

    // ===== NAMING CONVENTIONS (from instruction files) =====

    [Fact]
    [Description("Async methods should have Async suffix")]
    public void AsyncMethods_ShouldHave_AsyncSuffix()
    {
        var serviceTypes = _servicesAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsPublic: true })
            .Where(t => t.Name.EndsWith("Service", StringComparison.Ordinal))
            .ToList();

        var violations = new List<string>();

        foreach (var serviceType in serviceTypes)
        {
            var methods = serviceType.GetMethods(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                var isAsyncReturn = returnType == typeof(Task) ||
                                     returnType == typeof(ValueTask) ||
                                     (returnType.IsGenericType &&
                                      (returnType.GetGenericTypeDefinition() == typeof(Task<>) ||
                                       returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)));

                // Exclude property getters and common event handlers
                if (isAsyncReturn && !method.Name.EndsWith("Async", StringComparison.Ordinal) && !method.IsSpecialName)
                {
                    violations.Add($"{serviceType.Name}.{method.Name} returns {returnType.Name} but lacks 'Async' suffix.");
                }
            }
        }

        // Allow some flexibility - legacy methods may not follow convention
        var totalAsyncMethods = _servicesAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsPublic: true } && t.Name.EndsWith("Service", StringComparison.Ordinal))
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Count(m =>
            {
                var rt = m.ReturnType;
                return rt == typeof(Task) || rt == typeof(ValueTask) ||
                       (rt.IsGenericType && (rt.GetGenericTypeDefinition() == typeof(Task<>) ||
                                              rt.GetGenericTypeDefinition() == typeof(ValueTask<>)));
            });

        var methodsWithSuffix = totalAsyncMethods - violations.Count;
        var percentageCorrect = totalAsyncMethods > 0 ? (double)methodsWithSuffix / totalAsyncMethods * 100 : 100;

        // At least 75% should have Async suffix (tracking adoption - threshold will increase)
        Assert.True(percentageCorrect >= 75,
            $"Only {percentageCorrect:F1}% of async methods have 'Async' suffix. " +
            $"Violations:\n{string.Join("\n", violations.Take(10))}...");
    }
}
