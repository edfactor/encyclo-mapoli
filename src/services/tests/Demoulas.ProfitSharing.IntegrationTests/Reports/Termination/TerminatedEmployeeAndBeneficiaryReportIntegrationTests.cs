using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY443;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

[SuppressMessage("AsyncUsage", "AsyncFixer01:Unnecessary async/await usage")]
public class TerminatedEmployeeAndBeneficiaryReportIntegrationTests : PristineBaseTest
{
    public TerminatedEmployeeAndBeneficiaryReportIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    public async Task<string> CreateTextReport(DateOnly effectiveDateOfTestData)
    {
        // These are arguments to the program/rest endpoint
        // Plan admin may choose a range of dates (ie. Q2 ?)
        short profitSharingYear = 2025;
        DateOnly startDate = new(2024, 01, 01);
        DateOnly endDate = new(2024, 12, 31);

        // Set minimum level to Warning to avoid noisy logs during tests
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Warning));

        TerminatedEmployeeService mockService =
            new(DbFactory, TotalService, DemographicReaderService, loggerFactory, CalendarService, YearEndService, TimeProvider);

        TerminatedEmployeeAndBeneficiaryResponse data = await mockService.GetReportAsync(
            new FilterableStartAndEndDateRequest { SortBy = "Name", BeginningDate = startDate, EndingDate = endDate, Take = int.MaxValue },
            CancellationToken.None);

        // READY sorts with "Mc" coming after "MC"
        List<TerminatedEmployeeAndBeneficiaryDataResponseDto> sortedAndMiddleInitialStripped =
            data.Response.Results
                .Select(x => x with { Name = Pay443Tests.RemoveMiddleInitial(x.Name) }).OrderBy(x => x.Name?.Trim(), StringComparer.Ordinal)
                .ThenBy(x => x.PSN)
                .ToList();

        data = data with { Response = data.Response with { Results = sortedAndMiddleInitialStripped } };

        string actualText = CreateTextReport(effectiveDateOfTestData, startDate, endDate, profitSharingYear, data);
        // Convert to Windows-style line endings
        actualText = actualText.Replace("\r\n", "\n").Replace("\n", "\r\n");
        return actualText;
    }

    // This is used to validate the Vesting query informally
    [Fact]
    public async Task Validate2024VestedBalance()
    {
        var ssn = BadgeToSsn(700113);
        short lastCompletedYearEnd = 2024;
        var cancellationToken = CancellationToken.None;

        CalendarResponseDto priorYearDateRange = await CalendarService.GetYearStartAndEndAccountingDatesAsync(lastCompletedYearEnd, cancellationToken);

        await DbFactory.UseReadOnlyContext(async ctx =>
        {
            var tvb = await TotalService
                .TotalVestingBalance(ctx, /*PayProfit Year*/lastCompletedYearEnd, /*Desired Year*/lastCompletedYearEnd, priorYearDateRange.FiscalEndDate)
                .Where(x => x.Ssn == ssn)
                .ToListAsync<ParticipantTotalVestingBalance>();

            TestOutputHelper.WriteLine(JsonSerializer.Serialize(tvb));

            tvb[0].VestedBalance.ShouldBe(422.06m);
        });
    }

    [Fact]
    public async Task EnsureSmartReportMatchesReadyReport()
    {
        string expectedText = ReadEmbeddedResource(".golden.R03-QPAY066");
        DateOnly reportPrintedDate = QPay066ReportParser.ParseReportDate(expectedText);

        Stopwatch stopwatch = Stopwatch.StartNew();
        string actualText = await CreateTextReport(reportPrintedDate);
        TestOutputHelper.WriteLine($"Took: {stopwatch.ElapsedMilliseconds} To create SMART QPAY066 report");
        actualText.ShouldNotBeNullOrEmpty();
        QPay066Report a = new(actualText);

        QPay066Report e = new(expectedText);

        // Lets check the totals.
        a.ShouldBeEquivalentTo(e);

        // Toggle > to enable display of site by side diff
        if (DateTime.Now.Year < 2024)
        {
            ProfitShareUpdateTests.AssertReportsAreEquivalent(expectedText, actualText);
        }

        Dictionary<long, QPay066Record> expectedMap = AsDict(QPay066ReportParser.ParseRecords(expectedText));
        Dictionary<long, QPay066Record> actualMap = AsDict(QPay066ReportParser.ParseRecords(actualText));
        actualMap.Count.ShouldBeEquivalentTo(expectedMap.Count,
            $"We dont have the same number of employees in the two reports. SMART {actualMap.Count}, READY {expectedMap.Count}");
        actualMap.Keys.ShouldBe(expectedMap.Keys, true, "The reports dont have the same set of PSN/Badges.");

        // Get employee badges (not beneficiaries) to query READY authoritative data
        HashSet<int> employeeBadges = expectedMap.Values
            .Where(r => r.PsnSuffix == 0)
            .Select(r => r.BadgeNumber)
            .ToHashSet();

        // Query READY pscalcview2 for authoritative vested balance and percent
        Dictionary<int, (decimal VestedBalance, decimal VestedPercent)> readyVestingData =
            await GetReadyVestedBalancesByBadge(employeeBadges, DbFactory.ConnectionString);

        // Compare the values for each matching key
        List<string> differences = new();
        List<string> acceptedDifferences = new();

        foreach (long key in expectedMap.Keys)
        {
            QPay066Record expected = expectedMap[key];
            QPay066Record actual = actualMap[key];

            List<string> fieldDiffs = new();
            bool vestedBalanceDiff = false;
            bool vestedPercentDiff = false;

            if (actual.BeginningBalance != expected.BeginningBalance)
            {
                fieldDiffs.Add($"BeginningBalance {actual.BeginningBalance}/{expected.BeginningBalance}");
            }

            if (actual.BeneficiaryAllocation != expected.BeneficiaryAllocation)
            {
                fieldDiffs.Add($"BeneficiaryAllocation {actual.BeneficiaryAllocation}/{expected.BeneficiaryAllocation}");
            }

            if (actual.DistributionAmount != expected.DistributionAmount)
            {
                fieldDiffs.Add($"DistributionAmount {actual.DistributionAmount}/{expected.DistributionAmount}");
            }

            if (actual.Forfeit != expected.Forfeit)
            {
                fieldDiffs.Add($"Forfeit {actual.Forfeit}/{expected.Forfeit}");
            }

            if (actual.EndingBalance != expected.EndingBalance)
            {
                fieldDiffs.Add($"EndingBalance {actual.EndingBalance}/{expected.EndingBalance}");
            }

            if (actual.VestedBalance != expected.VestedBalance)
            {
                vestedBalanceDiff = true;
                fieldDiffs.Add($"VestedBalance {actual.VestedBalance}/{expected.VestedBalance}");
            }

            if (actual.DateTerm != expected.DateTerm)
            {
                fieldDiffs.Add($"DateTerm {actual.DateTerm}/{expected.DateTerm}");
            }

            if (actual.YtdVstPsHours != expected.YtdVstPsHours)
            {
                fieldDiffs.Add($"YtdVstPsHours {actual.YtdVstPsHours}/{expected.YtdVstPsHours}");
            }

            if (actual.VestedPercent != expected.VestedPercent)
            {
                vestedPercentDiff = true;
                fieldDiffs.Add($"VestedPercent {actual.VestedPercent}/{expected.VestedPercent}");
            }

            if (actual.Age != expected.Age)
            {
                fieldDiffs.Add($"Age {actual.Age}/{expected.Age}");
            }

            if (actual.EnrollmentCode != expected.EnrollmentCode)
            {
                fieldDiffs.Add($"EnrollmentCode {actual.EnrollmentCode}/{expected.EnrollmentCode}");
            }

            if (fieldDiffs.Count > 0)
            {
                string psnDisplay = expected.PsnSuffix > 0 ? $"{expected.BadgeNumber:D6}{expected.PsnSuffix:D4}" : expected.BadgeNumber.ToString();

                // Check if ONLY VestedBalance and VestedPercent differ, and if SMART matches READY pscalcview2
                bool smartMatchesReady = false;

                bool onlyVestingDiffs = fieldDiffs.Count == 2 && vestedBalanceDiff && vestedPercentDiff;
                if (onlyVestingDiffs && expected.PsnSuffix == 0 && readyVestingData.TryGetValue(expected.BadgeNumber, out (decimal VestedBalance, decimal VestedPercent) readyData))
                {
                    // Check if SMART values match READY pscalcview2 (authoritative)
                    if (actual.VestedBalance == decimal.Round(readyData.VestedBalance, 2) &&
                        actual.VestedPercent == readyData.VestedPercent * 100)
                    {
                        smartMatchesReady = true;
                        acceptedDifferences.Add($"PSN: {psnDisplay} ({expected.EmployeeName})  {string.Join(", ", fieldDiffs)} [SMART matches READY - OK]");
                    }

                    // If the balance is 0, then ignore the percent
                    if (actual.VestedBalance == 0 && decimal.Round(readyData.VestedBalance, 2) == 0)
                    {
                        smartMatchesReady = true;
                        acceptedDifferences.Add($"PSN: {psnDisplay} ({expected.EmployeeName})  {string.Join(", ", fieldDiffs)} [SMART matches READY - OK]");
                    }
                }

                if (fieldDiffs.Count == 1 && vestedBalanceDiff &&
                    readyVestingData.TryGetValue(expected.BadgeNumber, out (decimal VestedBalance, decimal VestedPercent) rreadyData) &&
                    actual.VestedBalance == decimal.Round(rreadyData.VestedBalance, 2))
                {
                    smartMatchesReady = true;
                    acceptedDifferences.Add($"PSN: {psnDisplay} ({expected.EmployeeName})  {string.Join(", ", fieldDiffs)} [SMART matches READY - OK]");
                }

                if (!smartMatchesReady)
                {
                    differences.Add($"PSN: {psnDisplay} ({expected.EmployeeName})  {string.Join(", ", fieldDiffs)}");
                }
            }
        }

        if (acceptedDifferences.Count > 0)
        {
            TestOutputHelper.WriteLine($"\n=== Accepted Differences (SMART matches READY pscalcview2): {acceptedDifferences.Count} records    ACTUAL / EXPECTED  ===");
        }

        if (differences.Count > 0)
        {
            TestOutputHelper.WriteLine($"\n=== Field Mismatches: {differences.Count} records ===");
            foreach (string diff in differences)
            {
                TestOutputHelper.WriteLine(diff);
            }
        }

        differences.Count.ShouldBe(0, $"{differences.Count} records have field mismatches");
    }

    private static Dictionary<long, QPay066Record> AsDict(List<QPay066Record> parseRecords)
    {
        return parseRecords
            .ToDictionary(
                qp => qp.PsnSuffix == 0
                    ? qp.BadgeNumber
                    : (long)qp.BadgeNumber * 10_000 + qp.PsnSuffix,
                qp => qp);
    }

    /// <summary>
    ///     Queries the READY schema's pscalcview2 view to get vested balances and vested percent for a set of badge numbers.
    /// </summary>
    /// <param name="badges">HashSet of badge numbers to query</param>
    /// <param name="connectionString">Connection string to the database</param>
    /// <returns>Dictionary mapping badge number to (VestedBalance, VestedPercent)</returns>
    private static async Task<Dictionary<int, (decimal VestedBalance, decimal VestedPercent)>> GetReadyVestedBalancesByBadge(
        HashSet<int> badges, string connectionString)
    {
        if (badges.Count == 0)
        {
            return new Dictionary<int, (decimal, decimal)>();
        }

        await using OracleConnection connection = new(connectionString);
        await connection.OpenAsync();

        Dictionary<int, (decimal, decimal)> result = new();

        // Oracle has a limit of 1000 items in an IN clause, so batch the queries
        const int batchSize = 1000;
        var badgeList = badges.ToList();

        for (int i = 0; i < badgeList.Count; i += batchSize)
        {
            var batch = badgeList.Skip(i).Take(batchSize);

            string query = @"
                SELECT d.DEM_BADGE, v.VESTED_BALANCE, v.VESTED_PERCENT
                FROM tbherrmann.pscalcview2 v
                INNER JOIN tbherrmann.DEMOGRAPHICS d ON v.SSN = d.DEM_SSN
                WHERE d.DEM_BADGE IN (" + string.Join(",", batch) + ")";

            await using OracleCommand command = new(query, connection);
            await using OracleDataReader? reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int badge = reader.GetInt32(0);
                decimal vestedBalance = reader.GetDecimal(1);
                decimal vestedPercent = reader.GetDecimal(2);
                result[badge] = (vestedBalance, vestedPercent);
            }
        }

        return result;
    }

    private static string CreateTextReport(DateOnly effectiveDateOfTestData, DateOnly startDate, DateOnly endDate, decimal profitSharingYearWithIteration,
        TerminatedEmployeeAndBeneficiaryResponse report)
    {
        TextReportGenerator textReportGenerator = new(effectiveDateOfTestData, startDate, endDate, profitSharingYearWithIteration);

        foreach (TerminatedEmployeeAndBeneficiaryDataResponseDto ms in report.Response.Results)
        {
            foreach (TerminatedEmployeeAndBeneficiaryYearDetailDto yd in ms.YearDetails)
            {
                textReportGenerator.PrintDetails(ms.PSN, ms.Name, yd.BeginningBalance,
                    yd.BeneficiaryAllocation, yd.DistributionAmount, yd.Forfeit,
                    yd.EndingBalance, yd.VestedBalance, yd.DateTerm, yd.YtdPsHours, yd.VestedPercent, yd.Age,
                    yd.EnrollmentId);
            }
        }

        textReportGenerator.PrintTotals(report.TotalEndingBalance, report.TotalVested, report.TotalForfeit, report.TotalBeneficiaryAllocation);
        return textReportGenerator.GetReport();
    }
}
