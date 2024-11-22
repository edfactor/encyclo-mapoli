using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class PAY444
{
    // classic pay444 structures
    private readonly ClientTot client_tot = new();
    private readonly DEM_REC dem_rec = new();
    private readonly EMPLOYEE_COUNT_TOT employee_count_tot = new();
    private readonly EMPLOYEE_COUNT_TOT_PAYBEN employee_count_tot_payben = new();
    private readonly GRAND_TOT grand_tot = new();
    private readonly HEADER_1 header_1 = new();
    private readonly HEADER_2 header_2 = new();
    private readonly HEADER_3 header_3 = new();
    private readonly PAYBEN_REC payben_rec = new();
    private readonly PayBenReader PAYBEN1 = new();
    private readonly PAYBEN1_REC payben1_rec = new();
    private readonly PAYPROF_REC payprof_rec = new();
    private readonly PayProfRecTableHelper PAYPROFIT_FILE = new();
    private readonly POINT_VALUES point_values = new();
    private readonly List<PRFT> prfts = new();
    private readonly PRINT_ADJ_LINE1 print_adj_line1 = new();
    private readonly PROFIT_DETAIL profit_detail = new();
    private readonly PROFIT_SS_DETAIL profit_ss_detail = new();
    private readonly REPORT_LINE report_line = new();
    private readonly REPORT_LINE_2 report_line_2 = new();
    private readonly RERUN_TOT rerun_tot = new();
    private readonly TOTAL_HEADER_1 total_header_1 = new();
    private readonly TOTAL_HEADER_2 total_header_2 = new();
    private readonly TOTAL_HEADER_3 total_header_3 = new();
    private readonly WS_COUNTERS ws_counters = new();
    private readonly WS_ENDING_BALANCE ws_ending_balance = new();
    private readonly WS_GRAND_TOTALS ws_grand_totals = new();
    private readonly WS_INDICATORS ws_indicators = new();
    private readonly WS_SOCIAL_SECURITY ws_social_security = new();
    private readonly XEROX_HEADER xerox_header = new();
    public OracleConnection connection = null;
    public DemRecTableHelper DemRecTableHelper;
    private INTERMEDIATE_VALUES intermediate_values = new();

    // new structures
    private Dictionary<int, int> META_SW = new();
    public List<string> outputLines = new();
    private PAYPROF_REC payprof_rec1 = new();
    private PRFT prft = new();
    private ProfitDetailTableHelper profitDetailTable; // initialized on demand
    private SD_PRFT sd_prft = new();
    private WS_CLIENT_TOTALS ws_client_totals = new();
    private WS_COMPUTE_TOTALS ws_compute_totals = new();
    private WS_MAXCONT_TOTALS ws_maxcont_totals = new();
    private WS_PAYPROFIT ws_payprofit = new();

    public string? PAYPROFIT_FILE_STATUS { get; set; }
    public string? PAYBEN_FILE_STATUS { get; set; }
    public string? DEMO_PROFSHARE_FILE_STATUS { get; set; } = "00";

    public long YEARS { get; set; }
    public long AGE { get; set; }
    public long FIRST_REC { get; set; }
    public long HOLD_SSN { get; set; }
    public long HOLD_PAYSSN { get; set; }
    public long INVALID_CNT { get; set; }

    public long WS_REWRITE_WHICH { get; set; }
    public long WS_ERROR { get; set; }
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

    public long SOC_SEC_NUMBER { get; set; }
    public DateTime TodaysDateTime { get; set; } = DateTime.Now;

    private long holdBadge;

    // It is annoying that forfeit proceeds earnings, but that is the way the Cobol prompts for them.  
    public void m015MainProcessing(Dictionary<int, int> META_SW, long year, decimal conributionPercent, decimal incomingForfeitPercent, decimal earningsPercent,  decimal secondaryEarningsPercent,
        long adjustBadge, decimal adjustContrib, decimal adjustForfeit, decimal adjustEarnings, long adjustBadgeSecondary, decimal adjustEarningsSecondary, long maxContribution)
    {
        // This connection is bound late.
        PAYBEN1.Connection = connection;
        PAYPROFIT_FILE.connection = connection;

        this.META_SW = META_SW;

        EffectiveYear = year;
        if (META_SW[3] == 0) // Do not ask for input values
        {
            point_values.PV_CONT_01 = conributionPercent;
            point_values.PV_FORF_01 = incomingForfeitPercent;
            point_values.PV_EARN_01 = earningsPercent;

            if (META_SW[5] == 1) // Secondary Earnings Flag
            {
                point_values.PV_EARN2_01 = secondaryEarningsPercent;  // Gather Input from User

                if (point_values.PV_EARN2_01 == 0)
                {
                    META_SW[5] = 0;
                }
            }

            if (META_SW[4] == 0) // Suppress Manual Adjustments Flag
            { 
                point_values.PV_ADJUST_BADGE = adjustBadge; // badge to adjust
                point_values.PV_ADJ_CONTRIB = adjustContrib; // amount to adjust employee
                point_values.PV_ADJ_FORFEIT = adjustForfeit;
                point_values.PV_ADJ_EARN = adjustEarnings;

                if (META_SW[5] == 1) // Secondary Earnings Flag
                {
                    point_values.PV_ADJUST_BADGE2 = adjustBadgeSecondary;
                    point_values.PV_ADJ_EARN2 = adjustEarningsSecondary;
                }
            }

            WS_CONTR_MAX = maxContribution;
        }

        HOLD_PAYSSN = 0;
        m200ProcessProfit();
        if (INVALID_CNT > 0)
        {
        }

        if (ws_indicators.WS_RERUN_IND == 1)
        {
            m020Rerun();
        }

        m805PrintSequence();

        m1000AdjustmentReport();
    }


    public void m020Rerun()
    {
        throw new IOException("Rerun of PAY444 is required.");
    }


    public void m110AcceptValues01(
        )
    {
    }


    public void m200ProcessProfit()
    {
        m201ProcessPayProfit();
        m202_PROCESS_PAYBEN();
    }


    public void m201ProcessPayProfit()
    {
        l201_PROCESS_PAYPROFIT:
        payprof_rec1 = PAYPROFIT_FILE.Read();
        if (PAYPROFIT_FILE.isEOF())
        {
            payprof_rec.PAYPROF_BADGE = holdBadge;
            if (FIRST_REC != 0) // BOBH
            {
                m210PayprofitComputation();
            }

            prft.FD_MAXOVER = 0;
            prft.FD_MAXPOINTS = 0;
            ws_payprofit.WS_PROF_POINTS = 0;
            ws_maxcont_totals.WS_OVER = 0;
            goto l201_EXIT;
        }

        if (FIRST_REC == 0)
        {
            ws_compute_totals = new WS_COMPUTE_TOTALS();
            ws_payprofit = new WS_PAYPROFIT();
            FIRST_REC = 1;
            HOLD_SSN = payprof_rec1.PAYPROF_SSN;
        }

        if (payprof_rec1.PAYPROF_SSN != HOLD_SSN)
        {
            payprof_rec.PAYPROF_BADGE = holdBadge;
            m210PayprofitComputation();
            ws_compute_totals = new WS_COMPUTE_TOTALS();
            ws_payprofit = new WS_PAYPROFIT();
            HOLD_SSN = payprof_rec1.PAYPROF_SSN;
        }

        if (DEMO_PROFSHARE_FILE_STATUS != "00")
        {
            holdBadge = payprof_rec1.PAYPROF_BADGE;
            goto l201_PROCESS_PAYPROFIT;
        }

        ws_payprofit.WS_PS_AMT = ws_payprofit.WS_PS_AMT + payprof_rec1.PY_PS_AMT;
        ws_payprofit.WS_PROF_POINTS = ws_payprofit.WS_PROF_POINTS + payprof_rec1.PY_PROF_POINTS;
        holdBadge = payprof_rec1.PAYPROF_BADGE;
        goto l201_PROCESS_PAYPROFIT;
        l201_EXIT: ;
    }


    private void m202_PROCESS_PAYBEN()
    {
        l202_PROCESS_PAYBEN:
        PAYBEN1.Read(payben1_rec);
        if (PAYBEN1.isEOF())
        {
            goto l202_EXIT;
        }

        if (payben1_rec.PYBEN_PAYSSN1 == HOLD_PAYSSN)
        {
            goto l202_PROCESS_PAYBEN;
        }

        HOLD_PAYSSN = payben1_rec.PYBEN_PAYSSN1;
        m208CheckPayprofitFromPayben();
        if (WS_ERROR == 1)
        {
            goto l202_PROCESS_PAYBEN;
        }

        ws_compute_totals = new WS_COMPUTE_TOTALS();
        ws_payprofit = new WS_PAYPROFIT();

        m220PaybenComputation();

        ws_compute_totals = new WS_COMPUTE_TOTALS();
        ws_payprofit = new WS_PAYPROFIT();

        goto l202_PROCESS_PAYBEN;

        l202_EXIT: ;
    }


    public void m208CheckPayprofitFromPayben()
    {
        payprof_rec.PAYPROF_SSN = payben1_rec.PYBEN_PAYSSN1;
        PAYPROFIT_FILE_STATUS = READ_ALT_KEY_PAYPROFIT(payprof_rec);
        WS_ERROR = 0;

        if (PAYPROFIT_FILE_STATUS == "00")
        {
            WS_ERROR = 1;
            // Indicates that an employee is also a bene
            payprof_rec.PY_PROF_NEWEMP = 2;
        }
    }

    private string? READ_ALT_KEY_PAYPROFIT(PAYPROF_REC payprof_rec)
    {
        if (PAYPROFIT_FILE.HasRecordBySsn(payprof_rec.PAYPROF_SSN))
        {
            return "00";
        }

        return "Something else";
    }


    public void m210PayprofitComputation()
    {
        PAYPROFIT_FILE_STATUS = READ_KEY_PAYPROFIT(payprof_rec);
        if (PAYPROFIT_FILE_STATUS != "00")
        {
            INVALID_CNT += 1;
            DISPLAY("INVALID PAYPROFIT RECORD NOT UPDATED :" + PAYPROFIT_FILE_STATUS);
            DISPLAY($"{payprof_rec.PAYPROF_BADGE} = INVALID PAYPROFIT RECORD NOT UPDATED");
            return;
        }

        //* If an employee has an ETVA amount and no years on the plan, employee is a
        //* beneficiary and should get earnings on the etva amt(8 record)
        if (payprof_rec.PY_PROF_NEWEMP == 0)
        {
            if (payprof_rec.PY_PS_ETVA > 0 && payprof_rec.PY_PS_AMT == 0)
            {
                payprof_rec.PY_PROF_NEWEMP = 2;
            }
        }

        if (payprof_rec.PY_PS_ENROLLED > 0 ||
            payprof_rec.PY_PROF_NEWEMP > 0 ||
            ws_payprofit.WS_PS_AMT > 0 || payprof_rec.PY_PS_YEARS > 0)
        {
        }
        else
        {
            return;
        }

        dem_rec.DEM_BADGE = payprof_rec.PAYPROF_BADGE;
        DEMO_PROFSHARE_FILE_STATUS = READ_KEY_DEMO_PROFSHARE(dem_rec);
        if (DEMO_PROFSHARE_FILE_STATUS != "00")
        {
            return;
        }

        WS_REWRITE_WHICH = 1;

        intermediate_values = new INTERMEDIATE_VALUES();

        m500GetDbInfo();

        prft = new PRFT();

        payprof_rec.PY_PROF_MAXCONT = 0;
        if (payprof_rec.PY_PROF_MAXCONT == 1 || META_SW[2] == 1) // Special Run 
        {
            m410LoadProfit();
            return;
        }

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

        intermediate_values.WS_FD_BADGE = payprof_rec.PAYPROF_BADGE;
        intermediate_values.WS_FD_NAME = dem_rec.PY_NAM;
        intermediate_values.WS_FD_SSN = payprof_rec.PAYPROF_SSN;
        intermediate_values.WS_FD_PSN = payprof_rec.PAYPROF_BADGE;

        m250ComputeEarnings();

        intermediate_values.WS_FD_XFER = ALLOCATION_TOTAL;
        intermediate_values.WS_FD_PXFER = PALLOCATION_TOTAL;
        intermediate_values.WS_FD_AMT = ws_payprofit.WS_PS_AMT;
        intermediate_values.WS_FD_DIST1 = DIST_TOTAL;
        intermediate_values.WS_FD_MIL = ws_payprofit.WS_PROF_MIL;
        intermediate_values.WS_FD_CAF = ws_payprofit.WS_PROF_CAF;
        intermediate_values.WS_FD_NEWEMP = payprof_rec.PY_PROF_NEWEMP;
        intermediate_values.WS_FD_POINTS = ws_payprofit.WS_PROF_POINTS;
        intermediate_values.WS_FD_POINTS_EARN = ws_compute_totals.WS_EARN_POINTS;
        intermediate_values.WS_FD_CONT = ws_payprofit.WS_PROF_CONT;
        intermediate_values.WS_FD_FORF = ws_payprofit.WS_PROF_FORF;
        intermediate_values.WS_FD_FORF = intermediate_values.WS_FD_FORF - FORFEIT_TOTAL;
        intermediate_values.WS_FD_EARN = payprof_rec.PY_PROF_EARN;
        intermediate_values.WS_FD_EARN += payprof_rec.PY_PROF_ETVA;
        intermediate_values.WS_FD_EARN2 = payprof_rec.PY_PROF_EARN2;
        intermediate_values.WS_FD_EARN2 += payprof_rec.PY_PROF_ETVA2;

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
        if (META_SW[8] == 0)
        {
            m420RewritePayprofit();
        }

        l210_EXIT: ;
    }

    private decimal Round(decimal v)
    {
        return Math.Round(v, MidpointRounding.AwayFromZero);
    }

    private decimal Round2(decimal v)
    {
        return Math.Round(v, 2, MidpointRounding.AwayFromZero);
    }


    private string? READ_KEY_DEMO_PROFSHARE(DEM_REC dem_rec)
    {
        if (DemRecTableHelper == null)
        {
            DemRecTableHelper = new DemRecTableHelper(connection, dem_rec);
        }

        return DemRecTableHelper.getByBadge(dem_rec);
    }

    private string? READ_KEY_PAYPROFIT(PAYPROF_REC payprof_rec)
    {
        PAYPROF_REC one = PAYPROFIT_FILE.findByBadge(payprof_rec.PAYPROF_BADGE);
        payprof_rec.PAYPROF_BADGE = one.PAYPROF_BADGE;
        payprof_rec.PAYPROF_SSN = one.PAYPROF_SSN;
        payprof_rec.PY_PH = one.PY_PH;
        payprof_rec.PY_PD = one.PY_PD;
        payprof_rec.PY_WEEKS_WORK = one.PY_WEEKS_WORK;
        payprof_rec.PY_PROF_CERT = one.PY_PROF_CERT;
        payprof_rec.PY_PS_ENROLLED = one.PY_PS_ENROLLED;
        payprof_rec.PY_PS_YEARS = one.PY_PS_YEARS;
        payprof_rec.PY_PROF_BENEFICIARY = one.PY_PROF_BENEFICIARY;
        payprof_rec.PY_PROF_INITIAL_CONT = one.PY_PROF_INITIAL_CONT;
        payprof_rec.PY_PS_AMT = one.PY_PS_AMT;
        payprof_rec.PY_PS_VAMT = one.PY_PS_VAMT;
        payprof_rec.PY_PH_LASTYR = one.PY_PH_LASTYR;
        payprof_rec.PY_PD_LASTYR = one.PY_PD_LASTYR;
        payprof_rec.PY_PROF_NEWEMP = one.PY_PROF_NEWEMP;
        payprof_rec.PY_PROF_POINTS = one.PY_PROF_POINTS;
        payprof_rec.PY_PROF_CONT = one.PY_PROF_CONT;
        payprof_rec.PY_PROF_FORF = one.PY_PROF_FORF;
        payprof_rec.PY_VESTED_FLAG = one.PY_VESTED_FLAG;
        payprof_rec.PY_PROF_MAXCONT = one.PY_PROF_MAXCONT;
        payprof_rec.PY_PROF_ZEROCONT = one.PY_PROF_ZEROCONT;
        payprof_rec.PY_WEEKS_WORK_LAST = one.PY_WEEKS_WORK_LAST;
        payprof_rec.PY_PROF_EARN = one.PY_PROF_EARN;
        payprof_rec.PY_PS_ETVA = one.PY_PS_ETVA;
        payprof_rec.PY_PRIOR_ETVA = one.PY_PRIOR_ETVA;
        payprof_rec.PY_PROF_ETVA = one.PY_PROF_ETVA;
        payprof_rec.PY_PROF_EARN2 = one.PY_PROF_EARN2;
        payprof_rec.PY_PROF_ETVA2 = one.PY_PROF_ETVA2;
        payprof_rec.PY_PH_EXEC = one.PY_PH_EXEC;
        payprof_rec.PY_PD_EXEC = one.PY_PD_EXEC;

        return "00";
    }


    public void m220PaybenComputation()
    {
        // <same key> PSKEY = payben1_rec.PYBEN_PSN1;
        payben_rec.PYBEN_PSN = payben1_rec.PYBEN_PSN1;
        PAYBEN_FILE_STATUS = READ_KEY_PAYBEN(payben_rec);
        if (PAYBEN_FILE_STATUS != "00")
        {
            INVALID_CNT += 1;
            DISPLAY(
                $"{PAYBEN_FILE_STATUS} {payben_rec.PYBEN_PSN} {payben_rec.PYBEN_PAYSSN} = INVALID PAYBEN RECORD NOT UPDATED");
            goto l220_EXIT;
        }

        WS_REWRITE_WHICH = 2;

        intermediate_values = new INTERMEDIATE_VALUES();

        payprof_rec.PAYPROF_SSN = payben_rec.PYBEN_PAYSSN;

        ws_payprofit.WS_PS_AMT = payben_rec.PYBEN_PSAMT;

        ws_compute_totals.WS_POINTS_DOLLARS = 0m;
        ws_compute_totals.WS_EARNINGS_BALANCE = 0m;
        ws_compute_totals.WS_EARN_POINTS = 0;
        payprof_rec.PAYPROF_BADGE = 0;

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
            ALLOCATION_TOTAL + (ws_payprofit.WS_PS_AMT - FORFEIT_TOTAL - PALLOCATION_TOTAL) - DIST_TOTAL;

        ws_compute_totals.WS_EARN_POINTS = (long)Round(ws_compute_totals.WS_POINTS_DOLLARS / 100);

        l220_CONTINUE:
        if (ALLOCATION_TOTAL != 0)
        {
            payprof_rec.PY_PROF_NEWEMP = 2;
            if (payprof_rec.PY_PROF_ETVA == 0)
            {
                payprof_rec.PY_PROF_ETVA = 0.01m;
            }
        }

        dem_rec.PY_NAM = payben_rec.PYBEN_NAME;
        payprof_rec.PAYPROF_BADGE = 0;
        m250ComputeEarnings();

        intermediate_values.WS_FD_BADGE = 0;
        intermediate_values.WS_FD_NAME = payben_rec.PYBEN_NAME;
        intermediate_values.WS_FD_SSN = payben_rec.PYBEN_PAYSSN;
        intermediate_values.WS_FD_PSN = payben_rec.PYBEN_PSN;
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

        intermediate_values.WS_FD_EARN = payben_rec.PYBEN_PROF_EARN;
        intermediate_values.WS_FD_EARN2 = payben_rec.PYBEN_PROF_EARN2;

        ws_maxcont_totals.WS_MAX = ws_payprofit.WS_PROF_CONT + ws_payprofit.WS_PROF_MIL + ws_payprofit.WS_PROF_FORF;

        if (ws_maxcont_totals.WS_MAX > WS_CONTR_MAX)
        {
            m260Maxcont();
        }

        m410LoadProfit();

        if (META_SW[8] == 0)
        {
            m430RewritePayben();
        }
        
        l220_EXIT: ;
    }

    private string? READ_KEY_PAYBEN(PAYBEN_REC payben_rec)
    {
        return PAYBEN1.findByPSN(payben_rec);
    }


    public void m230ComputeContribution()
    {
        ws_compute_totals.WS_CONT_AMT = Round2(point_values.PV_CONT_01 * ws_payprofit.WS_PROF_POINTS);

        if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
        {
            point_values.SV_SSN = HOLD_SSN;
            point_values.SV_CONT_AMT = ws_compute_totals.WS_CONT_AMT;
            ws_compute_totals.WS_CONT_AMT += point_values.PV_ADJ_CONTRIB;
            point_values.SV_CONT_ADJUSTED = ws_compute_totals.WS_CONT_AMT;
        }

        if (META_SW[2] == 1) // Special Run
        {
            ws_payprofit.WS_PROF_CONT += ws_compute_totals.WS_CONT_AMT;
        }
        else
        {
            ws_payprofit.WS_PROF_CONT = ws_compute_totals.WS_CONT_AMT;
        }
    }


    public void m240ComputeForfeitures()
    {
        ws_compute_totals.WS_FORF_AMT = Round2(point_values.PV_FORF_01 * ws_payprofit.WS_PROF_POINTS);
        if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
        {
            point_values.SV_FORF_AMT = ws_compute_totals.WS_FORF_AMT;
            ws_compute_totals.WS_FORF_AMT += point_values.PV_ADJ_FORFEIT;
            point_values.SV_FORF_ADJUSTED = ws_compute_totals.WS_FORF_AMT;
        }

        if (META_SW[2] == 1)
        {
            ws_payprofit.WS_PROF_FORF += ws_compute_totals.WS_FORF_AMT;
        }
        else
        {
            ws_payprofit.WS_PROF_FORF = ws_compute_totals.WS_FORF_AMT;
        }
    }

    public void m250ComputeEarnings()
    {
        if (META_SW[2] == 1)
        {
            goto l250_EXIT;
        }

        if (ws_compute_totals.WS_EARN_POINTS > 0 || WS_REWRITE_WHICH == 2)
        {
        }
        else
        {
            if (ws_compute_totals.WS_EARN_POINTS <= 0)
            {
                ws_compute_totals.WS_EARN_POINTS = 0;
                payprof_rec.PY_PROF_EARN = 0;
                ws_compute_totals.WS_EARN_AMT = 0;
                payben_rec.PYBEN_PROF_EARN = 0;
                payprof_rec.PY_PROF_EARN2 = 0;
                ws_compute_totals.WS_EARN2_AMT = 0;
                payben_rec.PYBEN_PROF_EARN2 = 0;
            }
        }

        if (WS_REWRITE_WHICH == 1 || WS_REWRITE_WHICH == 2)
        {
            ws_compute_totals.WS_EARN_AMT = Round2(point_values.PV_EARN_01 * ws_compute_totals.WS_EARN_POINTS);
            if (point_values.PV_ADJUST_BADGE > 0 && point_values.PV_ADJUST_BADGE == holdBadge)
            {
                point_values.SV_EARN_AMT = ws_compute_totals.WS_EARN_AMT;
                ws_compute_totals.WS_EARN_AMT += point_values.PV_ADJ_EARN;
                point_values.SV_EARN_ADJUSTED = ws_compute_totals.WS_EARN_AMT;
            }

            if (META_SW[5] == 1) // Secondary Earnings
            {
                ws_compute_totals.WS_EARN2_AMT = Round2(point_values.PV_EARN2_01 * ws_compute_totals.WS_EARN_POINTS);
                if (point_values.PV_ADJUST_BADGE2 > 0 && point_values.PV_ADJUST_BADGE2 == holdBadge)
                {
                    point_values.SV_EARN2_AMT = ws_compute_totals.WS_EARN2_AMT;
                    ws_compute_totals.WS_EARN2_AMT += point_values.PV_ADJ_EARN2;
                    point_values.SV_EARN2_ADJUSTED = ws_compute_totals.WS_EARN2_AMT;
                }
            }
            else
            {
                ws_compute_totals.WS_EARN2_AMT = 0m;
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
        if (payprof_rec.PY_PS_ETVA > 0)
        {
            if (payprof_rec.PY_PS_YEARS < 6)
            {
                WS_PY_PS_ETVA = payprof_rec.PY_PS_ETVA - ws_payprofit.WS_PROF_CAF;
            }
            else
            {
                payprof_rec.PY_PS_ETVA = WS_PY_PS_ETVA;
            }
        }

        if (WS_PY_PS_ETVA > 0 || WS_REWRITE_WHICH == 2)
        {
        }
        else
        {
            payprof_rec.PY_PROF_EARN = ws_compute_totals.WS_EARN_AMT;
            if (META_SW[5] == 1) // Secondary earnings
            {
                payprof_rec.PY_PROF_EARN2 = ws_compute_totals.WS_EARN2_AMT;
            }
            else
            {
                payprof_rec.PY_PROF_EARN2 = 0m;
            }

            payprof_rec.PY_PROF_ETVA = 0m;
            payben_rec.PYBEN_PROF_EARN = 0m;
            payprof_rec.PY_PROF_ETVA2 = 0m;
            payben_rec.PYBEN_PROF_EARN2 = 0m;
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
            payprof_rec.PY_PROF_EARN = ws_compute_totals.WS_EARN_AMT;
            payprof_rec.PY_PROF_ETVA = WS_ETVA_AMT;
        }

        if (WS_REWRITE_WHICH == 2)
        {
            payben_rec.PYBEN_PROF_EARN = 0m;
            payben_rec.PYBEN_PROF_EARN = ws_compute_totals.WS_EARN_AMT;
        }

        if (META_SW[5] == 1) // Secondary Earnings
        {
            WS_ETVA2_AMT = Round2(ws_compute_totals.WS_EARN2_AMT * WS_ETVA_PERCENT);
            ws_compute_totals.WS_EARN2_AMT -= WS_ETVA2_AMT;
            payprof_rec.PY_PROF_EARN2 = ws_compute_totals.WS_EARN2_AMT;
            payprof_rec.PY_PROF_ETVA2 = WS_ETVA2_AMT;
            if (WS_REWRITE_WHICH == 2)
            {
                payben_rec.PYBEN_PROF_EARN2 = WS_ETVA2_AMT;
            }
        }

        l250_EXIT: ;
    }


    public void m260Maxcont()
    {
        ws_maxcont_totals.WS_OVER = ws_maxcont_totals.WS_MAX - WS_CONTR_MAX;

        if (ws_maxcont_totals.WS_OVER < ws_payprofit.WS_PROF_FORF)
        {
            ws_payprofit.WS_PROF_FORF = ws_payprofit.WS_PROF_FORF - ws_maxcont_totals.WS_OVER;
        }
        else
        {
            DISPLAY($"FORFEITURES NOT ENOUGH FOR AMOUNT OVER MAX FOR EMPLOYEE BADGE #{holdBadge}");
            ws_payprofit.WS_PROF_FORF = 0;
        }

        prft.FD_MAXOVER = ws_maxcont_totals.WS_OVER;
        prft.FD_MAXPOINTS = ws_payprofit.WS_PROF_POINTS;
        payprof_rec.PY_PROF_MAXCONT = 1;
        ws_indicators.WS_RERUN_IND = 1;
    }

    private void DISPLAY(string v)
    {
        Console.WriteLine("DISPLAY: " + v);
    }


    public void m400LoadPayprofit()
    {
        payprof_rec.PY_PROF_CONT = ws_payprofit.WS_PROF_CONT;
        payprof_rec.PY_PROF_FORF = ws_payprofit.WS_PROF_FORF;
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
        if (payprof_rec.PY_PROF_NEWEMP == 2)
        {
            payprof_rec.PY_PROF_NEWEMP = 0;
        }

        PAYPROFIT_FILE_STATUS = REWRITE_KEY_PAYPROFIT(payprof_rec);
        if (PAYPROFIT_FILE_STATUS != "00")
        {
            throw new IOException($"BAD REWRITE OF PAYPROFIT EMPLOYEE BADGE # {payprof_rec.PAYPROF_BADGE}");
        }
    }

    private string? REWRITE_KEY_PAYPROFIT(PAYPROF_REC payprof_rec)
    {
        throw new NotImplementedException();
    }


    public void m430RewritePayben()
    {
        if (payben_rec.PYBEN_PROF_EARN == 0 && payben_rec.PYBEN_PROF_EARN2 == 0 || META_SW[8] == 1)
        {
            goto l430_EXIT;
        }

        PAYBEN_FILE_STATUS = REWRITE_KEY_PAYBEN(payben_rec);
        if (PAYBEN_FILE_STATUS != "00")
        {
            throw new IOException($"BAD REWRITE OF PAYBEN EMPLOYEE PSN # {payben_rec.PYBEN_PSN}");
        }

        l430_EXIT: ;
    }

    private string? REWRITE_KEY_PAYBEN(PAYBEN_REC payben_rec)
    {
        throw new NotImplementedException();
    }


    public void m500GetDbInfo()
    {
        DIST_TOTAL = 0m;
        FORFEIT_TOTAL = 0m;
        ALLOCATION_TOTAL = 0m;
        PALLOCATION_TOTAL = 0m;

        SOC_SEC_NUMBER = payprof_rec.PAYPROF_SSN;
        m510GetDetails();
    }

    public void m510GetDetails()
    {
        int dbStatus = MSTR_FIND_WITHIN_PR_DET_S();

        while (dbStatus == 0)
        {
            dbStatus = m520TotalUpDetails();
        }

        dbStatus = MSTR_FIND_WITHIN_PR_SS_D_S();


        while (dbStatus == 0)
        {
            dbStatus = m530TotalUpSsDetails();
        }
    }

    private int MSTR_FIND_WITHIN_PR_SS_D_S()
    {
        // throw new NotImplementedException();
        // Debug.WriteLine("WARNING: READING PROFIT_SS_DETAILS IS DISABLED.");
        return 77; // BOBH THIS IS DISABLED!!!
    }

    private int MSTR_FIND_WITHIN_PR_DET_S()
    {
        if (profitDetailTable == null || profitDetailTable.ssn != SOC_SEC_NUMBER)
        {
            profitDetailTable = new ProfitDetailTableHelper(connection, profit_detail, SOC_SEC_NUMBER);
        }

        // Load profit_detail using SOC_SEC_NUMBER;
        return profitDetailTable.LoadNextRecord();
    }


    public int m520TotalUpDetails()
    {
        // not needed?   DB_STATUS = MSTR_GET_REC(IDS2_REC_NAME);

        long WS_PROFIT_YEAR_FIRST_4 = (long)profit_detail.PROFIT_YEAR;
        string[] parts = profit_detail.PROFIT_YEAR.ToString().Split('.');
        long decimalPart = parts.Length > 1 ? long.Parse(parts[1]) : 0;
        long WS_PROFIT_YEAR_EXTENSION = decimalPart;

        if (WS_PROFIT_YEAR_FIRST_4 == EffectiveYear)
        {
            if (profit_detail.PROFIT_CODE == "1" || profit_detail.PROFIT_CODE == "3")
            {
                DIST_TOTAL = DIST_TOTAL + profit_detail.PROFIT_FORT;
            }

            if (profit_detail.PROFIT_CODE == "9")
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

            if (profit_detail.PROFIT_CODE == "2")
            {
                FORFEIT_TOTAL = FORFEIT_TOTAL + profit_detail.PROFIT_FORT;
            }

            if (profit_detail.PROFIT_CODE == "5")
            {
                PALLOCATION_TOTAL = PALLOCATION_TOTAL + profit_detail.PROFIT_FORT;
            }

            if (profit_detail.PROFIT_CODE == "6")
            {
                ALLOCATION_TOTAL = ALLOCATION_TOTAL + profit_detail.PROFIT_CONT;
            }

            if (WS_PROFIT_YEAR_EXTENSION == 1)
            {
                ws_payprofit.WS_PROF_MIL = ws_payprofit.WS_PROF_MIL + profit_detail.PROFIT_CONT;
            }

            if (WS_PROFIT_YEAR_EXTENSION == 2)
            {
                ws_payprofit.WS_PROF_CAF = ws_payprofit.WS_PROF_CAF + profit_detail.PROFIT_EARN;
            }
        }

        int dbStatus = MSTR_FIND_WITHIN_PR_DET_S();
        return dbStatus;
    }

    public int m530TotalUpSsDetails()
    {
        // MSTR_GET_REC(IDS2_REC_NAME); 

        long WS_PROFIT_YEAR_FIRST_4 = (long)profit_ss_detail.PROFIT_SS_YEAR;
        string[] parts = profit_ss_detail.PROFIT_SS_YEAR.ToString().Split('.');
        long WS_PROFIT_YEAR_EXTENSION = parts.Length > 1 ? long.Parse(parts[1]) : 0;

        if (WS_PROFIT_YEAR_FIRST_4 == EffectiveYear)
        {
            if (profit_ss_detail.PROFIT_SS_CODE == "1" || profit_ss_detail.PROFIT_SS_CODE == "3" ||
                profit_ss_detail.PROFIT_SS_CODE == "9")
            {
                DIST_TOTAL += profit_ss_detail.PROFIT_SS_FORT;
            }
        }

        int dbStatus = MSTR_FIND_WITHIN_PR_SS_D_S();
        return dbStatus;
    }


    public void m805PrintSequence()
    {
        WRITE("\f" + xerox_header.ToString().Trim());
        ws_indicators.WS_END_IND = 0;
        ws_maxcont_totals = new WS_MAXCONT_TOTALS();
        header_1.HDR1_YY = TodaysDateTime.Year - 2000;
        header_1.HDR1_MM = TodaysDateTime.Month;
        header_1.HDR1_DD = TodaysDateTime.Day;
        header_1.HDR1_YEAR1 = EffectiveYear;
        total_header_1.TOT_HDR1_YEAR1 = EffectiveYear;
        total_header_1.TOT_HDR1_DD = TodaysDateTime.Day;
        total_header_1.TOT_HDR1_MM = TodaysDateTime.Month;
        total_header_1.TOT_HDR1_YY = TodaysDateTime.Year - 2000;
        header_1.HDR1_HR = TodaysDateTime.Hour;
        header_1.HDR1_MN = TodaysDateTime.Minute;
        total_header_1.TOT_HDR1_HR = TodaysDateTime.Hour;
        total_header_1.TOT_HDR1_MN = TodaysDateTime.Minute;


        prfts.Sort((a, b) =>
        {
            int nameComparison = StringComparer.Ordinal.Compare(a.FD_NAME, b.FD_NAME);
            return nameComparison != 0 ? nameComparison : StringComparer.Ordinal.Compare(a.FD_BADGE, b.FD_BADGE);
        });

        foreach (SD_PRFT sd_sorted in prfts.Select(p => new SD_PRFT(p)).ToList())
        {
            sd_prft = sd_sorted;
            m810WriteReport();
        }

        m850PrintTotals();

        m855GrandTotals();

        // CLOSE(PRINT_FILE);
    }

    private void WRITE(object obj)
    {
        Console.WriteLine(obj.ToString());
        outputLines.Add(obj.ToString().TrimEnd());
    }


    public void m810WriteReport()
    {
        if (ws_counters.WS_LINE_CTR > 60)
        {
            m830PrintHeader();
        }

        if (sd_prft.SD_BADGE > 0)
        {
            report_line.BADGE_NBR = sd_prft.SD_BADGE;
            report_line.EMP_NAME = sd_prft.SD_NAME?.Length > 24 ? sd_prft.SD_NAME.Substring(0, 24) : sd_prft.SD_NAME;

            ws_social_security.WS_SSN = sd_prft.SD_SSN;
            report_line.BEG_BAL = sd_prft.SD_AMT;
            report_line.PR_DIST1 = sd_prft.SD_DIST1;

            if (sd_prft.SD_NEWEMP == 1)
            {
                report_line.PR_NEWEMP = "NEW";
            }
            else if (sd_prft.SD_NEWEMP == 2)
            {
                m820CheckPayben();
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

        ws_ending_balance.WS_ENDBAL = sd_prft.SD_AMT + sd_prft.SD_CONT + sd_prft.SD_EARN + sd_prft.SD_EARN2 +
            sd_prft.SD_FORF + sd_prft.SD_MIL + sd_prft.SD_CAF - sd_prft.SD_DIST1;
        report_line.END_BAL = ws_ending_balance.WS_ENDBAL;

        if (sd_prft.SD_BADGE == 0)
        {
            report_line_2.PR2_EMP_NAME = sd_prft.SD_NAME?.Length > 24 ? sd_prft.SD_NAME.Substring(0, 24) : sd_prft.SD_NAME;
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

            ws_ending_balance.WS_ENDBAL = sd_prft.SD_AMT + sd_prft.SD_CONT + sd_prft.SD_EARN + sd_prft.SD_EARN2 +
                sd_prft.SD_FORF + sd_prft.SD_MIL + sd_prft.SD_CAF - sd_prft.SD_DIST1;
            report_line_2.PR2_END_BAL = ws_ending_balance.WS_ENDBAL;
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
        ws_client_totals.WS_TOT_ENDBAL += ws_ending_balance.WS_ENDBAL;
        ws_client_totals.WS_TOT_XFER += sd_prft.SD_XFER;
        ws_client_totals.WS_TOT_PXFER -= sd_prft.SD_PXFER;
        ws_client_totals.WS_EARN_PTS_TOTAL += sd_prft.SD_POINTS_EARN;
        ws_client_totals.WS_PROF_PTS_TOTAL += sd_prft.SD_POINTS;
        ws_client_totals.WS_TOT_CAF += sd_prft.SD_CAF;
        ws_maxcont_totals.WS_TOT_OVER += sd_prft.SD_MAXOVER;
        ws_maxcont_totals.WS_TOT_POINTS += sd_prft.SD_MAXPOINTS;

        if (sd_prft.SD_AMT == 0m
            && sd_prft.SD_DIST1 == 0m
            && sd_prft.SD_CONT == 0m
            && sd_prft.SD_XFER == 0m
            && sd_prft.SD_PXFER == 0m
            && sd_prft.SD_MIL == 0m
            && sd_prft.SD_FORF == 0m
            && sd_prft.SD_EARN == 0m
            && sd_prft.SD_EARN2 == 0m)
        {
            goto l810_EXIT;
        }

        m840PrintReport();
        l810_EXIT: ;
    }

    public void m820CheckPayben()
    {
        payben_rec.PYBEN_PAYSSN = sd_prft.SD_SSN;
        PAYBEN_FILE_STATUS = READ_ALT_KEY_PAYBEN(payben_rec);
        if (PAYBEN_FILE_STATUS == "00")
        {
            report_line.PR_NEWEMP = "BEN";
        }
    }

    public void m830PrintHeader()
    {
        ws_counters.WS_PAGE_CTR += 1;
        header_1.HDR1_PAGE = ws_counters.WS_PAGE_CTR;
        WRITE("\f" + header_1);
        WRITE("");
        WRITE(header_2);
        WRITE(header_3);
        ws_counters.WS_LINE_CTR = 4;
    }

    public void m840PrintReport()
    {
        if (sd_prft.SD_BADGE > 0)
        {
            ws_counters.WS_EMPLOYEE_CTR += 1;
            WRITE(report_line);
        }

        if (sd_prft.SD_BADGE == 0)
        {
            ws_counters.WS_EMPLOYEE_CTR_PAYBEN += 1;
            WRITE(report_line_2);
        }

        ws_counters.WS_LINE_CTR += 1;
    }

    public void m850PrintTotals()
    {
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

        ws_grand_totals.WS_GRTOT_BEGBAL += ws_client_totals.WS_TOT_BEGBAL;
        ws_grand_totals.WS_GRTOT_DIST1 += ws_client_totals.WS_TOT_DIST1;
        ws_grand_totals.WS_GRTOT_MIL += ws_client_totals.WS_TOT_MIL;
        ws_grand_totals.WS_GRTOT_CONT += ws_client_totals.WS_TOT_CONT;
        ws_grand_totals.WS_GRTOT_FORF += ws_client_totals.WS_TOT_FORF;
        ws_grand_totals.WS_GRTOT_EARN += ws_client_totals.WS_TOT_EARN;
        ws_grand_totals.WS_GRTOT_EARN2 += ws_client_totals.WS_TOT_EARN2;
        ws_grand_totals.WS_GRTOT_ENDBAL += ws_client_totals.WS_TOT_ENDBAL;
        ws_grand_totals.WS_GRTOT_CAF += ws_client_totals.WS_TOT_CAF;
        WRITE("\f" + total_header_1);
        WRITE("");
        WRITE(total_header_2);
        WRITE(total_header_3);
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

        client_tot.CONT_TOT = ws_client_totals.WS_PROF_PTS_TOTAL; // Weird redefine CONT_TOT <-> CONT_PTS_TOT
        client_tot.EARN_TOT = ws_client_totals.WS_EARN_PTS_TOTAL; // Weird redefine EARN_TOT <-> EARN_PTS_TOT
        client_tot.useRedefineFormatting = true;
        client_tot.TOT_FILLER = "POINT";
        WRITE("");
        WRITE(client_tot);

        employee_count_tot.PR_TOT_EMPLOYEE_COUNT = ws_counters.WS_EMPLOYEE_CTR;
        WRITE("");
        WRITE(employee_count_tot);
        employee_count_tot_payben.PB_TOT_EMPLOYEE_COUNT = ws_counters.WS_EMPLOYEE_CTR_PAYBEN;
        WRITE("");
        WRITE(employee_count_tot_payben);

        ws_compute_totals = new WS_COMPUTE_TOTALS();
        ws_client_totals = new WS_CLIENT_TOTALS();

        rerun_tot.RERUN_OVER = ws_maxcont_totals.WS_TOT_OVER;
        rerun_tot.RERUN_POINTS = ws_maxcont_totals.WS_TOT_POINTS;
        rerun_tot.RERUN_MAX = WS_CONTR_MAX;

        outputLines.Add("\n\n\n\n\n\n\n\n\n");
        WRITE(rerun_tot);

        ws_maxcont_totals = new WS_MAXCONT_TOTALS();
    }

    public void m855GrandTotals()
    {
        employee_count_tot.PR_TOT_EMPLOYEE_COUNT = ws_counters.WS_EMPLOYEE_CTR;
        employee_count_tot_payben.PB_TOT_EMPLOYEE_COUNT = ws_counters.WS_EMPLOYEE_CTR_PAYBEN;
        grand_tot.BEG_BAL_GRTOT = ws_grand_totals.WS_GRTOT_BEGBAL;
        grand_tot.DIST1_GRTOT = ws_grand_totals.WS_GRTOT_DIST1;
        grand_tot.MIL_GRTOT = ws_grand_totals.WS_GRTOT_MIL;
        grand_tot.CONT_GRTOT = ws_grand_totals.WS_GRTOT_CONT;
        grand_tot.FORF_GRTOT = ws_grand_totals.WS_GRTOT_FORF;
        grand_tot.EARN_GRTOT = ws_grand_totals.WS_GRTOT_EARN;
        grand_tot.EARN2_GRTOT = ws_grand_totals.WS_GRTOT_EARN2;
        if (ws_grand_totals.WS_GRTOT_EARN2 != 0)
        {
            DISPLAY("WS_GROTOT_EARN2 NOT 0 " + ws_grand_totals.WS_GRTOT_EARN2);
        }

        grand_tot.EARN2_GRTOT = ws_grand_totals.WS_GRTOT_CAF;
        grand_tot.END_BAL_GRTOT = ws_grand_totals.WS_GRTOT_ENDBAL;
    }

    public void m1000AdjustmentReport()
    {
        if (point_values.PV_ADJUST_BADGE == 0)
        {
            goto l1000_EXIT;
        }

        header_1.HDR1_PAGE = 1;
        header_1.HDR1_RPT = "PAY444A";
        print_adj_line1.PL_ADJUST_BADGE = point_values.PV_ADJUST_BADGE;
        print_adj_line1.PL_ADJ_DESC = "INITIAL";
        print_adj_line1.PL_CONT_AMT = point_values.SV_CONT_AMT;
        print_adj_line1.PL_FORF_AMT = point_values.SV_FORF_AMT;
        print_adj_line1.PL_EARN_AMT = point_values.SV_EARN_AMT;
        print_adj_line1.PL_EARN2_AMT = point_values.SV_EARN2_AMT;
        print_adj_line1.PL_ADJUST_BADGE = 0;
        print_adj_line1.PL_ADJ_DESC = "ADJUSTMENT";
        print_adj_line1.PL_CONT_AMT = point_values.PV_ADJ_CONTRIB;
        print_adj_line1.PL_EARN_AMT = point_values.PV_ADJ_EARN;
        print_adj_line1.PL_EARN2_AMT = point_values.PV_ADJ_EARN2;
        print_adj_line1.PL_FORF_AMT = point_values.PV_ADJ_FORFEIT;
        print_adj_line1.PL_ADJ_DESC = "FINAL";
        print_adj_line1.PL_CONT_AMT = point_values.SV_CONT_ADJUSTED;
        print_adj_line1.PL_FORF_AMT = point_values.SV_FORF_ADJUSTED;
        print_adj_line1.PL_EARN_AMT = point_values.SV_EARN_ADJUSTED;
        print_adj_line1.PL_EARN2_AMT = point_values.SV_EARN2_ADJUSTED;
        if (point_values.SV_FORF_AMT == 0 && point_values.SV_EARN_AMT == 0)
        {
        }

        l1000_EXIT: ;
    }

    private string? READ_ALT_KEY_PAYBEN(PAYBEN_REC payben_rec)
    {
        throw new NotImplementedException();
    }

}
