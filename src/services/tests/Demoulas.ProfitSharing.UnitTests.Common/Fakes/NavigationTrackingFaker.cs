using Bogus;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class NavigationTrackingFaker : Faker<NavigationTracking>
{
    internal NavigationTrackingFaker()
    {
        RuleFor(m => m.Id, p => p.Random.Short(0, short.MaxValue));
        RuleFor(m => m.NavigationId, p => p.Random.Short(0, short.MaxValue));
        RuleFor(m => m.StatusId, p => p.Random.Byte(0, byte.MaxValue));
        RuleFor(m => m.Username, p => p.Random.String());
        RuleFor(m => m.LastModified, (p, o) => DateTimeOffset.UtcNow);

    }
}
