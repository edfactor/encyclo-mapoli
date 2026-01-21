using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.IntegrationTests.TotalSvc;
using Demoulas.ProfitSharing.Services.Services.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.UpdateSummary;

public class UpdateSummaryTests : PristineBaseTest
{
    private readonly FrozenReportService _frozenReportService;

    public UpdateSummaryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _frozenReportService = new FrozenReportService(DbFactory, new LoggerFactory(), TotalService, CalendarService,
            DemographicReaderService);
    }

    [Fact]
    public async Task ValidatePay450Report()
    {
        List<Pay450Record> ready = GetReadyRecords();
        List<Pay450Record> smart = await GetSmartRecords();

        // Load all executive SSNs (PY_FREQ = '2') for profit year 2025
        HashSet<string> executiveBadgeAndStore = await DbFactory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.PayProfits
                .Include(pp => pp.Demographic)
                .Where(pp => pp.ProfitYear == TestConstants.OpenProfitYear && pp.Demographic!.PayFrequencyId == PayFrequency.Constants.Monthly /*Executive*/)
                .Select(pp => $"0{pp.Demographic!.BadgeNumber} {pp.Demographic.StoreNumber:000}")
                .ToHashSetAsync(CancellationToken.None);
        });

        TestOutputHelper.WriteLine($"Executive Count (PY_FREQ=2): {executiveBadgeAndStore.Count}");

        CountBreakdown("Ready", ready);
        CountBreakdown("Smart", smart);

        List<Pay450Record> smartOnly = smart.ExceptBy(ready.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();
        List<Pay450Record> readyOnly = ready.ExceptBy(smart.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();

        // Ignore people with no money in READY
        foreach (Pay450Record pay450Record in readyOnly)
        {
            pay450Record.BeforeAmount.ShouldBe(0);
            pay450Record.AfterAmount.ShouldBe(0);
        }

        TestOutputHelper.WriteLine("");
        // Ignore people with no money in SMART
        foreach (Pay450Record pay450Record in smartOnly)
        {
            pay450Record.BeforeAmount.ShouldBe(0);
            pay450Record.AfterAmount.ShouldBe(0);
        }

        List<Pay450Record> intersection = smart.IntersectBy(ready.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();
        Dictionary<string, Pay450Record> readyByBadgeAndStore = ready.ToDictionary(k => k.BadgeAndStore, v => v);

        List<(Pay450Record Actual, Pay450Record Expected)> comparisons = new();

        foreach (Pay450Record smartInt in intersection)
        {
            Pay450Record readyInt = readyByBadgeAndStore[smartInt.BadgeAndStore];
            Pay450Record readyIntNorm = Normalize(executiveBadgeAndStore, readyInt);
            Pay450Record smartIntNorm = Normalize(executiveBadgeAndStore, smartInt);

            if (readyIntNorm != smartIntNorm)
            {
                comparisons.Add((smartIntNorm, readyIntNorm));
            }
        }

        TestOutputHelper.WriteLine($"Discrepancy Count {comparisons.Count}");

        if (comparisons.Count > 0)
        {
            TestOutputHelper.WriteLine($"\n=== DETAILED DISCREPANCIES (first 30 of {comparisons.Count}) ===");

            // Calculate summary statistics for all comparisons
            decimal totalAfterVestedDifference = 0;
            int afterVestedDifferenceCount = 0;

            foreach ((Pay450Record actual, Pay450Record expected) in comparisons)
            {
                decimal difference = actual.AfterVested - expected.AfterVested;
                if (Math.Abs(difference) >= 0.01m)
                {
                    totalAfterVestedDifference += difference;
                    afterVestedDifferenceCount++;
                }
            }

            // Common badges that appear on both TotalService and PAY450 reports
            // IE. these individuals have problems on both reports
            HashSet<string> commonBadges = new() { "700173", "700569", "700655", "702489", "706161" };

            // Get ETVA values for both Ready and Smart
            List<int> badgesForEtva = comparisons.Select(vci => int.Parse(ExtractBadge(vci.Actual.BadgeAndStore))).ToList();
            Dictionary<int, decimal> readyEtvaByBadge = await ReadyPayProfitLoader.GetReadyEtvaByBadge(DbFactory.ConnectionString, badgesForEtva);
            Dictionary<int, decimal> smartEtvaByBadge = await GetSmartEtvaByBadge(badgesForEtva);

            MarkdownTable detailedTable = new([
                "Badge",
                "Before Amt (R→S)", "Before Vested (R→S)", "Before Years (R→S)", "Before Enrl (R→S)",
                "After Amt (R→S)", "After Vested (R→S)", "After Years (R→S)", "After Enrl (R→S)",
                "ETVA (R→S)"
            ]);

            // Sort by Before Years for easier analysis
            var sortedComparisons = comparisons
                .OrderBy(c => c.Expected.BeforeYears ?? 0)
                .ThenBy(c => ExtractBadge(c.Expected.BadgeAndStore))
                .ToList();

            foreach ((Pay450Record actual, Pay450Record expected) in sortedComparisons.Take(30))
            {
                string badge = ExtractBadge(expected.BadgeAndStore);
                // Highlight common badges in grey
                string? cssClass = commonBadges.Contains(badge) ? "highlight" : null;

                int badgeInt = int.Parse(badge);
                detailedTable.AddRow(
                    cssClass,
                    badge,
                    FormatDifferenceMoney(expected.BeforeAmount, actual.BeforeAmount),
                    FormatDifferenceMoney(expected.BeforeVested, actual.BeforeVested),
                    FormatDifference(expected.BeforeYears, actual.BeforeYears),
                    FormatDifference(expected.BeforeEnroll, actual.BeforeEnroll),
                    FormatDifferenceMoney(expected.AfterAmount, actual.AfterAmount),
                    FormatDifferenceMoney(expected.AfterVested, actual.AfterVested),
                    FormatDifference(expected.AfterYears, actual.AfterYears),
                    FormatDifference(expected.AfterEnroll, actual.AfterEnroll),
                    FormatDifferenceMoney(readyEtvaByBadge[badgeInt], smartEtvaByBadge[badgeInt])
                );
            }

            TestOutputHelper.WriteLine(detailedTable.ToString());

            // Prepare summary statistics
            Dictionary<string, string> summaryStats = new()
            {
                { "Total Discrepancies", comparisons.Count.ToString("N0") },
                { "After Vested Differences Count", afterVestedDifferenceCount.ToString("N0") },
                { "Sum of After Vested Differences (Smart - Ready)", $"${totalAfterVestedDifference:N2}" }
            };

#if false
            // Save HTML version
            string title = $"PAY450 Discrepancies - Count of {comparisons.Count} of {intersection.Count:N0} considered";
            string outputfile = "/Users/robertherrmann/Desktop/demos/sprint-38/update-summary-discrepancies.html";
            detailedTable.SaveAsHtml(outputfile, title, summaryStats);
            TestOutputHelper.WriteLine($"HTML report saved to: {outputfile}");

#endif
            TestOutputHelper.WriteLine($"Sum of After Vested Differences (Smart - Ready): ${totalAfterVestedDifference:N2} across {afterVestedDifferenceCount} records");

            if (sortedComparisons.Count > 30)
            {
                TestOutputHelper.WriteLine($"... and {sortedComparisons.Count - 30} more discrepancies");
            }
        }

        comparisons.Count.ShouldBeLessThan(20);
    }

    private Pay450Record Normalize(HashSet<string> badgeAndStore, Pay450Record p)
    {
        return p with
        {
            BeforeAmount = Math.Round(p.BeforeAmount, 2),
            BeforeVested = Math.Round(p.BeforeVested, 2),
            BeforeYears = badgeAndStore.Contains(p.BadgeAndStore) ? 0 : IfNullThenZero(p.BeforeYears),
            BeforeEnroll = IfNullThenZero(p.BeforeEnroll),
            AfterAmount = Math.Round(p.AfterAmount, 2),
            AfterVested = Math.Round(p.AfterVested, 2),
            AfterYears = badgeAndStore.Contains(p.BadgeAndStore) ? 0 : IfNullThenZero(p.AfterYears),
            AfterEnroll = IfNullThenZero(p.AfterEnroll)
        };
    }


    private static int? IfNullThenZero(int? x)
    {
        return x ?? 0;
    }

    private void CountBreakdown(string system, List<Pay450Record> p1)
    {
        int bene = 0;
        int employee = 0;
        foreach (Pay450Record p in p1)
        {
            if (p.BadgeAndStore.Contains(' '))
            {
                employee++;
            }
            else
            {
                bene++;
            }
        }

        TestOutputHelper.WriteLine($"{system} Bene: {bene,4} Empl: {employee,6} Total: {p1.Count}");
    }


    private async Task<List<Pay450Record>> GetSmartRecords()
    {
        // R24-PAY450 golden file is for profit year 2025 (see file header)
        ProfitYearRequest pyr = new() { ProfitYear = TestConstants.OpenProfitYear, Take = int.MaxValue, Skip = 0 };
        UpdateSummaryReportResponse r = await _frozenReportService.GetUpdateSummaryReport(pyr, CancellationToken.None);

        List<Pay450Record> records = [];
        foreach (UpdateSummaryReportDetail rr in r.Response.Results)
        {
            records.Add(new Pay450Record
            {
                BadgeAndStore = rr.IsEmployee
                    ? $"0{rr.BadgeNumber} {rr.StoreNumber:000}"
                    : $"0{+rr.BadgeNumber}",
                Name = rr.FullName,
                BeforeAmount = rr.Before.ProfitSharingAmount,
                BeforeVested = rr.Before.VestedProfitSharingAmount,
                BeforeYears = rr.Before.YearsInPlan,
                BeforeEnroll = rr.Before.EnrollmentId,
                AfterAmount = rr.After.ProfitSharingAmount,
                AfterVested = rr.After.VestedProfitSharingAmount,
                AfterYears = rr.After.YearsInPlan,
                AfterEnroll = rr.After.EnrollmentId
            });
        }

        return records;
    }

    public static List<Pay450Record> GetReadyRecords()
    {
        string expectedReport = ReadEmbeddedResource(".golden.R24-PAY450");

        List<string> lines = expectedReport.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

        List<Pay450Record> records = [];

        foreach (string line in lines)
        {
            if (!line.StartsWith("07"))
            {
                continue;
            }

            Pay450Record record = Pay450ReportParser.ParseLine(line);

            records.Add(record);
        }

        return records;
    }

    private Task<Dictionary<int, decimal>> GetSmartEtvaByBadge(List<int> badges)
    {
        return DbFactory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.PayProfits
                .Include(pp => pp.Demographic)
                .Where(pp => pp.ProfitYear == TestConstants.OpenProfitYear && badges.Contains(pp.Demographic!.BadgeNumber))
                .ToDictionaryAsync(pp => pp.Demographic!.BadgeNumber, pp => pp.Etva, CancellationToken.None);
        });
    }

    public async Task<string> VestDetailsBadgeSimple(long badge, short profitYear)
    {
        DateOnly fiscalEndDate = (await CalendarService.GetYearStartAndEndAccountingDatesAsync(profitYear)).FiscalEndDate;
        return await DbFactory.UseReadOnlyContext(async ctx => await VestDetailsBadge(ctx, badge, profitYear, fiscalEndDate));
    }

    private async Task<string> VestDetailsBadge(ProfitSharingReadOnlyDbContext ctx, long badge, short profitYear, DateOnly fromDateTime)
    {
        try
        {
            PayProfit pp = await ctx.PayProfits.Include(d => d.Demographic).Where(pp => pp.ProfitYear == profitYear && pp.Demographic!.BadgeNumber == badge)
                .SingleAsync(CancellationToken.None);
            ParticipantTotalYear? pty = await TotalService.GetYearsOfService(ctx, profitYear, fromDateTime).Where(pty => pty.Ssn == pp.Demographic!.Ssn)
                .SingleOrDefaultAsync(CancellationToken.None);
            byte years = pty?.Years ?? 0;
            int age = fromDateTime.Year - pp.Demographic!.DateOfBirth.Year;
            if (fromDateTime < pp.Demographic.DateOfBirth)
            {
                age--;
            }

            short initContYear = await ctx.ProfitDetails.Where(pd => pd.Ssn == pp.Demographic!.Ssn).MinAsync(pd => pd.ProfitYear);

            decimal has2021ClassActionSum = await ctx.ProfitDetails
                .Where(pd => pd.Ssn == pp.Demographic!.Ssn && pd.ProfitYear == 2021 && pd.CommentType == CommentType.Constants.ClassAction).SumAsync(pd => pd.Earnings);

            int enrollmentId = pp.VestingScheduleId == 0
                ? EnrollmentConstants.NotEnrolled
                : pp.HasForfeited
                    ? (pp.VestingScheduleId == VestingSchedule.Constants.OldPlan
                        ? EnrollmentConstants.OldVestingPlanHasForfeitureRecords
                        : EnrollmentConstants.NewVestingPlanHasForfeitureRecords)
                    : (pp.VestingScheduleId == VestingSchedule.Constants.OldPlan
                        ? EnrollmentConstants.OldVestingPlanHasContributions
                        : EnrollmentConstants.NewVestingPlanHasContributions);

            return GetRow(
                has2021ClassActionSum,
                profitYear,
                age,
                enrollmentId,
                pp.CurrentHoursYear,
                pp.HoursExecutive,
                pp.Demographic.TerminationDate,
                pp.Demographic.TerminationCodeId?.ToString() ?? "",
                years,
                pp.ZeroContributionReasonId?.ToString() ?? " ",
                initContYear,
                pp.Etva,
                pp.Demographic!.Ssn
            );
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private static string GetRow(
        decimal has2021ClassActionSum, int profitYear, int age, int enrollmentId, decimal currentHoursYear,
        decimal hoursExecutive, DateOnly? terminationDate, string terminationCodeId, int years,
        string zeroContributionReasonId, int initContYear, decimal etva, int ssn)
    {
        return string.Format(
            "{0,13:C}  {1,4} {2,3} {3,12} {4,7:0.00} {5,7:0.00} {6,10} {7,-8} {8,5} {9,8} {10,8} {11,6:0.00} {12}",
            has2021ClassActionSum,
            profitYear,
            age,
            enrollmentId,
            currentHoursYear,
            hoursExecutive,
            terminationDate?.ToString("M/d/yyyy") ?? "",
            terminationCodeId ?? "",
            years,
            zeroContributionReasonId,
            initContYear,
            etva,
            ssn
        );
    }

    private static string ExtractBadge(string badgeAndStore)
    {
        // BadgeAndStore format is like "0705804 063" or "0700549" (beneficiary)
        // We want to extract just the badge number (first part before space) and remove leading zero
        int spaceIndex = badgeAndStore.IndexOf(' ');
        string badge = spaceIndex > 0
            ? badgeAndStore.Substring(0, spaceIndex)
            : badgeAndStore;

        // Remove leading zero
        return badge.TrimStart('0');
    }

    private static string FormatDifference(int? ready, int? smart)
    {
        if (ready == smart)
        {
            return ready?.ToString() ?? "";
        }

        return $"{ready} → {smart}";
    }

    private static string FormatDifferenceMoney(decimal ready, decimal smart)
    {
        if (Math.Abs(ready - smart) < 0.01m)
        {
            return ready == 0 ? "" : ready.ToString("N2");
        }

        return $"{ready:N2} → {smart:N2}";
    }
}
