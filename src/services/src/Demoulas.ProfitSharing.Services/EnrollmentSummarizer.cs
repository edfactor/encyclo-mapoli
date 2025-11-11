using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;

namespace Demoulas.ProfitSharing.Services;

internal enum VestingStateType
{
    NotVested,
    PartiallyVested,
    FullyVested
}

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

    public byte ComputeEnrollment(PayProfit pp, short years, List<ProfitDetail> pds)
    {
        if (pds.Count == 0)
        {
            return 0;
        }
        // COBOL: 300-PROCESS-PS SECTION

        foreach (ProfitDetail pd in pds)
        {
            ExamineProfitDetailRow(pds, pd, pp.ProfitYear);
        }

        ComputeVesting(years, pp);

        return GetEnrolled(years);
    }

    // COBOL: 530_PC_VESTED
    private void ComputeVesting(short years, PayProfit pp)
    {
        if (!IsNewVestingRules)
        {
            years--;
        }

        VestedState = VestingStateType.NotVested;
        if (years > 0)
        {
            VestedState = VestingStateType.PartiallyVested;
            if (years > /*5*/ ReferenceData.VestingYears())
            {
                VestedState = VestingStateType.FullyVested;
            }
        }

        if (EnrollmentId == /*3*/ Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
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
    public byte GetEnrolled(short years)
    {
        if (IsNewVestingRules)
        {
            if (EnrollmentId == /*1*/ Enrollment.Constants.OldVestingPlanHasContributions)
            {
                EnrollmentId = /*2*/ Enrollment.Constants.NewVestingPlanHasContributions;
            }
        }

        if (VestedState != VestingStateType.NotVested)
        {
            if (Forfeiture != 0 && IsAfter2006)
            {
                EnrollmentId = /*3*/ Enrollment.Constants.OldVestingPlanHasForfeitureRecords;
            }

            if (IsNewVestingRules)
            {
                if (EnrollmentId == /*1*/ Enrollment.Constants.OldVestingPlanHasContributions)
                {
                    EnrollmentId = /*2*/ Enrollment.Constants.NewVestingPlanHasContributions;
                }

                if (EnrollmentId == /*3*/ Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
                {
                    EnrollmentId = /*4*/ Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
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

        //* Any contribution given in 2007 or after will cause
        //* the new vesting rules to be in effect. 
        if (pd is { ProfitYear: >= 2007, Contribution: > 0 } && pd.ProfitCodeId != 6)
        {
            IsNewVestingRules = true;
        }

        if (pd is { ProfitYear: 2003, ProfitCodeId: 8 })
        {
            Is2003VoidProblem = true;
            EnrollmentId = Enrollment.Constants.OldVestingPlanHasForfeitureRecords;
        }

        if (pd.ProfitCodeId != /*2*/ ProfitCode.Constants.OutgoingForfeitures)
        {
            return;
        }

        if (pd.ProfitYear > 2006)
        {
            // Should this really be an OR into IsAfter2006 ?
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
                if (pd.ProfitYear > 2007)
                {
                    IsNewVestingRules = true;
                }

                EnrollmentId = /*1*/ Enrollment.Constants.OldVestingPlanHasContributions;
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
                        EnrollmentId = /*1*/ Enrollment.Constants.OldVestingPlanHasContributions;
                        LastYearSeen = pd.ProfitYear;
                        break;
                }
            }
        }

        if (Is2003VoidProblem)
        {
            EnrollmentId = /*3*/ Enrollment.Constants.OldVestingPlanHasForfeitureRecords;
        }

        if (pd is { ProfitYear: >= 2007, Contribution: > 0 })
        {
            EnrollmentId = /*2*/ Enrollment.Constants.NewVestingPlanHasContributions;
        }
    }
}
