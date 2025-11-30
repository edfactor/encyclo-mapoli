using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class ContactInfoFaker : Faker<ContactInfo>
{
    internal ContactInfoFaker()
    {
        RuleFor(ci => ci.PhoneNumber, f => f.Phone.PhoneNumber("###-###-####"))
            .RuleFor(ci => ci.MobileNumber, f => f.Phone.PhoneNumber("###-###-####"))
            .RuleFor(ci => ci.EmailAddress, f => f.Internet.Email())
            .RuleFor(ci => ci.FirstName, f => f.Name.FirstName())
            .RuleFor(ci => ci.MiddleName, f => f.Name.FirstName().OrNull(f))
            .RuleFor(ci => ci.LastName, f => f.Name.FirstName())
            .RuleFor(d => d.FullName, (f, d) => $"{d.LastName}, {d.FirstName}")
            .UseSeed(100);
    }
}
