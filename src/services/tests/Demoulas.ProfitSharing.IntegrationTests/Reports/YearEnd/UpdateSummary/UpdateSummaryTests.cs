using System.Diagnostics.CodeAnalysis;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY443;
using Demoulas.ProfitSharing.Services.Reports;
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
    public async Task ValidateReport2()
    {
        List<Pay450Record> ready = GetReadyRecords();

        List<Pay450Record> smart = await GetSmartRecords();

        CountBreakdown("Ready", ready);
        CountBreakdown("Smart", smart);

        List<Pay450Record> smartOnly = smart.ExceptBy(ready.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();
        List<Pay450Record> readyOnly = ready.ExceptBy(smart.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();
        List<Pay450Record> intersection = smart.IntersectBy(ready.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();
        Dictionary<string, Pay450Record> readyByBadgeAndStore = ready.ToDictionary(k => k.BadgeAndStore, v => v);

        List<(Pay450Record Actual, Pay450Record Expected)> comparisons = new();

        foreach (Pay450Record smartInt in intersection)
        {
            Pay450Record readyInt = readyByBadgeAndStore[smartInt.BadgeAndStore];
            Pay450Record readyIntNorm = Normalize(readyInt);
            Pay450Record smartIntNorm = Normalize(smartInt);

            if (readyIntNorm != smartIntNorm)
            {
                comparisons.Add((readyIntNorm, smartIntNorm));
            }
        }

        TestOutputHelper.WriteLine($"Discrepancy Count {comparisons.Count}");

        // Categorize discrepancies
        var categorized = CategorizeDiscrepancies(comparisons);

        TestOutputHelper.WriteLine("\n=== DISCREPANCY SUMMARY ===");
        TestOutputHelper.WriteLine($"Total Discrepancies: {comparisons.Count}");
        TestOutputHelper.WriteLine($"  Years of Service Mismatches: {categorized.YearsOfServiceMismatches.Count}");
        TestOutputHelper.WriteLine($"  Negative Vested Amount Issues: {categorized.NegativeVestedIssues.Count}");
        TestOutputHelper.WriteLine($"  Vesting Calculation Issues: {categorized.VestingCalculationIssues.Count}");
        TestOutputHelper.WriteLine($"  Enrollment Mismatches: {categorized.EnrollmentMismatches.Count}");
        TestOutputHelper.WriteLine($"  Other Mismatches: {categorized.OtherMismatches.Count}");

        if (categorized.YearsOfServiceMismatches.Count > 0)
        {
            TestOutputHelper.WriteLine("\n--- Years of Service Mismatches ---");
            foreach (var (actual, expected) in categorized.YearsOfServiceMismatches.Take(5))
            {
                TestOutputHelper.WriteLine($"  {expected.BadgeAndStore} {expected.Name}: " +
                    $"Before: {expected.BeforeYears} vs {actual.BeforeYears}, " +
                    $"After: {expected.AfterYears} vs {actual.AfterYears}");
            }
            if (categorized.YearsOfServiceMismatches.Count > 5)
                TestOutputHelper.WriteLine($"  ... and {categorized.YearsOfServiceMismatches.Count - 5} more");
        }

        if (categorized.NegativeVestedIssues.Count > 0)
        {
            TestOutputHelper.WriteLine("\n--- Negative Vested Amount Issues ---");
            foreach (var (actual, expected) in categorized.NegativeVestedIssues.Take(5))
            {
                TestOutputHelper.WriteLine($"  {expected.BadgeAndStore} {expected.Name}: " +
                    $"Before: Expected {expected.BeforeVested:C} vs Actual {actual.BeforeVested:C}, " +
                    $"After: Expected {expected.AfterVested:C} vs Actual {actual.AfterVested:C}");
            }
            if (categorized.NegativeVestedIssues.Count > 5)
                TestOutputHelper.WriteLine($"  ... and {categorized.NegativeVestedIssues.Count - 5} more");
        }

        if (categorized.VestingCalculationIssues.Count > 0)
        {
            TestOutputHelper.WriteLine("\n--- Vesting Calculation Issues ---");
            foreach (var (actual, expected) in categorized.VestingCalculationIssues)
            {
                TestOutputHelper.WriteLine($"  {expected.BadgeAndStore} {expected.Name}:");
                TestOutputHelper.WriteLine($"    Before: Amt={expected.BeforeAmount:C}, Vested Expected={expected.BeforeVested:C} vs Actual={actual.BeforeVested:C}");
                TestOutputHelper.WriteLine($"    After:  Amt={expected.AfterAmount:C}, Vested Expected={expected.AfterVested:C} vs Actual={actual.AfterVested:C}");
            }
        }

        if (categorized.EnrollmentMismatches.Count > 0)
        {
            TestOutputHelper.WriteLine("\n--- Enrollment Mismatches ---");
            foreach (var (actual, expected) in categorized.EnrollmentMismatches.Take(5))
            {
                TestOutputHelper.WriteLine($"  {expected.BadgeAndStore} {expected.Name}: " +
                    $"Before: {expected.BeforeEnroll} vs {actual.BeforeEnroll}, " +
                    $"After: {expected.AfterEnroll} vs {actual.AfterEnroll}");
            }
            if (categorized.EnrollmentMismatches.Count > 5)
                TestOutputHelper.WriteLine($"  ... and {categorized.EnrollmentMismatches.Count - 5} more");
        }

        int cnt = 0;
        TestOutputHelper.WriteLine("\n=== DETAILED DISCREPANCIES (first 100) ===");
        foreach ((Pay450Record actual, Pay450Record expected) in comparisons)
        {
            TestOutputHelper.WriteLine("Expect "+expected.ToString());
            TestOutputHelper.WriteLine("Actual "+actual.ToString());
            TestOutputHelper.WriteLine("");
            if (cnt++ > 100)
                break;
        }

        comparisons.Count.ShouldBe(0);
    }

    private Pay450Record Normalize(Pay450Record p)
    {
        return p with
        {
            BeforeAmount = Math.Round(p.BeforeAmount, 2),
            BeforeVested = Math.Round(p.BeforeVested, 2),
            BeforeYears = IfNullThenZero(p.BeforeYears),
            BeforeEnroll = IfNullThenZero(p.BeforeEnroll),

            AfterAmount = Math.Round(p.AfterAmount, 2),
            AfterVested = Math.Round(p.AfterVested, 2),
            AfterYears = IfNullThenZero(p.AfterYears),
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
                Name = rr.Name,
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
        string expectedReport = Pay443Tests.ReadEmbeddedResource(".golden.R24-PAY450");

        List<string> lines = expectedReport.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

        List<Pay450Record> records = [];

        foreach (string line in lines)
        {
            if (!line.StartsWith("07"))
            {
                continue;
            }

            Pay450Record record = ReportParser.ParseLine(line);

            records.Add(record);
        }

        return records;
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

            return GetRow(
                has2021ClassActionSum,
                profitYear,
                age,
                pp.EnrollmentId,
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

    private static DiscrepancyCategories CategorizeDiscrepancies(List<(Pay450Record Actual, Pay450Record Expected)> comparisons)
    {
        var result = new DiscrepancyCategories();

        foreach (var (actual, expected) in comparisons)
        {
            bool hasYearsMismatch = actual.BeforeYears != expected.BeforeYears || actual.AfterYears != expected.AfterYears;
            bool hasEnrollmentMismatch = actual.BeforeEnroll != expected.BeforeEnroll || actual.AfterEnroll != expected.AfterEnroll;

            // Negative vested or small vested amount clamping issues
            bool hasNegativeVestedIssue =
                (expected.BeforeVested < 0 && actual.BeforeVested >= 0) ||
                (expected.AfterVested < 0 && actual.AfterVested >= 0) ||
                (expected.BeforeVested > 0 && expected.BeforeVested < 100 && actual.BeforeVested == 0) ||
                (expected.AfterVested > 0 && expected.AfterVested < 100 && actual.AfterVested == 0);

            // Complex vesting calculation issues (large discrepancies in vested amounts)
            bool hasVestingCalculationIssue =
                !hasNegativeVestedIssue &&
                (Math.Abs(expected.BeforeVested - actual.BeforeVested) > 100 ||
                 Math.Abs(expected.AfterVested - actual.AfterVested) > 100);

            // Categorize
            if (hasYearsMismatch && !hasNegativeVestedIssue && !hasVestingCalculationIssue)
            {
                result.YearsOfServiceMismatches.Add((actual, expected));
            }
            else if (hasNegativeVestedIssue)
            {
                result.NegativeVestedIssues.Add((actual, expected));
            }
            else if (hasVestingCalculationIssue)
            {
                result.VestingCalculationIssues.Add((actual, expected));
            }
            else if (hasEnrollmentMismatch)
            {
                result.EnrollmentMismatches.Add((actual, expected));
            }
            else
            {
                result.OtherMismatches.Add((actual, expected));
            }
        }

        return result;
    }

    private sealed class DiscrepancyCategories
    {
        public List<(Pay450Record Actual, Pay450Record Expected)> YearsOfServiceMismatches { get; } = new();
        public List<(Pay450Record Actual, Pay450Record Expected)> NegativeVestedIssues { get; } = new();
        public List<(Pay450Record Actual, Pay450Record Expected)> VestingCalculationIssues { get; } = new();
        public List<(Pay450Record Actual, Pay450Record Expected)> EnrollmentMismatches { get; } = new();
        public List<(Pay450Record Actual, Pay450Record Expected)> OtherMismatches { get; } = new();
    }
}
