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
    public void Ensure2023ReportMatchesReady()
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
        String reportName = "psupdate-pay444-report2023.txt";
        pay444.TodaysDateTime = new DateTime(2024, 11, 12, 9, 43, 0);  // time report was generated

        // Act
        pay444.m015MainProcessing(metaSw, year);

        // Assert
        string actual = CollectLines(pay444.outputLines);
        string expected = ReadEmbeddedResource(reportName).Replace("\r", "");

        PopUpExternalMeld(actual, expected);

        actual.Should().Be(expected);
    }

    [Fact]
    public void EnsureUpdateWithValuesMatchesReady()
    {
        // Arrange
        Dictionary<int, int> metaSw = new();
        metaSw[2] = 0; // Special Run
        metaSw[3] = 0; // Do NOT ask for Input Values.
        metaSw[4] = 0; // Manual Adjustments
        metaSw[5] = 0; // Secondary Earnings
        metaSw[8] = 1; // Do NOT update PAYR/PAYBEN
        using OracleConnection connection = GetOracleConnection();
        connection.Open();
        PAY444 pay444 = new();
        int year = 2024;
        String reportName = "psupdate-pay444.15.1.2.0.30000-2024.txt";
        pay444.TodaysDateTime = new DateTime(2024, 11, 14, 10, 35, 0);  // time report was generated

        // We should pass in the point values, but ATM they are hard coded.
        pay444.connection = connection;

        // Act
        pay444.m015MainProcessing(metaSw, year);

        // Assert
        string actual = CollectLines(pay444.outputLines);
        string expected = ReadEmbeddedResource(reportName).Replace("\r", "");

        PopUpExternalMeld(actual, expected);

        actual.Should().Be(expected);
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

    private static void PopUpExternalMeld(string actual, string expected)
    {
        if (expected == actual || !File.Exists("/Program Files/Meld/Meld.exe"))
        {
            return;
        }

        string expectedFile = Path.GetTempFileName();
        //File.WriteAllText(expectedFile, expected, Encoding.ASCII);
        File.WriteAllBytes(expectedFile, Encoding.ASCII.GetBytes(expected));

        string actualFile = Path.GetTempFileName();
        // File.WriteAllText(actualFile, actual, Encoding.ASCII);
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

    public static string ReadEmbeddedResource(string resourceName)
    {
        using (Stream? stream = Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream("Demoulas.ProfitSharing.IntegrationTests.Resources." + resourceName))
        using (StreamReader reader = new(stream!))
        {
            return reader.ReadToEnd();
        }
    }
}
