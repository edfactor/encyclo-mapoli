using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return true;
        }

        return false;
    }
}
