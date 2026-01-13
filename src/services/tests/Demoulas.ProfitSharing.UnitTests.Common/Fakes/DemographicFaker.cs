using Bogus;
using Bogus.Extensions.UnitedStates;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

public sealed class DemographicFaker : Faker<Demographic>
{
    private static int _badgeNumberCounter = 10_000;
    private static int _idCounter = 10_000;
    private static long _oracleHcmIdCounter = 100_000;


    public DemographicFaker()
    {
        ContactInfoFaker contactInfoFaker = new ContactInfoFaker();
        AddressFaker addressFaker = new AddressFaker();
        PayClassificationFaker payClassificationFaker = new PayClassificationFaker();
        EmploymentStatusFaker employmentStatusFaker = new EmploymentStatusFaker();
        EmployeeTypeFaker employeeTypeFaker = new EmployeeTypeFaker();
        DepartmentFaker departmentFaker = new DepartmentFaker();

        RuleFor(d => d.BadgeNumber, f => _badgeNumberCounter++)
            .RuleFor(d => d.Id, f => _idCounter++)
            .RuleFor(d => d.Ssn, f => f.Person.Ssn().ConvertSsnToInt())
            .RuleFor(d => d.OracleHcmId, f => _oracleHcmIdCounter++)
            .RuleFor(d => d.StoreNumber, f => f.Random.Short(1, 99))
            .RuleFor(d => d.DepartmentId, f => f.PickRandom<byte>(Department.Constants.Grocery, Department.Constants.Bakery, Department.Constants.BeerAndWine, Department.Constants.Dairy, Department.Constants.Deli, Department.Constants.Meat, Department.Constants.Produce))
            .RuleFor(d => d.Department, f => departmentFaker.Generate())
            .RuleFor(d => d.PayClassification, f => payClassificationFaker.Generate())
            .RuleFor(d => d.PayClassificationId, f => f.PickRandom("1", "2", "4", "5", "6", "7", "10", "11", "13", "14", "15"))
            .RuleFor(d => d.ContactInfo, f => contactInfoFaker.Generate())
            .RuleFor(d => d.Address, f => addressFaker.Generate())
            .RuleFor(d => d.DateOfBirth, f => f.Date.Past(30, DateTime.Now.AddYears(-18)).ToDateOnly())
            .RuleFor(d => d.DateOfDeath, f => f.Random.Bool(0.1f) ? f.Date.Past(1, DateTime.Now).ToDateOnly() : null)
            .RuleFor(d => d.FullTimeDate, f => f.Date.Past(10, new DateTime(2024, 12, 01, 01, 01, 01, DateTimeKind.Local)).ToDateOnly())
            .RuleFor(d => d.HireDate, f => f.Date.Past(15, new DateTime(2024, 12, 01, 01, 01, 01, DateTimeKind.Local)).ToDateOnly())
            .RuleFor(d => d.ReHireDate, f => f.Date.Past(5, new DateTime(2024, 12, 01, 01, 01, 01, DateTimeKind.Local)).ToDateOnly())
            .RuleFor(d => d.TerminationCodeId, f => f.PickRandom(
                TerminationCode.Constants.LeftOnOwn,
                TerminationCode.Constants.PersonalOrFamilyReason,
                TerminationCode.Constants.CouldNotWorkAvailableHours,
                TerminationCode.Constants.Stealing,
                TerminationCode.Constants.NotFollowingCompanyPolicy,
                TerminationCode.Constants.FmlaExpired,
                TerminationCode.Constants.TerminatedPrivate,
                TerminationCode.Constants.JobAbandonment,
                TerminationCode.Constants.HealthReasonsNonFmla,
                TerminationCode.Constants.LayoffNoWork,
                TerminationCode.Constants.SchoolOrSports,
                TerminationCode.Constants.MoveOutOfArea,
                TerminationCode.Constants.PoorPerformance,
                TerminationCode.Constants.OffForSummer,
                TerminationCode.Constants.WorkmansCompensation,
                TerminationCode.Constants.Injured,
                TerminationCode.Constants.Transferred,
                TerminationCode.Constants.Retired,
                TerminationCode.Constants.Competition,
                TerminationCode.Constants.AnotherJob,
                TerminationCode.Constants.WouldNotRehire,
                TerminationCode.Constants.NeverReported,
                TerminationCode.Constants.RetiredReceivingPension,
                TerminationCode.Constants.Military,
                TerminationCode.Constants.FmlaApproved,
                TerminationCode.Constants.Deceased))
            .RuleFor(d => d.TerminationDate, f => f.Date.Past(5, new DateTime(2024, 12, 01, 01, 01, 01, DateTimeKind.Local)).ToDateOnly())
            .RuleFor(d => d.EmploymentTypeId, f => f.PickRandom<char>('P', 'H', 'G', 'F'))
            .RuleFor(d => d.EmploymentType, f => employeeTypeFaker.Generate())
            .RuleFor(d => d.PayFrequencyId, f => f.PickRandom<byte>(1, 2))
            .RuleFor(d => d.GenderId, f => f.PickRandom<char>('M', 'F', 'X'))
            .RuleFor(tc => tc.EmploymentStatus, f => employmentStatusFaker.Generate())
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
            .UseSeed(100);

    }
}
