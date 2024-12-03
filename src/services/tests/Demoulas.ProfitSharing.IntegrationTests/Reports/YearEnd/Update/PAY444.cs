using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.Formatters;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.ReportFormatters;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAY444
{
    // external parameters supplied by user.   Contribution points, earning points, employee adjustments
    // should probably become a parameter on the main method, or be passed from the main method down.
    private readonly AdjustmentAmounts _adjustmentAmounts = new();


    // Values collected for an "Adjustment Report" that we do not yet generate
    private readonly EmployeeAdjustmentApplied _employeeAdjustmentAppliedValues = new();


    // Data Helpers - respnosible for holding collections of entities (or entity summaries) and handy lookup methods
    private readonly PayBenDbHelper beneDataHelper;
    private readonly PayProfRecTableHelper emplDataHelper;

    // Collection of members
    private readonly List<MemberFinancials> membersFinancials = new();
    private readonly IProfitSharingDataContextFactory dbContextFactory;

    // new structures
    public List<string> reportLines = new();

    // Need to ensure this is surfaced correctly
    private bool rerunNeeded;

    // Are these duplicates?


    private WS_MAXCONT_TOTALS ws_maxcont_totals = new();
    private WS_PAYPROFIT ws_payprofit = new();

    public PAY444(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory, short profitYear)
    {
        emplDataHelper = new PayProfRecTableHelper(connection, dbContextFactory, profitYear);
        beneDataHelper = new PayBenDbHelper(connection, dbContextFactory);
        EffectiveYear = profitYear;
        this.dbContextFactory = dbContextFactory;
    }

    public long EffectiveYear { get; set; } // PIC 9(4)
    public DateTime TodaysDateTime { get; set; } = DateTime.Now;

    // Should be killed or made as local as possible
    public long WS_CONTR_MAX { get; set; }
    public decimal WS_PY_PS_ETVA { get; set; }
    public decimal WS_ETVA_PERCENT { get; set; }
    public decimal WS_ETVA_AMT { get; set; }
    public decimal WS_ETVA2_AMT { get; set; }
    public decimal DIST_TOTAL { get; set; }
    public decimal FORFEIT_TOTAL { get; set; }
    public decimal ALLOCATION_TOTAL { get; set; }
    public decimal PALLOCATION_TOTAL { get; set; }


    // It is annoying that forfeit proceeds earnings, but that is the way the Cobol prompts for them.  
    public void m015MainProcessing(decimal contributionPercent,
        decimal incomingForfeitPercent, decimal earningsPercent, decimal secondaryEarningsPercent,
        long adjustBadge, decimal adjustContrib, decimal adjustForfeit, decimal adjustEarnings,
        long adjustBadgeSecondary, decimal adjustEarningsSecondary, long maxContribution)
    {
        _adjustmentAmounts.PV_CONT_01 = contributionPercent;
        _adjustmentAmounts.PV_FORF_01 = incomingForfeitPercent;
        _adjustmentAmounts.PV_EARN_01 = earningsPercent;
        _adjustmentAmounts.PV_EARN2_01 = secondaryEarningsPercent; // Gather Input from User
        _adjustmentAmounts.PV_ADJUST_BADGE = adjustBadge; // badge to adjust
        _adjustmentAmounts.PV_ADJ_CONTRIB = adjustContrib; // amount to adjust employee
        _adjustmentAmounts.PV_ADJ_FORFEIT = adjustForfeit;
        _adjustmentAmounts.PV_ADJ_EARN = adjustEarnings;
        _adjustmentAmounts.PV_ADJUST_BADGE2 = adjustBadgeSecondary;
        _adjustmentAmounts.PV_ADJ_EARN2 = adjustEarningsSecondary;
        WS_CONTR_MAX = maxContribution;

        m201ProcessPayProfit();
        m202ProcessPayBen();

        if (rerunNeeded)
        {
            // This needs to get to the invoker - so they can make adjustments for MAX_CONTRIBUTIONS?
            throw new IOException("Rerun of PAY444 is required.  See output for list of issues.");
        }

        m805PrintSequence();

        m1000AdjustmentReport();
    }


    public void m201ProcessPayProfit()
    {
        foreach (EmployeeFinancials empl in emplDataHelper.rows)
        {
            ws_payprofit = new WS_PAYPROFIT();
            ws_payprofit.WS_PS_AMT = ws_payprofit.WS_PS_AMT + empl.CurrentAmount;
            ws_payprofit.WS_PROF_POINTS = ws_payprofit.WS_PROF_POINTS + empl.PointsEarned;

            m210PayprofitComputation(empl);
        }
    }


    private void m202ProcessPayBen()
    {
        foreach (BeneficiaryFinancials bene in beneDataHelper.rows)
        {
            // is already handled as an employee?
            if (emplDataHelper.HasRecordBySsn(bene.Ssn))
            {
                continue;
            }
            ws_payprofit = new WS_PAYPROFIT();
            m220PaybenComputation(bene.Psn);
        }
    }

    public void m210PayprofitComputation(EmployeeFinancials empl)
    {
        //* If an employee has an ETVA amount and no years on the plan, employee is a
        //* beneficiary and should get earnings on the etva amt(8 record)
        if (empl.EmployeeTypeId == 0) // 0 = not new, 1 == new in plan
        {
            if (empl.EtvaAfterVestingRules > 0 && empl.CurrentAmount == 0)
            {
                empl.EmployeeTypeId = 2; // empl is bene and gets earning on ETVA
            }
        }

        if (empl.EnrolledId <= 0 &&
            empl.EmployeeTypeId <= 0 &&
            ws_payprofit.WS_PS_AMT <= 0 && empl.YearsInPlan <= 0)
        {
            return;
        }

        m500GetDbInfo(empl.Ssn);


        WS_COMPUTE_TOTALS ws_compute_totals = new WS_COMPUTE_TOTALS();

        m230ComputeContribution(ws_compute_totals, empl.EmployeeId);
        m240ComputeForfeitures(ws_compute_totals, empl.EmployeeId);


        ws_compute_totals.WS_EARNINGS_BALANCE = ALLOCATION_TOTAL + ws_payprofit.WS_PROF_CAF +
            (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        ws_compute_totals.WS_EARNINGS_BALANCE -= ws_payprofit.WS_PROF_CAF; // BOBH WHAT?  Add it in then remove it???

        if (ws_compute_totals.WS_EARNINGS_BALANCE <= 0)
        {
            ws_compute_totals.WS_EARN_POINTS = 0;
            ws_compute_totals.WS_POINTS_DOLLARS = 0;
        }
        else
        {
            ws_compute_totals.WS_POINTS_DOLLARS =
                ALLOCATION_TOTAL + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

            ws_compute_totals.WS_EARN_POINTS = (long)Math.Round(ws_compute_totals.WS_POINTS_DOLLARS / 100,
                MidpointRounding.AwayFromZero);
        }

        MemberFinancials memb = new();
        memb.EmployeeId = empl.EmployeeId;
        memb.Psn = empl.EmployeeId;

        memb.Name = empl.Name;
        memb.Ssn = empl.Ssn;

        m250ComputeEarnings(ws_compute_totals, null, empl);

        memb.Xfer = ALLOCATION_TOTAL;
        memb.Pxfer = PALLOCATION_TOTAL;
        memb.CurrentAmount = ws_payprofit.WS_PS_AMT;
        memb.Distributions = DIST_TOTAL;
        memb.Military = ws_payprofit.WS_PROF_MIL;
        memb.Caf = ws_payprofit.WS_PROF_CAF;
        memb.EmployeeTypeId = empl.EmployeeTypeId;
        memb.ContributionPoints = ws_payprofit.WS_PROF_POINTS;
        memb.EarningPoints = ws_compute_totals.WS_EARN_POINTS;
        memb.Contributions = ws_payprofit.WS_PROF_CONT;
        memb.IncomingForfeitures = ws_payprofit.WS_PROF_FORF;
        memb.IncomingForfeitures = memb.IncomingForfeitures - FORFEIT_TOTAL;
        memb.Earnings = empl.Earnings;
        memb.Earnings += empl.EarningsOnEtva;
        memb.SecondaryEarnings = empl.SecondaryEarnings;
        memb.SecondaryEarnings += empl.SecondaryEtvaEarnings;

        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            m260Maxcont(memb, empl.EmployeeId);
        }
        else
        {
            memb.MaxOver = 0;
        }

        empl.Contributions = ws_payprofit.WS_PROF_CONT;
        empl.IncomeForfeiture = ws_payprofit.WS_PROF_FORF;

        membersFinancials.Add(memb);
        if (false /*rewrites are off ... the destination columns no longer exist in payprofit*/)
        {
            m420RewritePayprofit(empl);
        }
    }


    public void m220PaybenComputation(long psn)
    {
        BeneficiaryFinancials bene = beneDataHelper.findByPSN(psn);

        EmployeeFinancials payprof_rec = new(); // <--- uh what! BOBH UHWHAT
        payprof_rec.Ssn = bene.Ssn;
        payprof_rec.EmployeeId = 0;

        ws_payprofit.WS_PS_AMT = bene.CurrentAmount;

        WS_COMPUTE_TOTALS ws_compute_totals = new();
        ws_compute_totals.WS_POINTS_DOLLARS = 0m;
        ws_compute_totals.WS_EARNINGS_BALANCE = 0m;
        ws_compute_totals.WS_EARN_POINTS = 0;

        m500GetDbInfo(bene.Ssn);

        ws_compute_totals.WS_EARNINGS_BALANCE = ALLOCATION_TOTAL + ws_payprofit.WS_PROF_CAF +
            (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        ws_compute_totals.WS_EARNINGS_BALANCE -= ws_payprofit.WS_PROF_CAF; // BOBH What!??!

        if (ws_compute_totals.WS_EARNINGS_BALANCE <= 0)
        {
            ws_compute_totals.WS_EARN_POINTS = 0;
            ws_compute_totals.WS_POINTS_DOLLARS = 0;
        }
        else
        {
            ws_compute_totals.WS_POINTS_DOLLARS =
                Math.Round(ALLOCATION_TOTAL + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL,
                    2, MidpointRounding.AwayFromZero);

            ws_compute_totals.WS_EARN_POINTS = (long)Math.Round(ws_compute_totals.WS_POINTS_DOLLARS / 100,
                MidpointRounding.AwayFromZero);
        }

        //* Payben people are py-prof-newemp = 2 and need ETVA and an Allocation this year
        if (ALLOCATION_TOTAL != 0)
        {
            payprof_rec.EmployeeTypeId = 2;
            if (payprof_rec.EarningsOnEtva == 0)
            {
                payprof_rec.EarningsOnEtva = 0.01m;
            }
        }

        payprof_rec.EmployeeId = 0;
        m250ComputeEarnings(ws_compute_totals, bene, null);

        MemberFinancials memb = new();
        memb.EmployeeId = 0;
        memb.Name = bene.Name;
        memb.Ssn = bene.Ssn;
        memb.Psn = bene.Psn;
        memb.Distributions = DIST_TOTAL;
        memb.Military = 0;
        memb.EmployeeTypeId = 0;
        memb.ContributionPoints = 0;
        memb.Contributions = 0;
        memb.IncomingForfeitures = 0;

        if (ws_payprofit.WS_PROF_CAF > 0)
        {
            memb.Caf = ws_payprofit.WS_PROF_CAF;
        }
        else
        {
            memb.Caf = 0;
        }

        memb.Xfer = ALLOCATION_TOTAL;
        memb.Pxfer = PALLOCATION_TOTAL;

        memb.CurrentAmount = ws_payprofit.WS_PS_AMT;
        memb.EarningPoints = ws_compute_totals.WS_EARN_POINTS;
        memb.IncomingForfeitures = memb.IncomingForfeitures - FORFEIT_TOTAL;

        memb.Earnings = bene.Earnings;
        memb.SecondaryEarnings = bene.SecondaryEarnings;

        // BOBH Does this make sense, do bene's hit the max here?
        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            m260Maxcont(memb, 0);
        }

        membersFinancials.Add(memb);

        if (false /*rewrites are off ... the destination columns no longer exist in payprofit*/)
        {
            m430RewritePayben(bene);
        }
    }


    public void m230ComputeContribution(WS_COMPUTE_TOTALS ws_compute_totals,long badge)
    {
        ws_compute_totals.WS_CONT_AMT = Math.Round(_adjustmentAmounts.PV_CONT_01 * ws_payprofit.WS_PROF_POINTS, 2,
            MidpointRounding.AwayFromZero);

        if (_adjustmentAmounts.PV_ADJUST_BADGE > 0 && _adjustmentAmounts.PV_ADJUST_BADGE == badge)
        {
            _employeeAdjustmentAppliedValues.SV_CONT_AMT = ws_compute_totals.WS_CONT_AMT;
            ws_compute_totals.WS_CONT_AMT += _adjustmentAmounts.PV_ADJ_CONTRIB;
            _employeeAdjustmentAppliedValues.SV_CONT_ADJUSTED = ws_compute_totals.WS_CONT_AMT;
        }

        ws_payprofit.WS_PROF_CONT = ws_compute_totals.WS_CONT_AMT;
    }


    public void m240ComputeForfeitures(WS_COMPUTE_TOTALS ws_compute_totals, long badge)
    {
        ws_compute_totals.WS_FORF_AMT = Math.Round(_adjustmentAmounts.PV_FORF_01 * ws_payprofit.WS_PROF_POINTS, 2,
            MidpointRounding.AwayFromZero);
        if (_adjustmentAmounts.PV_ADJUST_BADGE > 0 && _adjustmentAmounts.PV_ADJUST_BADGE == badge)
        {
            _employeeAdjustmentAppliedValues.SV_FORF_AMT = ws_compute_totals.WS_FORF_AMT;
            ws_compute_totals.WS_FORF_AMT += _adjustmentAmounts.PV_ADJ_FORFEIT;
            _employeeAdjustmentAppliedValues.SV_FORF_ADJUSTED = ws_compute_totals.WS_FORF_AMT;
        }

        ws_payprofit.WS_PROF_FORF = ws_compute_totals.WS_FORF_AMT;
    }

    public void m250ComputeEarnings(WS_COMPUTE_TOTALS ws_compute_totals, BeneficiaryFinancials? bene, EmployeeFinancials? empl)
    {
        if (ws_compute_totals.WS_EARN_POINTS <= 0 && empl != null)
        {
            ws_compute_totals.WS_EARN_POINTS = 0;
            ws_compute_totals.WS_EARN_AMT = 0;
            ws_compute_totals.WS_EARN2_AMT = 0;
            empl.Earnings = 0;
            empl.SecondaryEarnings = 0;
        }


        ws_compute_totals.WS_EARN_AMT = Math.Round(_adjustmentAmounts.PV_EARN_01 * ws_compute_totals.WS_EARN_POINTS,
            2, MidpointRounding.AwayFromZero);
        if (_adjustmentAmounts.PV_ADJUST_BADGE > 0 && _adjustmentAmounts.PV_ADJUST_BADGE == (empl?.EmployeeId ?? 0))
        {
            _employeeAdjustmentAppliedValues.SV_EARN_AMT = ws_compute_totals.WS_EARN_AMT;
            ws_compute_totals.WS_EARN_AMT += _adjustmentAmounts.PV_ADJ_EARN;
            _employeeAdjustmentAppliedValues.SV_EARN_ADJUSTED = ws_compute_totals.WS_EARN_AMT;
        }

        ws_compute_totals.WS_EARN2_AMT =
            Math.Round(_adjustmentAmounts.PV_EARN2_01 * ws_compute_totals.WS_EARN_POINTS, 2,
                MidpointRounding.AwayFromZero);
        if (_adjustmentAmounts.PV_ADJUST_BADGE2 > 0 &&
            _adjustmentAmounts.PV_ADJUST_BADGE2 == (empl?.EmployeeId ?? 0))
        {
            _employeeAdjustmentAppliedValues.SV_EARN2_AMT = ws_compute_totals.WS_EARN2_AMT;
            ws_compute_totals.WS_EARN2_AMT += _adjustmentAmounts.PV_ADJ_EARN2;
            _employeeAdjustmentAppliedValues.SV_EARN2_ADJUSTED = ws_compute_totals.WS_EARN2_AMT;
        }

        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* ETVA EARNINGS ARE CALCULATED AND WRITTEN TO PY-PROF-ETVA
        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* need to subtract CAF out of PY-PS-ETVA for people not fully vested
        //* because  we can't give earnings for 2021 on class action funds -
        //* they were added in 2021.CAF was added to PY - PS - ETVA for
        //* PY - PS - YEARS < 6.

        WS_PY_PS_ETVA = 0;
        if (empl != null && empl.EtvaAfterVestingRules > 0)
        {
            if (empl.YearsInPlan < 6)
            {
                WS_PY_PS_ETVA = empl.EtvaAfterVestingRules - ws_payprofit.WS_PROF_CAF;
            }
            else
            {
                empl.EtvaAfterVestingRules = WS_PY_PS_ETVA;
            }
        }

        if (WS_PY_PS_ETVA <= 0 && empl != null)
        {
            empl.Earnings = ws_compute_totals.WS_EARN_AMT;
            empl.SecondaryEarnings = 0m;

            empl.EarningsOnEtva = 0m;
            empl.SecondaryEtvaEarnings = 0m;
            return;
        }


        if (empl != null && ws_compute_totals.WS_POINTS_DOLLARS > 0)
        {
            // Computes the ETVA amount
            WS_ETVA_PERCENT = WS_PY_PS_ETVA / ws_compute_totals.WS_POINTS_DOLLARS;
            WS_ETVA_AMT = Math.Round(ws_compute_totals.WS_EARN_AMT * WS_ETVA_PERCENT, 2, MidpointRounding.AwayFromZero);

            // subtracts that amount from the members total earnings
            ws_compute_totals.WS_EARN_AMT = ws_compute_totals.WS_EARN_AMT - WS_ETVA_AMT;

            // Sets Earn and ETVA amounts
            empl!.Earnings = ws_compute_totals.WS_EARN_AMT;
            empl.EarningsOnEtva = WS_ETVA_AMT;
        }

        if (bene != null)
        {
            bene.Earnings = 0m;
            bene.Earnings = ws_compute_totals.WS_EARN_AMT;
        }

        if (_adjustmentAmounts.PV_EARN2_01 != 0m) // Secondary Earnings
        {
            WS_ETVA2_AMT = Math.Round(ws_compute_totals.WS_EARN2_AMT * WS_ETVA_PERCENT, 2,
                MidpointRounding.AwayFromZero);
            ws_compute_totals.WS_EARN2_AMT -= WS_ETVA2_AMT;
            if (empl != null)
            {
                empl.SecondaryEarnings = ws_compute_totals.WS_EARN2_AMT;
                empl.SecondaryEtvaEarnings = WS_ETVA2_AMT;
            }

            if (bene != null)
            {
                bene.SecondaryEarnings = WS_ETVA2_AMT;
            }
        }
    }


    public void m260Maxcont(MemberFinancials memberFinancials, long badge)
    {
        ws_maxcont_totals.WS_OVER = ws_maxcont_totals.WS_MAX - WS_CONTR_MAX;

        if (ws_maxcont_totals.WS_OVER < ws_payprofit.WS_PROF_FORF)
        {
            ws_payprofit.WS_PROF_FORF -= ws_maxcont_totals.WS_OVER;
        }
        else
        {
            DISPLAY($"FORFEITURES NOT ENOUGH FOR AMOUNT OVER MAX FOR EMPLOYEE BADGE #{badge}");
            ws_payprofit.WS_PROF_FORF = 0;
        }

        memberFinancials.MaxOver = ws_maxcont_totals.WS_OVER;
        memberFinancials.MaxPoints = ws_payprofit.WS_PROF_POINTS;
        rerunNeeded = true;
    }

    private void DISPLAY(string v)
    {
        Console.WriteLine("DISPLAY: " + v);
    }


    public void m420RewritePayprofit(EmployeeFinancials emp)
    {
        if (emp.EmployeeTypeId == 2)
        {
            emp.EmployeeTypeId = 0;
        }

        // BOBH. should update employee.
    }

    public void m430RewritePayben(BeneficiaryFinancials bene)
    {
        if (bene.Earnings == 0 && bene.SecondaryEarnings == 0)
        {
        }

        // BOBH should update bene
    }


    public void m500GetDbInfo(int ssn)
    {
        DIST_TOTAL = 0m;
        FORFEIT_TOTAL = 0m;
        ALLOCATION_TOTAL = 0m;
        PALLOCATION_TOTAL = 0m;

        List<ProfitDetail> pds = dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.ProfitDetails.Where(pd => pd.Ssn == ssn && pd.ProfitYear == EffectiveYear)
                .OrderBy(pd => pd.ProfitYear).ThenBy(pd => pd.ProfitYearIteration).ThenBy(pd => pd.MonthToDate)
                .ThenBy(pd => pd.FederalTaxes)
                .ToListAsync()
        ).GetAwaiter().GetResult();

        foreach (ProfitDetail profit_detail in pds)
        {
            if (profit_detail.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal /*1*/ ||
                profit_detail.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments /*3*/)
            {
                DIST_TOTAL = DIST_TOTAL + profit_detail.Forfeiture;
            }

            if (profit_detail.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment /*9*/)
            {
                if (profit_detail.Remark![..6] == "XFER >" ||
                    profit_detail.Remark[..6] == "QDRO >" ||
                    profit_detail.Remark[..5] == "XFER>" ||
                    profit_detail.Remark[..5] == "QDRO>")
                {
                    PALLOCATION_TOTAL = PALLOCATION_TOTAL + profit_detail.Forfeiture;
                }
                else
                {
                    DIST_TOTAL = DIST_TOTAL + profit_detail.Forfeiture;
                }
            }

            if (profit_detail.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures /*2*/)
            {
                FORFEIT_TOTAL = FORFEIT_TOTAL + profit_detail.Forfeiture;
            }

            if (profit_detail.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary /*5*/)
            {
                PALLOCATION_TOTAL = PALLOCATION_TOTAL + profit_detail.Forfeiture;
            }

            if (profit_detail.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary /*6*/)
            {
                ALLOCATION_TOTAL = ALLOCATION_TOTAL + profit_detail.Contribution;
            }

            if (profit_detail.ProfitYearIteration == 1 /*Military*/)
            {
                ws_payprofit.WS_PROF_MIL = ws_payprofit.WS_PROF_MIL + profit_detail.Contribution;
            }

            if (profit_detail.ProfitYearIteration == 2 /*Class Action Fund*/)
            {
                ws_payprofit.WS_PROF_CAF = ws_payprofit.WS_PROF_CAF + profit_detail.Earnings;
            }
        }
    }

    public void m805PrintSequence()
    {
        WRITE("\fDJDE JDE=PAY426,JDL=PAYROL,END,;");
        ws_maxcont_totals = new WS_MAXCONT_TOTALS();
        HEADER_1 header_1 = new();
        header_1.HDR1_YY = TodaysDateTime.Year - 2000;
        header_1.HDR1_MM = TodaysDateTime.Month;
        header_1.HDR1_DD = TodaysDateTime.Day;
        header_1.HDR1_YEAR1 = EffectiveYear;
        header_1.HDR1_HR = TodaysDateTime.Hour;
        header_1.HDR1_MN = TodaysDateTime.Minute;


        membersFinancials.Sort((a, b) =>
        {
            int nameComparison = StringComparer.Ordinal.Compare(a.Name, b.Name);
            if (nameComparison != 0)
            {
                return nameComparison;
            }

            // This is so we converge on a consistant sort.  This effectively matches Ready's order.
            long aBadge = Convert.ToInt64(a.EmployeeId);
            long bBadge = Convert.ToInt64(b.EmployeeId);
            aBadge = aBadge == 0 ? a.Psn : aBadge;
            bBadge = bBadge == 0 ? b.Psn : bBadge;
            return aBadge < bBadge ? -1 : 1;
        });


        ReportCounters reportCounters = new();
        CollectTotals collectTotals = new();

        foreach (MemberFinancials memberFinancials in membersFinancials)
        {
            m810WriteReport(reportCounters, header_1, memberFinancials, collectTotals);
        }

        m850PrintTotals(reportCounters, collectTotals);
    }

    private void WRITE(object obj)
    {
        reportLines.Add(obj.ToString().TrimEnd());
    }


    public void m810WriteReport(ReportCounters reportCounters, HEADER_1 header_1, MemberFinancials memberFinancials, CollectTotals collectTotals)
    {
        if (reportCounters.LineCounter > 60)
        {
            m830PrintHeader(reportCounters, header_1);
        }

        ReportLine report_line = new();
        ReportLine2 report_line_2 = new();
        if (memberFinancials.EmployeeId > 0)
        {
            report_line.BADGE_NBR = memberFinancials.EmployeeId;
            report_line.EMP_NAME = memberFinancials.Name?.Length > 24
                ? memberFinancials.Name.Substring(0, 24)
                : memberFinancials.Name;

            report_line.BEG_BAL = memberFinancials.CurrentAmount;
            report_line.PR_DIST1 = memberFinancials.Distributions;

            if (memberFinancials.EmployeeTypeId == 1)
            {
                report_line.PR_NEWEMP = "NEW";
            }
            else if (memberFinancials.EmployeeTypeId == 2)
            {
                // This is checking to see if an employee also a bene, and they are a new employee, so not yet vested but they 
                throw new IOException("BOBH: this never happens.");
                // see if member is also a bene.
                // payben_rec.Ssn = memberFinancials.Ssn;
                // PAYBEN_FILE_STATUS = READ_ALT_KEY_PAYBEN(payben_rec);
                // if (PAYBEN_FILE_STATUS == "00")
                // {
                //    report_line.PR_NEWEMP = "BEN";
                //}
            }
            else
            {
                report_line.PR_NEWEMP = " ";
            }

            if (memberFinancials.Xfer != 0)
            {
                memberFinancials.Contributions = memberFinancials.Contributions + memberFinancials.Xfer;
            }

            if (memberFinancials.Pxfer != 0)
            {
                memberFinancials.Military = memberFinancials.Military - memberFinancials.Pxfer;
            }

            report_line.PR_CONT = memberFinancials.Contributions;
            report_line.PR_MIL = memberFinancials.Military;
            report_line.PR_FORF = memberFinancials.IncomingForfeitures;
            report_line.PR_EARN = memberFinancials.Earnings;

            if (memberFinancials.SecondaryEarnings != 0)
            {
                DISPLAY($"badge {memberFinancials.EmployeeId} earnings2 ${memberFinancials.SecondaryEarnings}");
            }

            report_line.PR_EARN2 = memberFinancials.SecondaryEarnings;
            report_line.PR_EARN2 = memberFinancials.Caf;
        }


        decimal endingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions +
                                memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
                                memberFinancials.IncomingForfeitures + memberFinancials.Military +
                                memberFinancials.Caf -
                                memberFinancials.Distributions;
        report_line.END_BAL = endingBalance;

        if (memberFinancials.EmployeeId == 0)
        {
            report_line_2.PR2_EMP_NAME =
                memberFinancials.Name?.Length > 24 ? memberFinancials.Name.Substring(0, 24) : memberFinancials.Name;
            report_line_2.PR2_PSN = memberFinancials.Psn;
            report_line_2.PR2_BEG_BAL = memberFinancials.CurrentAmount;
            report_line_2.PR2_DIST1 = memberFinancials.Distributions;
            report_line_2.PR2_NEWEMP = "BEN";
            memberFinancials.Contributions += memberFinancials.Xfer;
            report_line_2.PR2_CONT = memberFinancials.Contributions;
            report_line_2.PR2_MIL = memberFinancials.Military;
            report_line_2.PR2_FORF = memberFinancials.IncomingForfeitures;
            report_line_2.PR2_EARN = memberFinancials.Earnings;
            report_line_2.PR2_EARN2 = memberFinancials.SecondaryEarnings;
            report_line_2.PR2_EARN2 = memberFinancials.Caf;

            endingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions +
                            memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
                            memberFinancials.IncomingForfeitures + memberFinancials.Military + memberFinancials.Caf -
                            memberFinancials.Distributions;
            report_line_2.PR2_END_BAL = endingBalance;
        }

        collectTotals.WS_TOT_BEGBAL += memberFinancials.CurrentAmount;
        if (memberFinancials.Xfer != 0)
        {
            memberFinancials.Contributions -= memberFinancials.Xfer;
        }

        if (memberFinancials.Pxfer != 0)
        {
            memberFinancials.Military += memberFinancials.Pxfer;
        }

        collectTotals.WS_TOT_DIST1 += memberFinancials.Distributions;
        collectTotals.WS_TOT_CONT += memberFinancials.Contributions;
        collectTotals.WS_TOT_MIL += memberFinancials.Military;
        collectTotals.WS_TOT_FORF += memberFinancials.IncomingForfeitures;
        collectTotals.WS_TOT_EARN += memberFinancials.Earnings;
        collectTotals.WS_TOT_EARN2 += memberFinancials.SecondaryEarnings;
        collectTotals.WS_TOT_ENDBAL += endingBalance;
        collectTotals.WS_TOT_XFER += memberFinancials.Xfer;
        collectTotals.WS_TOT_PXFER -= memberFinancials.Pxfer;
        collectTotals.WS_EARN_PTS_TOTAL += memberFinancials.EarningPoints;
        collectTotals.WS_PROF_PTS_TOTAL += memberFinancials.ContributionPoints;
        collectTotals.WS_TOT_CAF += memberFinancials.Caf;
        ws_maxcont_totals.WS_TOT_OVER += memberFinancials.MaxOver;
        ws_maxcont_totals.WS_TOT_POINTS += memberFinancials.MaxPoints;

        if (memberFinancials.CurrentAmount != 0m
            || memberFinancials.Distributions != 0m
            || memberFinancials.Contributions != 0m
            || memberFinancials.Xfer != 0m
            || memberFinancials.Pxfer != 0m
            || memberFinancials.Military != 0m
            || memberFinancials.IncomingForfeitures != 0m
            || memberFinancials.Earnings != 0m
            || memberFinancials.SecondaryEarnings != 0m)
        {
            if (memberFinancials.EmployeeId > 0)
            {
                reportCounters.EmployeeCounter += 1;
                WRITE(report_line);
            }

            if (memberFinancials.EmployeeId == 0)
            {
                reportCounters.BeneficiaryCounter += 1;
                WRITE(report_line_2);
            }

            reportCounters.LineCounter += 1;
        }
    }

    public void m830PrintHeader(ReportCounters reportCounters, HEADER_1 header_1)
    {
        reportCounters.PageCounter += 1;
        header_1.HDR1_PAGE = reportCounters.PageCounter;
        WRITE("\f" + header_1);
        WRITE("");
        WRITE(new HEADER_2());
        WRITE(new HEADER_3());
        reportCounters.LineCounter = 4;
    }


    public void m850PrintTotals(ReportCounters reportCounters, CollectTotals ws_client_totals)
    {
        ClientTot client_tot = new();
        client_tot.BEG_BAL_TOT = ws_client_totals.WS_TOT_BEGBAL;
        client_tot.DIST1_TOT = ws_client_totals.WS_TOT_DIST1;
        client_tot.MIL_TOT = ws_client_totals.WS_TOT_MIL;
        client_tot.CONT_TOT = ws_client_totals.WS_TOT_CONT;
        client_tot.FORF_TOT = ws_client_totals.WS_TOT_FORF;
        client_tot.EARN_TOT = ws_client_totals.WS_TOT_EARN;
        client_tot.EARN2_TOT = ws_client_totals.WS_TOT_EARN2;
        if (ws_client_totals.WS_TOT_EARN2 != 0)
        {
            DISPLAY("WS_TOT_EARN2 NOT 0 " + ws_client_totals.WS_TOT_EARN2);
        }

        client_tot.EARN2_TOT = ws_client_totals.WS_TOT_CAF;
        client_tot.END_BAL_TOT = ws_client_totals.WS_TOT_ENDBAL;


        TOTAL_HEADER_1 total_header_1 = new();
        total_header_1.TOT_HDR1_YEAR1 = EffectiveYear;
        total_header_1.TOT_HDR1_DD = TodaysDateTime.Day;
        total_header_1.TOT_HDR1_MM = TodaysDateTime.Month;
        total_header_1.TOT_HDR1_YY = TodaysDateTime.Year - 2000;
        total_header_1.TOT_HDR1_HR = TodaysDateTime.Hour;
        total_header_1.TOT_HDR1_MN = TodaysDateTime.Minute;

        WRITE("\f" + total_header_1);
        WRITE("");
        WRITE(new TOTAL_HEADER_2());
        WRITE(new TOTAL_HEADER_3());
        WRITE("");
        WRITE(client_tot);

        client_tot.BEG_BAL_TOT = 0m;
        client_tot.DIST1_TOT = 0m;
        client_tot.CONT_TOT = 0m;
        client_tot.MIL_TOT = 0m;
        client_tot.FORF_TOT = 0m;
        client_tot.EARN_TOT = 0m;
        client_tot.END_BAL_TOT = 0m;
        client_tot.EARN2_TOT = 0m;

        client_tot.CONT_TOT = ws_client_totals.WS_TOT_XFER;
        client_tot.MIL_TOT = ws_client_totals.WS_TOT_PXFER;
        client_tot.END_BAL_TOT = ws_client_totals.WS_TOT_PXFER + ws_client_totals.WS_TOT_XFER;
        client_tot.TOT_FILLER = "ALLOC   ";
        WRITE(client_tot);

        client_tot.BEG_BAL_TOT = 0m;
        client_tot.DIST1_TOT = 0m;
        client_tot.CONT_TOT = 0m;
        client_tot.MIL_TOT = 0m;
        client_tot.FORF_TOT = 0m;
        client_tot.EARN_TOT = 0m;
        client_tot.END_BAL_TOT = 0m;
        client_tot.EARN2_TOT = 0m;

        client_tot.CONT_TOT = ws_client_totals.WS_PROF_PTS_TOTAL;
        client_tot.EARN_TOT = ws_client_totals.WS_EARN_PTS_TOTAL;
        client_tot.useRedefineFormatting = true;
        client_tot.TOT_FILLER = "POINT";
        WRITE("");
        WRITE(client_tot);

        EMPLOYEE_COUNT_TOT employee_count_tot = new();
        employee_count_tot.PR_TOT_EMPLOYEE_COUNT = reportCounters.EmployeeCounter;
        WRITE("");
        WRITE(employee_count_tot);
        EMPLOYEE_COUNT_TOT_PAYBEN employee_count_tot_payben = new();
        employee_count_tot_payben.PB_TOT_EMPLOYEE_COUNT = reportCounters.BeneficiaryCounter;
        WRITE("");
        WRITE(employee_count_tot_payben);

        RERUN_TOT rerun_tot = new();
        rerun_tot.RERUN_OVER = ws_maxcont_totals.WS_TOT_OVER;
        rerun_tot.RERUN_POINTS = ws_maxcont_totals.WS_TOT_POINTS;
        rerun_tot.RERUN_MAX = WS_CONTR_MAX;

        reportLines.Add("\n\n\n\n\n\n\n\n\n");
        WRITE(rerun_tot);

        ws_maxcont_totals = new WS_MAXCONT_TOTALS();
    }


    public void m1000AdjustmentReport()
    {
        if (_adjustmentAmounts.PV_ADJUST_BADGE == 0)
        {
            return;
        }

        HEADER_1 header_1 = new();
        HEADER_4 header_4 = new();
        HEADER_5 header_5 = new();


        header_1.HDR1_PAGE = 1;
        header_1.HDR1_RPT = "PAY444A";
        WRITE2_afterPage(header_1);
        WRITE2_advance2(header_4);
        WRITE2_advance2(header_5);

        PRINT_ADJ_LINE1 print_adj_line1 = new();
        print_adj_line1.PL_ADJUST_BADGE = _adjustmentAmounts.PV_ADJUST_BADGE;
        print_adj_line1.PL_ADJ_DESC = "INITIAL";
        print_adj_line1.PL_CONT_AMT = _employeeAdjustmentAppliedValues.SV_CONT_AMT;
        print_adj_line1.PL_FORF_AMT = _employeeAdjustmentAppliedValues.SV_FORF_AMT;
        print_adj_line1.PL_EARN_AMT = _employeeAdjustmentAppliedValues.SV_EARN_AMT;
        print_adj_line1.PL_EARN2_AMT = _employeeAdjustmentAppliedValues.SV_EARN2_AMT;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJUST_BADGE = 0;
        print_adj_line1.PL_ADJ_DESC = "ADJUSTMENT";
        print_adj_line1.PL_CONT_AMT = _adjustmentAmounts.PV_ADJ_CONTRIB;
        print_adj_line1.PL_EARN_AMT = _adjustmentAmounts.PV_ADJ_EARN;
        print_adj_line1.PL_EARN2_AMT = _adjustmentAmounts.PV_ADJ_EARN2;
        print_adj_line1.PL_FORF_AMT = _adjustmentAmounts.PV_ADJ_FORFEIT;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJ_DESC = "FINAL";
        print_adj_line1.PL_CONT_AMT = _employeeAdjustmentAppliedValues.SV_CONT_ADJUSTED;
        print_adj_line1.PL_FORF_AMT = _employeeAdjustmentAppliedValues.SV_FORF_ADJUSTED;
        print_adj_line1.PL_EARN_AMT = _employeeAdjustmentAppliedValues.SV_EARN_ADJUSTED;
        print_adj_line1.PL_EARN2_AMT = _employeeAdjustmentAppliedValues.SV_EARN2_ADJUSTED;

        WRITE2_advance2(print_adj_line1);

        if (_employeeAdjustmentAppliedValues.SV_FORF_AMT == 0 && _employeeAdjustmentAppliedValues.SV_EARN_AMT == 0)
        {
            WRITE2_advance2("No adjustment - employee not found.");
        }
    }

    private void WRITE2_advance2(object header4)
    {
        // We dont currently support this second report.    We may have to.
        //throw new NotImplementedException();
    }

    private void WRITE2_afterPage(HEADER_1 header1)
    {
        // We dont currently support this second report.    We may have to.
        // throw new NotImplementedException();
    }
}
