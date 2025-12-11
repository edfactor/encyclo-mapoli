using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Shouldly;
#pragma warning disable CS0162 //  using if(true) forces both sides to get compiled/refactored

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]

// PAY444 Testing
public class ProfitShareUpdateTests : PristineBaseTest
{
    public ProfitShareUpdateTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task ReportWithUpdates()
    {
        // Arrange
        short profitYear = 2025;
        ProfitShareUpdateReport profitShareUpdateService = CreateProfitShareUpdateService();

        profitShareUpdateService.TodaysDateTime =
            new DateTime(2025, 11, 14, 21, 15, 0, DateTimeKind.Local); // time this report was generated

        Stopwatch sw = Stopwatch.StartNew();
        // Act
        await profitShareUpdateService.ProfitSharingUpdatePaginated(
            new ProfitShareUpdateRequest
            {
                Skip = null,
                Take = null,
                ProfitYear = profitYear,
                ContributionPercent = 15,
                IncomingForfeitPercent = 0.876678m,
                EarningsPercent = 9.280136m,
                SecondaryEarningsPercent = 0,
                MaxAllowedContributions = 57_000,
                BadgeToAdjust = 0,
                BadgeToAdjust2 = 0,
                AdjustContributionAmount = 0,
                AdjustEarningsAmount = 0,
                AdjustIncomingForfeitAmount = 0,
                AdjustEarningsSecondaryAmount = 0
            }, DemographicReaderService);
        sw.Stop();
        TestOutputHelper.WriteLine($"Query took {sw.Elapsed}");

        // We cannot do a simple report to report comparison because I believe that READY's sorting random
        // when users have the same name.   To cope with this, we extract lines with employee/bene information and compare lines.

        string expectedReport = ReadEmbeddedResource(".golden.R21-PAY444").Replace("\f", "\n\n");

        if (false)
        {
            // Enabling this path enables the diff program to pop up the differences

            // The sort order on READY is not great, this maybe tweaked soon.
            string expected = HandleSortingOddness(expectedReport);
            string actual = HandleSortingOddness(CollectLines(profitShareUpdateService.ReportLines));
            AssertReportsAreEquivalent(expected, actual);
        }
        else
        {
            // This path compares individuals and provides a list of differences. 

            var employeeExpectedReportLines = expectedReport.Split("\n").Where(ex => extractBadge(ex) != (null, null)).Select(t => t.TrimEnd()).ToList();
            var employeeActualReportLines = profitShareUpdateService.ReportLines.Where(ex => extractBadge(ex) != (null, null)).Select(t => t.TrimEnd()).ToList();

            var readyHash = employeeExpectedReportLines.ToHashSet();
            var smartHash = employeeActualReportLines.ToHashSet();

            var onlyReady = readyHash.Except(smartHash);
            var onlySmart = smartHash.Except(readyHash);

            TestOutputHelper.WriteLine($"READY member in report count {employeeExpectedReportLines.Count}, Only SMART member in report count {employeeActualReportLines.Count}");

            TestOutputHelper.WriteLine($"only READY count {onlyReady.Count()}, Only SMART count {onlySmart.Count()}");

            TestOutputHelper.WriteLine("Only Ready");
            foreach (string se in onlyReady)
            {
                TestOutputHelper.WriteLine(se);
            }

            TestOutputHelper.WriteLine("Only Smart");
            foreach (string se in onlySmart)
            {
                TestOutputHelper.WriteLine(se);
            }

            onlyReady.Count().ShouldBeLessThan(5);
            onlySmart.Count().ShouldBeLessThan(5);
        }
    }


#if false
      This needs a special report which we dont yet automatically generate

    [Fact]
    public async Task EnsureUpdateWithValues_andEmployeeAdjustment_MatchesReady()
    {
        // Arrange
        short profitYear = 2025;
        ProfitShareUpdateReport profitShareUpdateService = CreateProfitShareUpdateService();
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
                IncomingForfeitPercent = 4,
                EarningsPercent = 5,
                SecondaryEarningsPercent = 0,
                MaxAllowedContributions = 76_500,
                BadgeToAdjust = 700174,
                BadgeToAdjust2 = 0,
                AdjustContributionAmount = 44.77m,
                AdjustEarningsAmount = 22.33m,
                AdjustIncomingForfeitAmount = 18.16m,
                AdjustEarningsSecondaryAmount = 0
            }, DemographicReaderService);

        // Assert
        string expected = HandleSortingOddness(LoadExpectedReport(reportName));
        string actual = HandleSortingOddness(CollectLines(profitShareUpdateService.ReportLines));

        AssertReportsAreEquivalent(expected, actual);
    }
#endif

    // I think that if two people have the same name, that READY is not picking a particular order.
    // In the obfuscated data there are sometimes 5 people with the same name.
    private static string HandleSortingOddness(string allLines)
    {
        List<string> lines = allLines.TrimEnd().Split("\n").ToList();
        bool flipped = true;
        while (flipped)
        {
            flipped = false;
            for (int i = 0; i < lines.Count - 1; i++)
            {
                (string? badge1, string? person1) = extractBadge(lines[i]);
                (string? badge2, string? person2) = extractBadge(lines[i + 1]);
                if (person1 != null && person2 != null && person1 == person2 && badge1!.CompareTo(badge2) > 0)
                {
                    flipped = true;
                    (lines[i], lines[i + 1]) = (lines[i + 1], lines[i]);
                }
            }
        }

        return string.Join("\n", lines);
    }

    private static (string? badge, string? fullname) extractBadge(string line)
    {
        string pattern = @"^ {0,4}(\d{7}|\d{11}) (.{1,23})";
        Match match = Regex.Match(line, pattern);
        if (match.Success)
        {
            string badge = match.Groups[1].Value;
            string name = match.Groups[2].Value.TrimEnd();
            return (badge, name);
        }

        return (null, null);
    }


    private ProfitShareUpdateReport CreateProfitShareUpdateService()
    {
        return new ProfitShareUpdateReport(DbFactory, CalendarService);
    }

    private static string CollectLines(List<string> lines)
    {
        StringBuilder sb = new();
        for (int i = 0; i < lines.Count; i++)
        {
            sb.Append(lines[i]);
            // Cobol is smart enough to not emit a Newline if the next character is a form feed.
            int x = 4;
            if (x > 9 && i < lines.Count - 2 && !lines[i + 1].StartsWith('\f'))
            {
                sb.Append("\n");
            }

            sb.Append("\n");
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
            actual.ShouldBe(expected);
            return;
        }

        // This trim is a slight cheat, but it's a good enough approximation for now.
        if (actual.Trim() == expected.Trim())
        {
            actual.ShouldBe(expected);
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

}
