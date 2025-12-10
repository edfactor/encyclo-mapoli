using System.Runtime.CompilerServices;

namespace Demoulas.ProfitSharing.UnitTests;

/// <summary>
/// Module initializer for the unit test project itself.
/// Ensures ASPNETCORE_ENVIRONMENT is set to "Testing" before any test code executes.
/// </summary>
/// <remarks>
/// In xUnit v3 / Microsoft Testing Platform, test projects are standalone executables.
/// The ApplicationName is the test project name (e.g., "Demoulas.ProfitSharing.UnitTests"),
/// NOT "testhost" as it was in xUnit v2. This breaks the IsTestEnvironment() check in
/// Demoulas.Util which checks for ApplicationName == "testhost".
/// 
/// By setting ASPNETCORE_ENVIRONMENT to "Testing" early, we ensure the environment check
/// passes regardless of ApplicationName.
/// </remarks>
internal static class TestEnvironmentInitializer
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
