using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Xunit;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;

public class ProftShareUpdateTests
{
    [Fact]
    public void EnsureSmartReportMatchesCobolReport()
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<ProftShareUpdateTests>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing-ObfuscatedPristine"]!;

        using (var connection = new OracleConnection(connectionString))
        {
            connection.Open();
            runPay444(connection);
        }
    }

    private static void runPay444(OracleConnection connection)
    {
        //- * Meta-sw (2) = 1 : Special Run
        //- * Meta-sw (3) = 1 : Do NOT ask for Input Values.
        //- * Meta-sw (4) = 1 : Manual Adjustments
        //- * Meta-sw (5) = 1 : Secondary Earnings
        //- * Meta-sw (8) = 1 : Do NOT update PAYR/PAYBEN

        Dictionary<int, int> metaSw = new();
        metaSw[2] = 0;
        metaSw[3] = 1;
        metaSw[4] = 0;
        metaSw[5] = 0;
        metaSw[8] = 1; // reports only mode

        PAY444 pay444 = new();
        var etext = "2023*Something";
        pay444.connection = connection;
        pay444.m015MainProcessing(metaSw, etext);

        // Does not put NL in front of formfeeds, something that COBOL seems to handle
        var sb = new StringBuilder();
        for (var i = 0; i < pay444.outputLines.Count; i++)
        {
            sb.Append(pay444.outputLines[i]);
            if (i < pay444.outputLines.Count - 2 && !pay444.outputLines[i + 1].StartsWith("\f")) sb.Append("\n");
        }
        sb.Append("\n");

        var actual = sb.ToString();

        File.WriteAllText("/tmp/ref3.txt", sb.ToString(), Encoding.ASCII);

        string expected = File.ReadAllText("/tmp/ref1.txt");

        if (expected != sb.ToString())
        {

            var startInfo = new ProcessStartInfo
            {
                FileName = "/Program Files/Meld/Meld.exe",
                ArgumentList = { "/tmp/ref1.txt", "/tmp/ref3.txt" },
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Start the process
            using var process = Process.Start(startInfo);
        }

        sb.ToString().Should().Be(expected);
    }
}
