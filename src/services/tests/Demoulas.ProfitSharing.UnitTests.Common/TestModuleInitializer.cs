using System.Runtime.CompilerServices;

namespace Demoulas.ProfitSharing.UnitTests.Common;

/// <summary>
/// Module initializer that runs before any test code executes.
/// This ensures the ASPNETCORE_ENVIRONMENT is set to "Testing" before
/// WebApplicationFactory or any ASP.NET Core host is created.
/// </summary>
/// <remarks>
/// In xUnit v3 / Microsoft Testing Platform, test projects are standalone executables.
/// The WebApplicationFactory may initialize the host before our test class constructors run.
/// This module initializer ensures the environment is set at the earliest possible point.
/// </remarks>
internal static class TestModuleInitializer
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        // Set the environment to Testing before any ASP.NET Core host is created.
        // This is critical for xUnit v3 / Microsoft Testing Platform compatibility.
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");

        // Also set YEMATCH_USE_TEST_CERTS as a fallback mechanism.
        // SecurityExtension.cs checks for this when IsTestEnvironment() returns false.
        Environment.SetEnvironmentVariable("YEMATCH_USE_TEST_CERTS", "true");
    }
}
