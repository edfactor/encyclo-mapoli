using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class AddressFaker : Faker<Address>
{
    internal AddressFaker()
    {
        RuleFor(a => a.Street, f => f.Address.StreetAddress())
            .RuleFor(a => a.Street2, f => f.Address.SecondaryAddress())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.State, f => f.Address.StateAbbr())
            .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
            .RuleFor(a => a.CountryIso, f => f.Address.CountryCode())
            .UseSeed(100);
    }
}
