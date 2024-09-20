using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;
internal sealed class PayProfitFaker : Faker<PayProfit>
{
    internal PayProfitFaker(IList<Demographic> demographicFakes)
    {
        var demographicQueue = new Queue<Demographic>(demographicFakes);

        Demographic currentDemographic = demographicQueue.Dequeue();

        RuleFor(pc => pc.OracleHcmId, (f, o) => (currentDemographic.OracleHcmId))
            .RuleFor(d => d.Demographic, (f, o) =>
            {
                if (demographicQueue.Any()) // demographic record that contains the both of them
                {
                    // So by keeping a state field outside the lamdba, we can refer to an existing demographic
                    currentDemographic = demographicQueue.Dequeue(); // record and copy its values.
                }

                return currentDemographic;
            })
            .RuleFor(pc => pc.CurrentHoursYear, f => f.Random.Int(min: 0, max: 3000))
            .RuleFor(pc => pc.HoursExecutive, f => f.Random.Int(min: 0, max: 1000))
            .RuleFor(pc => pc.WeeksWorkedYear, f => f.Random.Byte(min: 0, max: 53))
            .RuleFor(pc => pc.CurrentIncomeYear, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2))
            .RuleFor(pc => pc.SecondaryEarnings, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2));

    }
}
