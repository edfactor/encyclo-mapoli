using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services.EnrollmentFlag;

/// <summary>
/// Summarizes enrollment status by analyzing profit detail transaction history.
/// The enrollment flag (enrollment_id / py_ps_enrolled) indicates at a glance if an employee
/// is new, returning, or has forfeited and/or left the plan.
/// </summary>
/// <remarks>
/// This logic is extracted from PAY450.cbl.
/// By scanning profit_detail transactions, we determine one of 5 enrollment states:
/// <list type="bullet">
/// <item><description>0 - Not enrolled</description></item>
/// <item><description>1 - Old vesting plan has Contributions (7 years to full vesting)</description></item>
/// <item><description>2 - New vesting plan has Contributions (6 years to full vesting)</description></item>
/// <item><description>3 - Old vesting plan has Forfeiture records</description></item>
/// <item><description>4 - New vesting plan has Forfeiture records</description></item>
/// </list>
/// </remarks>
internal class EnrollmentSummarizer
{
    private readonly IVestingScheduleService _vestingScheduleService;

    public EnrollmentSummarizer(IVestingScheduleService vestingScheduleService)
    {
        _vestingScheduleService = vestingScheduleService;
    }

    // Used to ensure we only process each year once.  For some years there are multiple contributions in a single year, but we only care about a single contribution instance per year
    // COBOL: HDLD_YEAR
    private int LastYearSeen { get; set; }

    // COBOL: IN_PLAN_FLAG
    private byte EnrollmentId { get; set; }

    // COBOL: PFORF_01
    private decimal Forfeiture { get; set; }

    // COBOL: W_PC_VEST
    private VestingStateType VestedState { get; set; }

    // COBOL: WS_2003_VOID_PROBLEM
    private bool Is2003VoidProblem { get; set; }

    // COBOL: WS_AFTER_2006 
    private bool IsAfter2006 { get; set; }

    // COBOL: WS_NEW_VESTING_RULES
    private bool IsNewVestingRules { get; set; }

    // Cached new plan effective year loaded from database (default 2007)
    private int _newPlanEffectiveYear = 2007;

    public async Task<byte> ComputeEnrollmentAsync(PayProfit pp, short years, List<ProfitDetail> pds, CancellationToken ct)
    {
        if (pds.Count == 0)
        {
            return 0;
        }

        // Load new plan effective year from database (cached after first load)
        _newPlanEffectiveYear = await _vestingScheduleService.GetNewPlanEffectiveYearAsync(ct);

        // COBOL: 300-PROCESS-PS SECTION

        foreach (ProfitDetail pd in pds)
        {
            ExamineProfitDetailRow(pds, pd, pp.ProfitYear);
        }

        await ComputeVestingAsync(years, pp, ct);

        return GetEnrolled();
    }

