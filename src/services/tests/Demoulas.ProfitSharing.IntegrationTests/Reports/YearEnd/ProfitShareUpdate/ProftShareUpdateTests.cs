using System.Diagnostics;
using System.Reflection;
using System.Text;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.ServiceDto;
using FluentAssertions;
using Microsoft.Extensions.Configuration;


namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;

public class ProfitShareUpdateTests
{
    private readonly AccountingPeriodsService _aps = new();
    private readonly CalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dbFactory;

    public ProfitShareUpdateTests()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<ProfitShareUpdateTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        _dbFactory = new PristineDataContextFactory(connectionString);
        _calendarService = new CalendarService(_dbFactory, _aps);
    }

    /*
     * Currently this test requires that PAY_PROFIT rows for 2023 and PAY_PROFIT rows 2024 be clones of each other.
     */
    [Fact]
    public async Task ReportWithUpdates()
    {
        // Arrange
        short profitYear = 2024;
        ProfitShareUpdateReport profitShareUpdateService = createProfitShareUpdateService();

        string reportName = "psupdate-pay444-r2.txt";
        profitShareUpdateService.TodaysDateTime =
            new DateTime(2024, 11, 14, 10, 35, 0, DateTimeKind.Local); // time report was generated

        // Act
        await profitShareUpdateService.ProfitSharingUpdatePaginated(
            new ProfitShareUpdateRequest
            {
                Skip = null,
                Take = null,
                ProfitYear = profitYear,
                ContributionPercent = 15,
                IncomingForfeitPercent = 1,
                EarningsPercent = 2,
                SecondaryEarningsPercent = 0,
                MaxAllowedContributions = 30_000,
                BadgeToAdjust = 0,
                BadgeToAdjust2 = 0,
                AdjustContributionAmount = 0,
                AdjustEarningsAmount = 0,
                AdjustIncomingForfeitAmount = 0,
                AdjustEarningsSecondaryAmount = 0
            });

        // Assert
        string expected = LoadExpectedReport(reportName);
        string actual = CollectLines(profitShareUpdateService.ReportLines);

        AssertReportsAreEquivalent(expected, actual);
    }

    [Fact]
    public async Task EnsureUpdateWithValues_andEmployeeAdjustment_MatchesReady()
    {
        // Arrange
        short profitYear = 2024;
        ProfitShareUpdateReport profitShareUpdateService = createProfitShareUpdateService();
        string reportName = "psupdate-pay444-r3.txt";
        profitShareUpdateService.TodaysDateTime =
            new DateTime(2024, 11, 19, 19, 18, 0, DateTimeKind.Local); // time report was generated

        // Act
        await profitShareUpdateService.ProfitSharingUpdatePaginated(
            new ProfitShareUpdateRequest
            {
                Skip = null,
                Take = null,
                ProfitYear = profitYear,
                ContributionPercent = 15,
                IncomingForfeitPercent = 1,
                EarningsPercent = 2,
                SecondaryEarningsPercent = 0,
                MaxAllowedContributions = 30_000,
                BadgeToAdjust = 700174,
                BadgeToAdjust2 = 0,
                AdjustContributionAmount = 44.77m,
                AdjustEarningsAmount = 22.33m,
                AdjustIncomingForfeitAmount = 18.16m,
                AdjustEarningsSecondaryAmount = 0
            });

        // Assert
        string expected = LoadExpectedReport(reportName);
        string actual = CollectLines(profitShareUpdateService.ReportLines);

        AssertReportsAreEquivalent(expected, actual);
    }


    private ProfitShareUpdateReport createProfitShareUpdateService()
    {
        return new ProfitShareUpdateReport(_dbFactory, _calendarService);
    }


    private static string CollectLines(List<string> lines)
    {
        StringBuilder sb = new();
        for (int i = 0; i < lines.Count; i++)
        {
            sb.Append(lines[i]);
            // Cobol is smart enough to not emit a Newline if the next character is a form feed.
            if (i < lines.Count - 2 && !lines[i + 1].StartsWith('\f'))
            {
                sb.Append("\n");
            }
        }

        sb.Append("\n");
        return sb.ToString();
    }

#pragma warning disable xUnit1013
    public static void AssertReportsAreEquivalent(string expected, string actual)
    {
        string? externalDiffTool = Environment.GetEnvironmentVariable("EXTERNAL_DIFF_TOOL");
        if (externalDiffTool == null)
        {
            actual.Should().Be(expected);
            return;
        }

        if (actual == expected)
        {
            actual.Should().Be(expected);
            return;
        }

        string expectedFile = Path.GetTempFileName();
        File.WriteAllBytes(expectedFile, Encoding.ASCII.GetBytes(expected));

        string actualFile = Path.GetTempFileName();
        File.WriteAllBytes(actualFile, Encoding.ASCII.GetBytes(actual));


        ProcessStartInfo startInfo = new()
        {
            FileName = externalDiffTool,
            ArgumentList = { expectedFile, actualFile },
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Start the process
        using Process? process = Process.Start(startInfo);
        process?.WaitForExit();
    }

    public static string LoadExpectedReport(string resourceName)
    {
        using (Stream? stream = Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream($"Demoulas.ProfitSharing.IntegrationTests.Resources.{resourceName}"))
        using (StreamReader reader = new(stream!))
        {
            return reader.ReadToEnd().Replace("\r", "");
        }
    }

}
