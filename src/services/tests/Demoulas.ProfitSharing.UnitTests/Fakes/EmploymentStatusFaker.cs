using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Fakes;
internal sealed class EmploymentStatusFaker : Faker<EmploymentStatus>
{
    internal EmploymentStatusFaker()
    {
        RuleFor(tc => tc.Id,
            fake => fake.PickRandom(EmploymentStatus.Constants.Active, EmploymentStatus.Constants.Delete, EmploymentStatus.Constants.Inactive,
                EmploymentStatus.Constants.Terminated));
            RuleFor(tc => tc.Name, fake => fake.Name.JobType());
    }
}
