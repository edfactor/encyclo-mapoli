using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Data.Entities;
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
    private readonly CalendarService _calendarService;
    private short _profitYear;

    /// <summary>
    ///     A testing layer which generates Ready style reports.
    /// </summary>
    public ProfitShareUpdateReport(IProfitSharingDataContextFactory dbFactory, CalendarService calendarService)
    {
        _dbFactory = dbFactory;
        _calendarService = calendarService;
    }

    public DateTime TodaysDateTime { get; set; }
    public List<string> ReportLines { get; set; } = [];

    public async Task ProfitSharingUpdatePaginated(ProfitShareUpdateRequest profitShareUpdateRequest)
    {
        TotalService totalService = new TotalService(_dbFactory, _calendarService);
        ProfitShareUpdateService psu = new(_dbFactory, totalService, _calendarService);
        _profitYear = profitShareUpdateRequest.ProfitYear;

        (List<MemberFinancials> members, AdjustmentsSummaryDto adjustmentsApplied, TotalsDto totalsDto, bool _) =
            await psu.ProfitSharingUpdate(profitShareUpdateRequest, TestContext.Current.CancellationToken);

        m805PrintSequence(members, profitShareUpdateRequest.MaxAllowedContributions, totalsDto);
        m1000AdjustmentReport(profitShareUpdateRequest, adjustmentsApplied);
    }

    public void m805PrintSequence(List<MemberFinancials> members, long maxAllowedContribution, TotalsDto totalsDto)
    {
        Header1 header_1 = new();
        header_1.HDR1_YY = TodaysDateTime.Year - 2000;
        header_1.HDR1_MM = TodaysDateTime.Month;
        header_1.HDR1_DD = TodaysDateTime.Day;
        header_1.HDR1_YEAR1 = _profitYear;
        header_1.HDR1_HR = TodaysDateTime.Hour;
        header_1.HDR1_MN = TodaysDateTime.Minute;

        ReportCounters reportCounters = new();

        foreach (MemberFinancials memberFinancials in members)
        {
            m810WriteReport(reportCounters, header_1, memberFinancials);
        }

        m850PrintTotals(reportCounters, totalsDto, maxAllowedContribution);
    }

    private void WRITE(object obj)
    {
        ReportLines.Add(obj.ToString()!.TrimEnd());
    }


    public void m810WriteReport(ReportCounters reportCounters, Header1 header1, MemberFinancials memberFinancials)
    {
        if (reportCounters.LineCounter > 60)
        {
            m830PrintHeader(reportCounters, header1);
        }

        ReportLine employeeReportLine = new();
        ReportLine2 beneReportLine = new();
        if (memberFinancials.Psn == memberFinancials.BadgeNumber.ToString())
        {
            employeeReportLine.BADGE_NBR = memberFinancials.BadgeNumber;
            employeeReportLine.EMP_NAME = memberFinancials.Name?.Length > 24
                ? memberFinancials.Name.Substring(0, 24)
                : memberFinancials.Name;

            employeeReportLine.BEG_BAL = memberFinancials.CurrentAmount;
            employeeReportLine.PR_DIST1 = memberFinancials.Distributions;

            if (memberFinancials.EmployeeTypeId == /*1*/ EmployeeType.Constants.NewLastYear)
            {
                employeeReportLine.PR_NEWEMP = "NEW";
            }
            else if (memberFinancials.TreatAsBeneficiary)
            {
                employeeReportLine.PR_NEWEMP = "BEN";
            }
            else
            {
                employeeReportLine.PR_NEWEMP = " ";
            }

            employeeReportLine.PR_CONT = memberFinancials.Contributions + memberFinancials.Xfer;
            employeeReportLine.PR_MIL = memberFinancials.Military - memberFinancials.Pxfer;
            employeeReportLine.PR_FORF = memberFinancials.IncomingForfeitures;
            employeeReportLine.PR_EARN = memberFinancials.AllEarnings;
            employeeReportLine.PR_EARN2 = memberFinancials.AllSecondaryEarnings;
            employeeReportLine.PR_EARN2 = memberFinancials.Caf;

            employeeReportLine.END_BAL = memberFinancials.EndingBalance;
        }

        if (memberFinancials.Psn != memberFinancials.BadgeNumber.ToString())
        {
            beneReportLine.PR2_EMP_NAME =
                memberFinancials.Name?.Length > 24 ? memberFinancials.Name.Substring(0, 24) : memberFinancials.Name;
            beneReportLine.PR2_PSN = long.Parse(memberFinancials.Psn ?? "0");
            beneReportLine.PR2_BEG_BAL = memberFinancials.CurrentAmount;
            beneReportLine.PR2_DIST1 = memberFinancials.Distributions;
            beneReportLine.PR2_NEWEMP = "BEN";
            beneReportLine.PR2_CONT = memberFinancials.Contributions + memberFinancials.Xfer;
            beneReportLine.PR2_MIL = memberFinancials.Military;
            beneReportLine.PR2_FORF = memberFinancials.IncomingForfeitures;
            beneReportLine.PR2_EARN = memberFinancials.AllEarnings;
            beneReportLine.PR2_EARN2 = memberFinancials.AllSecondaryEarnings;
            beneReportLine.PR2_EARN2 = memberFinancials.Caf;

            beneReportLine.PR2_END_BAL = memberFinancials.EndingBalance;
        }

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
                WRITE(employeeReportLine);
            }

            if (memberFinancials.BadgeNumber == 0)
            {
                reportCounters.BeneficiaryCounter += 1;
                WRITE(beneReportLine);
            }

            reportCounters.LineCounter += 1;
        }
    }

    const string prefix = "\n\nDJDE JDE=PAY426,JDL=PAYROL,END,;\n";

    public void m830PrintHeader(ReportCounters reportCounters, Header1 header1)
    {
        reportCounters.PageCounter += 1;
        header1.HDR1_PAGE = reportCounters.PageCounter;
        WRITE($"{((ReportLines.Count == 0) ? prefix : "")}\n{header1}");
        WRITE("");
        WRITE(new Header2());
        WRITE(new Header3());
        reportCounters.LineCounter = 4;
    }


    public void m850PrintTotals(ReportCounters reportCounters, TotalsDto wsClientTotalsDto,
        long maxAllowedContribution)
    {
        ClientTot client_tot = new();
        client_tot.BEG_BAL_TOT = wsClientTotalsDto.BeginningBalance;
        client_tot.DIST1_TOT = wsClientTotalsDto.Distributions;
        client_tot.MIL_TOT = wsClientTotalsDto.Military;
        client_tot.CONT_TOT = wsClientTotalsDto.TotalContribution;
        client_tot.FORF_TOT = wsClientTotalsDto.Forfeiture;
        client_tot.EARN_TOT = wsClientTotalsDto.Earnings;
        client_tot.EARN2_TOT = wsClientTotalsDto.Earnings2;
        if (wsClientTotalsDto.Earnings2 != 0)
        {
            Console.WriteLine($"Earnings2 NOT 0 {wsClientTotalsDto.Earnings2}");
        }

        client_tot.EARN2_TOT = wsClientTotalsDto.ClassActionFund;
        client_tot.END_BAL_TOT = wsClientTotalsDto.EndingBalance;

        TotalHeader1 total_header_1 = new();
        total_header_1.TOT_HDR1_YEAR1 = _profitYear;
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

        client_tot = new();
        client_tot.CONT_TOT = wsClientTotalsDto.Allocations;
        client_tot.MIL_TOT = wsClientTotalsDto.PaidAllocations;
        client_tot.END_BAL_TOT = wsClientTotalsDto.PaidAllocations + wsClientTotalsDto.Allocations;
        client_tot.TOT_FILLER = "ALLOC   ";
        WRITE(client_tot);

        client_tot = new();
        client_tot.CONT_TOT = wsClientTotalsDto.ContributionPoints;
        client_tot.EARN_TOT = wsClientTotalsDto.EarningPoints;
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
        rerunTotals.RERUN_OVER = wsClientTotalsDto.MaxOverTotal;
        rerunTotals.RERUN_POINTS = wsClientTotalsDto.MaxPointsTotal;
        rerunTotals.RERUN_MAX = maxAllowedContribution;

        ReportLines.Add("\n\n\n\n\n\n\n\n\n");
        WRITE(rerunTotals);
    }


    public void m1000AdjustmentReport(ProfitShareUpdateRequest profitShareUpdateRequest,
        AdjustmentsSummaryDto adjustmentsApplied)
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
