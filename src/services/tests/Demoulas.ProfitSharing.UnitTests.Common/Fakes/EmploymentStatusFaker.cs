using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;
internal sealed class EmploymentStatusFaker : Faker<EmploymentStatus>
{
    internal EmploymentStatusFaker()
    {
        var employmentStatuses = new[]
        {
            new { Id = EmploymentStatus.Constants.Active, Name = "Active" },
            new { Id = EmploymentStatus.Constants.Inactive, Name = "Inactive" },
            new { Id = EmploymentStatus.Constants.Terminated, Name = "Terminated" },
            new { Id = EmploymentStatus.Constants.Delete, Name = "Delete" }
        };

        RuleFor(tc => tc.Id, fake => fake.PickRandom(employmentStatuses).Id)
            .RuleFor(tc => tc.Name, fake => fake.PickRandom(employmentStatuses).Name)
            .UseSeed(100);

    }
}