    // COBOL: 530_PC_VESTED
    // Determines vesting state based on years of service and vesting schedule.
    // READY (PAY450) skips forfeiture enrollment updates when vesting percent is 0%.
    // This matches that behavior by using actual vesting percentage, not just years > 0.
    //
    // COBOL PAY450 vesting table (P-VEST): "000000020040060080100"
    // This is a 7-entry table where each entry is 3 digits (PIC 9V99):
    //   P-VEST(1)=0%, P-VEST(2)=0%, P-VEST(3)=20%, P-VEST(4)=40%, P-VEST(5)=60%, P-VEST(6)=80%, P-VEST(7)=100%
    //
    // COBOL maps years to table index differently for old vs new vesting:
    //   Old vesting (530-OLD-VESTING): Year 0→ZERO, Year 1→P-VEST(1), Year 2→P-VEST(2), etc.
    //   New vesting (530-NEW-VESTING): Year 0→P-VEST(1), Year 1→P-VEST(2), Year 2→P-VEST(3), etc.
    //
    // Net effect for NEW vesting (the common case):
    //   Year 0 → 0%, Year 1 → 0%, Year 2 → 20%, Year 3 → 40%, Year 4 → 60%, Year 5 → 80%, Year 6+ → 100%
    //
    // This means employees with 0-1 years under new vesting have 0% vesting and should NOT
    // have their enrollment changed to 3/4 (forfeited) since there's no vested money to protect.
    private async Task ComputeVestingAsync(short years, PayProfit pp, CancellationToken ct)
    {
        VestedState = VestingStateType.NotVested;

        // Determine which vesting schedule to use (old plan = 1, new plan = 2)
        int scheduleId = IsNewVestingRules
            ? VestingSchedule.Constants.NewPlan
            : VestingSchedule.Constants.OldPlan;

        // Retrieve vesting percentage from database-driven service
        decimal vestingPercent = await _vestingScheduleService.GetVestingPercentAsync(scheduleId, years, ct);

        // Only consider vested if actual vesting percent > 0
        // This matches PAY450's "IF W-PC-VEST = 0 GO TO 310-PART-II" logic
        if (vestingPercent > 0)
        {
            VestedState = VestingStateType.PartiallyVested;
            if (vestingPercent >= 100)
            {
                VestedState = VestingStateType.FullyVested;
            }
        }

        if (EnrollmentId == /*3*/ EnrollmentConstants.OldVestingPlanHasForfeitureRecords)
        {
            VestedState = VestingStateType.PartiallyVested;
        }

        if (pp.ZeroContributionReasonId == /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested ||
            pp.Demographic!.TerminationCodeId == /*Z*/TerminationCode.Constants.Deceased)
        {
            VestedState = VestingStateType.FullyVested;
        }
    }

    // COBOL: 310-Get-Balance
    public byte GetEnrolled()
    {
        if (IsNewVestingRules && EnrollmentId == /*1*/ EnrollmentConstants.OldVestingPlanHasContributions)
        {
            EnrollmentId = /*2*/ EnrollmentConstants.NewVestingPlanHasContributions;
        }

        if (VestedState != VestingStateType.NotVested)
        {
            if (Forfeiture != 0 && IsAfter2006)
            {
                EnrollmentId = /*3*/ EnrollmentConstants.OldVestingPlanHasForfeitureRecords;
            }

            if (IsNewVestingRules)
            {
                if (EnrollmentId == /*1*/ EnrollmentConstants.OldVestingPlanHasContributions)
                {
                    EnrollmentId = /*2*/ EnrollmentConstants.NewVestingPlanHasContributions;
                }

                if (EnrollmentId == /*3*/ EnrollmentConstants.OldVestingPlanHasForfeitureRecords)
                {
                    EnrollmentId = /*4*/ EnrollmentConstants.NewVestingPlanHasForfeitureRecords;
                }
            }
        }

        return EnrollmentId;
    }

    // COBOL: 330-GET-DETAIL-DATA.
    private void ExamineProfitDetailRow(List<ProfitDetail> pds, ProfitDetail pd, short profitYear)
    {
        if (pd.ProfitYear > profitYear)
        {
            return;
        }

        decimal profitFort = pd.Forfeiture * -1;

        if (pd.ProfitCodeId == /*0*/ ProfitCode.Constants.IncomingContributions)
        {
            NumberOfYears(pd);
        }

        //* Any contribution given in new plan year or after will cause
        //* the new vesting rules to be in effect. 
        if (pd.ProfitYear >= _newPlanEffectiveYear && pd.Contribution > 0 && pd.ProfitCodeId != 6)
        {
            IsNewVestingRules = true;
        }

        if (pd is { ProfitYear: 2003, ProfitCodeId: 8 })
        {
            Is2003VoidProblem = true;
            EnrollmentId = EnrollmentConstants.OldVestingPlanHasForfeitureRecords;
        }

        if (pd.ProfitCodeId != /*2*/ ProfitCode.Constants.OutgoingForfeitures)
        {
            return;
        }

        if (pd.ProfitYear >= _newPlanEffectiveYear)
        {
            // Track if forfeitures occurred in new plan era
            IsAfter2006 = profitFort < 0;
        }

        if (IsRealForfeitAndNotClassActionForfeit(pds, pd))
        {
            Forfeiture += profitFort;
        }
    }

