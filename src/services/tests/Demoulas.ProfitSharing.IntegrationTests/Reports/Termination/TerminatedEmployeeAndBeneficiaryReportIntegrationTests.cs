using System.Diagnostics;
using System.Reflection;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

public class TerminatedEmployeeAndBeneficiaryReportIntegrationTests : PristineBaseTest
{
    public TerminatedEmployeeAndBeneficiaryReportIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    public async Task<string> CreateTextReport()
    {
        // These are arguments to the program/rest endpoint
        // Plan admin may choose a range of dates (ie. Q2 ?)
        short profitSharingYear = 2025;
        DateOnly startDate = new(2025, 01, 4);
        DateOnly endDate = new(2025, 12, 27);
        DateOnly effectiveDateOfTestData = new(2025, 11, 04);

        ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            _ = builder
                .SetMinimumLevel(LogLevel.Debug)
                .AddConsole();
        });

        TerminatedEmployeeService mockService =
            new(DbFactory, TotalService, DemographicReaderService, _loggerFactory.CreateLogger<TerminatedEmployeeReportService>(), CalendarService, YearEndService);

        TerminatedEmployeeAndBeneficiaryResponse data = await mockService.GetReportAsync(
            new StartAndEndDateRequest { SortBy = "Name", BeginningDate = startDate, EndingDate = endDate, Take = int.MaxValue },
            CancellationToken.None);

        // READY sorts with "Mc" coming after "MC"
        List<TerminatedEmployeeAndBeneficiaryDataResponseDto> sortedResults = data.Response.Results.OrderBy(x => x.Name, StringComparer.Ordinal).ToList();

        data = data with { Response = data.Response with { Results = sortedResults } };

        string actualText = CreateTextReport(effectiveDateOfTestData, startDate, endDate, profitSharingYear, data);
        return actualText;
    }

    [Fact]
    public async Task CompareMemberListsBetweenReadyAndSmart()
    {
        // Generate SMART report
        string smartReport = await CreateTextReport();
        smartReport.ShouldNotBeNullOrEmpty();

        // Load READY report
        string readyReport = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R3-QPAY066");

        // Parse records from both reports
        List<QPay066Record> readyRecords = QPay066ReportParser.ParseRecords(readyReport);
        List<QPay066Record> smartRecords = QPay066ReportParser.ParseRecords(smartReport);

        // Create sets of badge/PSN + name for comparison
        var readyMembers = readyRecords
            .Select(r => new { BadgePsn = r.PsnSuffix > 0 ? $"{r.BadgeNumber}{r.PsnSuffix:D4}" : r.BadgeNumber.ToString(), r.EmployeeName, r.BadgeNumber, r.PsnSuffix })
            .ToHashSet();

        var smartMembers = smartRecords
            .Select(r => new { BadgePsn = r.PsnSuffix > 0 ? $"{r.BadgeNumber}{r.PsnSuffix:D4}" : r.BadgeNumber.ToString(), r.EmployeeName, r.BadgeNumber, r.PsnSuffix })
            .ToHashSet();

        // Find members only in READY
        var onlyInReady = readyMembers.ExceptBy(smartMembers.Select(m => m.BadgePsn), m => m.BadgePsn).OrderBy(m => m.EmployeeName).ToList();

        // Find members only in SMART
        var onlyInSmart = smartMembers.ExceptBy(readyMembers.Select(m => m.BadgePsn), m => m.BadgePsn).OrderBy(m => m.EmployeeName).ToList();

        // Report differences
        TestOutputHelper.WriteLine($"READY report: {readyRecords.Count} records");
        TestOutputHelper.WriteLine($"SMART report: {smartRecords.Count} records");
        TestOutputHelper.WriteLine($"Members only in READY: {onlyInReady.Count}");
        TestOutputHelper.WriteLine($"Members only in SMART: {onlyInSmart.Count}");
        TestOutputHelper.WriteLine("");

        if (onlyInReady.Count > 0)
        {
            TestOutputHelper.WriteLine("=== Members ONLY in READY (missing from SMART) ===");
            AsciiTable readyOnlyTable = new("Badge/PSN", "Name", "Type");
            foreach (var member in onlyInReady)
            {
                string type = member.PsnSuffix > 0 ? "Beneficiary" : "Employee";
                readyOnlyTable.Add(member.BadgePsn, member.EmployeeName, type);
            }

            TestOutputHelper.WriteLine(readyOnlyTable.ToString());
            TestOutputHelper.WriteLine("");
        }

        if (onlyInSmart.Count > 0)
        {
            TestOutputHelper.WriteLine("=== Members ONLY in SMART (missing from READY) ===");
            AsciiTable smartOnlyTable = new("Badge/PSN", "Name", "Type");
            foreach (var member in onlyInSmart)
            {
                string type = member.PsnSuffix > 0 ? "Beneficiary" : "Employee";
                smartOnlyTable.Add(member.BadgePsn, member.EmployeeName, type);
            }

            TestOutputHelper.WriteLine(smartOnlyTable.ToString());
        }

        // This assertion will fail if there are differences, showing the counts
        (onlyInReady.Count, onlyInSmart.Count).ShouldBe((0, 0),
            $"Member lists differ: {onlyInReady.Count} only in READY, {onlyInSmart.Count} only in SMART");
    }

    [Fact]
    public async Task EnsureSmartReportMatchesReadyReport()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        string actualText = await CreateTextReport();
        TestOutputHelper.WriteLine($"Took: {stopwatch.ElapsedMilliseconds} To create SMART QPAY066 report");
        actualText.ShouldNotBeNullOrEmpty();
        QPay066Report a = new(actualText);

        string expectedText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R3-QPAY066");
        QPay066Report e = new(expectedText);

        // Lets check the totals.
        a.ShouldBeEquivalentTo(e);

        // Uncomment to for the Visual diff
        // ProfitShareUpdateTests.AssertReportsAreEquivalent(expectedText, actualText)
        
        Dictionary<long, QPay066Record> expectedMap = AsDict(QPay066ReportParser.ParseRecords(expectedText));
        Dictionary<long, QPay066Record> actualMap = AsDict(QPay066ReportParser.ParseRecords(actualText));
        actualMap.Keys.ShouldBe(expectedMap.Keys, true);

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

                if (fieldDiffs.Count == 1 && vestedBalanceDiff && readyVestingData.TryGetValue(expected.BadgeNumber, out (decimal VestedBalance, decimal VestedPercent) rreadyData) && actual.VestedBalance == decimal.Round(rreadyData.VestedBalance, 2))
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

        string query = @"
            SELECT d.DEM_BADGE, v.VESTED_BALANCE, v.VESTED_PERCENT
            FROM tbherrmann.pscalcview2 v
            INNER JOIN tbherrmann.DEMOGRAPHICS d ON v.SSN = d.DEM_SSN
            WHERE d.DEM_BADGE IN (" + string.Join(",", badges) + ")";

        Dictionary<int, (decimal, decimal)> result = new();

        await using OracleCommand command = new(query, connection);
        await using OracleDataReader? reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int badge = reader.GetInt32(0);
            decimal vestedBalance = reader.GetDecimal(1);
            decimal vestedPercent = reader.GetDecimal(2);
            result[badge] = (vestedBalance, vestedPercent);
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
