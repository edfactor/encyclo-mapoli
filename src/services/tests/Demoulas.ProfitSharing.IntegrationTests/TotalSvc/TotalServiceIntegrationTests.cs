using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

public class TotalServiceIntegrationTests : PristineBaseTest
{
    public TotalServiceIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }


    [Fact]
    public async Task Verify_prior_year_data_from_ready_match_smart()
    {
        short yearsUpTo = 2024;
        var calInfo = await CalendarService.GetYearStartAndEndAccountingDatesAsync(yearsUpTo, CancellationToken.None);
        Dictionary<int, PayProfitData> ppReady = await ReadyPayProfitLoader.GetReadyPayProfitByBadge(DbFactory.ConnectionString);
        Dictionary<int, PayProfitData> ppSmartYis = await SmartPayProfitLoader.GetSmartPayProfitDataByBadge(TotalService, DbFactory, yearsUpTo, calInfo.FiscalEndDate);

        // Normalize money values to 2 decimal places using standard rounding
        ppReady = ppReady.ToDictionary(kv => kv.Key, kv => NormalizeProfitData(kv.Value));
        ppSmartYis = ppSmartYis.ToDictionary(kv => kv.Key, kv => NormalizeProfitData(kv.Value));

        await CompareReadyAndSmart(ppReady, ppSmartYis);
    }

    private static PayProfitData NormalizeProfitData(PayProfitData data)
    {
        return data with
        {
            Amount = Math.Round(data.Amount, 2, MidpointRounding.AwayFromZero),
            VestedAmount = Math.Round(data.VestedAmount, 2, MidpointRounding.AwayFromZero)
        };
    }

    private async Task CompareReadyAndSmart(Dictionary<int, PayProfitData> ppReady, Dictionary<int, PayProfitData> ppSmart)
    {
        // Get the keys present in both dictionaries
        List<int> commonKeys = ppReady.Keys.Intersect(ppSmart.Keys).ToList();

        // Get keys only in "ready"
        List<int> onlyInReady = ppReady.Keys.Except(ppSmart.Keys).ToList();

        int yearsUnaccountedFor = onlyInReady.Sum(badge => ppReady[badge].Years);

        // Get keys only in "smart"
        List<int> onlyInSmart = ppSmart.Keys.Except(ppReady.Keys).ToList();

        // Count records with differences in any field
        int mismatchCount = 0;
        foreach (int badge in commonKeys)
        {
            PayProfitData ready = ppReady[badge];
            PayProfitData smart = ppSmart[badge];

            if (ready != smart)
            {
                mismatchCount++;
            }
        }

        // Output the summary results
        TestOutputHelper.WriteLine($"READY Payprofit rows: {ppReady.Count}");
        TestOutputHelper.WriteLine($"SMART Payprofit/YIS rows for 2024: {ppSmart.Count}");
        TestOutputHelper.WriteLine($"Common badges: {commonKeys.Count}");
        TestOutputHelper.WriteLine($"badges only in READY: {onlyInReady.Count}");
        TestOutputHelper.WriteLine($"  Years in READY only badges: {yearsUnaccountedFor}");
        TestOutputHelper.WriteLine($"badges only in SMART: {onlyInSmart.Count}");
        TestOutputHelper.WriteLine($"Badges with mismatched fields (in common badges): {mismatchCount}");
        TestOutputHelper.WriteLine("");

        // Sort by Exec (Y at bottom), then termination code for easier pattern analysis
        List<int> sortedKeys = commonKeys
            .Where(badge => ppReady[badge] != ppSmart[badge])
            .OrderBy(badge => ppReady[badge].Frequency == 2 ? 1 : 0) // Non-executives first, executives last
            .ThenBy(badge => ppReady[badge].TerminationCodeId ?? '\uffff') // Put nulls at end
            .ThenBy(badge => ppReady[badge].TerminationDate ?? DateOnly.MaxValue) // Then by date
            .ToList();

        // Fetch READY ETVA values for mismatched badges
        Dictionary<int, decimal> readyEtvaByBadge = await ReadyPayProfitLoader.GetReadyEtvaByBadge(DbFactory.ConnectionString, sortedKeys);

        // Common badges that appear on both TotalService and PAY450 reports
        HashSet<int> commonBadges = new() { 700173, 700569, 700655, 702489, 706161 };

        // Create detailed comparison table showing only fields that differ
        MarkdownTable mdTable = new([
            "Badge", "Ssn",
            "Enrl (R → S)",
            "Years (R → S)",
            "Amt (R → S)",
            "VAmt (R → S)",
            "R ETVA",
            "Exec",
            "Term Date",
            "Term Code"
        ]);

        int displayedRows = 0;
        const int maxDisplayRows = 30;

        foreach (int badge in sortedKeys)
        {
            PayProfitData ready = ppReady[badge];
            PayProfitData smart = ppSmart[badge];

            if (ready != smart && displayedRows < maxDisplayRows)
            {
                // Build columns showing only fields that differ
                string enrlDiff = ready.Enrollment != smart.Enrollment
                    ? $"{ready.Enrollment} → {smart.Enrollment}"
                    : ready.Enrollment.ToString();

                string yearsDiff = ready.Years != smart.Years
                    ? $"{ready.Years} → {smart.Years}"
                    : ready.Years.ToString();

                string amtDiff = ready.Amount != smart.Amount
                    ? $"{ready.Amount:N2} → {smart.Amount:N2}"
                    : "";

                string vamtDiff = ready.VestedAmount != smart.VestedAmount
                    ? $"{ready.VestedAmount:N2} → {smart.VestedAmount:N2}"
                    : "";

                string readyEtva = "";
                if (readyEtvaByBadge.TryGetValue(badge, out decimal etvaValue) && etvaValue != 0.00m)
                {
                    readyEtva = etvaValue.ToString("N2");
                }

                string? cssClass = commonBadges.Contains(badge) ? "highlight" : null;

                mdTable.AddRow(
                    cssClass,
                    badge.ToString(),
                    ready.Ssn.ToString(),
                    enrlDiff,
                    yearsDiff,
                    amtDiff,
                    vamtDiff,
                    readyEtva,
                    ready.Frequency == 2 ? "Y" : "",
                    ready.TerminationDate?.ToString("yyyy-MM-dd") ?? "",
                    ready.TerminationCodeId?.ToString() ?? ""
                );
                displayedRows++;
            }
        }

        if (displayedRows > 0)
        {
            TestOutputHelper.WriteLine($"Showing first {displayedRows} of {mismatchCount} mismatched records:");
            TestOutputHelper.WriteLine(mdTable.ToString());

            // Save HTML version to desktop
            #if false
            string title = $"Total Service Discrepancies - Count of {mismatchCount} of {commonKeys.Count:N0} considered";
            mdTable.SaveAsHtml("/Users/robertherrmann/Desktop/demos/sprint-37/total-service-discrepencies.html", title);
            TestOutputHelper.WriteLine("HTML report saved to: /Users/robertherrmann/Desktop/demos/sprint-38/total-service-discrepencies.html");
            #endif
        }

        TestOutputHelper.WriteLine($"Total mismatched records: {mismatchCount}");

        mismatchCount.ShouldBe(3);
    }
}