    // If the current row is a forfeit of class action, then we ignore it.
    private static bool IsRealForfeitAndNotClassActionForfeit(List<ProfitDetail> pds, ProfitDetail forfeitPd)
    {
        decimal classActionAndEarnings = pds.Where(pd =>
                pd.ProfitCodeId == 8 && (pd.CommentTypeId == CommentType.Constants.ClassAction || pd.CommentTypeId == CommentType.Constants.OneHundredPercentEarnings))
            .Sum(pd => pd.Earnings);
        return classActionAndEarnings != forfeitPd.Forfeiture;
    }


    // COBOL: 520-NO-OF-YEARS
    private void NumberOfYears(ProfitDetail pd)
    {
        if (pd.ProfitYearIteration is 1 /*Military*/ or 2 /*Class Action*/)
        {
            if (pd.ProfitYearIteration == 1 /*Military*/ && pd.CommentTypeId == CommentType.Constants.Military.Id)
            {
                if (pd.ProfitYear >= _newPlanEffectiveYear)
                {
                    IsNewVestingRules = true;
                }

                EnrollmentId = /*1*/ EnrollmentConstants.OldVestingPlanHasContributions;
                LastYearSeen = pd.ProfitYear;
            }
        }
        else
        {
            if (pd.ProfitYear != LastYearSeen)
            {
                byte zeroCont = pd.ZeroContributionReasonId ?? 0;
                if (pd.ZeroContributionReasonId is < /*0*/ ZeroContributionReason.Constants.Normal or > /*7*/
                    ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay)
                {
                    zeroCont = /*0*/ ZeroContributionReason.Constants.Normal;
                }

                switch (zeroCont)
                {
                    case /*0*/ ZeroContributionReason.Constants.Normal
                        when pd.Contribution != 0:
                    case /*1*/ ZeroContributionReason.Constants.Under21WithOver1Khours
                        when pd.CommentTypeId == CommentType.Constants.VOnly && pd.Contribution == 0:
                    case /*2*/ ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested
                        when pd.CommentTypeId == CommentType.Constants.VOnly && pd.Contribution == 0:

#pragma warning disable CS0618 // Type or member is obsolete
                    case /*3*/ ZeroContributionReason.Constants.Over64WithLess1000Hours1YearVesting
                        when pd.ProfitYear < 2002:
                    case /*4*/ ZeroContributionReason.Constants.Over64WithLess1000Hours2YearsVesting
                        when pd.ProfitYear < 2002:
                    case /*5*/ ZeroContributionReason.Constants.Over64WithOver1000Hours3YearsVesting
                        when pd.ProfitYear < 2002:
                    case /*5*/ ZeroContributionReason.Constants.Over64WithOver1000Hours3YearsVesting
                        when pd.ProfitYear >= 2002 && pd.Contribution != 0:
#pragma warning restore CS0618 // Type or member is obsolete
                    case /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
                        when pd.Contribution != 0:
                    case /*7*/ ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay
                        when pd.Contribution != 0:
                        EnrollmentId = /*1*/ EnrollmentConstants.OldVestingPlanHasContributions;
                        LastYearSeen = pd.ProfitYear;
                        break;
                }
            }
        }

        if (Is2003VoidProblem)
        {
            EnrollmentId = /*3*/ EnrollmentConstants.OldVestingPlanHasForfeitureRecords;
        }

        if (pd.ProfitYear >= _newPlanEffectiveYear && pd.Contribution > 0)
        {
            EnrollmentId = /*2*/ EnrollmentConstants.NewVestingPlanHasContributions;
        }
    }
}
