using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.ProfitSharing.Services.ItOperations;
using Demoulas.ProfitSharing.Services.Reports.Breakdown;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Breakdown;

public class BreakdownReportByStoreTests
{
    private readonly AccountingPeriodsService _aps = new();
    private readonly IBreakdownService _breakdownService;
    private readonly CalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;
    private readonly IEmbeddedSqlService _embeddedSqlService;

    public BreakdownReportByStoreTests()
    {
        _dataContextFactory = new PristineDataContextFactory();
        var distributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        _calendarService = new CalendarService(_dataContextFactory, _aps, distributedCache);
        _embeddedSqlService = new EmbeddedSqlService();
        _totalService = new TotalService(_dataContextFactory, _calendarService, _embeddedSqlService,
            new DemographicReaderService(new FrozenService(_dataContextFactory), new HttpContextAccessor()));
        var frozenService = new FrozenService(_dataContextFactory);
        IHttpContextAccessor httpCtxAcc = Mock.Of<IHttpContextAccessor>();
        _breakdownService = new BreakdownReportService(_dataContextFactory, _calendarService, _totalService, new DemographicReaderService(frozenService, httpCtxAcc));
    }

    [Fact]
    public async Task RunReport()
    {
        ReportResponseBase<MemberYearSummaryDto> results =
            await _breakdownService.GetActiveMembersByStore(new BreakdownByStoreRequest { ProfitYear = 2024, Take = int.MaxValue }, CancellationToken.None);

        List<(short Key, List<MemberYearSummaryDto>)> groupedEmployees =
        [
            .. results.Response.Results
                .GroupBy(m => m.StoreNumber)
                .OrderBy(g => g.Key)
                .Select(storeGroup => (storeGroup.Key, storeGroup.ToList()))
        ];

        string actual = CreateTextReport(1, groupedEmployees);
        string expected = ProfitShareUpdateTests.LoadExpectedReport("qpay066ta.breakdown-by-store.txt");
        ProfitShareUpdateTests.AssertReportsAreEquivalent(expected, actual);
    }

    [Fact]
    public async Task RunReport700()
    {
        ReportResponseBase<MemberYearSummaryDto> results =
            await _breakdownService.GetActiveMembersByStore(new BreakdownByStoreRequest { StoreNumber = 700, ProfitYear = 2024,  Take = int.MaxValue },
                CancellationToken.None);

        List<(short Key, List<MemberYearSummaryDto>)> groupedEmployees =
        [
            .. results.Response.Results
                .GroupBy(m => m.StoreNumber)
                .OrderBy(g => g.Key)
                .Select(storeGroup => (storeGroup.Key, storeGroup.ToList()))
        ];

        string actual = CreateTextReport(180, groupedEmployees);
        string expected = ProfitShareUpdateTests.LoadExpectedReport("qpay066ta.store-700.txt");
        ProfitShareUpdateTests.AssertReportsAreEquivalent(expected, actual);
    }

    private static string CreateTextReport(short startPage, List<(short StoreNumber, List<MemberYearSummaryDto> employees)> groupedEmployees)
    {
        PaperPrinter pap = new(startPage);
        if (startPage == 1)
        {
            pap.newLine();
            pap.line("DJDE JDE=LANIQS,JDL=DFLT4,END,;");
            pap.newLine();
        }

        bool first = true;
        foreach ((short StoreNumber, List<MemberYearSummaryDto> employees) store in groupedEmployees)
        {
            if (!first)
            {
                pap.line("\f");
                pap.newLine();
            }

            first = false;
            // update header with new store number
            pap.HeaderTemplate = pageNumber => $"""
                                                QPAY066TA               PROFIT SHARING BREAKDOWN REPORT    DATE MAR 10, 2025  YEAR:   2024.0     PAGE:   {pageNumber:D5}
                                                
                                                   STORE  
                                                """
                                               + store.StoreNumber + """


                                                                     BADGE #     EMPLOYEE NAME              BEGINNING     EARNINGS         CONT         FORF        DIST.       ENDING      V E S T E D E
                                                                                                              BALANCE                                                          BALANCE       AMOUNT  %  C

                                                                     """;
            printStore(pap, store.employees);
        }

        return pap.ToString();
    }

    private static void printStore(PaperPrinter pap, List<MemberYearSummaryDto> employees)
    {
        pap.insertHeader();

        foreach (IGrouping<string?, MemberYearSummaryDto> grouping in employees.GroupBy(e => e.PayClassificationName))
        {
            pap.newLine();
            pap.line(grouping.Key ?? "");
            pap.newLine();
            foreach (MemberYearSummaryDto employee in grouping)
            {
                pap.line(PrintEmployee(employee));
            }

            pap.newLine();
        }
    }

    public static string PrintEmployee(MemberYearSummaryDto member)
    {
#pragma warning disable S125
        string ecStr = string.Empty; //member.EnrollmentId == 1 || member.EnrollmentId == 3 || member.EnrollmentId == 4 ? " " + member.EnrollmentId : "";
#pragma warning restore S125

        string formattedLine =
            $"     {member.BadgeNumber,-5} {member.FullName,-24} " +
            $"{(member.BeginningBalance != 0 ? FormatTrailingNegative(member.BeginningBalance) : ""),12} " +
            $"{(member.Earnings != 0 ? FormatTrailingNegative(member.Earnings) : ""),12} " +
            $"{(member.Contributions != 0 ? FormatTrailingNegative(member.Contributions) : ""),12} " +
            $"{(member.Forfeitures != 0 ? FormatTrailingNegative(member.Forfeitures) : ""),12} " +
            $"{(member.Distributions != 0 ? FormatTrailingNegative(member.Distributions) : ""),12} " +
            $"{(member.EndingBalance != 0 ? FormatTrailingNegative(member.EndingBalance) : ""),12} " +
            $"{(member.VestedAmount != 0 ? FormatTrailingNegative(member.VestedAmount) : ""),12}" +
            $"{(member.VestedPercent != 0 ? member.VestedPercent.ToString("N0") : ""),3}{ecStr}";

        return formattedLine.TrimEnd();
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
