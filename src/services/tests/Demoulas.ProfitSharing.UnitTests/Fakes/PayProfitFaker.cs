using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;
internal sealed class PayProfitFaker : Faker<PayProfit>
{
    internal PayProfitFaker()
    {
        RuleFor(pc => pc.EmployeeBadge, f => f.PickRandom<long>(int.MaxValue, uint.MaxValue))
            .RuleFor(d => d.EmployeeSSN, f => f.Person.Ssn().ConvertSsnToLong())
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
