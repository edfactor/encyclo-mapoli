using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Reports.YearEnd.Update.Formatters;
using Demoulas.ProfitSharing.Services.Reports.YearEnd.Update.ReportFormatters;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.YearEnd.ProfitShareUpdate;

/// <summary>
///     A testing layer which generates Ready style reports.
/// </summary>
internal sealed class ProfitShareUpdateReport
{
    private readonly IProfitSharingDataContextFactory _dbFactory;
    private readonly CalendarService calendarService;
    private short profitYear;

    /// <summary>
    ///     A testing layer which generates Ready style reports.
    /// </summary>
    public ProfitShareUpdateReport(IProfitSharingDataContextFactory dbFactory, CalendarService calendarService)
    {
        _dbFactory = dbFactory;
        this.calendarService = calendarService;
    }

    public DateTime TodaysDateTime { get; set; }
    public List<string> ReportLines { get; set; }

    public void ApplyAdjustments(short profitYear, UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest)
    {
        ReportLines = [];

        LoggerFactory loggerFactory = new();

        ProfitShareUpdateService psu = new(_dbFactory, loggerFactory, calendarService);
        this.profitYear = profitYear;


        (List<MemberFinancials> members, AdjustmentReportData adjustmentsApplied, bool rerunNeeded) =
            psu.ApplyAdjustments(updateAdjustmentAmountsRequest).GetAwaiter().GetResult();

        m805PrintSequence(members, updateAdjustmentAmountsRequest.MaxAllowedContributions);
        m1000AdjustmentReport(updateAdjustmentAmountsRequest, adjustmentsApplied);
    }

    public void m805PrintSequence(List<MemberFinancials> members, long maxAllowedContribution)
    {
        WRITE("\fDJDE JDE=PAY426,JDL=PAYROL,END,;");
        HEADER_1 header_1 = new();
        header_1.HDR1_YY = TodaysDateTime.Year - 2000;
        header_1.HDR1_MM = TodaysDateTime.Month;
        header_1.HDR1_DD = TodaysDateTime.Day;
        header_1.HDR1_YEAR1 = profitYear;
        header_1.HDR1_HR = TodaysDateTime.Hour;
        header_1.HDR1_MN = TodaysDateTime.Minute;


        members.Sort((a, b) =>
        {
            int nameComparison = StringComparer.Ordinal.Compare(a.Name, b.Name);
            if (nameComparison != 0)
            {
                return nameComparison;
            }

            // This is so we converge on a stable sort.  This effectively matches Ready's order.
            long aBadge = Convert.ToInt64(a.EmployeeId);
            long bBadge = Convert.ToInt64(b.EmployeeId);
            aBadge = aBadge == 0 ? a.Psn : aBadge;
            bBadge = bBadge == 0 ? b.Psn : bBadge;
            return aBadge < bBadge ? -1 : 1;
        });


        ReportCounters reportCounters = new();
        CollectTotals collectTotals = new();

        foreach (MemberFinancials memberFinancials in members)
        {
            m810WriteReport(reportCounters, header_1, memberFinancials, collectTotals);
        }

        m850PrintTotals(reportCounters, collectTotals, maxAllowedContribution);
    }

    private void WRITE(object obj)
    {
        ReportLines.Add(obj.ToString().TrimEnd());
    }


