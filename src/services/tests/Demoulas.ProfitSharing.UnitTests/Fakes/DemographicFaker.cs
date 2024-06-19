using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Enums;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Department = Demoulas.ProfitSharing.Common.Enums.Department;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;

internal sealed class DemographicFaker : Faker<Demographic>
{
    private static int _badgeNumberCounter = 1000;

    internal DemographicFaker()
    {
        ContactInfoFaker contactInfoFaker = new ContactInfoFaker();
        AddressFaker addressFaker = new AddressFaker();
        PayClassificationFaker payClassificationFaker = new PayClassificationFaker();


        RuleFor(d => d.BadgeNumber, f => _badgeNumberCounter++)
            .RuleFor(d => d.SSN, f => ConvertSsnToLong(f.Person.Ssn()))
            .RuleFor(d => d.OracleHcmId, f => f.Random.Long(100000, 999999))
            .RuleFor(d => d.LastName, f => f.Name.LastName())
            .RuleFor(d => d.FirstName, f => f.Name.FirstName())
            .RuleFor(d => d.MiddleName, f => f.Name.FirstName())
            .RuleFor(d => d.FullName, (f, d) => $"{d.FirstName} {d.LastName}")
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

    internal static long ConvertSsnToLong(string ssn)
    {
        // Remove non-numeric characters if any (e.g., dashes)
        string numericSsn = new string(ssn.Where(char.IsDigit).ToArray());

        // Convert to long
        return long.Parse(numericSsn);
    }
}
