using System.Diagnostics;
using System.Reflection;
using System.Text;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Reports.YearEnd.Update.DbHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.Services.Reports.YearEnd.Update;

public class ProfitShareUpdateTests
{
    [Fact]
    public void BasicReport()
    {
        // Arrange
        short profitYear = 2023;
        ProfitShareUpdateReport profitShareUpdateService = createProfitShareUpdateService();

        string reportName = "psupdate-pay444-r1.txt";
        profitShareUpdateService.TodaysDateTime = new DateTime(2024, 11, 12, 9, 43, 0); // time report was generated

        // Act
        profitShareUpdateService.ApplyAdjustments(profitYear, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        // Assert
        string actual = CollectLines(profitShareUpdateService.ReportLines);
        string expected = LoadExpectedReport(reportName);

        AssertReportsAreEquivalent(expected, actual);
    }


    [Fact]
    public void ReportWithUpdates()
    {
        // Arrange
        short profitYear = 2024;
        ProfitShareUpdateReport profitShareUpdateService = createProfitShareUpdateService();

        string reportName = "psupdate-pay444-r2.txt";
        profitShareUpdateService.TodaysDateTime = new DateTime(2024, 11, 14, 10, 35, 0); // time report was generated

        // Act
        profitShareUpdateService.ApplyAdjustments(profitYear,15, 1, 2, 0, 0, 0, 0, 0, 0, 0, 30_000);

        // Assert
        string expected = LoadExpectedReport(reportName);
        string actual = CollectLines(profitShareUpdateService.ReportLines);

        AssertReportsAreEquivalent(expected, actual);
    }

    [Fact]
    public void EnsureUpdateWithValues_andEmployeeAdjustment_MatchesReady()
    {
        // Arrange
        short profitYear = 2024;
        ProfitShareUpdateReport profitShareUpdateService = createProfitShareUpdateService();
        string reportName = "psupdate-pay444-r3.txt";
        profitShareUpdateService.TodaysDateTime = new DateTime(2024, 11, 19, 19, 18, 0); // time report was generated

        // Act
        profitShareUpdateService.ApplyAdjustments(profitYear,15, 1, 2, 0, 700174, 44.77m, 18.16m, 22.33m, 0, 0, 30_000);

        // Assert
        string expected = LoadExpectedReport(reportName);
        string actual = CollectLines(profitShareUpdateService.ReportLines);

        AssertReportsAreEquivalent(expected, actual);
    }

    private ProfitShareUpdateReport createProfitShareUpdateService()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<ProfitShareUpdateTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        DbContextOptions<ProfitSharingReadOnlyDbContext> options =
            new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>().UseOracle(connectionString)
                .EnableSensitiveDataLogging().Options;
        ProfitSharingReadOnlyDbContext ctx = new(options);

        IProfitSharingDataContextFactory dbFactory = new DbFactory(ctx);

        OracleConnection connection = GetOracleConnection();
        connection.Open();

        return new ProfitShareUpdateReport(connection, dbFactory);
    }

    private static OracleConnection GetOracleConnection()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<ProfitShareUpdateTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        return new OracleConnection(connectionString);
    }

    private static string CollectLines(List<string> lines)
    {
        StringBuilder sb = new();
        for (int i = 0; i < lines.Count; i++)
        {
            sb.Append(lines[i]);
            // Cobol is smart enough to not emit a Newline if the next character is a form feed.
            if (i < lines.Count - 2 && !lines[i + 1].StartsWith("\f"))
            {
                sb.Append("\n");
            }
        }

        sb.Append("\n");
        return sb.ToString();
    }

    private static void AssertReportsAreEquivalent(string expected, string actual)
    {
        if (!File.Exists("/Program Files/Meld/Meld.exe"))
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
            FileName = "/Program Files/Meld/Meld.exe",
            ArgumentList = { expectedFile, actualFile },
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Start the process
        using Process? process = Process.Start(startInfo);
    }

    public static string LoadExpectedReport(string resourceName)
    {
        using (Stream? stream = Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream("Demoulas.ProfitSharing.IntegrationTests.Resources." + resourceName))
        using (StreamReader reader = new(stream!))
        {
            return reader.ReadToEnd().Replace("\r", "");
        }
    }
}
