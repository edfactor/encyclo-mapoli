using System.Diagnostics;
using System.Reflection;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;

public class TerminatedEmployeeAndBeneficiaryReportIntegrationTests : PristineBaseTest
{
    public TerminatedEmployeeAndBeneficiaryReportIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task EnsureSmartReportMatchesReadyReport()
    {
        // These are arguments to the program/rest endpoint
        // Plan admin may choose a range of dates (ie. Q2 ?)
        short profitSharingYear = 2024;
        DateOnly startDate = new DateOnly(2024, 01, 6);
        DateOnly endDate = new DateOnly(2024, 12, 28);
        DateOnly effectiveDateOfTestData = new DateOnly(2024, 04, 08);

        var distributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        var calendarService = new CalendarService(DbFactory, new AccountingPeriodsService(), distributedCache);
        var totalService = new TotalService(DbFactory,
            calendarService, new EmbeddedSqlService(),
            new DemographicReaderService(new FrozenService(DbFactory, new Mock<ICommitGuardOverride>().Object), new HttpContextAccessor()));
        DemographicReaderService demographicReaderService = new(new FrozenService(DbFactory,  new Mock<ICommitGuardOverride>().Object), new HttpContextAccessor());
        TerminatedEmployeeService mockService =
            new TerminatedEmployeeService(DbFactory, totalService, demographicReaderService);

        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        var data = await mockService.GetReportAsync(new StartAndEndDateRequest{ BeginningDate = startDate, EndingDate = endDate, Take = int.MaxValue}, CancellationToken.None);

        string actualText = CreateTextReport(effectiveDateOfTestData, startDate, endDate, profitSharingYear, data);
        stopwatch.Stop();
        TestOutputHelper.WriteLine($"Took: {stopwatch.ElapsedMilliseconds} Rows: {data.Response.Results.Count()}");

        actualText.ShouldNotBeNullOrEmpty();

        string expectedText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.terminatedEmployeeAndBeneficiaryReport-correct.txt");

        ProfitShareUpdateTests.AssertReportsAreEquivalent(expectedText, actualText);
    }

    public static string ReadEmbeddedResource(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }


    private static string CreateTextReport(DateOnly effectiveDateOfTestData, DateOnly startDate, DateOnly endDate, decimal profitSharingYearWithIteration,
        TerminatedEmployeeAndBeneficiaryResponse report)
    {
        TextReportGenerator textReportGenerator = new TextReportGenerator(effectiveDateOfTestData, startDate, endDate, profitSharingYearWithIteration);

        foreach (var ms in report.Response.Results)
        {
            foreach (var yd in ms.YearDetails)
            {
                textReportGenerator.PrintDetails(ms.BadgePSn, ms.Name, yd.BeginningBalance,
                    yd.BeneficiaryAllocation, yd.DistributionAmount, yd.Forfeit,
                    yd.EndingBalance, yd.VestedBalance, yd.DateTerm, yd.YtdPsHours, yd.VestedPercent, yd.Age,
                    yd.EnrollmentCode ?? 0);
            }
        }
        textReportGenerator.PrintTotals(report.TotalEndingBalance, report.TotalVested, report.TotalForfeit, report.TotalBeneficiaryAllocation);
        return textReportGenerator.GetReport();
    }
}
