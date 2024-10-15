using System.Diagnostics;
using System.Reflection;
using Demoulas.AccountsReceivable.Tests.Common.Fixtures;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Helpers;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using FluentAssertions;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;
public class TerminatedEmployeeAndBeneficiaryReportIntegrationTests : TestClassBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TerminatedEmployeeAndBeneficiaryReportIntegrationTests(ITestOutputHelper testOutputHelper, IntegrationTestsFixture fixture) : base(testOutputHelper, fixture)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task EnsureSmartReportMatchesReadyReport()
    {
        // These are arguments to the program/rest endpoint
        // Plan admin may choose a range of dates (ie. Q2 ?)
        DateOnly startDate = new DateOnly(2023, 01, 07);
        DateOnly endDate = new DateOnly(2024, 01, 02);
        short profitSharingYear = 2023;


        DateOnly effectiveDateOfTestData = new DateOnly(2024, 9, 17);
        TerminatedEmployeeAndBeneficiaryReport terminatedEmployeeAndBeneficiaryReport = new TerminatedEmployeeAndBeneficiaryReport(ProfitSharingDataContextFactory);
        ProfitYearRequest req = new()
        {
            ProfitYear = profitSharingYear,
        };
        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        TerminatedEmployeeAndBeneficiaryResponse data = await terminatedEmployeeAndBeneficiaryReport.CreateData(req, CancellationToken.None);
        string actualText = CreateTextReport(effectiveDateOfTestData, startDate, endDate, profitSharingYear, data);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Took: " + stopwatch.ElapsedMilliseconds);

        actualText.Should().NotBeNullOrEmpty();

        string expectedText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.terminatedEmployeeAndBeneficiaryReport-correct.txt");
        expectedText.Should().BeEquivalentTo(actualText);
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
