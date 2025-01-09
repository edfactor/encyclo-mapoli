using System.Diagnostics;
using System.Reflection;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Fixtures;
using Demoulas.ProfitSharing.IntegrationTests.Helpers;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Demoulas.ProfitSharing.Services.Reports.YearEnd.Update;
using FluentAssertions;
using Microsoft.Testing.Platform.Services;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;

public class TerminatedEmployeeAndBeneficiaryReportIntegrationTests : TestClassBase
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IntegrationTestsFixture _fixture;

    public TerminatedEmployeeAndBeneficiaryReportIntegrationTests(ITestOutputHelper testOutputHelper, IntegrationTestsFixture fixture) : base(fixture)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    [Fact]
    public async Task EnsureSmartReportMatchesReadyReport()
    {
        // These are arguments to the program/rest endpoint
        // Plan admin may choose a range of dates (ie. Q2 ?)
        short profitSharingYear = 2024;
        DateOnly startDate = new DateOnly(2024, 01, 01);
        DateOnly endDate = new DateOnly(2024, 12, 31);
        DateOnly effectiveDateOfTestData = new DateOnly(2024, 12, 31);

        var calendarService = _fixture.Services.GetRequiredService<ICalendarService>()!;
        var totalService = _fixture.Services.GetRequiredService<TotalService>()!;
        var contributionService = _fixture.Services.GetRequiredService<ContributionService>()!;
        TerminatedEmployeeAndBeneficiaryReportService mockService =
            new TerminatedEmployeeAndBeneficiaryReportService(ProfitSharingDataContextFactory, calendarService, totalService, contributionService);

        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        var data = await mockService.GetReportAsync(new ProfitYearRequest { ProfitYear = profitSharingYear }, CancellationToken.None);

        string actualText = CreateTextReport(effectiveDateOfTestData, startDate, endDate, profitSharingYear, data);
        stopwatch.Stop();
        _testOutputHelper.WriteLine($"Took: {stopwatch.ElapsedMilliseconds}");

        actualText.Should().NotBeNullOrEmpty();

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
            textReportGenerator.PrintDetails(ms.BadgePSn, ms.Name, ms.BeginningBalance,
                ms.BeneficiaryAllocation, ms.DistributionAmount, ms.Forfeit,
                ms.EndingBalance, ms.VestedBalance, ms.DateTerm, ms.YtdPsHours, ms.VestedPercent, ms.Age,
                ms.EnrollmentCode ?? 0);
        }
        textReportGenerator.PrintTotals(report.TotalEndingBalance, report.TotalVested, report.TotalForfeit, report.TotalBeneficiaryAllocation);
        return textReportGenerator.GetReport();

    }
}
