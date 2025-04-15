using System.Text;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Reports.Breakdown;
using Microsoft.Extensions.Configuration;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Breakdown;

public class BreakdownReportByStoreUnder21Tests
{
    private readonly AccountingPeriodsService _aps = new();
    private readonly IBreakdownService _breakdownService;
    private readonly CalendarService _calendarService;
    private readonly EmbeddedSqlService _embeddedSqlService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;

    public BreakdownReportByStoreUnder21Tests()
    {
        _dataContextFactory = new PristineDataContextFactory();
        _calendarService = new CalendarService(_dataContextFactory, _aps);
        _embeddedSqlService = new EmbeddedSqlService();
        _totalService = new TotalService(_dataContextFactory, _calendarService, _embeddedSqlService);
        _breakdownService = new BreakdownReportService(_dataContextFactory, _calendarService, _totalService);
    }

    [Fact]
    public async Task RunReportUnder21()
    {
        ReportResponseBase<MemberYearSummaryDto> results =
            await _breakdownService.GetActiveMembersByStore(new BreakdownByStoreRequest { ProfitYear = 2024, Under21Only = true, Take = int.MaxValue }, CancellationToken.None);

        List<(short Key, List<MemberYearSummaryDto>)> groupedEmployees = results.Response.Results
            .GroupBy(m => m.StoreNumber)
            .OrderBy(g => g.Key)
            .Select(storeGroup => (storeGroup.Key, storeGroup.ToList()))
            .ToList();

        string actual = CreateTextReport(2, groupedEmployees);
        string expected = ProfitShareUpdateTests.LoadExpectedReport("qpay066ta.breakdown-by-store-under21.txt");
        ProfitShareUpdateTests.AssertReportsAreEquivalent(expected, actual);
    }

    private static string CreateTextReport(short startPage, List<(short StoreNumber, List<MemberYearSummaryDto> employees)> groupedEmployees)
    {
        StringBuilder sb = new();
        int pageNumber = startPage;

        foreach ((short StoreNumber, List<MemberYearSummaryDto> employees) store in groupedEmployees)
        {
            sb.Append(printStore(store.StoreNumber, store.employees, ref pageNumber));
            sb.Append("\n\f\n\n");
        }

        return sb.ToString();
    }

    private static string printStore(short storeNumber, List<MemberYearSummaryDto> employees, ref int pageNumber)
    {
        StringBuilder sb = new();
        sb.Append(createHeader(storeNumber, ref pageNumber));
        sb.Append("\n");
        sb.Append("STORE MANAGEMENT\n");
        sb.Append("\n");
        int managementCount = 3;
        foreach (MemberYearSummaryDto employee in employees
                     .Where(mys => mys.EmployeeSortRank != 1999)
                     .OrderBy(mys => mys.EmployeeSortRank)
                     .ThenBy(mys => mys.FullName)
                )
        {
            sb.Append(PrintEmployee(employee));
            managementCount++;
        }

        if (employees.Any(mys => mys.EmployeeSortRank == 1999))
        {
            sb.Append("\n\nASSOCIATES\n\n");
            int associateCount = 0;
            foreach (MemberYearSummaryDto employee in employees.Where(mys =>
                         mys.EmployeeSortRank == 1999
                     ))
            {
                // If we are about to run out of lines on the paper, bust out to a new page
                if (managementCount + associateCount == 48 - (storeNumber == 1 ? 1 : 0))
                {
                    managementCount = 0;
                    associateCount = 0;
                    sb.Append("\n\f\n\n");
                    sb.Append(createHeader(storeNumber, ref pageNumber));
                }

                sb.Append(PrintEmployee(employee));
                associateCount++;
            }
        }

        sb.Append("\n");

        return sb.ToString();
    }

    private static string createHeader(short storeNumber, ref int pageNumber)
    {
        // QPAY066TA-UNDR21        PROFIT SHARING BREAKDOWN REPORT    DATE MAR 10, 2025  YEAR:   2024.0     PAGE:   00002
        //
        // STORE  1
        //
        // BADGE# EMPLOYEE NAME               BEGINNING     EARNINGS         CONT         FORF       DIST.       ENDING    V E S T E D   E
        // BALANCE                                                         BALANCE       AMOUNT  %  C

        StringBuilder sb = new();
        sb.Append($"QPAY066TA-UNDR21        PROFIT SHARING BREAKDOWN REPORT    DATE MAR 10, 2025  YEAR:   2024.0     PAGE:   {pageNumber:D5}\n");
        pageNumber++;
        sb.Append("\n");
        sb.Append($"   STORE  {storeNumber}\n");
        sb.Append("\n");
        sb.Append("BADGE# EMPLOYEE NAME               BEGINNING     EARNINGS         CONT         FORF       DIST.       ENDING    V E S T E D   E\n");
        sb.Append("                                     BALANCE                                                         BALANCE       AMOUNT  %  C\n");

        return sb.ToString();
    }

    public static string PrintEmployee(MemberYearSummaryDto member)
    {
        string ecStr = member.EnrollmentId == 1 || member.EnrollmentId == 3 || member.EnrollmentId == 4 ? " " + member.EnrollmentId : "";

        string formattedLine =
            $" {member.BadgeNumber,-5} {member.FullName,-24} " +
            $"{(member.BeginningBalance != 0 ? FormatTrailingNegative(member.BeginningBalance) : ""),12} " +
            $"{(member.Earnings != 0 ? FormatTrailingNegative(member.Earnings) : ""),12} " +
            $"{(member.Contributions != 0 ? FormatTrailingNegative(member.Contributions) : ""),12} " +
            $"{(member.Forfeiture != 0 ? FormatTrailingNegative(member.Forfeiture) : ""),12} " +
            $"{(member.Distributions != 0 ? FormatTrailingNegative(member.Distributions) : ""),11} " +
            $"{(member.EndingBalance != 0 ? FormatTrailingNegative(member.EndingBalance) : ""),12} " +
            $"{(member.VestedAmount != 0 ? FormatTrailingNegative(member.VestedAmount) : ""),12}" +
            $"{(member.VestedPercentage != 0 ? member.VestedPercentage.ToString("N0") : ""),3}{ecStr}";

        return formattedLine.TrimEnd() + "\n";
    }

    private static string FormatTrailingNegative(decimal number)
    {
        string numberStr = number.ToString("#,##0.00 ;#,##0.00-");

        string[] parts = numberStr.Split(',');
        Console.WriteLine(parts);
        if (parts.Length == 3)
        {
            return $"{parts[0]}{parts[1]},{parts[2]}";
        }

        return numberStr;
    }
}
