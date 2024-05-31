using Bogus;
using Demoulas.ProfitSharing.Common.Enums;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.StoreInfo.Entities.Entities;
using Department = Demoulas.ProfitSharing.Common.Enums.Department;

namespace Demoulas.ProfitSharing.IntegrationTests.Mocks;

internal sealed class DemographicFaker : Faker<Demographic>
{
    internal DemographicFaker()
    {
        var contactInfoFaker = new Faker<ContactInfo>()
            .RuleFor(ci => ci.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(ci => ci.MobileNumber, f => f.Phone.PhoneNumber())
            .RuleFor(ci => ci.EmailAddress, f => f.Internet.Email());

        var addressFaker = new Faker<Address>()
            .RuleFor(a => a.Street, f => f.Address.StreetAddress())
            .RuleFor(a => a.Street2, f => f.Address.SecondaryAddress())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.State, f => f.Address.StateAbbr())
            .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
            .RuleFor(a => a.CountryISO, f => f.Address.CountryCode());

        var payClassificationFaker = new Faker<PayClassification>()
            .RuleFor(pc => pc.Id, f => f.PickRandom<byte>(1, 2, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20))
            .RuleFor(pc => pc.Name, f => f.Name.JobTitle());

            RuleFor(d => d.BadgeNumber, f => f.Random.Int(1000, 9999))
            .RuleFor(d => d.OracleHcmId, f => f.Random.Long(100000, 999999))
            .RuleFor(d => d.FullName, (f, d) => $"{d.FirstName} {d.LastName}")
            .RuleFor(d => d.LastName, f => f.Name.LastName())
            .RuleFor(d => d.FirstName, f => f.Name.FirstName())
            .RuleFor(d => d.MiddleName, f => f.Name.FirstName())
            .RuleFor(d => d.StoreNumber, f => f.Random.Short(1, 100))
            .RuleFor(d => d.Department, f => f.PickRandom<Department>())
            .RuleFor(d => d.PayClassification, f => payClassificationFaker.Generate())
            .RuleFor(d => d.PayClassificationId, f => f.PickRandom<byte>(1, 2, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20))
            .RuleFor(d => d.ContactInfo, f => contactInfoFaker.Generate())
            .RuleFor(d => d.Address, f => addressFaker.Generate())
            .RuleFor(d => d.DateOfBirth, f => f.Date.Past(30, DateTime.Now.AddYears(-18)).ToDateOnly())
            .RuleFor(d => d.FullTimeDate, f => f.Date.Past(10).ToDateOnly())
            .RuleFor(d => d.HireDate, f => f.Date.Past(15).ToDateOnly())
            .RuleFor(d => d.ReHireDate, f => f.Date.Past(5).ToDateOnly())
            .RuleFor(d => d.TerminationCode, f => f.PickRandom<TerminationCode>())
            .RuleFor(d => d.TerminationDate, f => f.Date.Past(5).ToDateOnly())
            .RuleFor(d => d.EmploymentType, f => f.PickRandom<EmploymentType>())
            .RuleFor(d => d.PayFrequency, f => f.PickRandom<PayFrequency>())
            .RuleFor(d => d.Gender, f => f.PickRandom<Gender>());
    }
}
