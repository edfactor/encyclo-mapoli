using Bogus;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
internal sealed class ParticipantTotalVestingBalanceFaker : Faker<ParticipantTotalVestingBalance>
{
    public ParticipantTotalVestingBalanceFaker(IList<Demographic> demographicFakes, IList<Beneficiary> beneficiaryFakes)
    {
        var demoSsns = demographicFakes.Select(x => x.Ssn).ToList();
        var beneSsns = beneficiaryFakes.Where(z => !demoSsns.Contains(z.Contact!.Ssn)).Select(x => x.Contact!.Ssn).ToList();
        var ssnQueue = new Queue<int>(demoSsns.Union(beneSsns).Distinct());
        int junkSsn = int.MinValue;

        RuleFor(x => x.Id, (faker => faker.IndexFaker));
        _ = RuleFor(x => x.Ssn, (f, o) =>
        {
            if (!ssnQueue.Any())
            {
                return ++junkSsn; //Dups are bad in this table.
            }

            return ssnQueue.Dequeue();
        })
        .RuleFor(x => x.YearsInPlan, f => f.Random.Byte(0, 40))
        .RuleFor(x => x.VestingPercent, f => f.Random.Decimal(0, 1))
        .RuleFor(x => x.CurrentBalance, f => f.Random.Decimal(0, 500000))
        .RuleFor(x => x.VestedBalance, f => f.Random.Decimal(0, 500000));

    }
}
