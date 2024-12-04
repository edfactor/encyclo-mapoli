using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;

/// <summary>
///   Faker for <c>Beneficiary</c>
/// </summary>
internal sealed class BeneficiaryContactFaker : Faker<BeneficiaryContact>
{
    private static int _iDCounter = 1000;

    /// <summary>
    ///   Initializes a default instance of <c>BeneficiaryFaker</c>
    /// </summary>
    internal BeneficiaryContactFaker()
    {
        RuleFor(d => d.Id, f => _iDCounter++);
        RuleFor(b => b.Ssn, f => f.Person.Ssn().ConvertSsnToInt());
        RuleFor(b => b.DateOfBirth, f => f.Date.Past(50, DateTime.Now.AddYears(-18)).ToDateOnly());
        RuleFor(b => b.CreatedDate, f => f.Date.RecentDateOnly());

        RuleFor(b => b.Address,
            f => new Address
            {
                Street = f.Address.StreetAddress(),
                Street2 = f.Address.SecondaryAddress(),
                City = f.Address.City(),
                State = f.Address.StateAbbr(),
                PostalCode = f.Address.ZipCode(),
                CountryIso = Country.Constants.Us
            });
        RuleFor(ci => ci.ContactInfo.PhoneNumber, f => f.Phone.PhoneNumber("###-###-####"))
            .RuleFor(ci => ci.ContactInfo.MobileNumber, f => f.Phone.PhoneNumber("###-###-####"))
            .RuleFor(ci => ci.ContactInfo.EmailAddress, f => f.Internet.Email())
            .RuleFor(ci => ci.ContactInfo.FirstName, f => f.Name.FirstName())
            .RuleFor(ci => ci.ContactInfo.MiddleName, f => f.Name.FirstName().OrNull(f))
            .RuleFor(ci => ci.ContactInfo.LastName, f => f.Name.LastName())
            .RuleFor(d => d.ContactInfo.FullName, (f, d) => $"{d.ContactInfo.LastName}, {d.ContactInfo.FirstName}");
    }
}
