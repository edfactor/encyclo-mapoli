using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
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
        short yearsUpTo = 2023;
        var calInfo = await CalendarService.GetYearStartAndEndAccountingDatesAsync(yearsUpTo, CancellationToken.None);
        Dictionary<int, PayProfitData> ppReady = await ReadyPayProfitLoader.GetReadyPayProfitByBadge(DbFactory.ConnectionString);
        Dictionary<int, PayProfitData> ppSmartYis = await SmartPayProfitLoader.GetSmartPayProfitDataByBadge(TotalService, DbFactory, yearsUpTo, calInfo.FiscalEndDate);

        Dictionary<int, int> readyYearByBadge = ppReady.ToDictionary(k => k.Key, v => v.Value.Years);
        Dictionary<int, int> smartYearByBadge = ppSmartYis.ToDictionary(k => k.Key, v => v.Value.Years);

        Dictionary<int, int> readySsnByBadge = ppReady.ToDictionary(k => k.Key, v => v.Value.Ssn);

        CompareReadyAndSmart(readySsnByBadge, readyYearByBadge, smartYearByBadge);
    }

    private void CompareReadyAndSmart(Dictionary<int, int> readySsnByBadge, Dictionary<int, int> readyBadgeByYears, Dictionary<int, int> smartBadgeByYears)
    {
        // Get the keys present in both dictionaries
        List<int> commonKeys = readyBadgeByYears.Keys.Intersect(smartBadgeByYears.Keys).ToList();

        // Get keys only in "ready"
        List<int> onlyInReady = readyBadgeByYears.Keys.Except(smartBadgeByYears.Keys).ToList();

        int yearsUnaccountedFor = onlyInReady.Sum(badge => readyBadgeByYears[badge]);

        // Get keys only in "smart"
        List<int> onlyInSmart = smartBadgeByYears.Keys.Except(readyBadgeByYears.Keys).ToList();

        // Count differences in "years" values for common keys
        int differentYearsCount = commonKeys.Count(badge => readyBadgeByYears[badge] != smartBadgeByYears[badge]);

        // Output the results
        TestOutputHelper.WriteLine($"READY Payprofit rows: {readyBadgeByYears.Count}");
        TestOutputHelper.WriteLine($"SMART Payprofit/YIS rows for 2024: {smartBadgeByYears.Count}");
        TestOutputHelper.WriteLine($"Common badges: {commonKeys.Count}");
        TestOutputHelper.WriteLine($"badges only in READY: {onlyInReady.Count}");
        TestOutputHelper.WriteLine($" Hours in READY only badges: {yearsUnaccountedFor}");
        TestOutputHelper.WriteLine($"badges only in SMART: {onlyInSmart.Count}");
        TestOutputHelper.WriteLine($"Badges with different years (in common badges): {differentYearsCount}");
        TestOutputHelper.WriteLine("");

        Dictionary<int, string> notes = new() { [700000045] = "1989.5", [700000237] = "Monthly PY_FREQ=2", [700005972] = "Monthly PY_FREQ=2" };


        TestOutputHelper.WriteLine("This table show cases where the systems done agree and READY's or SMART's years are below 7 years.");
        MarkdownTable mdTable = new(["Badge", "Ssn", "READY Years", "SMART Years", "Notes"]);
        int rowCnt = 0;
        foreach (int badge in commonKeys)
        {
            if (readyBadgeByYears[badge] != smartBadgeByYears[badge]) // && (readyBadgeByYears[badge] > 6 || smartBadgeByYears[badge] >6 ))
            {
                int ssn = readySsnByBadge[badge];
                mdTable.AddRow(badge.ToString(), ssn.ToString(), readyBadgeByYears[badge].ToString(), smartBadgeByYears[badge].ToString(),
                    notes.ContainsKey(ssn) ? notes[ssn] : "");
                rowCnt++;
            }
        }

        // TestOutputHelper.WriteLine("ssns: " + string.Join(", ", mdTable.rows().Select(r => r[1])))

        TestOutputHelper.WriteLine(mdTable.ToString());
        TestOutputHelper.WriteLine($"Total Rows {rowCnt}");

        rowCnt.ShouldBe(3);
    }
}
