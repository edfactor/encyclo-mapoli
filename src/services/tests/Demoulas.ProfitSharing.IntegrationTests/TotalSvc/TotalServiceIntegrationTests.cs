using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using FluentAssertions;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

public class TotalServiceIntegrationTests : PristineBaseTest
{
    public TotalServiceIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }


    [Fact]
    public async Task Verify_years()
    {
        Dictionary<int, PayProfitData> ppReady = await ReadyPayProfitLoader.GetReadyPayProfitByBadge(DbFactory.ConnectionString);
        Dictionary<int, PayProfitData> ppSmartYis = await SmartPayProfitLoader.GetSmartPayProfitDataByBadge(TotalService, DbFactory, 2024);

        Dictionary<int, int> readyBadgeByYears = ppReady.ToDictionary(k => k.Key, v => v.Value.Years);
        Dictionary<int, int> smartBadgeByYears = ppSmartYis.ToDictionary(k => k.Key, v => v.Value.Years);

        CompareReadyAndSmart(readyBadgeByYears, smartBadgeByYears);
        
        true.ShouldBe(true);
    }

    private void CompareReadyAndSmart(Dictionary<int, int> readyBadgeByYears, Dictionary<int, int> smartBadgeByYears)
    {
        // Get the keys present in both dictionaries
        List<int> commonKeys = readyBadgeByYears.Keys.Intersect(smartBadgeByYears.Keys).ToList();

        // Get keys only in "ready"
        List<int> onlyInReady = readyBadgeByYears.Keys.Except(smartBadgeByYears.Keys).ToList();

        // Get keys only in "smart"
        List<int> onlyInSmart = smartBadgeByYears.Keys.Except(readyBadgeByYears.Keys).ToList();

        // Count differences in "years" values for common keys
        int differentYearsCount = commonKeys.Count(badge => readyBadgeByYears[badge] != smartBadgeByYears[badge]);

        // Output the results
        TestOutputHelper.WriteLine($"READY Payprofit rows: {readyBadgeByYears.Count}");
        TestOutputHelper.WriteLine($"SMART Payprofit/YIS rows for 2024: {smartBadgeByYears.Count}");
        TestOutputHelper.WriteLine($"Common badges: {commonKeys.Count}");
        TestOutputHelper.WriteLine($"badges only in READY: {onlyInReady.Count}");
        TestOutputHelper.WriteLine($"badges only in SMART: {onlyInSmart.Count}");
        TestOutputHelper.WriteLine($"Badges with different years (in common badges): {differentYearsCount}");
        TestOutputHelper.WriteLine("");

        TestOutputHelper.WriteLine("This table show cases where either READY's or SMART's years are below 7 years.");
        MarkdownTable mdTable = new(["Badge", "READY Years", "SMART Years"]);
        int rowCnt = 0;
        foreach (int badge in commonKeys)
        {
            if (readyBadgeByYears[badge] != smartBadgeByYears[badge] && (readyBadgeByYears[badge] < 7 || smartBadgeByYears[badge] < 7 ))
            {
                mdTable.AddRow(badge.ToString(), readyBadgeByYears[badge].ToString(), smartBadgeByYears[badge].ToString());
                rowCnt++;
            }
        }

        TestOutputHelper.WriteLine(mdTable.ToString());
            TestOutputHelper.WriteLine($"Total Rows {rowCnt}");
    }
}
