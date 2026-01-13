using System.Diagnostics;

namespace Demoulas.ProfitSharing.Common.ActivitySources;

public static class OracleHcmActivitySource
{

    public static readonly ActivitySource Instance = new ActivitySource("Synchronize OracleHcm");

}
