using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;

namespace Demoulas.ProfitSharing.UnitTests.Common.Helpers;

public static class OracleEmployeeFactory
{
    public static OracleEmployee[] Generate(int count = 1)
    {
        var faker = new DemographicFaker();
        var list = faker.Generate(count);
        return list.Select(d => d.ToOracleFromDemographic()).ToArray();
    }
}