    public void m810WriteReport(ReportCounters reportCounters, HEADER_1 header_1, MemberFinancials memberFinancials,
        CollectTotals collectTotals)
    {
        if (reportCounters.LineCounter > 60)
        {
            m830PrintHeader(reportCounters, header_1);
        }

        if (memberFinancials.Xfer != 0)
        {
            memberFinancials.Contributions = memberFinancials.Contributions + memberFinancials.Xfer;
        }

        if (memberFinancials.Pxfer != 0)
        {
            memberFinancials.Military = memberFinancials.Military - memberFinancials.Pxfer;
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
            else
            {
                report_line.PR_NEWEMP = " ";
            }


            report_line.PR_CONT = memberFinancials.Contributions;
            report_line.PR_MIL = memberFinancials.Military;
            report_line.PR_FORF = memberFinancials.IncomingForfeitures;
            report_line.PR_EARN = memberFinancials.Earnings;

            if (memberFinancials.SecondaryEarnings != 0)
            {
                Console.WriteLine(
                    $"badge {memberFinancials.EmployeeId} earnings2 ${memberFinancials.SecondaryEarnings}");
            }

            report_line.PR_EARN2 = memberFinancials.SecondaryEarnings;
            report_line.PR_EARN2 = memberFinancials.Caf;

            report_line.END_BAL = memberFinancials.EndingBalance;
        }


        if (memberFinancials.EmployeeId == 0)
        {
            report_line_2.PR2_EMP_NAME =
                memberFinancials.Name?.Length > 24 ? memberFinancials.Name.Substring(0, 24) : memberFinancials.Name;
            report_line_2.PR2_PSN = memberFinancials.Psn;
            report_line_2.PR2_BEG_BAL = memberFinancials.CurrentAmount;
            report_line_2.PR2_DIST1 = memberFinancials.Distributions;
            report_line_2.PR2_NEWEMP = "BEN";
            report_line_2.PR2_CONT = memberFinancials.Contributions;
            report_line_2.PR2_MIL = memberFinancials.Military;
            report_line_2.PR2_FORF = memberFinancials.IncomingForfeitures;
            report_line_2.PR2_EARN = memberFinancials.Earnings;
            report_line_2.PR2_EARN2 = memberFinancials.SecondaryEarnings;
            report_line_2.PR2_EARN2 = memberFinancials.Caf;

            report_line_2.PR2_END_BAL = memberFinancials.EndingBalance;
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
        collectTotals.WS_TOT_ENDBAL += memberFinancials.EndingBalance;
        collectTotals.WS_TOT_XFER += memberFinancials.Xfer;
        collectTotals.WS_TOT_PXFER -= memberFinancials.Pxfer;
        collectTotals.WS_EARN_PTS_TOTAL += memberFinancials.EarningPoints;
        collectTotals.WS_PROF_PTS_TOTAL += memberFinancials.ContributionPoints;
        collectTotals.WS_TOT_CAF += memberFinancials.Caf;
        collectTotals.MaxOverTotal += memberFinancials.MaxOver;
        collectTotals.MaxPointsTotal += memberFinancials.MaxPoints;

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


    public void m850PrintTotals(ReportCounters reportCounters, CollectTotals ws_client_totals,
        long maxAllowedContribution)
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
            Console.WriteLine("WS_TOT_EARN2 NOT 0 " + ws_client_totals.WS_TOT_EARN2);
        }

        client_tot.EARN2_TOT = ws_client_totals.WS_TOT_CAF;
        client_tot.END_BAL_TOT = ws_client_totals.WS_TOT_ENDBAL;


        TOTAL_HEADER_1 total_header_1 = new();
        total_header_1.TOT_HDR1_YEAR1 = profitYear;
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
        rerun_tot.RERUN_OVER = ws_client_totals.MaxOverTotal;
        rerun_tot.RERUN_POINTS = ws_client_totals.MaxPointsTotal;
        rerun_tot.RERUN_MAX = maxAllowedContribution;

        ReportLines.Add("\n\n\n\n\n\n\n\n\n");
        WRITE(rerun_tot);
    }


    public void m1000AdjustmentReport(UpdateAdjustmentAmountsRequest updateAdjustmentAmountsRequest,
        AdjustmentReportData adjustmentsApplied)
    {
        if (updateAdjustmentAmountsRequest.BadgeToAdjust == 0)
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
        print_adj_line1.PL_ADJUST_BADGE = updateAdjustmentAmountsRequest.BadgeToAdjust;
        print_adj_line1.PL_ADJ_DESC = "INITIAL";
        print_adj_line1.PL_CONT_AMT = adjustmentsApplied.ContributionAmountUnadjusted;
        print_adj_line1.PL_FORF_AMT = adjustmentsApplied.IncomingForfeitureAmountUnadjusted;
        print_adj_line1.PL_EARN_AMT = adjustmentsApplied.EarningsAmountUnadjusted;
        print_adj_line1.PL_EARN2_AMT = adjustmentsApplied.SecondaryEarningsAmountUnadjusted;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJUST_BADGE = 0;
        print_adj_line1.PL_ADJ_DESC = "ADJUSTMENT";
        print_adj_line1.PL_CONT_AMT = updateAdjustmentAmountsRequest.AdjustContributionAmount;
        print_adj_line1.PL_EARN_AMT = updateAdjustmentAmountsRequest.AdjustEarningsAmount;
        print_adj_line1.PL_EARN2_AMT = updateAdjustmentAmountsRequest.AdjustEarningsSecondaryAmount;
        print_adj_line1.PL_FORF_AMT = updateAdjustmentAmountsRequest.AdjustIncomingForfeitAmount;
        WRITE2_advance2(print_adj_line1);

        print_adj_line1.PL_ADJ_DESC = "FINAL";
        print_adj_line1.PL_CONT_AMT = adjustmentsApplied.ContributionAmountAdjusted;
        print_adj_line1.PL_FORF_AMT = adjustmentsApplied.IncomingForfeitureAmountAdjusted;
        print_adj_line1.PL_EARN_AMT = adjustmentsApplied.EarningsAmountAdjusted;
        print_adj_line1.PL_EARN2_AMT = adjustmentsApplied.SecondaryEarningsAmountAdjusted;

        WRITE2_advance2(print_adj_line1);

        if (adjustmentsApplied.IncomingForfeitureAmountUnadjusted == 0 &&
            adjustmentsApplied.EarningsAmountUnadjusted == 0)
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
