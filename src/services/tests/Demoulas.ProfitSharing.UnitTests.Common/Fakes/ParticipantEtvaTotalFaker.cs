using Bogus;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
public sealed class ParticipantEtvaTotalFaker : Faker<ParticipantTotal>
{
    public ParticipantEtvaTotalFaker(IList<ProfitDetail> profitDetails)
    {
        var demoSsns = profitDetails.Select(x=>x.Ssn).ToList();
        var ssnQueue = new Queue<int>(demoSsns);

        RuleFor(x => x.Ssn, (f, o) =>
        {
            if (!ssnQueue.Any())
            {
                ssnQueue = new Queue<int>(demoSsns);
            }

            return ssnQueue.Dequeue();
        })
        .RuleFor(x=>x.TotalAmount, f=>f.Random.Decimal(0,50000));
    }
}
