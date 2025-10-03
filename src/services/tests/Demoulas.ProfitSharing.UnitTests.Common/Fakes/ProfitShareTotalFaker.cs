using Bogus;
using Demoulas.ProfitSharing.Data.Entities.Virtual;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

public class ProfitShareTotalFaker : Faker<ProfitShareTotal>
{
    public ProfitShareTotalFaker()
    {
        CustomInstantiator(f => new ProfitShareTotal
        {
            WagesTotal = f.Random.Decimal(1000, 5000), // WagesTotal
            HoursTotal = f.Random.Decimal(100, 500), // HoursTotal
            PointsTotal = f.Random.Decimal(10, 50), // PointsTotal
            BalanceTotal = f.Random.Decimal(500, 2000), // BalanceTotal
            NumberOfEmployees = f.Random.Int(50, 200), // NumberOfEmployees
            NumberOfNewEmployees = f.Random.Int(5, 20), // NumberOfNewEmployees
            NumberOfEmployeesUnder21 = f.Random.Int(1, 10) // NumberOfEmployeesUnder21
        });
    }
}
