using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate.Formatters;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.ProfitShareEdit;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;

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
    public List<string> ReportLines { get; set; } = [];

    public async Task ProfitSharingUpdatePaginated(ProfitShareUpdateRequest profitShareUpdateRequest)
    {
        TotalService totalService = new TotalService(_dbFactory, calendarService);
        ProfitShareUpdateService psu = new(_dbFactory, totalService, calendarService);
        this.profitYear = profitShareUpdateRequest.ProfitYear;

        (List<MemberFinancials> members, AdjustmentReportData adjustmentsApplied, bool _) =
            await psu.ProfitSharingUpdatePaginated(profitShareUpdateRequest, TestContext.Current.CancellationToken);

        m805PrintSequence(members, profitShareUpdateRequest.MaxAllowedContributions);
        m1000AdjustmentReport(profitShareUpdateRequest, adjustmentsApplied);
    }

    public void m805PrintSequence(List<MemberFinancials> members, long maxAllowedContribution)
    {
        WRITE("\fDJDE JDE=PAY426,JDL=PAYROL,END,;");
        Header1 header_1 = new();
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
            long aBadge = Convert.ToInt64(a.BadgeNumber);
            long bBadge = Convert.ToInt64(b.BadgeNumber);
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
        ReportLines.Add(obj.ToString()!.TrimEnd());
    }


    public void m810WriteReport(ReportCounters reportCounters, Header1 header1, MemberFinancials memberFinancials,
        CollectTotals collectTotals)
    {
        if (reportCounters.LineCounter > 60)
        {
            m830PrintHeader(reportCounters, header1);
        }

        ReportLine report_line = new();
        ReportLine2 report_line_2 = new();
        if (memberFinancials.BadgeNumber > 0)
        {
            report_line.BADGE_NBR = memberFinancials.BadgeNumber;
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


            report_line.PR_CONT = memberFinancials.Contributions + memberFinancials.Xfer;
            report_line.PR_MIL = memberFinancials.Military - memberFinancials.Pxfer;
            report_line.PR_FORF = memberFinancials.IncomingForfeitures;
            report_line.PR_EARN = memberFinancials.AllEarnings;
            report_line.PR_EARN2 = memberFinancials.AllSecondaryEarnings;
            report_line.PR_EARN2 = memberFinancials.Caf;

            report_line.END_BAL = memberFinancials.EndingBalance;
        }


        if (memberFinancials.BadgeNumber == 0)
        {
            report_line_2.PR2_EMP_NAME =
                memberFinancials.Name?.Length > 24 ? memberFinancials.Name.Substring(0, 24) : memberFinancials.Name;
            report_line_2.PR2_PSN = memberFinancials.Psn;
            report_line_2.PR2_BEG_BAL = memberFinancials.CurrentAmount;
            report_line_2.PR2_DIST1 = memberFinancials.Distributions;
            report_line_2.PR2_NEWEMP = "BEN";
            report_line_2.PR2_CONT = memberFinancials.Contributions + memberFinancials.Xfer;
            report_line_2.PR2_MIL = memberFinancials.Military;
            report_line_2.PR2_FORF = memberFinancials.IncomingForfeitures;
            report_line_2.PR2_EARN = memberFinancials.AllEarnings;
            report_line_2.PR2_EARN2 = memberFinancials.AllSecondaryEarnings;
            report_line_2.PR2_EARN2 = memberFinancials.Caf;

            report_line_2.PR2_END_BAL = memberFinancials.EndingBalance;
        }

        collectTotals.WS_TOT_BEGBAL += memberFinancials.CurrentAmount;
        collectTotals.WS_TOT_DIST1 += memberFinancials.Distributions;
        collectTotals.WS_TOT_CONT += memberFinancials.Contributions;
        collectTotals.WS_TOT_MIL += memberFinancials.Military;
        collectTotals.WS_TOT_FORF += memberFinancials.IncomingForfeitures;
        collectTotals.WS_TOT_EARN += memberFinancials.AllEarnings;
        collectTotals.WS_TOT_EARN2 += memberFinancials.AllSecondaryEarnings;
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
            || memberFinancials.AllEarnings != 0m
            || memberFinancials.AllSecondaryEarnings != 0m)
        {
            if (memberFinancials.BadgeNumber > 0)
            {
                reportCounters.EmployeeCounter += 1;
                WRITE(report_line);
            }

            if (memberFinancials.BadgeNumber == 0)
            {
                reportCounters.BeneficiaryCounter += 1;
                WRITE(report_line_2);
            }

            reportCounters.LineCounter += 1;
        }
    }

    public void m830PrintHeader(ReportCounters reportCounters, Header1 header1)
    {
        reportCounters.PageCounter += 1;
        header1.HDR1_PAGE = reportCounters.PageCounter;
        WRITE($"\f{header1}");
        WRITE("");
        WRITE(new Header2());
        WRITE(new Header3());
        reportCounters.LineCounter = 4;
    }


    public void m850PrintTotals(ReportCounters reportCounters, CollectTotals wsClientTotals,
        long maxAllowedContribution)
    {
        ClientTot client_tot = new();
        client_tot.BEG_BAL_TOT = wsClientTotals.WS_TOT_BEGBAL;
        client_tot.DIST1_TOT = wsClientTotals.WS_TOT_DIST1;
        client_tot.MIL_TOT = wsClientTotals.WS_TOT_MIL;
        client_tot.CONT_TOT = wsClientTotals.WS_TOT_CONT;
        client_tot.FORF_TOT = wsClientTotals.WS_TOT_FORF;
        client_tot.EARN_TOT = wsClientTotals.WS_TOT_EARN;
        client_tot.EARN2_TOT = wsClientTotals.WS_TOT_EARN2;
        if (wsClientTotals.WS_TOT_EARN2 != 0)
        {
            Console.WriteLine($"WS_TOT_EARN2 NOT 0 {wsClientTotals.WS_TOT_EARN2}");
        }

        client_tot.EARN2_TOT = wsClientTotals.WS_TOT_CAF;
        client_tot.END_BAL_TOT = wsClientTotals.WS_TOT_ENDBAL;


        TotalHeader1 total_header_1 = new();
        total_header_1.TOT_HDR1_YEAR1 = profitYear;
        total_header_1.TOT_HDR1_DD = TodaysDateTime.Day;
        total_header_1.TOT_HDR1_MM = TodaysDateTime.Month;
        total_header_1.TOT_HDR1_YY = TodaysDateTime.Year - 2000;
        total_header_1.TOT_HDR1_HR = TodaysDateTime.Hour;
        total_header_1.TOT_HDR1_MN = TodaysDateTime.Minute;

        WRITE($"\f{total_header_1}");
        WRITE("");
        WRITE(new TotalHeader2());
        WRITE(new TotalHeader3());
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

        client_tot.CONT_TOT = wsClientTotals.WS_TOT_XFER;
        client_tot.MIL_TOT = wsClientTotals.WS_TOT_PXFER;
        client_tot.END_BAL_TOT = wsClientTotals.WS_TOT_PXFER + wsClientTotals.WS_TOT_XFER;
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

        client_tot.CONT_TOT = wsClientTotals.WS_PROF_PTS_TOTAL;
        client_tot.EARN_TOT = wsClientTotals.WS_EARN_PTS_TOTAL;
        client_tot.useRedefineFormatting = true;
        client_tot.TOT_FILLER = "POINT";
        WRITE("");
        WRITE(client_tot);

        EmployeeCountTotal employeeCountTotal = new();
        employeeCountTotal.PR_TOT_EMPLOYEE_COUNT = reportCounters.EmployeeCounter;
        WRITE("");
        WRITE(employeeCountTotal);
        BeneficiaryCountTotal beneficiaryCountTotPayben = new();
        beneficiaryCountTotPayben.PB_TOT_EMPLOYEE_COUNT = reportCounters.BeneficiaryCounter;
        WRITE("");
        WRITE(beneficiaryCountTotPayben);

        RerunTotals rerunTotals = new();
        rerunTotals.RERUN_OVER = wsClientTotals.MaxOverTotal;
        rerunTotals.RERUN_POINTS = wsClientTotals.MaxPointsTotal;
        rerunTotals.RERUN_MAX = maxAllowedContribution;

        ReportLines.Add("\n\n\n\n\n\n\n\n\n");
        WRITE(rerunTotals);
    }


    public void m1000AdjustmentReport(ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentReportData adjustmentsApplied)
    {
        if (profitShareUpdateRequest.BadgeToAdjust == 0)
        {
            return;
        }

        Header1 header_1 = new();
        Header4 header_4 = new();
        Header5 header_5 = new();


        header_1.HDR1_PAGE = 1;
        header_1.HDR1_RPT = "PAY444A";
        WRITE2_afterPage(header_1);
        WRITE2_advance2(header_4);
        WRITE2_advance2(header_5);

        PrintAdjustLine1 printAdjustLine1 = new();
        printAdjustLine1.PL_ADJUST_BADGE = profitShareUpdateRequest.BadgeToAdjust;
        printAdjustLine1.PL_ADJ_DESC = "INITIAL";
        printAdjustLine1.PL_CONT_AMT = adjustmentsApplied.ContributionAmountUnadjusted;
        printAdjustLine1.PL_FORF_AMT = adjustmentsApplied.IncomingForfeitureAmountUnadjusted;
        printAdjustLine1.PL_EARN_AMT = adjustmentsApplied.EarningsAmountUnadjusted;
        printAdjustLine1.PL_EARN2_AMT = adjustmentsApplied.SecondaryEarningsAmountUnadjusted;
        WRITE2_advance2(printAdjustLine1);

        printAdjustLine1.PL_ADJUST_BADGE = 0;
        printAdjustLine1.PL_ADJ_DESC = "ADJUSTMENT";
        printAdjustLine1.PL_CONT_AMT = profitShareUpdateRequest.AdjustContributionAmount;
        printAdjustLine1.PL_EARN_AMT = profitShareUpdateRequest.AdjustEarningsAmount;
        printAdjustLine1.PL_EARN2_AMT = profitShareUpdateRequest.AdjustEarningsSecondaryAmount;
        printAdjustLine1.PL_FORF_AMT = profitShareUpdateRequest.AdjustIncomingForfeitAmount;
        WRITE2_advance2(printAdjustLine1);

        printAdjustLine1.PL_ADJ_DESC = "FINAL";
        printAdjustLine1.PL_CONT_AMT = adjustmentsApplied.ContributionAmountAdjusted;
        printAdjustLine1.PL_FORF_AMT = adjustmentsApplied.IncomingForfeitureAmountAdjusted;
        printAdjustLine1.PL_EARN_AMT = adjustmentsApplied.EarningsAmountAdjusted;
        printAdjustLine1.PL_EARN2_AMT = adjustmentsApplied.SecondaryEarningsAmountAdjusted;

        WRITE2_advance2(printAdjustLine1);

        if (adjustmentsApplied.IncomingForfeitureAmountUnadjusted == 0 &&
            adjustmentsApplied.EarningsAmountUnadjusted == 0)
        {
            WRITE2_advance2("No adjustment - employee not found.");
        }
    }

    private void WRITE2_advance2(object header4)
    {
        // We dont currently support this second report.    We may have to.
        //throw new NotImplementedException()
    }

    private void WRITE2_afterPage(Header1 header1)
    {
        // We dont currently support this second report.    We may have to.
        // throw new NotImplementedException()
    }
}
