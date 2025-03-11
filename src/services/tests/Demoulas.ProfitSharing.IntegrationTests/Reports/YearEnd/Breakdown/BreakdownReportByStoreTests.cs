using System.Text;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Reports.Breakdown;
using Microsoft.Extensions.Configuration;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Breakdown;

public class BreakdownReportByStoreTests
{
    private readonly AccountingPeriodsService _aps = new();
    private readonly IBreakdownService _breakdownService;
    private readonly CalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;

    public BreakdownReportByStoreTests()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<ProfitShareUpdateTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        _dataContextFactory = new PristineDataContextFactory(connectionString);
        _calendarService = new CalendarService(_dataContextFactory, _aps);
        _totalService = new TotalService(_dataContextFactory, _calendarService);
        _breakdownService = new BreakdownReportService(_dataContextFactory, _calendarService, _totalService);
    }

    [Fact]
    public async Task RunReport()
    {
        ReportResponseBase<MemberYearSummaryDto> results =
            await _breakdownService.GetActiveMembersByStore(new BreakdownByStoreRequest { ProfitYear = 2024, Under21Only = false, Take = int.MaxValue }, CancellationToken.None);

        List<(short Key, List<MemberYearSummaryDto>)> groupedEmployees = results.Response.Results
            .GroupBy(m => m.StoreNumber)
            .OrderBy(g => g.Key)
            .Select(storeGroup => (storeGroup.Key, storeGroup.OrderBy(e => e.FullName).ToList()))
            .ToList();

        string actual = CreateTextReport(groupedEmployees);
        string expected = ProfitShareUpdateTests.LoadExpectedReport("QPAY066TA-18Feb2025-by-store.txt");
        ProfitShareUpdateTests.AssertReportsAreEquivalent(expected, actual);
    }

    [Fact]
    public async Task RunReportUnder21()
    {
        ReportResponseBase<MemberYearSummaryDto> results =
            await _breakdownService.GetActiveMembersByStore(new BreakdownByStoreRequest { ProfitYear = 2024, Under21Only = true, Take = int.MaxValue }, CancellationToken.None);

        List<(short Key, List<MemberYearSummaryDto>)> groupedEmployees = results.Response.Results
            .GroupBy(m => m.StoreNumber)
            .OrderBy(g => g.Key)
            .Select(storeGroup => (storeGroup.Key, storeGroup.OrderBy(e => e.FullName).ToList()))
            .ToList();

        string actual = CreateTextReport(groupedEmployees);
        string expected = ProfitShareUpdateTests.LoadExpectedReport("QPAY066TA-18Feb2025-by-store.txt");
        ProfitShareUpdateTests.AssertReportsAreEquivalent(expected, actual);
    }

    private static string CreateTextReport(List<(short StoreNumber, List<MemberYearSummaryDto> employees)> groupedEmployees)
    {
        bool first = true;
        int pageNumber = 1;
        StringBuilder sb = new();
        foreach ((short StoreNumber, List<MemberYearSummaryDto> employees) store in groupedEmployees)
        {
            bool isIndianRidge = store.StoreNumber == StoreTypes.IndianRidge;
            // if there are no active employees with a balance, skip this store
            if (!isIndianRidge && !store.employees.Any(mys => mys.EmploymentStatusId == EmploymentStatus.Constants.Active
                                                              && mys.EndingBalance != 0))
            {
                continue;
            }

            if (first)
            {
                sb.Append("\nDJDE JDE=LANIQS,JDL=DFLT4,END,;\n");
                first = false;
            }
            else
            {
                sb.Append("\n\f\n");
            }

            sb.Append($"\nQPAY066TA               PROFIT SHARING BREAKDOWN REPORT    DATE FEB 18, 2025  YEAR:   2024.0     PAGE:   {pageNumber:D5}\n\n");
            pageNumber++;

            sb.Append($"   STORE  {store.StoreNumber}\n\n");

            sb.Append("BADGE #     EMPLOYEE NAME              BEGINNING     EARNINGS         CONT         FORF        DIST.       ENDING      V E S T E D E\n");
            sb.Append("                                         BALANCE                                                          BALANCE       AMOUNT  %  C\n");

            sb.Append("\nSTORE MANAGEMENT\n\n");
            foreach (MemberYearSummaryDto employee in store.employees.Where(mys =>
                         mys.EmployeeRank != 1999
                         && (isIndianRidge || mys.EmploymentStatusId == EmploymentStatus.Constants.Active)
                         && mys.EndingBalance != 0
                     )
                    )
            {
                sb.Append(PrintEmployee(employee));
            }

            sb.Append("\n\nASSOCIATES\n\n");
            foreach (MemberYearSummaryDto employee in store.employees.Where(mys =>
                         mys.EmployeeRank == 1999
                         && (isIndianRidge || mys.EmploymentStatusId == EmploymentStatus.Constants.Active)
                         && mys.EndingBalance != 0
                     ))
            {
                sb.Append(PrintEmployee(employee));
            }

            sb.Append("\n");
        }

        return sb.ToString();
    }

    public static string PrintEmployee(MemberYearSummaryDto member)
    {
        // BADGE #     EMPLOYEE NAME              BEGINNING     EARNINGS         CONT         FORF        DIST.       ENDING      V E S T E D E
        //                                          BALANCE                                                          BALANCE       AMOUNT  %  C
        //      700453 BANKS, JASON               11,778.95       590.00     3,330.00       888.00                 16,586.95     9,452.17  60
        //      700441 BLACKBURN, LEVI            17,111.19       855.00     3,075.00       820.00                 21,861.19    21,861.19 100

        string FormatTrailingNegative(decimal value, int width)
        {
            return value < 0
                ? $"{Math.Abs(value):N2}".PadLeft(width - 1) + "-"
                : $"{value:N2}".PadLeft(width - 1) + " ";
        }

        string ecStr = member.EnrollmentId == 1 || member.EnrollmentId == 3 || member.EnrollmentId == 4 ? " " + member.EnrollmentId : "";

        string formattedLine =
            $"     {member.BadgeNumber,-5} {member.FullName,-24} " +
            $"{(member.BeginningBalance != 0 ? FormatTrailingNegative(member.BeginningBalance, 12) : ""),12} " +
            $"{(member.Earnings != 0 ? FormatTrailingNegative(member.Earnings, 12) : ""),12} " +
            $"{(member.Contributions != 0 ? FormatTrailingNegative(member.Contributions, 12) : ""),12} " +
            $"{(member.Forfeiture != 0 ? FormatTrailingNegative(member.Forfeiture, 12) : ""),12} " +
            $"{(member.Distributions != 0 ? FormatTrailingNegative(member.Distributions, 12) : ""),12} " +
            $"{(member.EndingBalance != 0 ? FormatTrailingNegative(member.EndingBalance, 12) : ""),12} " +
            $"{(member.VestedAmount != 0 ? FormatTrailingNegative(member.VestedAmount, 12) : ""),12}" +
            $"{(member.VestedPercentage != 0 ? member.VestedPercentage.ToString("N0") : ""),3}{ecStr}";

        return formattedLine.TrimEnd() + "\n";
    }
}
