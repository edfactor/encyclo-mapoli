using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.ReportFormatters;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAY444
{
    private readonly Counters _counters = new();

    // Data records
    private readonly BeneficiaryFinancials payben_rec = new();

    // Data Helpers
    private readonly PayBenDbHelper payBenDbHelper;

    private readonly EmployeeFinancials payprof_rec = new();
    private readonly PayProfRecTableHelper payProfitDbHelper;

    // Where input values are stored
    private readonly POINT_VALUES point_values = new();
    private readonly AdjustmentReportData _adjustmentReportDataValues = new AdjustmentReportData();


    // Collection of modified payprofit data
    private readonly List<MemberFinancials> membersFinancials = new();


    private long holdBadge;


    // new structures
    public List<string> reportLines = new();

    private bool rerunNeeded;
    private WS_CLIENT_TOTALS ws_client_totals = new();
    private WS_COMPUTE_TOTALS ws_compute_totals = new();
    private WS_MAXCONT_TOTALS ws_maxcont_totals = new();
    private WS_PAYPROFIT ws_payprofit = new();

    private IProfitSharingDataContextFactory dbContextFactory;

    public PAY444(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory, short profitYear)
    {
        payProfitDbHelper = new PayProfRecTableHelper(connection, dbContextFactory, profitYear);
        payBenDbHelper = new PayBenDbHelper(connection,dbContextFactory);
        EffectiveYear = profitYear;
        this.dbContextFactory = dbContextFactory;
    }

    public string? PAYPROFIT_FILE_STATUS { get; set; }
    public string? PAYBEN_FILE_STATUS { get; set; }

    public long HOLD_SSN { get; set; }
    public long HOLD_PAYSSN { get; set; }

    public long WS_REWRITE_WHICH { get; set; }
    public long WS_CONTR_MAX { get; set; }
    public decimal WS_PY_PS_ETVA { get; set; }
    public decimal WS_ETVA_PERCENT { get; set; }
    public decimal WS_ETVA_AMT { get; set; }
    public decimal WS_ETVA2_AMT { get; set; }
    public decimal DIST_TOTAL { get; set; }
    public decimal FORFEIT_TOTAL { get; set; }
    public decimal ALLOCATION_TOTAL { get; set; }

    public decimal PALLOCATION_TOTAL { get; set; }

    public long EffectiveYear { get; set; } // PIC 9(4)

    public DateTime TodaysDateTime { get; set; } = DateTime.Now;

    // It is annoying that forfeit proceeds earnings, but that is the way the Cobol prompts for them.  
    public void m015MainProcessing(decimal contributionPercent,
        decimal incomingForfeitPercent, decimal earningsPercent, decimal secondaryEarningsPercent,
        long adjustBadge, decimal adjustContrib, decimal adjustForfeit, decimal adjustEarnings,
        long adjustBadgeSecondary, decimal adjustEarningsSecondary, long maxContribution)
    {
        point_values.PV_CONT_01 = contributionPercent;
        point_values.PV_FORF_01 = incomingForfeitPercent;
        point_values.PV_EARN_01 = earningsPercent;
        point_values.PV_EARN2_01 = secondaryEarningsPercent; // Gather Input from User
        point_values.PV_ADJUST_BADGE = adjustBadge; // badge to adjust
        point_values.PV_ADJ_CONTRIB = adjustContrib; // amount to adjust employee
        point_values.PV_ADJ_FORFEIT = adjustForfeit;
        point_values.PV_ADJ_EARN = adjustEarnings;
        point_values.PV_ADJUST_BADGE2 = adjustBadgeSecondary;
        point_values.PV_ADJ_EARN2 = adjustEarningsSecondary;
        WS_CONTR_MAX = maxContribution;

        HOLD_PAYSSN = 0;
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
        foreach (EmployeeFinancials pp in payProfitDbHelper.rows)
        {
            ws_compute_totals = new WS_COMPUTE_TOTALS();
            ws_payprofit = new WS_PAYPROFIT();
            ws_payprofit.WS_PS_AMT = ws_payprofit.WS_PS_AMT + pp.CurrentAmount;
            ws_payprofit.WS_PROF_POINTS = ws_payprofit.WS_PROF_POINTS + pp.PointsEarned;

            HOLD_SSN = pp.Ssn;
            holdBadge = pp.EmployeeId;
            payprof_rec.EmployeeId = pp.EmployeeId;

            m210PayprofitComputation();
        }
    }


    private void m202ProcessPayBen()
    {
        foreach (var bene in payBenDbHelper.rows)
        {
            HOLD_PAYSSN = bene.Ssn;
            // is already handled as an employee?
            if (payProfitDbHelper.HasRecordBySsn(bene.Ssn))
            {
                continue;
            }

            ws_compute_totals = new WS_COMPUTE_TOTALS();
            ws_payprofit = new WS_PAYPROFIT();
            m220PaybenComputation(bene.Psn);
        }
    }

    public void m210PayprofitComputation()
    {
        PAYPROFIT_FILE_STATUS = READ_KEY_PAYPROFIT(payprof_rec);
        if (PAYPROFIT_FILE_STATUS != "00")
        {
            throw new IOException($"{payprof_rec.EmployeeId} = INVALID PAYPROFIT RECORD NOT UPDATED");
        }

        //* If an employee has an ETVA amount and no years on the plan, employee is a
        //* beneficiary and should get earnings on the etva amt(8 record)
        if (payprof_rec.EmployeeTypeId == 0)
        {
            if (payprof_rec.EtvaAfterVestingRules > 0 && payprof_rec.CurrentAmount == 0)
            {
                payprof_rec.EmployeeTypeId = 2;
            }
        }

        if (payprof_rec.EnrolledId > 0 ||
            payprof_rec.EmployeeTypeId > 0 ||
            ws_payprofit.WS_PS_AMT > 0 || payprof_rec.YearsInPlan > 0)
        {
        }
        else
        {
            return;
        }

        WS_REWRITE_WHICH = 1;


        m500GetDbInfo();

        MemberFinancials memberFinancials = new MemberFinancials();


        m230ComputeContribution();
        m240ComputeForfeitures();


        ws_compute_totals.WS_EARNINGS_BALANCE = ALLOCATION_TOTAL + ws_payprofit.WS_PROF_CAF +
            (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        ws_compute_totals.WS_EARNINGS_BALANCE -= ws_payprofit.WS_PROF_CAF; // BOBH WHAT?  Add it in then remove it???

        if (ws_compute_totals.WS_EARNINGS_BALANCE <= 0)
        {
            ws_compute_totals.WS_EARN_POINTS = 0;
            ws_compute_totals.WS_POINTS_DOLLARS = 0;
            goto l210_CONTINUE;
        }


        ws_compute_totals.WS_POINTS_DOLLARS =
            ALLOCATION_TOTAL + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        ws_compute_totals.WS_EARN_POINTS = (long)Math.Round(ws_compute_totals.WS_POINTS_DOLLARS / 100, MidpointRounding.AwayFromZero);

        l210_CONTINUE:

        memberFinancials.EmployeeId = payprof_rec.EmployeeId;
        memberFinancials.Psn = payprof_rec.EmployeeId;

        memberFinancials.Name = payprof_rec.Name;
        memberFinancials.Ssn = payprof_rec.Ssn;

        m250ComputeEarnings();

        memberFinancials.Xfer = ALLOCATION_TOTAL;
        memberFinancials.Pxfer = PALLOCATION_TOTAL;
        memberFinancials.CurrentAmount = ws_payprofit.WS_PS_AMT;
        memberFinancials.Distributions = DIST_TOTAL;
        memberFinancials.Military = ws_payprofit.WS_PROF_MIL;
        memberFinancials.Caf = ws_payprofit.WS_PROF_CAF;
        memberFinancials.EmployeeTypeId = payprof_rec.EmployeeTypeId;
        memberFinancials.ContributionPoints = ws_payprofit.WS_PROF_POINTS;
        memberFinancials.EarningPoints = ws_compute_totals.WS_EARN_POINTS;
        memberFinancials.Contributions = ws_payprofit.WS_PROF_CONT;
        memberFinancials.IncomingForfeitures = ws_payprofit.WS_PROF_FORF;
        memberFinancials.IncomingForfeitures = memberFinancials.IncomingForfeitures - FORFEIT_TOTAL;
        memberFinancials.Earnings = payprof_rec.Earnings;
        memberFinancials.Earnings += payprof_rec.EarningsOnEtva;
        memberFinancials.SecondaryEarnings = payprof_rec.SecondaryEarnings;
        memberFinancials.SecondaryEarnings += payprof_rec.SecondaryEtvaEarnings;

        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            m260Maxcont(memberFinancials);
        }
        else
        {
            memberFinancials.MaxOver = 0;
        }

        m400LoadPayprofit();
        membersFinancials.Add(memberFinancials);
        if (false /*rewrites are off ... the destination columns no longer exist in payprofit*/){
            m420RewritePayprofit();
        }
    }

    private string? READ_KEY_PAYPROFIT(EmployeeFinancials payprof_rec)
    {
        EmployeeFinancials one = payProfitDbHelper.findByBadge(payprof_rec.EmployeeId);

        payprof_rec.Name = one.Name;
        payprof_rec.EmployeeId = one.EmployeeId;
        payprof_rec.Ssn = one.Ssn;
        payprof_rec.EnrolledId = one.EnrolledId;
        payprof_rec.YearsInPlan = one.YearsInPlan;
        payprof_rec.CurrentAmount = one.CurrentAmount;
        payprof_rec.EmployeeTypeId = one.EmployeeTypeId;
        payprof_rec.PointsEarned = one.PointsEarned;
        payprof_rec.Contributions = one.Contributions;
        payprof_rec.IncomeForfeiture = one.IncomeForfeiture;
        payprof_rec.Earnings = one.Earnings;
        payprof_rec.EtvaAfterVestingRules = one.EtvaAfterVestingRules;
        payprof_rec.EarningsOnEtva = one.EarningsOnEtva;
        payprof_rec.SecondaryEarnings = one.SecondaryEarnings;
        payprof_rec.SecondaryEtvaEarnings = one.SecondaryEtvaEarnings;

        return "00";
    }


    public void m220PaybenComputation(long psn)
    {
        // <same key> PSKEY = payben1_rec.PYBEN_PSN1;
        payben_rec.Psn = psn;
        PAYBEN_FILE_STATUS = READ_KEY_PAYBEN(payben_rec);
        if (PAYBEN_FILE_STATUS != "00")
        {
            throw new IOException(
                "STAT:{PAYBEN_FILE_STATUS} PSN:{payben_rec.PYBEN_PSN} SSN:{payben_rec.PYBEN_PAYSSN} = INVALID PAYBEN RECORD NOT UPDATED");
        }

        WS_REWRITE_WHICH = 2;


        payprof_rec.Ssn = payben_rec.Ssn;

        ws_payprofit.WS_PS_AMT = payben_rec.CurrentAmount;

        ws_compute_totals.WS_POINTS_DOLLARS = 0m;
        ws_compute_totals.WS_EARNINGS_BALANCE = 0m;
        ws_compute_totals.WS_EARN_POINTS = 0;
        payprof_rec.EmployeeId = 0;

        m500GetDbInfo();


        ws_compute_totals.WS_EARNINGS_BALANCE = ALLOCATION_TOTAL + ws_payprofit.WS_PROF_CAF +
            (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        ws_compute_totals.WS_EARNINGS_BALANCE -= ws_payprofit.WS_PROF_CAF; // BOBH What!??!

        if (ws_compute_totals.WS_EARNINGS_BALANCE <= 0)
        {
            ws_compute_totals.WS_EARN_POINTS = 0;
            ws_compute_totals.WS_POINTS_DOLLARS = 0;
            goto l220_CONTINUE;
        }

        ws_compute_totals.WS_POINTS_DOLLARS =
            Math.Round(ALLOCATION_TOTAL + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL, 2, MidpointRounding.AwayFromZero);

        ws_compute_totals.WS_EARN_POINTS = (long)Math.Round(ws_compute_totals.WS_POINTS_DOLLARS / 100, MidpointRounding.AwayFromZero);

        l220_CONTINUE:

        if (ALLOCATION_TOTAL != 0)
        {
            payprof_rec.EmployeeTypeId = 2;
            if (payprof_rec.EarningsOnEtva == 0)
            {
                payprof_rec.EarningsOnEtva = 0.01m;
            }
        }

        payprof_rec.EmployeeId = 0;
        m250ComputeEarnings();

        MemberFinancials memberFinancials = new MemberFinancials();
        memberFinancials.EmployeeId = 0;
        memberFinancials.Name = payben_rec.Name;
        memberFinancials.Ssn = payben_rec.Ssn;
        memberFinancials.Psn = payben_rec.Psn;
        memberFinancials.Distributions = DIST_TOTAL;
        memberFinancials.Military = 0;
        memberFinancials.EmployeeTypeId = 0;
        memberFinancials.ContributionPoints = 0;
        memberFinancials.Contributions = 0;
        memberFinancials.IncomingForfeitures = 0;

        if (ws_payprofit.WS_PROF_CAF > 0)
        {
            memberFinancials.Caf = ws_payprofit.WS_PROF_CAF;
        }
        else
        {
            memberFinancials.Caf = 0;
        }

        memberFinancials.Xfer = ALLOCATION_TOTAL;
        memberFinancials.Pxfer = PALLOCATION_TOTAL;

        memberFinancials.CurrentAmount = ws_payprofit.WS_PS_AMT;
        memberFinancials.EarningPoints = ws_compute_totals.WS_EARN_POINTS;
        memberFinancials.IncomingForfeitures = memberFinancials.IncomingForfeitures - FORFEIT_TOTAL;

        memberFinancials.Earnings = payben_rec.Earnings;
        memberFinancials.SecondaryEarnings = payben_rec.SecondaryEarnings;

        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            m260Maxcont(memberFinancials);
        }

        membersFinancials.Add(memberFinancials);

        if (false /*rewrites are off ... the destination columns no longer exist in payprofit*/)
        {
            m430RewritePayben();
        }
    }

    private string? READ_KEY_PAYBEN(BeneficiaryFinancials payben_rec)
    {
        return payBenDbHelper.findByPSN(payben_rec);
    }


    public void m230ComputeContribution()
    {
        ws_compute_totals.WS_CONT_AMT = Math.Round(point_values.PV_CONT_01 * ws_payprofit.WS_PROF_POINTS, 2, MidpointRounding.AwayFromZero);

        if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
        {
            _adjustmentReportDataValues.SV_CONT_AMT = ws_compute_totals.WS_CONT_AMT;
            ws_compute_totals.WS_CONT_AMT += point_values.PV_ADJ_CONTRIB;
            _adjustmentReportDataValues.SV_CONT_ADJUSTED = ws_compute_totals.WS_CONT_AMT;
        }

        ws_payprofit.WS_PROF_CONT = ws_compute_totals.WS_CONT_AMT;
    }


    public void m240ComputeForfeitures()
    {
        ws_compute_totals.WS_FORF_AMT = Math.Round(point_values.PV_FORF_01 * ws_payprofit.WS_PROF_POINTS, 2, MidpointRounding.AwayFromZero);
        if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
        {
            _adjustmentReportDataValues.SV_FORF_AMT = ws_compute_totals.WS_FORF_AMT;
            ws_compute_totals.WS_FORF_AMT += point_values.PV_ADJ_FORFEIT;
            _adjustmentReportDataValues.SV_FORF_ADJUSTED = ws_compute_totals.WS_FORF_AMT;
        }

        ws_payprofit.WS_PROF_FORF = ws_compute_totals.WS_FORF_AMT;
    }

    public void m250ComputeEarnings()
    {
        if (ws_compute_totals.WS_EARN_POINTS > 0 || WS_REWRITE_WHICH == 2)
        {
        }
        else
        {
            if (ws_compute_totals.WS_EARN_POINTS <= 0)
            {
                ws_compute_totals.WS_EARN_POINTS = 0;
                payprof_rec.Earnings = 0;
                ws_compute_totals.WS_EARN_AMT = 0;
                payben_rec.Earnings = 0;
                payprof_rec.SecondaryEarnings = 0;
                ws_compute_totals.WS_EARN2_AMT = 0;
                payben_rec.SecondaryEarnings = 0;
            }
        }

        if (WS_REWRITE_WHICH == 1 || WS_REWRITE_WHICH == 2)
        {
            ws_compute_totals.WS_EARN_AMT = Math.Round(point_values.PV_EARN_01 * ws_compute_totals.WS_EARN_POINTS, 2, MidpointRounding.AwayFromZero);
            if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
            {
                _adjustmentReportDataValues.SV_EARN_AMT = ws_compute_totals.WS_EARN_AMT;
                ws_compute_totals.WS_EARN_AMT += point_values.PV_ADJ_EARN;
                _adjustmentReportDataValues.SV_EARN_ADJUSTED = ws_compute_totals.WS_EARN_AMT;
            }

            ws_compute_totals.WS_EARN2_AMT = Math.Round(point_values.PV_EARN2_01 * ws_compute_totals.WS_EARN_POINTS, 2, MidpointRounding.AwayFromZero);
            if (point_values.PV_ADJUST_BADGE2 > 0 && point_values.PV_ADJUST_BADGE2 == holdBadge)
            {
                _adjustmentReportDataValues.SV_EARN2_AMT = ws_compute_totals.WS_EARN2_AMT;
                ws_compute_totals.WS_EARN2_AMT += point_values.PV_ADJ_EARN2;
                _adjustmentReportDataValues.SV_EARN2_ADJUSTED = ws_compute_totals.WS_EARN2_AMT;
            }
        }

        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* ETVA EARNINGS ARE CALCULATED AND WRITTEN TO PY-PROF-ETVA
        //* -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
        //* need to subtract CAF out of PY-PS-ETVA for people not fully vested
        //* because  we can't give earnings for 2021 on class action funds -
        //* they were added in 2021.CAF was added to PY - PS - ETVA for
        //* PY - PS - YEARS < 6.

        WS_PY_PS_ETVA = 0;
        if (payprof_rec.EtvaAfterVestingRules > 0)
        {
            if (payprof_rec.YearsInPlan < 6)
            {
                WS_PY_PS_ETVA = payprof_rec.EtvaAfterVestingRules - ws_payprofit.WS_PROF_CAF;
            }
            else
            {
                payprof_rec.EtvaAfterVestingRules = WS_PY_PS_ETVA;
            }
        }

        if (WS_PY_PS_ETVA > 0 || WS_REWRITE_WHICH == 2)
        {
        }
        else
        {
            payprof_rec.Earnings = ws_compute_totals.WS_EARN_AMT;
            payprof_rec.SecondaryEarnings = 0m;

            payprof_rec.EarningsOnEtva = 0m;
            payben_rec.Earnings = 0m;
            payprof_rec.SecondaryEtvaEarnings = 0m;
            payben_rec.SecondaryEarnings = 0m;

            goto l250_EXIT;
        }

        if (WS_REWRITE_WHICH == 1 && ws_compute_totals.WS_POINTS_DOLLARS > 0)
        {
            // Computes the ETVA amount
            WS_ETVA_PERCENT = WS_PY_PS_ETVA / ws_compute_totals.WS_POINTS_DOLLARS;
            WS_ETVA_AMT = Math.Round(ws_compute_totals.WS_EARN_AMT * WS_ETVA_PERCENT, 2, MidpointRounding.AwayFromZero);

            // subtracts that amount from the members total earnings
            ws_compute_totals.WS_EARN_AMT = ws_compute_totals.WS_EARN_AMT - WS_ETVA_AMT;

            // Sets Earn and ETVA amounts
            payprof_rec.Earnings = ws_compute_totals.WS_EARN_AMT;
            payprof_rec.EarningsOnEtva = WS_ETVA_AMT;
        }

        if (WS_REWRITE_WHICH == 2)
        {
            payben_rec.Earnings = 0m;
            payben_rec.Earnings = ws_compute_totals.WS_EARN_AMT;
        }

        if (point_values.PV_EARN2_01 != 0m) // Secondary Earnings
        {
            WS_ETVA2_AMT = Math.Round(ws_compute_totals.WS_EARN2_AMT * WS_ETVA_PERCENT, 2, MidpointRounding.AwayFromZero);
            ws_compute_totals.WS_EARN2_AMT -= WS_ETVA2_AMT;
            payprof_rec.SecondaryEarnings = ws_compute_totals.WS_EARN2_AMT;
            payprof_rec.SecondaryEtvaEarnings = WS_ETVA2_AMT;
            if (WS_REWRITE_WHICH == 2)
            {
                payben_rec.SecondaryEarnings = WS_ETVA2_AMT;
            }
        }

        l250_EXIT: ;
    }


    public void m260Maxcont(MemberFinancials memberFinancials)
    {
        ws_maxcont_totals.WS_OVER = ws_maxcont_totals.WS_MAX - WS_CONTR_MAX;

        if (ws_maxcont_totals.WS_OVER < ws_payprofit.WS_PROF_FORF)
        {
            ws_payprofit.WS_PROF_FORF -= ws_maxcont_totals.WS_OVER;
        }
        else
        {
            DISPLAY($"FORFEITURES NOT ENOUGH FOR AMOUNT OVER MAX FOR EMPLOYEE BADGE #{holdBadge}");
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


    public void m400LoadPayprofit()
    {
        payprof_rec.Contributions = ws_payprofit.WS_PROF_CONT;
        payprof_rec.IncomeForfeiture = ws_payprofit.WS_PROF_FORF;
    }


    public void m420RewritePayprofit()
    {
        if (payprof_rec.EmployeeTypeId == 2)
        {
            payprof_rec.EmployeeTypeId = 0;
        }

        PAYPROFIT_FILE_STATUS = REWRITE_KEY_PAYPROFIT(payprof_rec);
        if (PAYPROFIT_FILE_STATUS != "00")
        {
            throw new IOException($"BAD REWRITE OF PAYPROFIT EMPLOYEE BADGE # {payprof_rec.EmployeeId}");
        }
    }

    private string? REWRITE_KEY_PAYPROFIT(EmployeeFinancials payprof_rec)
    {
        throw new NotImplementedException();
    }


    public void m430RewritePayben()
    {
        if (payben_rec.Earnings == 0 && payben_rec.SecondaryEarnings == 0)
        {
            return; // humm, dont save the zeros?
        }

        PAYBEN_FILE_STATUS = REWRITE_KEY_PAYBEN(payben_rec);
        if (PAYBEN_FILE_STATUS != "00")
        {
            throw new IOException($"BAD REWRITE OF PAYBEN EMPLOYEE PSN # {payben_rec.Psn}");
        }
    }

    private string? REWRITE_KEY_PAYBEN(BeneficiaryFinancials payben_rec)
    {
        throw new NotImplementedException();
    }


    public void m500GetDbInfo()
    {
        DIST_TOTAL = 0m;
        FORFEIT_TOTAL = 0m;
        ALLOCATION_TOTAL = 0m;
        PALLOCATION_TOTAL = 0m;

        m510GetDetails(payprof_rec.Ssn);
    }

    public void m510GetDetails(int ssn)
    {
        var pds = dbContextFactory.UseReadOnlyContext(ctx =>
            ctx.ProfitDetails.Where(pd => pd.Ssn == ssn && pd.ProfitYear == EffectiveYear)
                .OrderBy(pd => pd.ProfitYear).ThenBy(pd => pd.ProfitYearIteration).ThenBy((pd => pd.MonthToDate))
                .ThenBy(pd => pd.FederalTaxes)
                .ToListAsync()
        ).GetAwaiter().GetResult();

        foreach (var profit_detail in pds)
        {

            long WS_PROFIT_YEAR_FIRST_4 = (long)profit_detail.ProfitYear;
            string[] parts = profit_detail.ProfitYear.ToString().Split('.');
            long decimalPart = parts.Length > 1 ? long.Parse(parts[1]) : 0;
            long WS_PROFIT_YEAR_EXTENSION = decimalPart;

            if (WS_PROFIT_YEAR_FIRST_4 == EffectiveYear)
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

                if (WS_PROFIT_YEAR_EXTENSION == 1 /*Military*/)
                {
                    ws_payprofit.WS_PROF_MIL = ws_payprofit.WS_PROF_MIL + profit_detail.Contribution;
                }

                if (WS_PROFIT_YEAR_EXTENSION == 2 /*Class Action Fund*/)
                {
                    ws_payprofit.WS_PROF_CAF = ws_payprofit.WS_PROF_CAF + profit_detail.Earnings;
                }
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

        foreach (var prft in membersFinancials)
        {
            m810WriteReport(header_1, prft);
        }

        m850PrintTotals();
    }

    private void WRITE(object obj)
    {
        reportLines.Add(obj.ToString().TrimEnd());
    }


    public void m810WriteReport(HEADER_1 header_1, MemberFinancials memberFinancials)
    {
        if (_counters.LineCounter > 60)
        {
            m830PrintHeader(header_1);
        }

        REPORT_LINE report_line = new();
        REPORT_LINE_2 report_line_2 = new();
        if (memberFinancials.EmployeeId > 0)
        {
            report_line.BADGE_NBR = memberFinancials.EmployeeId;
            report_line.EMP_NAME = memberFinancials.Name?.Length > 24 ? memberFinancials.Name.Substring(0, 24) : memberFinancials.Name;

            report_line.BEG_BAL = memberFinancials.CurrentAmount;
            report_line.PR_DIST1 = memberFinancials.Distributions;

            if (memberFinancials.EmployeeTypeId == 1)
            {
                report_line.PR_NEWEMP = "NEW";
            }
            else if (memberFinancials.EmployeeTypeId == 2)
            {
                payben_rec.Ssn = memberFinancials.Ssn;
                PAYBEN_FILE_STATUS = READ_ALT_KEY_PAYBEN(payben_rec);
                if (PAYBEN_FILE_STATUS == "00")
                {
                    report_line.PR_NEWEMP = "BEN";
                }
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


        decimal endingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions + memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
            memberFinancials.IncomingForfeitures + memberFinancials.Military + memberFinancials.Caf - memberFinancials.Distributions;
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

            endingBalance = memberFinancials.CurrentAmount + memberFinancials.Contributions + memberFinancials.Earnings + memberFinancials.SecondaryEarnings +
                memberFinancials.IncomingForfeitures + memberFinancials.Military + memberFinancials.Caf - memberFinancials.Distributions;
            report_line_2.PR2_END_BAL = endingBalance;
        }

        ws_client_totals.WS_TOT_BEGBAL += memberFinancials.CurrentAmount;
        if (memberFinancials.Xfer != 0)
        {
            memberFinancials.Contributions -= memberFinancials.Xfer;
        }

        if (memberFinancials.Pxfer != 0)
        {
            memberFinancials.Military += memberFinancials.Pxfer;
        }

        ws_client_totals.WS_TOT_DIST1 += memberFinancials.Distributions;
        ws_client_totals.WS_TOT_CONT += memberFinancials.Contributions;
        ws_client_totals.WS_TOT_MIL += memberFinancials.Military;
        ws_client_totals.WS_TOT_FORF += memberFinancials.IncomingForfeitures;
        ws_client_totals.WS_TOT_EARN += memberFinancials.Earnings;
        ws_client_totals.WS_TOT_EARN2 += memberFinancials.SecondaryEarnings;
        ws_client_totals.WS_TOT_ENDBAL += endingBalance;
        ws_client_totals.WS_TOT_XFER += memberFinancials.Xfer;
        ws_client_totals.WS_TOT_PXFER -= memberFinancials.Pxfer;
        ws_client_totals.WS_EARN_PTS_TOTAL += memberFinancials.EarningPoints;
        ws_client_totals.WS_PROF_PTS_TOTAL += memberFinancials.ContributionPoints;
        ws_client_totals.WS_TOT_CAF += memberFinancials.Caf;
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
                _counters.EmployeeCounter += 1;
                WRITE(report_line);
            }

            if (memberFinancials.EmployeeId == 0)
            {
                _counters.BeneficiaryCounter += 1;
                WRITE(report_line_2);
            }

            _counters.LineCounter += 1;
        }
    }

    public void m830PrintHeader(HEADER_1 header_1)
    {
        _counters.PageCounter += 1;
        header_1.HDR1_PAGE = _counters.PageCounter;
        WRITE("\f" + header_1);
        WRITE("");
        WRITE(new HEADER_2());
        WRITE(new HEADER_3());
        _counters.LineCounter = 4;
    }


    public void m850PrintTotals()
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
        employee_count_tot.PR_TOT_EMPLOYEE_COUNT = _counters.EmployeeCounter;
        WRITE("");
        WRITE(employee_count_tot);
        EMPLOYEE_COUNT_TOT_PAYBEN employee_count_tot_payben = new();
        employee_count_tot_payben.PB_TOT_EMPLOYEE_COUNT = _counters.BeneficiaryCounter;
        WRITE("");
        WRITE(employee_count_tot_payben);

        ws_compute_totals = new WS_COMPUTE_TOTALS();
        ws_client_totals = new WS_CLIENT_TOTALS();

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
        if (point_values.PV_ADJUST_BADGE == 0)
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
        print_adj_line1.PL_ADJUST_BADGE = point_values.PV_ADJUST_BADGE;
        print_adj_line1.PL_ADJ_DESC = "INITIAL";
        print_adj_line1.PL_CONT_AMT = _adjustmentReportDataValues.SV_CONT_AMT;
        print_adj_line1.PL_FORF_AMT = _adjustmentReportDataValues.SV_FORF_AMT;
        print_adj_line1.PL_EARN_AMT = _adjustmentReportDataValues.SV_EARN_AMT;
        print_adj_line1.PL_EARN2_AMT = _adjustmentReportDataValues.SV_EARN2_AMT;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJUST_BADGE = 0;
        print_adj_line1.PL_ADJ_DESC = "ADJUSTMENT";
        print_adj_line1.PL_CONT_AMT = point_values.PV_ADJ_CONTRIB;
        print_adj_line1.PL_EARN_AMT = point_values.PV_ADJ_EARN;
        print_adj_line1.PL_EARN2_AMT = point_values.PV_ADJ_EARN2;
        print_adj_line1.PL_FORF_AMT = point_values.PV_ADJ_FORFEIT;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJ_DESC = "FINAL";
        print_adj_line1.PL_CONT_AMT = _adjustmentReportDataValues.SV_CONT_ADJUSTED;
        print_adj_line1.PL_FORF_AMT = _adjustmentReportDataValues.SV_FORF_ADJUSTED;
        print_adj_line1.PL_EARN_AMT = _adjustmentReportDataValues.SV_EARN_ADJUSTED;
        print_adj_line1.PL_EARN2_AMT = _adjustmentReportDataValues.SV_EARN2_ADJUSTED;

        WRITE2_advance2(print_adj_line1);

        if (_adjustmentReportDataValues.SV_FORF_AMT == 0 && _adjustmentReportDataValues.SV_EARN_AMT == 0)
        {
            WRITE2_advance2("No adjustment - employee not found.");  
        }
    }

    private void WRITE2_advance2(object header4)
    {
        //throw new NotImplementedException();
    }

    private void WRITE2_afterPage(HEADER_1 header1)
    {
        // throw new NotImplementedException();
    }


    private string? READ_ALT_KEY_PAYBEN(BeneficiaryFinancials payben_rec)
    {
        throw new NotImplementedException();
    }
}
