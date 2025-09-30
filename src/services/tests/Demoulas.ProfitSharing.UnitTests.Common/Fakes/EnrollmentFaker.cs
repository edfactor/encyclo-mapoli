using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class EnrollmentFaker : Faker<Enrollment>
{
    internal EnrollmentFaker()
    {
        var enrollments = new[]
        {
            new { Id = Enrollment.Constants.NotEnrolled, Name = "Not Enrolled" },
            new { Id = Enrollment.Constants.OldVestingPlanHasContributions, Name = "Old vesting plan has Contributions (7 years to full vesting)" },
            new { Id = Enrollment.Constants.NewVestingPlanHasContributions, Name = "New vesting plan has Contributions (6 years to full vesting)" }, 
            new { Id = Enrollment.Constants.OldVestingPlanHasForfeitureRecords, Name = "Old vesting plan has Forfeiture records" },
            new { Id = Enrollment.Constants.NewVestingPlanHasForfeitureRecords, Name = "New vesting plan has Forfeiture records" },
            new { Id = Enrollment.Constants.Import_Status_Unknown, Name = "Previous years enrollment is unknown. (History not previously tracked)" }
        };

        RuleFor(e => e.Id, f => f.PickRandom(enrollments).Id)
            .RuleFor(e => e.Name, (f, e) => enrollments.First(x => x.Id == e.Id).Name)
            .UseSeed(100);
    }
}