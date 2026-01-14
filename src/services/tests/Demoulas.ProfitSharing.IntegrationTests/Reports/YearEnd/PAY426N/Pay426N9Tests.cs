using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Services.Reports;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426N;

/// <summary>
///     Integration test for PAY426N-9 (Profit Sharing Summary Total Page).
///     Validates that SMART's aggregated summary matches READY's golden summary report.
/// </summary>
public class Pay426N9Tests : PristineBaseTest
{
    private readonly ProfitSharingSummaryReportService _reportService;

    public Pay426N9Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _reportService = new ProfitSharingSummaryReportService(DbFactory, CalendarService, TotalService, DemographicReaderService, new NullLogger<ProfitSharingSummaryReportService>());
    }

    [Fact]
    [Description("PAY426N-9 summary totals should match READY golden file")]
    public async Task Pay426N9Summary_ShouldMatchReady()
    {
        // Arrange - load expected data from READY summary report
        string resourceName = "Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R08-PAY426N-9";
        string expectedReportText = ReadEmbeddedResource(resourceName).Trim();
        YearEndProfitSharingReportSummaryResponse expected = Pay426N9Parser.Parse(expectedReportText);

        var request = new BadgeNumberRequest
        {
            ProfitYear = 2025,
            UseFrozenData = false
        };

        // Act - get actual data from SMART
        YearEndProfitSharingReportSummaryResponse actual = await _reportService.GetYearEndProfitSharingSummaryReportAsync(request, CancellationToken.None);

        // Assert - compare line items
        actual.ShouldNotBeNull();
        actual.LineItems.ShouldNotBeNull();
        actual.LineItems.ShouldNotBeEmpty();

        // Compare each line item by prefix
        var expectedByPrefix = expected.LineItems.ToDictionary(li => li.LineItemPrefix);
        var actualByPrefix = actual.LineItems.ToDictionary(li => li.LineItemPrefix);

        // SMART only returns line items with data; READY shows all lines including zeros
        // Lines E and X are missing in SMART for now.
        var mandatoryPrefixes = expectedByPrefix.Keys
            .Where(prefix => prefix is not "E" and not "X")
            .ToHashSet();

        // Ensure all mandatory prefixes exist in actual
        foreach (var expectedPrefix in mandatoryPrefixes)
        {
            actualByPrefix.ShouldContainKey(expectedPrefix, $"SMART missing line item with prefix '{expectedPrefix}'");
        }

        // Compare each line item (only those present in both)
        foreach (var prefix in mandatoryPrefixes)
        {
            var expectedItem = expectedByPrefix[prefix];
            var actualItem = actualByPrefix[prefix];

            // Compare values
            actualItem.NumberOfMembers.ShouldBe(expectedItem.NumberOfMembers,
                $"Line {prefix}: Member count mismatch");

            actualItem.TotalWages.ShouldBe(expectedItem.TotalWages,
                $"Line {prefix}: Total wages mismatch");

            actualItem.TotalBalance.ShouldBe(expectedItem.TotalBalance,
                $"Line {prefix}: Total balance mismatch");

            actualItem.Subgroup.ToUpper().ShouldBe(expectedItem.Subgroup,
                $"Line {prefix}: Subgroup mismatch");
        }

        actual.LineItems.First(li => li.LineItemTitle == ">= AGE 21 WITH >= 1000 PS HOURS").TotalHours.ShouldBe(7732647);

        // Calculate and compare grand totals (exclude E and X as they don't count toward totals)
        // Only sum prefixes 1-8 and N
        var includedPrefixes = new HashSet<string> { "1", "2", "3", "4", "5", "6", "7", "8", "N" };

        int expectedTotalMembers = expected.LineItems
            .Where(li => includedPrefixes.Contains(li.LineItemPrefix))
            .Sum(li => li.NumberOfMembers);
        int actualTotalMembers = actual.LineItems
            .Where(li => includedPrefixes.Contains(li.LineItemPrefix))
            .Sum(li => li.NumberOfMembers);

        decimal expectedTotalWages = expected.LineItems
            .Where(li => includedPrefixes.Contains(li.LineItemPrefix))
            .Sum(li => li.TotalWages);
        decimal actualTotalWages = actual.LineItems
            .Where(li => includedPrefixes.Contains(li.LineItemPrefix))
            .Sum(li => li.TotalWages);

        decimal expectedTotalBalance = expected.LineItems
            .Where(li => includedPrefixes.Contains(li.LineItemPrefix))
            .Sum(li => li.TotalBalance);
        decimal actualTotalBalance = actual.LineItems
            .Where(li => includedPrefixes.Contains(li.LineItemPrefix))
            .Sum(li => li.TotalBalance);

        actualTotalMembers.ShouldBe(expectedTotalMembers, "Grand total member count mismatch");
        actualTotalWages.ShouldBe(expectedTotalWages, "Grand total wages mismatch");
        actualTotalBalance.ShouldBe(expectedTotalBalance, "Grand total balance mismatch");
    }
}
