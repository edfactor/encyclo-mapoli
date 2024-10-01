using System.Diagnostics;
using System.Reflection;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;
public class TerminatedEmployeeAndBeneficiaryReportIntegrationTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TerminatedEmployeeAndBeneficiaryReportIntegrationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task EnsureSmartReportMatchesReadyReport()
    {
        // Turned off because of PayProfit changes
        if (new DateOnly().Day > 0)
            return;

        // We get a connection to the database.     
        var configuration = new ConfigurationBuilder().AddUserSecrets<TerminatedEmployeeAndBeneficiaryReportIntegrationTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing-ObfuscatedPristine"]!;
        var options = new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>().UseOracle(connectionString).EnableSensitiveDataLogging().Options;
        ProfitSharingReadOnlyDbContext ctx = new ProfitSharingReadOnlyDbContext(options);

        // This is to warm up the connection, so the timing below is correct.
        _ = await ctx.Demographics.Where(d => d.Ssn == 123456789012345).ToListAsync();

        // These are arguments to the program/rest endpoint
        // Plan admin may choose a range of dates (ie. Q2 ?)
        DateOnly startDate = new DateOnly(2023, 01, 07);
        DateOnly endDate = new DateOnly(2024, 01, 02);
        decimal profitSharingYear = 2023.0m;

        Mock<ILogger> ilogger = new Mock<ILogger>();

        DateOnly effectiveDateOfTestData = new DateOnly(2024, 9, 17);
        TerminatedEmployeeAndBeneficiaryReport terminatedEmployeeAndBeneficiaryReport = new TerminatedEmployeeAndBeneficiaryReport(ilogger.Object!, ctx, effectiveDateOfTestData);
        TerminatedEmployeeAndBeneficiaryDataRequest req = new()
        {
            EndDate = endDate, ProfitShareYear = profitSharingYear, StartDate = startDate
        };
        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        TerminatedEmployeeAndBeneficiaryResponse data = await terminatedEmployeeAndBeneficiaryReport.CreateData(req);
        string actualText = CreateTextReport(effectiveDateOfTestData, startDate, endDate, profitSharingYear, data);
        stopwatch.Stop();
        _testOutputHelper.WriteLine("Took: "+stopwatch.ElapsedMilliseconds);

        actualText.Should().NotBeNullOrEmpty();

        string expectedText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.terminatedEmployeeAndBeneficiaryReport-correct.txt");
        expectedText.Should().BeEquivalentTo(actualText);
    }

    public static string ReadEmbeddedResource(string resourceName)
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        using (var reader = new StreamReader(stream!))
        {
            return reader.ReadToEnd();
        }
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
