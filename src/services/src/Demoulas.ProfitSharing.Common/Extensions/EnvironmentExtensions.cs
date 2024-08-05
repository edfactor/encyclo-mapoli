using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Common.Extensions;
public static class EnvironmentExtensions
{
    public static bool IsTestEnvironment(this IHostEnvironment environment)
    {
        if (environment.EnvironmentName == "Testing" ||
            environment.ApplicationName == "ReSharperTestRunner" ||
            environment.ApplicationName == "testhost")
        {
            environment.EnvironmentName = "Testing";

            return true;
        }

        return false;
    }
}
