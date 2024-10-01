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
        RuleFor(b => b.Ssn, f => f.Person.Ssn().ConvertSsnToLong());
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
            CountryIso = Country.Constants.Us
        });
        RuleFor(b => b.ContactInfo, f => new ContactInfo
        {
            PhoneNumber = f.Phone.PhoneNumber("###-###-####"),
            MobileNumber = f.Phone.PhoneNumber("###-###-####"),
            EmailAddress = f.Internet.Email()
        });
    }
}
