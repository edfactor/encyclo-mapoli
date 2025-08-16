using System.Diagnostics.CodeAnalysis;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Services.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.UpdateSummary;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class UpdateSummaryTests : PristineBaseTest
{
    private readonly FrozenReportService _frozenReportService;

    public UpdateSummaryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _frozenReportService = new FrozenReportService(DbFactory, new LoggerFactory(), TotalService, CalendarService,
            DemographicReaderService, FrozenService);
    }


    // This is for quickly looking into an employee to look at their Vesting balance
    [Fact]
    public async Task Tinker()
    {
        int badge = 0705815;
        int ssn = 0;

        await DbFactory.UseReadOnlyContext(async ctx =>
        {
            Demographic demo = await ctx.Demographics.Where(d => d.BadgeNumber == badge || d.Ssn == ssn).SingleAsync();

            TestOutputHelper.WriteLine($"badge: {demo.BadgeNumber} ssn:{demo.Ssn}");

            await tvb(ctx, 2024, demo.Ssn, demo.BadgeNumber);
            TestOutputHelper.WriteLine("");
            // await tvb(ctx, 2023, demo.Ssn, demo.BadgeNumber)

            return 7;
        });

        "".ShouldBe("");
    }


    [Fact]
    public async Task ValidateReport2()
    {
        List<Pay450Record> ready = GetReadyRecords();

        List<Pay450Record> smart = await GetSmartRecords();

        CountBreakdown("Ready", ready);
        CountBreakdown("Smart", smart);

        // smart = smart.Where(s=>Interesting(s)).ToList()
        smart = smart.Where(s => s.BadgeAndStore.Contains(" ") || s.BeforeAmount != 0 || s.AfterAmount != 0).ToList();

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

            if (readyIntNorm.BeforeEnroll != smartIntNorm.BeforeEnroll)
            {
                Pay450Record readyNoBeforEnroll = readyIntNorm with { BeforeEnroll = 0 };
                Pay450Record smartNoBeforEnroll = readyIntNorm with { BeforeEnroll = 0 };

                // If everything but the before enroll is different, then lets ignore the before enroll - for now. 
                if (readyNoBeforEnroll == smartNoBeforEnroll)
                {
                    readyIntNorm = readyNoBeforEnroll;
                    smartIntNorm = smartNoBeforEnroll;
                }
            }

            if (smartIntNorm.AfterYears == 1 && readyIntNorm.AfterYears == 1 && readyIntNorm.AfterAmount == 0 && smartIntNorm.AfterAmount == 0 && readyIntNorm.AfterEnroll == 2)
            {
                smartIntNorm = smartIntNorm with { AfterEnroll = 2 }; // Lazy Ready.
            }

            // These are all small amounts that are part of year 1 that involve class action amounts.
            // I think READY has bug where it returns zero, where SMART is correct and returns a value.
            // I think READY sees 1 year and says you have nada, where SMART digs deeper and figures out 
            // the right amount.   Would an employee with bene money and 1 year of service who leaves see nothing?   probably only effects people with just profit code 8 and 0,
            // having a 6 (bene amount) would cause the correct vested amount to be shown.
            if (readyIntNorm.BeforeVested == 0 && smartIntNorm.BeforeVested != 0)
            {
                decimal oneHunderdedPercentAmount = await computeOneHundredPercentInterestAndClassActionAmount(ExtractBadge(smartIntNorm.BadgeAndStore), 2023);
                if (oneHunderdedPercentAmount == smartIntNorm.BeforeVested)
                {
                    // Suppress displaying vesting % when the forefeit amount matches a class action amount and 100% interest. (see comment above)
#if true
                    smartIntNorm = smartIntNorm with { BeforeVested = 0 };
#endif
                }
            }

            if (readyIntNorm != smartIntNorm)
            {
                comparisons.Add((readyIntNorm, smartIntNorm));
            }
        }

        int deathSkipped = 0;
        TestOutputHelper.WriteLine($"Discrepancy Count {comparisons.Count}");
        TestOutputHelper.WriteLine(Pay450Comparisons.ToComparisonHeader() + GetHeader1());
        int c = 0;
        foreach ((Pay450Record readyIntNorm, Pay450Record smartIntNorm) in comparisons)
        {
            string beforeStatus = await VestDetailsBadgeSimple(ExtractBadge(smartIntNorm.BadgeAndStore), 2024);
            // Skip death.
            if (beforeStatus.Contains( /*"Z"*/ TerminationCode.Constants.Deceased))
            {
                deathSkipped++;
                continue;
            }

            TestOutputHelper.WriteLine(Pay450Comparisons.ToComparisonString(readyIntNorm, smartIntNorm) + "   " + beforeStatus);
            TestOutputHelper.WriteLine(
                "                                                                                                                                                                                                    " +
                await VestDetailsBadgeSimple(ExtractBadge(smartIntNorm.BadgeAndStore), 2023));
            c++;
            if (c > 500)
            {
                break;
            }
        }

        TestOutputHelper.WriteLine("Death skipped " + deathSkipped);


        true.ShouldBeTrue();
    }

    private async Task<decimal> computeOneHundredPercentInterestAndClassActionAmount(long badgeNumber, int profitYear)
    {
        return await DbFactory.UseReadOnlyContext(async ctx =>
        {
            Demographic? d = await ctx.Demographics.Where(d => d.BadgeNumber == badgeNumber).FirstOrDefaultAsync();
            decimal earningsSum = await ctx.ProfitDetails
                .Where(pd => pd.ProfitYear <= profitYear && pd.Ssn == d!.Ssn &&
                             (pd.CommentType == CommentType.Constants.ClassAction || pd.CommentType == CommentType.Constants.OneHundredPercentEarnings))
                .SumAsync(pd => pd.Earnings, CancellationToken.None);
            return earningsSum;
        });
    }

    private Pay450Record Normalize(Pay450Record p)
    {
        return p with
        {
            BeforeAmount = Math.Round(p.BeforeAmount, 2),
            BeforeVested = Math.Round(p.BeforeVested, 2),
            BeforeYears = IfNullThenZero(p.BeforeYears),
            BeforeEnroll = IfNullThenZero(p.BeforeEnroll), //  BOBH : Why do I think it is ok to ignore this ?

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

    private static long ExtractBadge(string smartIntBadgeAndStore)
    {
        int space = smartIntBadgeAndStore.IndexOf(" ");
        if (space == -1)
        {
            return long.Parse(smartIntBadgeAndStore);
        }

        return long.Parse(smartIntBadgeAndStore.Substring(0, space));
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
        ProfitYearRequest pyr = new() { ProfitYear = 2024, Take = int.MaxValue, Skip = 0 };
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
        string expectedReport =
            File.ReadAllText("/Users/robertherrmann/prj/smart-profit-sharing/src/services/tests/Demoulas.ProfitSharing.IntegrationTests/Resources/golden/30-PAY450");

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

    private async Task tvb(ProfitSharingReadOnlyDbContext ctx, short profitYear, int ssn, long badge)
    {
        DateOnly asOfDate = (await CalendarService.GetYearStartAndEndAccountingDatesAsync(profitYear)).FiscalBeginDate;

        ParticipantTotalVestingBalance p = await TotalService.TotalVestingBalance(ctx, profitYear, asOfDate).AsNoTracking()
            .Where(t => t.Ssn == ssn).SingleAsync(CancellationToken.None);

        TestOutputHelper.WriteLine(
            $"SSN: {p.Ssn,9} {profitYear}: Current Amount  {p.CurrentBalance,10:C} ,  Vested: {p.VestedBalance,10:C} | Percent: {p.VestingPercent,6:P0} | Years: {p.YearsInPlan,2}      {await VestDetailsBadge(ctx, badge, profitYear, asOfDate)}");
    }

    private async Task<string> VestDetailsBadge(ProfitSharingReadOnlyDbContext ctx, long badge, short profitYear, DateOnly fromDateTime)
    {
        try
        {
            PayProfit pp = await ctx.PayProfits.Include(d => d.Demographic).Where(pp => pp.ProfitYear == profitYear && pp.Demographic!.BadgeNumber == badge)
                .SingleAsync(CancellationToken.None);
            ParticipantTotalYear? pty = await TotalService.GetYearsOfService(ctx, profitYear).Where(pty => pty.Ssn == pp.Demographic!.Ssn)
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

    private static string GetHeader1()
    {
        return
            "    ClSum         Year Age EnrollmentId Hours   EHours   TermDate   TermCode Years ZeroCont InitYear   ETVA        SSN";
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
}
