using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;
internal sealed class PayProfitFaker : Faker<PayProfit>
{
    internal PayProfitFaker(IList<Demographic> demographicFakes)
    {
        var demographicQueue = new Queue<Demographic>(demographicFakes);

        Demographic currentDemographic = demographicQueue.Dequeue();

        RuleFor(pc => pc.PSN, (f,o) => (currentDemographic.BadgeNumber))
            .RuleFor(d => d.SSN, (f,o) => {                                    // This code is non-intuitive.   The idea is that when the demographic
                var rslt = currentDemographic.SSN;                             // association is made, we want the badge number and SSN to relate to existing
                if (demographicQueue.Any())                                    // demographic record that contains the both of them
                {                                                              // So by keeping a state field outside the lamdba, we can refer to an existing demographic
                    currentDemographic = demographicQueue.Dequeue();           // record and copy its values.
                }
                return rslt;
             })
            .RuleFor(pc => pc.HoursCurrentYear, f => f.Random.Int(min: 0, max: 3000))
            .RuleFor(pc => pc.HoursLastYear, f => f.Random.Int(min: 0, max: 3000))
            .RuleFor(pc => pc.WeeksWorkedYear, f => f.Random.Byte(min: 0, max: 53))
            .RuleFor(pc => pc.WeeksWorkedLastYear, f => f.Random.Byte(min: 0, max: 53))
            .RuleFor(pc => pc.CompanyContributionYears, f => f.Random.Byte(min: 0, max: 40))
            .RuleFor(pc => pc.EarningsCurrentYear, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2))
            .RuleFor(pc => pc.EarningsLastYear, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2))
            .RuleFor(pc => pc.EarningsAfterApplyingVestingRules, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2))
            .RuleFor(pc => pc.EarningsEtvaValue, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2))
            .RuleFor(pc => pc.EarningsPriorEtvaValue, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2))
            .RuleFor(pc => pc.SecondaryEarnings, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2))
            .RuleFor(pc => pc.SecondaryEtvaEarnings, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2));
    }
}
