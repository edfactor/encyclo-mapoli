using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;

/// <summary>
///   Faker for <c>Beneficiary</c>
/// </summary>
internal sealed class BeneficiaryFaker : Faker<Beneficiary>
{
    private static int _iDCounter = 1000;
    
    /// <summary>
    ///   Initializes a default instance of <c>BeneficiaryFaker</c>
    /// </summary>
    internal BeneficiaryFaker()
    {
        RuleFor(d => d.Id, f => _iDCounter++);
        RuleFor(b => b.Psn, f => f.Random.Long(100_000_000, 9_999_999_999));
        RuleFor(b => b.Ssn, f => f.Person.Ssn().ConvertSsnToLong());
        RuleFor(b => b.FirstName, f => f.Name.FirstName());
        RuleFor(b => b.MiddleName, f => f.Name.FirstName().OrNull(f));
        RuleFor(b => b.LastName, f => f.Name.LastName());
        RuleFor(b => b.DateOfBirth, f => f.Date.Past(50, DateTime.Now.AddYears(-18)).ToDateOnly());
        RuleFor(pc => pc.Distribution, f => f.Finance.Amount(min: 100, max: 20_000, decimals: 2));
        RuleFor(pc => pc.Amount, f => f.Finance.Amount(min: 100, max: 20_000, decimals: 2));
        RuleFor(pc => pc.Earnings, f => f.Finance.Amount(min: 100, max: 20_000, decimals: 2));
        RuleFor(pc => pc.SecondaryEarnings, f => f.Finance.Amount(min: 100, max: 2_000, decimals: 2));
        
        RuleFor(b => b.Address, f => new Address
        {
            Street = f.Address.StreetAddress(),
            Street2 = f.Address.SecondaryAddress(),
            City = f.Address.City(),
            State = f.Address.StateAbbr(),
            PostalCode = f.Address.ZipCode(),
            CountryIso = Country.Constants.Us
        });
        RuleFor(b => b.ContactInfo, f => new ContactInfo
        {
            PhoneNumber = f.Phone.PhoneNumber("###-###-####"),
            MobileNumber = f.Phone.PhoneNumber("###-###-####"),
            EmailAddress = f.Internet.Email()
        });
        RuleFor(b => b.Kind, f => f.PickRandom(new List<BeneficiaryKind>
        {
            new BeneficiaryKind
            {
                Id = BeneficiaryKind.Constants.Primary, Name = "Primary",
            },
            new BeneficiaryKind
            {
                Id = BeneficiaryKind.Constants.Secondary, Name = "Secondary",

            }
        }));
    }
}
