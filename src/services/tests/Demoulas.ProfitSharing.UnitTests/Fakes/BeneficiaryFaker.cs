using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;

/// <summary>
///   Faker for <c>Beneficiary</c>
/// </summary>
internal sealed class BeneficiaryFaker : Faker<Beneficiary>
{
    /// <summary>
    ///   Initializes a default instance of <c>BeneficiaryFaker</c>
    /// </summary>
    internal BeneficiaryFaker()
    {
        RuleFor(b => b.PSN, f => f.Random.Long(1000000000, 9999999999));
        RuleFor(b => b.SSN, f => f.Person.Ssn().ConvertSsnToLong());
        RuleFor(b => b.FirstName, f => f.Name.FirstName());
        RuleFor(b => b.MiddleName, f => f.Name.FirstName().OrNull(f));
        RuleFor(b => b.LastName, f => f.Name.LastName());
        RuleFor(b => b.DateOfBirth, f => f.Date.Past(50, DateTime.Now.AddYears(-18)).ToDateOnly());
        RuleFor(b => b.Address, f => new Address
        {
            Street = f.Address.StreetAddress(),
            Street2 = f.Address.SecondaryAddress(),
            City = f.Address.City(),
            State = f.Address.StateAbbr(),
            PostalCode = f.Address.ZipCode(),
            CountryISO = Constants.US
        });
        RuleFor(b => b.ContactInfo, f => new ContactInfo
        {
            PhoneNumber = f.Phone.PhoneNumber("###-###-####"),
            MobileNumber = f.Phone.PhoneNumber("###-###-####"),
            EmailAddress = f.Internet.Email()
        });
        RuleFor(b => b.Type, f => f.PickRandom(new List<BeneficiaryType>
        {
            new BeneficiaryType
            {
                Id = BeneficiaryType.Constants.Beneficiary, Name = "Beneficiary",
            },
            new BeneficiaryType
            {
                Id = BeneficiaryType.Constants.Employee, Name = "Employee",

            }
        }));
    }
}
