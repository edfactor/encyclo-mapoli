using System.Diagnostics;
using System.Reflection;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;
public class TerminatedEmployeeAndBeneficiaryReportIntegrationTests
{

    [Fact]
    public void EnsureSmartReportMatchesReadyReport()
    {
        // We get a connection to the database.     
        var configuration = new ConfigurationBuilder().AddUserSecrets<TerminatedEmployeeAndBeneficiaryReportIntegrationTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>().UseOracle(connectionString).EnableSensitiveDataLogging().Options;
        ProfitSharingDbContext ctx = new ProfitSharingDbContext(options);

        // These are arguments to the program/rest endpoint
        // Plan admin may choose a range of dates (ie. Q2 ?)
        DateOnly startDate = new DateOnly(2023, 01, 07);
        DateOnly endDate = new DateOnly(2024, 01, 02);
        decimal profitSharingYear = 2023.0m;

        Mock<ILogger> ilogger = new Mock<ILogger>();

        DateOnly effectiveDateOfTestData = new DateOnly(2024, 9, 17);

        TerminatedEmployeeAndBeneficiaryReport terminatedEmployeeAndBeneficiaryReport = new TerminatedEmployeeAndBeneficiaryReport(ilogger.Object!, ctx, effectiveDateOfTestData);
        string actualText = terminatedEmployeeAndBeneficiaryReport.CreateReport(startDate, endDate, profitSharingYear);

        actualText.Should().NotBeNullOrEmpty();

        string expectedText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.terminatedEmployeeAndBeneficiaryReport-correct.txt");
        expectedText.Should().NotBeNullOrEmpty();

        if (expectedText != actualText && File.Exists(@"c:\Program Files\Meld\Meld.exe"))
        {
            // This is for post execution analysis
            string actualPath = Path.Combine(Path.GetTempPath(), "actual.txt");
            File.WriteAllText(actualPath, actualText);
            string expectedPath = Path.Combine(Path.GetTempPath(), "expected.txt");
            File.WriteAllText(expectedPath, expectedText);

            // see https://meldmerge.org/
            Process.Start(@"c:\Program Files\Meld\Meld.exe", actualPath + " " + expectedPath);
        }

        expectedText.Should().BeEquivalentTo(actualText);
        Assert.Equal(expectedText, actualText);

    }

    public static string ReadEmbeddedResource(string resourceName)
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        using (var reader = new StreamReader(stream!))
        {
            return reader.ReadToEnd();
        }
    }
}
