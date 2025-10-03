using Bogus;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
public sealed class ParticipantTotalFaker : Faker<ParticipantTotal>
{
    public ParticipantTotalFaker(IList<Demographic> demographicFakes, IList<Beneficiary> beneficiaryFakes)
    {
        var demoSsns = demographicFakes.Select(x => x.Ssn).ToList();
        var beneSsns = beneficiaryFakes.Where(z => !demoSsns.Contains(z.Contact!.Ssn)).Select(x => x.Contact!.Ssn).ToList();
        var ssnQueue = new Queue<int>(demoSsns.Union(beneSsns));

        RuleFor(x => x.Ssn, (f, o) =>
        {
            if (!ssnQueue.Any())
            {
                ssnQueue = new Queue<int>(demoSsns.Union(beneSsns));
            }

            return ssnQueue.Dequeue();
        })
        .RuleFor(x => x.TotalAmount, f => f.Random.Decimal(0, 500000));
    }
}
