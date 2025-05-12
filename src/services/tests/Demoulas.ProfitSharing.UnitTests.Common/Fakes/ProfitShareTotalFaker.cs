using Bogus;
using Demoulas.ProfitSharing.Data.Entities.Virtual;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

public class ProfitShareTotalFaker : Faker<ProfitShareTotal>
{
    public ProfitShareTotalFaker()
    {
        CustomInstantiator(f => new ProfitShareTotal(
            f.Random.Decimal(1000, 5000), // WagesTotal
            f.Random.Decimal(100, 500),  // HoursTotal
            f.Random.Decimal(10, 50),    // PointsTotal
            f.Random.Decimal(500, 2000), // TerminatedWagesTotal
            f.Random.Decimal(50, 200),   // TerminatedHoursTotal
            f.Random.Decimal(5, 20),     // TerminatedPointsTotal
            f.Random.Int(50, 200),       // NumberOfEmployees
            f.Random.Int(5, 20),         // NumberOfNewEmployees
            f.Random.Int(1, 10)          // NumberOfEmployeesUnder21
        ));
    }
}
