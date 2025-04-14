using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
public sealed class ParticipantTotalFaker : Faker<ParticipantTotal>
{
    public ParticipantTotalFaker(IList<Demographic> demographicFakes, IList<BeneficiaryContact> beneficiaryContactFakes)
    {
        var demoSsns = demographicFakes.Select(x=>x.Ssn).ToList();
        var beneSsns = beneficiaryContactFakes.Where(z=>!demoSsns.Contains(z.Ssn)).Select(x => x.Ssn).ToList();
        var ssnQueue = new Queue<int>(demoSsns.Union(beneSsns));

        RuleFor(x => x.Ssn, (f, o) =>
        {
            if (!ssnQueue.Any())
            {
                ssnQueue = new Queue<int>(demoSsns.Union(beneSsns));
            }

            return ssnQueue.Dequeue();
        })
        .RuleFor(x=>x.Total, f=>f.Random.Decimal(0,500000));
    }
}
