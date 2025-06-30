using System.Diagnostics.CodeAnalysis;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Reports;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.UpdateSummary;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
[SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed")]
[SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out")]
public class UpdateSummaryTests : PristineBaseTest
{
    private readonly FrozenReportService _frozenReportService;

    public UpdateSummaryTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _frozenReportService = new FrozenReportService(DbFactory, new LoggerFactory(), TotalService, CalendarService,
            DemographicReaderService, FrozenService);
    }

    [Fact]
    public async Task ValidateReport2()
    {
        List<Pay450Record> ready = GetReadyRecords();
        List<Pay450Record> smart = await GetSmartRecords();

        // smart = smart.Where(s=>Interesting(s)).ToList()
        smart = smart.Where(s => s.BadgeAndStore.Contains(" ") || s.BeforeAmount != 0 || s.AfterAmount != 0).ToList();

        List<Pay450Record> smartOnly = smart.ExceptBy(ready.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();
        List<Pay450Record> readyOnly = ready.ExceptBy(smart.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();
        List<Pay450Record> intersection = smart.IntersectBy(ready.Select(r => r.BadgeAndStore), r => r.BadgeAndStore).ToList();
        Dictionary<string, Pay450Record> readyByBadgeAndStore = ready.ToDictionary(k => k.BadgeAndStore, v => v);


        int uninterestingCount = smart.Count(s => !Interesting(s));

        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine($"intersection {intersection.Count}");
        TestOutputHelper.WriteLine($"READY Only {readyOnly.Count}");
        TestOutputHelper.WriteLine($"SMART Only {smartOnly.Count}");
        TestOutputHelper.WriteLine($"Smart uninteresting {uninterestingCount}");
        TestOutputHelper.WriteLine("");

        Console.WriteLine("SMART ONLY;");

        foreach (Pay450Record s in smartOnly)
        {
            if (Interesting(s))
            {
                TestOutputHelper.WriteLine("Interesting>   " + s);
            }
        }

        TestOutputHelper.WriteLine($"READY Only {readyOnly.Count}");

        foreach (Pay450Record s in readyOnly.Slice(0, 10))
        {
            TestOutputHelper.WriteLine("   " + s);
        }

        int vestDiff = 0;
        foreach (Pay450Record smartInt in intersection)
        {
            Pay450Record readyInt = readyByBadgeAndStore[smartInt.BadgeAndStore];
            if (smartInt != readyInt)
            {
                if (smartInt.BeforeAmount != readyInt.BeforeAmount)
                {
                    throw new AccessViolationException($"READY/SMART BeforeAmount {readyInt.BeforeAmount}/{smartInt.BeforeAmount}");
                }

                if (!SamePenny(smartInt.BeforeVested, readyInt.BeforeVested))
                {
                    TestOutputHelper.WriteLine($"READY/SMART BeforeVested {readyInt.BeforeVested,12:N2}/{smartInt.BeforeVested,12:N2}");
                    vestDiff++;
                }
            }
        }

        TestOutputHelper.WriteLine($"Vest Diff Count: {vestDiff}");

        true.ShouldBeTrue();
    }

    private static bool SamePenny(decimal smartIntBeforeVested, decimal readyIntBeforeVested)
    {
        return Math.Round(smartIntBeforeVested, 2) == Math.Round(readyIntBeforeVested, 2);
    }

    // This is an attempt to suppress records which do not have any changes.
    // BeforeAmount = 0, BeforeVested = 0, BeforeYears = 5, BeforeEnroll = 3, AfterAmount = 0, AfterVested = 0, AfterYears = 5, AfterEnroll = 1 }
    private static bool Interesting(Pay450Record r)
    {
        if (r.BeforeAmount != 0 || r.AfterAmount != 0 || r.BeforeVested != 0 || r.AfterVested != 0)
        {
            return true;
        }

        if (r.BeforeEnroll != r.AfterEnroll)
        {
            return true;
        }

        if (r.BeforeYears != r.AfterYears)
        {
            return true;
        }

        return false;
    }

    private async Task<List<Pay450Record>> GetSmartRecords()

    {
        TestOutputHelper.WriteLine("---------- SMART ----- ");

        ProfitYearRequest pyr = new() { ProfitYear = 2024, Take = int.MaxValue, Skip = 0 };
        UpdateSummaryReportResponse r = await _frozenReportService.GetUpdateSummaryReport(pyr, CancellationToken.None);

        List<Pay450Record> records = [];
        foreach (UpdateSummaryReportDetail rr in r.Response.Results)
        {
            records.Add(new Pay450Record
            {
                BadgeAndStore = rr.IsEmployee ? $"0{rr.BadgeNumber} {rr.StoreNumber:000}" : $"0{+rr.BadgeNumber}",
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

        decimal totalBeforeAmount = 0;
        decimal totalBeforeVested = 0;

        decimal totalAfterAmount = 0;
        decimal totalAfterVested = 0;
        foreach (Pay450Record record in records)
        {
            totalBeforeAmount += record.BeforeAmount;
            totalBeforeVested += record.BeforeVested;
            totalAfterAmount += record.AfterAmount;
            totalAfterVested += record.AfterVested;

            if (isNeg(record.BeforeAmount) || isNeg(record.BeforeVested) || isNeg(record.AfterAmount) || isNeg(record.AfterVested))
            {
                TestOutputHelper.WriteLine($"YIKES, neg amount: {record}");
            }
        }

        TestOutputHelper.WriteLine($"Looked at {records.Count} rows");
        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine($"Total Before Amount: {totalBeforeAmount:N2}");
        TestOutputHelper.WriteLine($"Total Before Vested: {totalBeforeVested:N2}");
        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine($"Total After Amount: {totalAfterAmount:N2}");
        TestOutputHelper.WriteLine($"Total After Vested: {totalAfterVested:N2}");

        return records;
    }


    public List<Pay450Record> GetReadyRecords()
    {
        TestOutputHelper.WriteLine("---------- READY ----- ");

        string expectedReport =
            File.ReadAllText("/Users/robertherrmann/prj/smart-profit-sharing/src/services/tests/Demoulas.ProfitSharing.IntegrationTests/Resources/golden/30-PAY450");

        List<string> lines = expectedReport.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

        List<Pay450Record> records = [];
        decimal totalBeforeAmount = 0;
        decimal totalBeforeVested = 0;

        decimal totalAfterAmount = 0;
        decimal totalAfterVested = 0;

        foreach (string line in lines)
        {
            if (!line.StartsWith("07"))
            {
                continue;
            }

            Pay450Record record = ReportParser.ParseLine(line);

            records.Add(record);

            totalBeforeAmount += record.BeforeAmount;
            totalBeforeVested += record.BeforeVested;
            totalAfterAmount += record.AfterAmount;
            totalAfterVested += record.AfterVested;

            if (isNeg(record.BeforeAmount) || isNeg(record.BeforeVested) || isNeg(record.AfterAmount) || isNeg(record.AfterVested))
            {
                TestOutputHelper.WriteLine($"Neg amount: {record}");
            }
        }

        TestOutputHelper.WriteLine($"Looked at {records.Count} rows");
        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine($"Total Before Amount: {totalBeforeAmount:N2}");
        TestOutputHelper.WriteLine($"Total Before Vested: {totalBeforeVested:N2}");
        TestOutputHelper.WriteLine("");
        TestOutputHelper.WriteLine($"Total After Amount: {totalAfterAmount:N2}");
        TestOutputHelper.WriteLine($"Total After Vested: {totalAfterVested:N2}");

        return records;
    }

    private static bool isNeg(decimal amount)
    {
        return amount < 0;
    }
}
