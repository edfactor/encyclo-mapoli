using Bogus;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.UnitTests.Common.Fakes;

internal sealed class PayProfitFaker : Faker<PayProfit>
{
    private readonly HashSet<(long OracleHcmId, short ProfitYear)> _existingCombinations = new HashSet<(long, short)>();

    internal PayProfitFaker(IList<Demographic> demographicFakes)
    {
        var demographicQueue = new Queue<Demographic>(demographicFakes);
        Demographic currentDemographic = demographicQueue.Peek();

        RuleFor(d => d.Demographic, (f, o) =>
        {
            if (demographicQueue.Any())
            {
                currentDemographic = demographicQueue.Dequeue();
            }
            else
            {
                demographicQueue = new Queue<Demographic>(demographicFakes);
                currentDemographic = demographicQueue.Dequeue();
            }

            return currentDemographic;
        })
        .RuleFor(pc => pc.DemographicId, (f, o) => currentDemographic.Id)
        .RuleFor(pc => pc.ProfitYear, (f, o) =>
        {
            short profitYear;
            do
            {
                profitYear = f.PickRandom<short>(2018, 2019, 2020, 2021, 2022, 2023);
            }
            while (_existingCombinations.Contains((currentDemographic.Id, profitYear)));

            _existingCombinations.Add((currentDemographic.Id, profitYear));
            return profitYear;
        })
        .RuleFor(pc => pc.CurrentHoursYear, f => f.Random.Int(min: 0, max: 3000))
        .RuleFor(pc => pc.HoursExecutive, f => f.Random.Int(min: 0, max: 1000))
        .RuleFor(pc => pc.WeeksWorkedYear, f => f.Random.Byte(min: 0, max: 53))
        .RuleFor(pc => pc.CurrentIncomeYear, f => f.Finance.Amount(min: 100, max: 1_200_000, decimals: 2))
        .RuleFor(pc => pc.Etva, f => f.Finance.Amount(min: 10, max: 2_000, decimals: 2))
        .RuleFor(pc => pc.EnrollmentId, f => f.PickRandom(Enrollment.Constants.NotEnrolled,
            Enrollment.Constants.OldVestingPlanHasContributions,
            Enrollment.Constants.NewVestingPlanHasContributions,
            Enrollment.Constants.OldVestingPlanHasForfeitureRecords,
            Enrollment.Constants.NewVestingPlanHasForfeitureRecords,
            Enrollment.Constants.Import_Status_Unknown))
        .RuleFor(pc => pc.BeneficiaryTypeId, f => f.PickRandom(BeneficiaryType.Constants.Beneficiary,
            BeneficiaryType.Constants.Employee))
        .RuleFor(pc => pc.EmployeeTypeId, f => f.PickRandom(EmployeeType.Constants.NewLastYear,
            EmployeeType.Constants.NotNewLastYear))
        .RuleFor(pc => pc.ZeroContributionReasonId, f => f.PickRandom(ZeroContributionReason.Constants.Normal,
            ZeroContributionReason.Constants.Under21WithOver1Khours,
            ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested,
            ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
            ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay
            ))
        .UseSeed(100);
    }
}
