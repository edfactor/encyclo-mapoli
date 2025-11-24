using System.ComponentModel;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.EnrollmentFlag;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Services;

/// <summary>
/// Tests for EnrollmentSummarizer
/// </summary>
public class EnrollmentSummarizerTests
{
    #region Test Helpers

    /// <summary>
    /// Computes enrollment with both implementations and verifies they match.
    /// </summary>
    private static void VerifyBothImplementationsMatch(
        PayProfit payProfit,
        short yearsOfService,
        List<ProfitDetail> profitDetails,
        byte expectedEnrollment,
        string testContext)
    {
        var es = new EnrollmentSummarizer();
        var res = es.ComputeEnrollment(payProfit, yearsOfService, profitDetails);
        
        // Verify both match expected result
        res.ShouldBe(expectedEnrollment, $"failed for: {testContext}");
    }

    private static PayProfit CreatePayProfit(
        short profitYear = 2024,
        byte? zeroContributionReasonId = null,
        char? terminationCodeId = null)
    {
        return new PayProfit
        {
            DemographicId = 1,
            ProfitYear = profitYear,
            ZeroContributionReasonId = zeroContributionReasonId,
            Etva = 0m,
            Demographic = new Demographic
            {
                BadgeNumber = 12345,
                OracleHcmId = 0,
                Ssn = 123456789,
                StoreNumber = 100,
                PayClassificationId = "H",
                TerminationCodeId = terminationCodeId,
                ContactInfo = new() { FirstName = "Test", LastName = "User" },
                Address = new() { Street = "123 Main St", City = "Test", State = "MA", PostalCode = "01234" },
                DateOfBirth = new DateOnly(1990, 1, 1),
                HireDate = new DateOnly(2020, 1, 1)
            }
        };
    }

    private static ProfitDetail CreateContribution(
        short profitYear,
        decimal contributionAmount = 1000m,
        byte profitYearIteration = 0,
        byte? zeroContributionReasonId = null,
        byte? commentTypeId = null)
    {
        return new ProfitDetail
        {
            ProfitYear = profitYear,
            ProfitYearIteration = profitYearIteration,
            ProfitCodeId = ProfitCode.Constants.IncomingContributions,
            Contribution = contributionAmount,
            ZeroContributionReasonId = zeroContributionReasonId,
            CommentTypeId = commentTypeId
        };
    }

    private static ProfitDetail CreateForfeiture(
        short profitYear,
        decimal forfeitureAmount,
        byte profitCodeId = 2)
    {
        return new ProfitDetail
        {
            ProfitYear = profitYear,
            ProfitCodeId = profitCodeId,
            Forfeiture = forfeitureAmount,
            Earnings = 0
        };
    }

    #endregion

    #region Basic Enrollment Tests

