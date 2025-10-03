using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class TerminationCodeFaker : Faker<TerminationCode>
{
    internal TerminationCodeFaker()
    {
        var terminationCodes = new[]
        {
            new { Id = TerminationCode.Constants.LeftOnOwn, Name = "Left On Own" },
            new { Id = TerminationCode.Constants.PersonalOrFamilyReason, Name = "Personal Or Family Reason" },
            new { Id = TerminationCode.Constants.CouldNotWorkAvailableHours, Name = "Could Not Work Available Hours" },
            new { Id = TerminationCode.Constants.Stealing, Name = "Stealing" },
            new { Id = TerminationCode.Constants.NotFollowingCompanyPolicy, Name = "Not Following Company Policy" },
            new { Id = TerminationCode.Constants.FmlaExpired, Name = "FMLA Expired" },
            new { Id = TerminationCode.Constants.TerminatedPrivate, Name = "Terminated Private" },
            new { Id = TerminationCode.Constants.JobAbandonment, Name = "Job Abandonment" },
            new { Id = TerminationCode.Constants.HealthReasonsNonFmla, Name = "Health Reasons Non-FMLA" },
            new { Id = TerminationCode.Constants.LayoffNoWork, Name = "Layoff No Work" },
            new { Id = TerminationCode.Constants.SchoolOrSports, Name = "School Or Sports" },
            new { Id = TerminationCode.Constants.MoveOutOfArea, Name = "Move Out of Area" },
            new { Id = TerminationCode.Constants.PoorPerformance, Name = "Poor Performance" },
            new { Id = TerminationCode.Constants.OffForSummer, Name = "Off For Summer" },
            new { Id = TerminationCode.Constants.WorkmansCompensation, Name = "Workmans Compensation" },
            new { Id = TerminationCode.Constants.Injured, Name = "Injured" },
            new { Id = TerminationCode.Constants.Transferred, Name = "Transferred" },
            new { Id = TerminationCode.Constants.Retired, Name = "Retired" },
            new { Id = TerminationCode.Constants.Competition, Name = "Competition" },
            new { Id = TerminationCode.Constants.AnotherJob, Name = "Another Job" },
            new { Id = TerminationCode.Constants.WouldNotRehire, Name = "Would Not Rehire" },
            new { Id = TerminationCode.Constants.NeverReported, Name = "Never Reported" },
            new { Id = TerminationCode.Constants.RetiredReceivingPension, Name = "Retired Receiving Pension" },
            new { Id = TerminationCode.Constants.Military, Name = "Military" },
            new { Id = TerminationCode.Constants.FmlaApproved, Name = "FMLA Approved" },
            new { Id = TerminationCode.Constants.Deceased, Name = "IsDeceased" }
        };

        RuleFor(tc => tc.Id, f => f.PickRandom(terminationCodes).Id)
            .RuleFor(tc => tc.Name, (f, tc) => terminationCodes.First(x => x.Id == tc.Id).Name)
            .UseSeed(100);
    }
}
