using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class FrozenStateFaker : Faker<FrozenState>
{
    internal FrozenStateFaker()
    {
        RuleFor(x => x.Id, (f, o) => 1);
        RuleFor(x => x.IsActive, (f, o) => true);
        RuleFor(x => x.ProfitYear, (f, o) => (short)2023);
        RuleFor(x => x.AsOfDateTime, (f, o) => DateTime.UtcNow)
            .UseSeed(100);
    }
}
