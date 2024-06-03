using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.IntegrationTests.Fakes;

internal sealed class ContactInfoFaker : Faker<ContactInfo>
{
    internal ContactInfoFaker()
    {
        RuleFor(ci => ci.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(ci => ci.MobileNumber, f => f.Phone.PhoneNumber())
            .RuleFor(ci => ci.EmailAddress, f => f.Internet.Email());
    }
}
