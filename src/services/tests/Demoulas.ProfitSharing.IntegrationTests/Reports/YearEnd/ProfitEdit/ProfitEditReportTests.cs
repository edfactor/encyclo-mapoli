using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY443;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitMaster;
using Demoulas.ProfitSharing.Services.ProfitShareEdit;
using Shouldly;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitEdit;

// This matches the columns of the READY PAY447 report.
public record Pay477Entry(
    long Badge,
    decimal Cont,
    decimal Earn,
    decimal Forfeit,
    string Reason,
    byte? Code,
    string Name
);

// PAY447 Testing
public class ProfitEditReportTests : PristineBaseTest
{
    public ProfitEditReportTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task UpdateTest()
    {
        // Arrange
        const short profitYear = 2025;
        ProfitShareUpdateService psu = new(DbFactory, TotalService, CalendarService, DemographicReaderService);
        ProfitShareEditService profitShareEditService = new(psu, CalendarService);
        ProfitShareUpdateRequest req = new()
        {
            Skip = null,
            Take = int.MaxValue,
            SortBy = "Psn",
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
        };
        List<Pay477Entry> readyResults = LoadReadyResults(ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R22-PAY447"));

        // Act
        Stopwatch sw = Stopwatch.StartNew();
        ProfitShareEditResponse r = await profitShareEditService.ProfitShareEdit(req, CancellationToken.None);
        List<ProfitShareEditMemberRecordResponse> smartResults = r.Response.Results.ToList();
        TestOutputHelper.WriteLine($"Edit {sw.Took()}");

        // Assert

        // Lets widdle it down to just benes.
        // smartResults = smartResults.Where(r => r.Psn > 7000001000).ToList()
        // readyResults = readyResults.Where(r => r.Badge > 7000001000).ToList()

        TestOutputHelper.WriteLine($"SMART PAY447 has {smartResults.Count} rows");
        TestOutputHelper.WriteLine($"READY PAY447 has {readyResults.Count} rows");

        // Convert the SMART Response into a form closely matching PAY447 (SMART returns a superset of data.)
        List<Pay477Entry> smartResultsUniform = smartResults.Select(re => new Pay477Entry
            (
                re.Psn!,
                re.ContributionAmount,
                re.EarningsAmount,
                re.ForfeitureAmount,
                re.RecordChangeSummary?.ToUpper() ?? "",
                re.DisplayedZeroContStatus,
                Pay443Tests.RemoveMiddleInitial(re.Name!)!
            )
        ).ToList();

        HashSet<Pay477Entry> smartResultsUniformHash = smartResultsUniform.ToHashSet();
        HashSet<Pay477Entry> readyHash = readyResults.ToHashSet();

        List<Pay477Entry> onlySmart = smartResultsUniformHash.Except(readyHash).ToList();
        List<Pay477Entry> onlyReady = readyHash.Except(smartResultsUniformHash).ToList();

        for (int i = 0; i < onlyReady.Count; i++)
        {
            TestOutputHelper.WriteLine($"ONLY READY PAY447 {onlyReady[i]}");
            if (i > 5)
            {
                break;
            }
        }

        for (int i = 0; i < onlySmart.Count; i++)
        {
            TestOutputHelper.WriteLine($"ONLY SMART PAY447 {onlySmart[i]}");
            if (i > 5)
            {
                break;
            }
        }

        onlyReady.Count.ShouldBe(0);
        onlySmart.Count.ShouldBeLessThanOrEqualTo(0);
    }

    private static List<Pay477Entry> LoadReadyResults(string rawPay447Report)
    {
        NumberStyles numberStyle = NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        List<Pay477Entry> readyResults = new();
        foreach (string rawLine in rawPay447Report.Split('\n'))
        {
            string line = rawLine.Split('\f')[0]; // strip after form feed
            if (!Regex.IsMatch(line, @"^ ?\d"))
            {
                continue;
            }

            //TestOutputHelper.WriteLine("Chewing: "+line)
            // Match columns by their fixed-width layout
            string badge = line.Substring(0, 11).Trim();
            string contStr = line.Substring(11, 14).Trim();
            string earnStr = line.Substring(25, 15).Trim();
            string forfeitStr = line.Substring(40, 15).Trim();
            string reason = line.Substring(55, 30).Trim();
            string code = line.Substring(97, 1).Trim();
            string name = line.Substring(99).Trim();

            Pay477Entry e = new(
                long.Parse(badge),
                decimal.Parse(NormalizeNumber(contStr), numberStyle),
                decimal.Parse(NormalizeNumber(earnStr), numberStyle),
                decimal.Parse(NormalizeNumber(forfeitStr), numberStyle),
                reason.ToUpper(),
                code?.Length == 0 ? null : byte.Parse(code!),
                name
            );
            readyResults.Add(e);
        }

        return readyResults;
    }

    private static string NormalizeNumber(string value)
    {
        value = value.Replace(",", "").Trim();
        if (value.EndsWith('-'))
        {
            return "-" + value[..^1];
        }

        return value;
    }

    public static string LoadExpectedReport(string resourceName)
    {
        using Stream? stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"Demoulas.ProfitSharing.IntegrationTests.Resources.{resourceName}");
        using StreamReader reader = new(stream!);
        return reader.ReadToEnd();
    }
}
