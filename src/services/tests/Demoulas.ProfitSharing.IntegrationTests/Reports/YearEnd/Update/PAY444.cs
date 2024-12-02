using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.ReportFormatters;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAY444
{
    private readonly Counters _counters = new();

    // Data records
    private readonly BeneficiaryFinancials payben_rec = new();

    // Data Helpers
    private readonly PayBenDbHelper payBenDbHelper;

    //private readonly PAYBEN1_REC payben1_rec = new();
    private readonly EmployeeFinancials payprof_rec = new();
    private readonly PayProfRecTableHelper payProfitDbHelper;


    // Where input values are stored
    private readonly POINT_VALUES point_values = new();
    private readonly AdjustmentReport adjustmentReportValues = new AdjustmentReport();


    // Collection of modified payprofit data
    private readonly List<PRFT> prfts = new();
    private readonly PROFIT_DETAIL profit_detail = new();
    private readonly ProfitDetailTableHelper profitDetailTable;

    private long holdBadge;
    private INTERMEDIATE_VALUES intermediate_values = new();

    // new structures
    public List<string> outputLines = new();
    private PRFT prft = new();
    private bool rerunNeeded;
    private SD_PRFT sd_prft = new();
    private WS_CLIENT_TOTALS ws_client_totals = new();
    private WS_COMPUTE_TOTALS ws_compute_totals = new();
    private WS_MAXCONT_TOTALS ws_maxcont_totals = new();
    private WS_PAYPROFIT ws_payprofit = new();

    public PAY444(OracleConnection connection, IProfitSharingDataContextFactory dbContextFactory, short profitYear)
    {
        payProfitDbHelper = new PayProfRecTableHelper(connection, dbContextFactory, profitYear);
        payBenDbHelper = new PayBenDbHelper(connection,dbContextFactory);
        profitDetailTable = new ProfitDetailTableHelper(connection, dbContextFactory, profitYear);
        EffectiveYear = profitYear;
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

        intermediate_values = new INTERMEDIATE_VALUES();

        m500GetDbInfo();

        prft = new PRFT();


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

        ws_compute_totals.WS_EARN_POINTS = (long)Round(ws_compute_totals.WS_POINTS_DOLLARS / 100);

        l210_CONTINUE:

        intermediate_values.WS_FD_BADGE = payprof_rec.EmployeeId;
        intermediate_values.WS_FD_NAME = payprof_rec.Name;
        intermediate_values.WS_FD_SSN = payprof_rec.Ssn;
        intermediate_values.WS_FD_PSN = payprof_rec.EmployeeId;

        m250ComputeEarnings();

        intermediate_values.WS_FD_XFER = ALLOCATION_TOTAL;
        intermediate_values.WS_FD_PXFER = PALLOCATION_TOTAL;
        intermediate_values.WS_FD_AMT = ws_payprofit.WS_PS_AMT;
        intermediate_values.WS_FD_DIST1 = DIST_TOTAL;
        intermediate_values.WS_FD_MIL = ws_payprofit.WS_PROF_MIL;
        intermediate_values.WS_FD_CAF = ws_payprofit.WS_PROF_CAF;
        intermediate_values.WS_FD_NEWEMP = payprof_rec.EmployeeTypeId;
        intermediate_values.WS_FD_POINTS = ws_payprofit.WS_PROF_POINTS;
        intermediate_values.WS_FD_POINTS_EARN = ws_compute_totals.WS_EARN_POINTS;
        intermediate_values.WS_FD_CONT = ws_payprofit.WS_PROF_CONT;
        intermediate_values.WS_FD_FORF = ws_payprofit.WS_PROF_FORF;
        intermediate_values.WS_FD_FORF = intermediate_values.WS_FD_FORF - FORFEIT_TOTAL;
        intermediate_values.WS_FD_EARN = payprof_rec.Earnings;
        intermediate_values.WS_FD_EARN += payprof_rec.EarningsOnEtva;
        intermediate_values.WS_FD_EARN2 = payprof_rec.SecondaryEarnings;
        intermediate_values.WS_FD_EARN2 += payprof_rec.SecondaryEtvaEarnings;

        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            m260Maxcont();
        }
        else
        {
            prft.FD_MAXOVER = 0;
        }

        m400LoadPayprofit();
        m410LoadProfit();
        if (false /*rewrites are off*/)
        {
            m420RewritePayprofit();
        }
    }

    private decimal Round(decimal v)
    {
        return Math.Round(v, MidpointRounding.AwayFromZero);
    }

    private decimal Round2(decimal v)
    {
        return Math.Round(v, 2, MidpointRounding.AwayFromZero);
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

        intermediate_values = new INTERMEDIATE_VALUES();

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
            Round2(ALLOCATION_TOTAL + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL);

        ws_compute_totals.WS_EARN_POINTS = (long)Round(ws_compute_totals.WS_POINTS_DOLLARS / 100);

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

        intermediate_values.WS_FD_BADGE = 0;
        intermediate_values.WS_FD_NAME = payben_rec.Name;
        intermediate_values.WS_FD_SSN = payben_rec.Ssn;
        intermediate_values.WS_FD_PSN = payben_rec.Psn;
        intermediate_values.WS_FD_DIST1 = DIST_TOTAL;
        intermediate_values.WS_FD_MIL = 0;
        intermediate_values.WS_FD_NEWEMP = 0;
        intermediate_values.WS_FD_POINTS = 0;
        intermediate_values.WS_FD_CONT = 0;
        intermediate_values.WS_FD_FORF = 0;

        if (ws_payprofit.WS_PROF_CAF > 0)
        {
            intermediate_values.WS_FD_CAF = ws_payprofit.WS_PROF_CAF;
        }
        else
        {
            intermediate_values.WS_FD_CAF = 0;
        }

        intermediate_values.WS_FD_XFER = ALLOCATION_TOTAL;
        intermediate_values.WS_FD_PXFER = PALLOCATION_TOTAL;

        intermediate_values.WS_FD_AMT = ws_payprofit.WS_PS_AMT;
        intermediate_values.WS_FD_POINTS_EARN = ws_compute_totals.WS_EARN_POINTS;
        intermediate_values.WS_FD_FORF = intermediate_values.WS_FD_FORF - FORFEIT_TOTAL;

        intermediate_values.WS_FD_EARN = payben_rec.Earnings;
        intermediate_values.WS_FD_EARN2 = payben_rec.SecondaryEarnings;

        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            m260Maxcont();
        }

        m410LoadProfit();

        if (false /*rewrites are off*/)
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
        ws_compute_totals.WS_CONT_AMT = Round2(point_values.PV_CONT_01 * ws_payprofit.WS_PROF_POINTS);

        if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
        {
            adjustmentReportValues.SV_CONT_AMT = ws_compute_totals.WS_CONT_AMT;
            ws_compute_totals.WS_CONT_AMT += point_values.PV_ADJ_CONTRIB;
            adjustmentReportValues.SV_CONT_ADJUSTED = ws_compute_totals.WS_CONT_AMT;
        }

        ws_payprofit.WS_PROF_CONT = ws_compute_totals.WS_CONT_AMT;
    }


    public void m240ComputeForfeitures()
    {
        ws_compute_totals.WS_FORF_AMT = Round2(point_values.PV_FORF_01 * ws_payprofit.WS_PROF_POINTS);
        if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
        {
            adjustmentReportValues.SV_FORF_AMT = ws_compute_totals.WS_FORF_AMT;
            ws_compute_totals.WS_FORF_AMT += point_values.PV_ADJ_FORFEIT;
            adjustmentReportValues.SV_FORF_ADJUSTED = ws_compute_totals.WS_FORF_AMT;
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
            ws_compute_totals.WS_EARN_AMT = Round2(point_values.PV_EARN_01 * ws_compute_totals.WS_EARN_POINTS);
            if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
            {
                adjustmentReportValues.SV_EARN_AMT = ws_compute_totals.WS_EARN_AMT;
                ws_compute_totals.WS_EARN_AMT += point_values.PV_ADJ_EARN;
                adjustmentReportValues.SV_EARN_ADJUSTED = ws_compute_totals.WS_EARN_AMT;
            }

            ws_compute_totals.WS_EARN2_AMT = Round2(point_values.PV_EARN2_01 * ws_compute_totals.WS_EARN_POINTS);
            if (point_values.PV_ADJUST_BADGE2 > 0 && point_values.PV_ADJUST_BADGE2 == holdBadge)
            {
                adjustmentReportValues.SV_EARN2_AMT = ws_compute_totals.WS_EARN2_AMT;
                ws_compute_totals.WS_EARN2_AMT += point_values.PV_ADJ_EARN2;
                adjustmentReportValues.SV_EARN2_ADJUSTED = ws_compute_totals.WS_EARN2_AMT;
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
            WS_ETVA_AMT = Round2(ws_compute_totals.WS_EARN_AMT * WS_ETVA_PERCENT);

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
            WS_ETVA2_AMT = Round2(ws_compute_totals.WS_EARN2_AMT * WS_ETVA_PERCENT);
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


    public void m260Maxcont()
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

        prft.FD_MAXOVER = ws_maxcont_totals.WS_OVER;
        prft.FD_MAXPOINTS = ws_payprofit.WS_PROF_POINTS;
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

    public void m410LoadProfit()
    {
        prft.FD_BADGE = intermediate_values.WS_FD_BADGE;
        prft.FD_NAME = intermediate_values.WS_FD_NAME;
        prft.FD_SSN = intermediate_values.WS_FD_SSN;
        prft.FD_PSN = intermediate_values.WS_FD_PSN;
        prft.FD_AMT = intermediate_values.WS_FD_AMT;
        prft.FD_DIST1 = intermediate_values.WS_FD_DIST1;
        prft.FD_MIL = intermediate_values.WS_FD_MIL;
        prft.FD_XFER = intermediate_values.WS_FD_XFER;
        prft.FD_PXFER = intermediate_values.WS_FD_PXFER;
        prft.FD_NEWEMP = intermediate_values.WS_FD_NEWEMP;
        prft.FD_POINTS = intermediate_values.WS_FD_POINTS;
        prft.FD_POINTS_EARN = intermediate_values.WS_FD_POINTS_EARN;
        prft.FD_CONT = intermediate_values.WS_FD_CONT;
        prft.FD_FORF = intermediate_values.WS_FD_FORF;
        prft.FD_EARN = intermediate_values.WS_FD_EARN;
        prft.FD_CAF = intermediate_values.WS_FD_CAF;
        prft.FD_EARN += intermediate_values.WS_FD_ETVA;
        prft.FD_EARN2 = intermediate_values.WS_FD_EARN2;
        prft.FD_EARN2 += intermediate_values.WS_FD_ETVA2;
        prfts.Add(prft);
        prft = new PRFT();
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
        int dbStatus = profitDetailTable.LoadNextRecord(ssn, profit_detail);

        while (dbStatus == 0)
        {
            dbStatus = m520TotalUpDetails(ssn);
        }
    }


    public int m520TotalUpDetails(int ssn)
    {
        // not needed?   DB_STATUS = MSTR_GET_REC(IDS2_REC_NAME);

        long WS_PROFIT_YEAR_FIRST_4 = (long)profit_detail.PROFIT_YEAR;
        string[] parts = profit_detail.PROFIT_YEAR.ToString().Split('.');
        long decimalPart = parts.Length > 1 ? long.Parse(parts[1]) : 0;
        long WS_PROFIT_YEAR_EXTENSION = decimalPart;

        if (WS_PROFIT_YEAR_FIRST_4 == EffectiveYear)
        {
            if (profit_detail.PROFIT_CODE == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal/*1*/ || profit_detail.PROFIT_CODE == ProfitCode.Constants.OutgoingDirectPayments /*3*/)
            {
                DIST_TOTAL = DIST_TOTAL + profit_detail.PROFIT_FORT;
            }

            if (profit_detail.PROFIT_CODE == ProfitCode.Constants.Outgoing100PercentVestedPayment /*9*/)
            {
                if (profit_detail.PROFIT_CMNT![..6] == "XFER >" ||
                    profit_detail.PROFIT_CMNT[..6] == "QDRO >" ||
                    profit_detail.PROFIT_CMNT[..5] == "XFER>" ||
                    profit_detail.PROFIT_CMNT[..5] == "QDRO>")
                {
                    PALLOCATION_TOTAL = PALLOCATION_TOTAL + profit_detail.PROFIT_FORT;
                }
                else
                {
                    DIST_TOTAL = DIST_TOTAL + profit_detail.PROFIT_FORT;
                }
            }

            if (profit_detail.PROFIT_CODE == ProfitCode.Constants.OutgoingForfeitures /*2*/)
            {
                FORFEIT_TOTAL = FORFEIT_TOTAL + profit_detail.PROFIT_FORT;
            }

            if (profit_detail.PROFIT_CODE == ProfitCode.Constants.OutgoingXferBeneficiary /*5*/)
            {
                PALLOCATION_TOTAL = PALLOCATION_TOTAL + profit_detail.PROFIT_FORT;
            }

            if (profit_detail.PROFIT_CODE == ProfitCode.Constants.IncomingQdroBeneficiary /*6*/)
            {
                ALLOCATION_TOTAL = ALLOCATION_TOTAL + profit_detail.PROFIT_CONT;
            }

            if (WS_PROFIT_YEAR_EXTENSION == 1 /*Military*/)
            {
                ws_payprofit.WS_PROF_MIL = ws_payprofit.WS_PROF_MIL + profit_detail.PROFIT_CONT;
            }

            if (WS_PROFIT_YEAR_EXTENSION == 2 /*Class Action Fund*/)
            {
                ws_payprofit.WS_PROF_CAF = ws_payprofit.WS_PROF_CAF + profit_detail.PROFIT_EARN;
            }
        }

        int dbStatus = profitDetailTable.LoadNextRecord(ssn, profit_detail);
        return dbStatus;
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


        prfts.Sort((a, b) =>
        {
            int nameComparison = StringComparer.Ordinal.Compare(a.FD_NAME, b.FD_NAME);
            if (nameComparison != 0)
            {
                return nameComparison;
            }
            // This is so we converge on a consistant sort.  This effectively matches Ready's order.
            long aBadge = Convert.ToInt64(a.FD_BADGE);
            long bBadge = Convert.ToInt64(b.FD_BADGE);
            aBadge = aBadge == 0 ? a.FD_PSN : aBadge;
            bBadge = bBadge == 0 ? b.FD_PSN : bBadge;
            return aBadge < bBadge ? -1 : 1;
        });

        foreach (SD_PRFT sd_sorted in prfts.Select(p => new SD_PRFT(p)).ToList())
        {
            sd_prft = sd_sorted;
            m810WriteReport(header_1);
        }

        m850PrintTotals();
    }

    private void WRITE(object obj)
    {
        outputLines.Add(obj.ToString().TrimEnd());
    }


    public void m810WriteReport(HEADER_1 header_1)
    {
        if (_counters.LineCounter > 60)
        {
            m830PrintHeader(header_1);
        }

        REPORT_LINE report_line = new();
        REPORT_LINE_2 report_line_2 = new();
        if (sd_prft.SD_BADGE > 0)
        {
            report_line.BADGE_NBR = sd_prft.SD_BADGE;
            report_line.EMP_NAME = sd_prft.SD_NAME?.Length > 24 ? sd_prft.SD_NAME.Substring(0, 24) : sd_prft.SD_NAME;

            report_line.BEG_BAL = sd_prft.SD_AMT;
            report_line.PR_DIST1 = sd_prft.SD_DIST1;

            if (sd_prft.SD_NEWEMP == 1)
            {
                report_line.PR_NEWEMP = "NEW";
            }
            else if (sd_prft.SD_NEWEMP == 2)
            {
                payben_rec.Ssn = sd_prft.SD_SSN;
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

            if (sd_prft.SD_XFER != 0)
            {
                sd_prft.SD_CONT = sd_prft.SD_CONT + sd_prft.SD_XFER;
            }

            if (sd_prft.SD_PXFER != 0)
            {
                sd_prft.SD_MIL = sd_prft.SD_MIL - sd_prft.SD_PXFER;
            }

            report_line.PR_CONT = sd_prft.SD_CONT;
            report_line.PR_MIL = sd_prft.SD_MIL;
            report_line.PR_FORF = sd_prft.SD_FORF;
            report_line.PR_EARN = sd_prft.SD_EARN;

            if (sd_prft.SD_EARN2 != 0)
            {
                DISPLAY($"badge {sd_prft.SD_BADGE} earnings2 ${sd_prft.SD_EARN2}");
            }

            report_line.PR_EARN2 = sd_prft.SD_EARN2;
            report_line.PR_EARN2 = sd_prft.SD_CAF;
        }


        decimal endingBalance = sd_prft.SD_AMT + sd_prft.SD_CONT + sd_prft.SD_EARN + sd_prft.SD_EARN2 +
            sd_prft.SD_FORF + sd_prft.SD_MIL + sd_prft.SD_CAF - sd_prft.SD_DIST1;
        report_line.END_BAL = endingBalance;

        if (sd_prft.SD_BADGE == 0)
        {
            report_line_2.PR2_EMP_NAME =
                sd_prft.SD_NAME?.Length > 24 ? sd_prft.SD_NAME.Substring(0, 24) : sd_prft.SD_NAME;
            report_line_2.PR2_PSN = sd_prft.SD_PSN;
            report_line_2.PR2_BEG_BAL = sd_prft.SD_AMT;
            report_line_2.PR2_DIST1 = sd_prft.SD_DIST1;
            report_line_2.PR2_NEWEMP = "BEN";
            sd_prft.SD_CONT += sd_prft.SD_XFER;
            report_line_2.PR2_CONT = sd_prft.SD_CONT;
            report_line_2.PR2_MIL = sd_prft.SD_MIL;
            report_line_2.PR2_FORF = sd_prft.SD_FORF;
            report_line_2.PR2_EARN = sd_prft.SD_EARN;
            report_line_2.PR2_EARN2 = sd_prft.SD_EARN2;
            report_line_2.PR2_EARN2 = sd_prft.SD_CAF;

            endingBalance = sd_prft.SD_AMT + sd_prft.SD_CONT + sd_prft.SD_EARN + sd_prft.SD_EARN2 +
                sd_prft.SD_FORF + sd_prft.SD_MIL + sd_prft.SD_CAF - sd_prft.SD_DIST1;
            report_line_2.PR2_END_BAL = endingBalance;
        }

        ws_client_totals.WS_TOT_BEGBAL += sd_prft.SD_AMT;
        if (sd_prft.SD_XFER != 0)
        {
            sd_prft.SD_CONT -= sd_prft.SD_XFER;
        }

        if (sd_prft.SD_PXFER != 0)
        {
            sd_prft.SD_MIL += sd_prft.SD_PXFER;
        }

        ws_client_totals.WS_TOT_DIST1 += sd_prft.SD_DIST1;
        ws_client_totals.WS_TOT_CONT += sd_prft.SD_CONT;
        ws_client_totals.WS_TOT_MIL += sd_prft.SD_MIL;
        ws_client_totals.WS_TOT_FORF += sd_prft.SD_FORF;
        ws_client_totals.WS_TOT_EARN += sd_prft.SD_EARN;
        ws_client_totals.WS_TOT_EARN2 += sd_prft.SD_EARN2;
        ws_client_totals.WS_TOT_ENDBAL += endingBalance;
        ws_client_totals.WS_TOT_XFER += sd_prft.SD_XFER;
        ws_client_totals.WS_TOT_PXFER -= sd_prft.SD_PXFER;
        ws_client_totals.WS_EARN_PTS_TOTAL += sd_prft.SD_POINTS_EARN;
        ws_client_totals.WS_PROF_PTS_TOTAL += sd_prft.SD_POINTS;
        ws_client_totals.WS_TOT_CAF += sd_prft.SD_CAF;
        ws_maxcont_totals.WS_TOT_OVER += sd_prft.SD_MAXOVER;
        ws_maxcont_totals.WS_TOT_POINTS += sd_prft.SD_MAXPOINTS;

        if (sd_prft.SD_AMT != 0m
            || sd_prft.SD_DIST1 != 0m
            || sd_prft.SD_CONT != 0m
            || sd_prft.SD_XFER != 0m
            || sd_prft.SD_PXFER != 0m
            || sd_prft.SD_MIL != 0m
            || sd_prft.SD_FORF != 0m
            || sd_prft.SD_EARN != 0m
            || sd_prft.SD_EARN2 != 0m)
        {
            if (sd_prft.SD_BADGE > 0)
            {
                _counters.EmployeeCounter += 1;
                WRITE(report_line);
            }

            if (sd_prft.SD_BADGE == 0)
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

        outputLines.Add("\n\n\n\n\n\n\n\n\n");
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
        print_adj_line1.PL_CONT_AMT = adjustmentReportValues.SV_CONT_AMT;
        print_adj_line1.PL_FORF_AMT = adjustmentReportValues.SV_FORF_AMT;
        print_adj_line1.PL_EARN_AMT = adjustmentReportValues.SV_EARN_AMT;
        print_adj_line1.PL_EARN2_AMT = adjustmentReportValues.SV_EARN2_AMT;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJUST_BADGE = 0;
        print_adj_line1.PL_ADJ_DESC = "ADJUSTMENT";
        print_adj_line1.PL_CONT_AMT = point_values.PV_ADJ_CONTRIB;
        print_adj_line1.PL_EARN_AMT = point_values.PV_ADJ_EARN;
        print_adj_line1.PL_EARN2_AMT = point_values.PV_ADJ_EARN2;
        print_adj_line1.PL_FORF_AMT = point_values.PV_ADJ_FORFEIT;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJ_DESC = "FINAL";
        print_adj_line1.PL_CONT_AMT = adjustmentReportValues.SV_CONT_ADJUSTED;
        print_adj_line1.PL_FORF_AMT = adjustmentReportValues.SV_FORF_ADJUSTED;
        print_adj_line1.PL_EARN_AMT = adjustmentReportValues.SV_EARN_ADJUSTED;
        print_adj_line1.PL_EARN2_AMT = adjustmentReportValues.SV_EARN2_ADJUSTED;

        WRITE2_advance2(print_adj_line1);

        if (adjustmentReportValues.SV_FORF_AMT == 0 && adjustmentReportValues.SV_EARN_AMT == 0)
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
