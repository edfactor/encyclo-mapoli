using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

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
        ContactInfoFaker contactInfoFaker = new ContactInfoFaker();

        RuleFor(d => d.Id, f => _iDCounter++)
            .RuleFor(b => b.Ssn, f => f.Person.Ssn().ConvertSsnToInt())
            .RuleFor(b => b.DateOfBirth, f => f.Date.Past(50, DateTime.Now.AddYears(-18)).ToDateOnly())
            .RuleFor(b => b.CreatedDate, f => f.Date.RecentDateOnly())

            .RuleFor(b => b.Address,
            f => new Address
            {
                Street = f.Address.StreetAddress(),
                Street2 = f.Address.SecondaryAddress(),
                City = f.Address.City(),
                State = f.Address.StateAbbr(),
                PostalCode = f.Address.ZipCode(),
                CountryIso = Country.Constants.Us
            })
            .RuleFor(d => d.ContactInfo, f => contactInfoFaker.Generate())
            .UseSeed(100);
    }
}
