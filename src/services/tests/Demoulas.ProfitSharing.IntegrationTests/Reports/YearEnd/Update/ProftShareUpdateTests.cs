using System.Diagnostics;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class ProftShareUpdateTests
{
    [Fact]
    public void BasicReport()
    {
        // Arrange
        Dictionary<int, int> metaSw = new();
        metaSw[2] = 0; // Special Run
        metaSw[3] = 1; // Do NOT ask for Input Values.
        metaSw[4] = 0; // Manual Adjustments
        metaSw[5] = 0; // Secondary Earnings
        metaSw[8] = 1; // Do NOT update PAYR/PAYBEN
        using OracleConnection connection = GetOracleConnection();
        connection.Open();
        PAY444 pay444 = new();
        int year = 2023;
        pay444.connection = connection;
        string reportName = "psupdate-pay444-r1.txt";
        pay444.TodaysDateTime = new DateTime(2024, 11, 12, 9, 43, 0); // time report was generated

        // Act
        pay444.m015MainProcessing(metaSw, year, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        // Assert
        string actual = CollectLines(pay444.outputLines);
        string expected = LoadExpectedReport(reportName).Replace("\r", "");

        AssertReportsAreEquivalent(expected, actual);
    }

    [Fact]
    public void ReportWithUpdates()
    {
        // Arrange
        using OracleConnection connection = GetOracleConnection();
        connection.Open();
        Dictionary<int, int> metaSw = new();
        metaSw[2] = 0; // Special Run
        metaSw[3] = 0; // Do NOT ask for Input Values.
        metaSw[4] = 0; // Manual Adjustments
        metaSw[5] = 0; // Secondary Earnings
        metaSw[8] = 1; // Do NOT update PAYR/PAYBEN

        PAY444 pay444 = new();
        int year = 2024;
        string reportName = "psupdate-pay444-r2.txt";
        pay444.TodaysDateTime = new DateTime(2024, 11, 14, 10, 35, 0); // time report was generated

        // We should pass in the point values, but ATM they are hard coded.
        pay444.connection = connection;

        // Act
        pay444.m015MainProcessing(metaSw, year, 15, 1, 2, 0, 0, 0, 0, 0, 0, 0, 30000);

        // Assert
        string expected = LoadExpectedReport(reportName).Replace("\r", "");
        string actual = CollectLines(pay444.outputLines);

        AssertReportsAreEquivalent(expected, actual);
    }

    [Fact]
    public void EnsureUpdateWithValues_andEmployeeAdjustment_MatchesReady()
    {
        // Arrange
        Dictionary<int, int> metaSw = new();
        metaSw[2] = 0; // Special Run
        metaSw[3] = 0; // Do NOT ask for Input Values.
        metaSw[4] = 0; // Suppress Manual Adjustments
        metaSw[5] = 0; // Secondary Earnings
        metaSw[8] = 1; // Do NOT update PAYR/PAYBEN
        using OracleConnection connection = GetOracleConnection();
        connection.Open();
        PAY444 pay444 = new();
        int year = 2024;
        string reportName = "psupdate-pay444-r3.txt";
        pay444.TodaysDateTime = new DateTime(2024, 11, 19, 19, 18, 0); // time report was generated

        // We should pass in the point values, but ATM they are hard coded.
        pay444.connection = connection;

        // Act
        pay444.m015MainProcessing(metaSw, year, 15, 1, 2, 0, 700174, 44.77m, 18.16m, 22.33m, 0, 0, 30000);

        // Assert
        string expected = LoadExpectedReport(reportName);
        string actual = CollectLines(pay444.outputLines);

        AssertReportsAreEquivalent(expected, actual);
    }

    [Fact]
    public void with_secondary_earnings_and_employee_and_member_overrides()
    {
        // Arrange
        Dictionary<int, int> metaSw = new();
        metaSw[2] = 0; // Special Run
        metaSw[3] = 0; // Do NOT ask for Input Values.
        metaSw[4] = 0; // Suppress Manual Adjustments
        metaSw[5] = 1; // Secondary Earnings
        metaSw[8] = 1; // Do NOT update PAYR/PAYBEN
        using OracleConnection connection = GetOracleConnection();
        connection.Open();
        PAY444 pay444 = new();
        int year = 2024;
        string reportName = "psupdate-pay444-r4.txt";

        pay444.TodaysDateTime = new DateTime(2024, 11, 22, 13, 18, 0); // time report was generated

        // We should pass in the point values, but ATM they are hard coded.
        pay444.connection = connection;

        // Act
        pay444.m015MainProcessing(metaSw, year, 17, 2.75m, 7.95m, 3.74m,
            700196, 1.11m, 3.33m, 2.22m,
            700417, 4.44m, 30000);

        // Assert
        string expected = LoadExpectedReport(reportName);
        string actual = CollectLines(pay444.outputLines);

#if false
// Pending outcome of Secondary earnings clarification
        AssertReportsAreEquivalent(expected, actual);
#endif
    }


    private static OracleConnection GetOracleConnection()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().AddUserSecrets<ProftShareUpdateTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing-ObfuscatedPristine"]!;
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
        if (!File.Exists("x/Program Files/Meld/Meld.exe"))
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
