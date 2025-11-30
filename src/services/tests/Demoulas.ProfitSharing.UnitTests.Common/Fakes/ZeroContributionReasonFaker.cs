using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class ZeroContributionReasonFaker : Faker<ZeroContributionReason>
{
    internal ZeroContributionReasonFaker()
    {
        var zeroContributionReasons = new[]
        {
            new { Id = ZeroContributionReason.Constants.Normal, Name = "Normal" },
            new { Id = ZeroContributionReason.Constants.Under21WithOver1Khours, Name = "18, 19, OR 20 WITH > 1000 HOURS" },
            new { Id = ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested, Name = "TERMINATED EMPLOYEE > 1000 HOURS WORKED GETS YEAR VESTED" },
            new { Id = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested, Name = ">=65 AND 1st CONTRIBUTION >= 5 YEARS AGO GETS 100% VESTED" },
            new { Id = ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay, Name = "=64 AND 1ST CONTRIBUTION >=5 YEARS AGO GETS 100% VESTED ON THEIR BIRTHDAY" }
        };

        RuleFor(zcr => zcr.Id, f => f.PickRandom(zeroContributionReasons).Id)
            .RuleFor(zcr => zcr.Name, (f, zcr) => zeroContributionReasons.First(x => x.Id == zcr.Id).Name)
            .UseSeed(100);
    }
}
