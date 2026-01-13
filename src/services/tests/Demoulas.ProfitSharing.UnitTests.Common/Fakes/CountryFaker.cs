using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class CountryFaker : Faker<Country>
{
    internal CountryFaker()
    {
        RuleFor(c => c.Id, f => f.Random.Byte(1, 255))
            .RuleFor(c => c.Name, f => f.Address.Country())
            .RuleFor(c => c.Iso, f => f.Address.CountryCode())
            .RuleFor(c => c.TelephoneCode, f => f.Address.CountryCode() + f.Random.Number(1, 9))
            .UseSeed(100);
    }
}