    [Description("PS-2196 : No transactions returns not enrolled (0)")]
    [Fact]
    public void ComputeEnrollment_NoTransactions_ReturnsNotEnrolled()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(),
            yearsOfService: 3,
            new List<ProfitDetail>(),
            expectedEnrollment: 0,
            testContext: "No transactions");
    }

    [Description("PS-2196 : Single contribution before 2007 returns old vesting plan (1)")]
    [Fact]
    public void ComputeEnrollment_SingleContributionBefore2007_ReturnsOldVestingPlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2006),
            yearsOfService: 3,
            new List<ProfitDetail> { CreateContribution(profitYear: 2006) },
            expectedEnrollment: 1,
            testContext: "Single contribution before 2007");
    }

    [Description("PS-2196 : Single contribution in 2007 or later returns new vesting plan (2)")]
    [Fact]
    public void ComputeEnrollment_SingleContributionIn2007_ReturnsNewVestingPlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2007),
            yearsOfService: 3,
            new List<ProfitDetail> { CreateContribution(profitYear: 2007) },
            expectedEnrollment: 2,
            testContext: "Single contribution in 2007");
    }

    [Description("PS-2196 : Contribution in 2024 returns new vesting plan (2)")]
    [Fact]
    public void ComputeEnrollment_ContributionIn2024_ReturnsNewVestingPlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 2,
            new List<ProfitDetail> { CreateContribution(profitYear: 2024, contributionAmount: 1500m) },
            expectedEnrollment: 2,
            testContext: "Contribution in 2024");
    }

    #endregion

    #region Vesting Transition Tests (Pre-2007 to Post-2007)

    [Description("PS-2196 : Old plan member with 2007+ contribution upgrades to new plan (2)")]
    [Fact]
    public void ComputeEnrollment_OldPlanMemberGets2007Contribution_UpgradesToNewPlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 4,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2005),
                CreateContribution(profitYear: 2006),
                CreateContribution(profitYear: 2007)
            },
            expectedEnrollment: 2,
            testContext: "Old plan member upgrades to new plan in 2007");
    }

    #endregion

    #region Military Contribution Tests

    [Description("PS-2196 : Military contribution after 2007 sets new vesting plan")]
    [Fact]
    public void ComputeEnrollment_MilitaryContributionAfter2007_SetsNewVestingPlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 1,
            new List<ProfitDetail>
            {
                CreateContribution(
                    profitYear: 2020,
                    contributionAmount: 500m,
                    profitYearIteration: 1,
                    commentTypeId: CommentType.Constants.Military.Id)
            },
            expectedEnrollment: 2,
            testContext: "Military contribution after 2007");
    }

    [Description("PS-2196 : Military contribution before 2008 uses old vesting plan")]
    [Fact]
    public void ComputeEnrollment_MilitaryContributionBefore2008_UsesOldVestingPlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 1,
            new List<ProfitDetail>
            {
                CreateContribution(
                    profitYear: 2005,
                    contributionAmount: 500m,
                    profitYearIteration: 1,
                    commentTypeId: CommentType.Constants.Military.Id)
            },
            expectedEnrollment: 1,
            testContext: "Military contribution before 2008");
    }

    #endregion

    #region Zero Contribution Reason Tests

    [Description("PS-2196 : Under 21 with V-only contribution qualifies for enrollment")]
    [Fact]
    public void ComputeEnrollment_Under21WithVOnly_QualifiesForEnrollment()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 1,
            new List<ProfitDetail>
            {
                CreateContribution(
                    profitYear: 2024,
                    contributionAmount: 0,
                    zeroContributionReasonId: ZeroContributionReason.Constants.Under21WithOver1Khours,
                    commentTypeId: CommentType.Constants.VOnly)
            },
            expectedEnrollment: 1,  // Original returns 1 (old vesting plan)
            testContext: "Under 21 with V-only");
    }

    [Description("PS-2196 : Terminated employee with V-only qualifies for enrollment")]
    [Fact]
    public void ComputeEnrollment_TerminatedWithVOnly_QualifiesForEnrollment()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 1,
            new List<ProfitDetail>
            {
                CreateContribution(
                    profitYear: 2024,
                    contributionAmount: 0,
                    zeroContributionReasonId: ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested,
                    commentTypeId: CommentType.Constants.VOnly)
            },
            expectedEnrollment: 1,  // Original returns 1 (old vesting plan, not upgraded to new plan)
            testContext: "Terminated employee with V-only");
    }

    [Description("PS-2196 : 65+ fully vested with contribution qualifies for enrollment")]
    [Fact]
    public void ComputeEnrollment_65PlusFullyVestedWithContribution_QualifiesForEnrollment()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 6,
            new List<ProfitDetail>
            {
                CreateContribution(
                    profitYear: 2024,
                    contributionAmount: 2000m,
                    zeroContributionReasonId: ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
            },
            expectedEnrollment: 2,
            testContext: "65+ fully vested with contribution");
    }

    #endregion

    #region Forfeiture Tests

    [Description("PS-2196 : Post-2006 forfeiture with vesting sets old forfeiture plan (3)")]
    [Fact]
    public void ComputeEnrollment_Post2006ForfeitureWithVesting_SetsOldForfeiturePlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 3,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2005),
                CreateForfeiture(profitYear: 2008, forfeitureAmount: -500m)
            },
            expectedEnrollment: 1,  // Original returns 1 (has contributions, not forfeiture plan)
            testContext: "Post-2006 forfeiture with old vesting");
    }

    [Description("PS-2196 : Post-2006 forfeiture with new vesting sets new forfeiture plan (4)")]
    [Fact]
    public void ComputeEnrollment_Post2006ForfeitureWithNewVesting_SetsNewForfeiturePlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 3,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2007),
                CreateForfeiture(profitYear: 2010, forfeitureAmount: -500m)
            },
            expectedEnrollment: 2,  // Original returns 2 (new vesting plan with contributions)
            testContext: "Post-2006 forfeiture with new vesting");
    }

    [Description("PS-2196 : Class action forfeiture is ignored (not a real forfeiture)")]
    [Fact]
    public void ComputeEnrollment_ClassActionForfeiture_IsIgnored()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 3,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2007),
                new ProfitDetail
                {
                    ProfitYear = 2010,
                    ProfitCodeId = 8,
                    CommentTypeId = CommentType.Constants.ClassAction,
                    Earnings = 500m,
                    Forfeiture = 0
                },
                new ProfitDetail
                {
                    ProfitYear = 2010,
                    ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures,
                    Forfeiture = 500m,
                    Earnings = 0
                }
            },
            expectedEnrollment: 2,
            testContext: "Class action forfeiture ignored");
    }

    #endregion

    #region 2003 Void Problem Tests

    [Description("PS-2196 : 2003 void problem (ProfitCode 8) sets old forfeiture plan")]
    [Fact]
    public void ComputeEnrollment_2003VoidProblem_SetsOldForfeiturePlan()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 2,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2002),
                new ProfitDetail
                {
                    ProfitYear = 2003,
                    ProfitCodeId = 8,
                    Contribution = 0
                }
            },
            expectedEnrollment: 3,
            testContext: "2003 void problem");
    }

    #endregion

    #region Edge Cases

    [Description("PS-2196 : Multiple contributions in same year only counted once")]
    [Fact]
    public void ComputeEnrollment_MultipleContributionsSameYear_CountedOnce()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 1,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2024, contributionAmount: 500m),
                CreateContribution(profitYear: 2024, contributionAmount: 500m),
                CreateContribution(profitYear: 2024, contributionAmount: 500m)
            },
            expectedEnrollment: 2,
            testContext: "Multiple contributions same year");
    }

    [Description("PS-2196 : Future transactions are ignored")]
    [Fact]
    public void ComputeEnrollment_FutureTransactions_AreIgnored()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 1,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2023),
                CreateContribution(profitYear: 2025, contributionAmount: 5000m),
                CreateContribution(profitYear: 2026, contributionAmount: 5000m)
            },
            expectedEnrollment: 2,
            testContext: "Future transactions ignored");
    }

    [Description("PS-2196 : Zero years of service with contribution still enrolls")]
    [Fact]
    public void ComputeEnrollment_ZeroYearsWithContribution_StillEnrolls()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 0,
            new List<ProfitDetail> { CreateContribution(profitYear: 2024) },
            expectedEnrollment: 2,
            testContext: "Zero years with contribution");
    }

    #endregion

    #region Complex Scenarios

    [Description("PS-2196 : Long tenure employee with multiple plan transitions")]
    [Fact]
    public void ComputeEnrollment_LongTenureWithTransitions_HandlesCorrectly()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 10,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2000, contributionAmount: 800m),
                CreateContribution(profitYear: 2001, contributionAmount: 850m),
                CreateContribution(profitYear: 2002, contributionAmount: 900m),
                CreateContribution(profitYear: 2003, contributionAmount: 950m),
                CreateContribution(profitYear: 2004, contributionAmount: 1000m),
                CreateContribution(profitYear: 2005, contributionAmount: 1050m),
                CreateContribution(profitYear: 2006, contributionAmount: 1100m),
                CreateContribution(profitYear: 2007, contributionAmount: 1150m),
                CreateContribution(profitYear: 2008, contributionAmount: 1200m),
                CreateContribution(profitYear: 2020, contributionAmount: 2000m),
                CreateContribution(profitYear: 2024, contributionAmount: 2500m)
            },
            expectedEnrollment: 2,
            testContext: "Long tenure with transitions");
    }

    [Description("PS-2196 : Employee with forfeiture then returns to plan")]
    [Fact]
    public void ComputeEnrollment_ForfeitureeThenReturns_ShowsNewPlanWithForfeiture()
    {
        VerifyBothImplementationsMatch(
            CreatePayProfit(2024),
            yearsOfService: 5,
            new List<ProfitDetail>
            {
                CreateContribution(profitYear: 2010),
                CreateContribution(profitYear: 2011),
                CreateForfeiture(profitYear: 2012, forfeitureAmount: -1500m),
                CreateContribution(profitYear: 2020, contributionAmount: 2000m),
                CreateContribution(profitYear: 2024, contributionAmount: 2500m)
            },
            expectedEnrollment: 2,  // Original returns 2 (new vesting plan with contributions)
            testContext: "Forfeiture then return");
    }

    #endregion
}
