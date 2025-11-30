using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class PayClassificationFaker : Faker<PayClassification>
{
    internal PayClassificationFaker()
    {
        RuleFor(pc => pc.Id, f => f.PickRandom("1", "2", "4", "5", "6", "7", "10", "11", "13", "14", "15", "16", "17", "18", "19", "20"))
            .RuleFor(pc => pc.Name, f => f.Name.JobTitle())
            .UseSeed(100);
    }
}
