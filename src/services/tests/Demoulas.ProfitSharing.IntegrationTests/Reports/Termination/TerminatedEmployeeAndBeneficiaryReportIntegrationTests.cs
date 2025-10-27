using System.Diagnostics;
using System.Reflection;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Microsoft.EntityFrameworkCore;
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
        DateOnly effectiveDateOfTestData = new(2025, 10, 24);

        ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            _ = builder
                .SetMinimumLevel(LogLevel.Debug)
                .AddConsole();
        });

        TerminatedEmployeeService mockService =
            new(DbFactory, TotalService, DemographicReaderService, _loggerFactory.CreateLogger<TerminatedEmployeeReportService>(), CalendarService);

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
        // Fetch Master Inquiry Data
        string content = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.MasterInquiry.22Oct2025.outfl");
        List<OutFL> outties = OutFLParser.ParseStringIntoRecords(content);
        Dictionary<long, OutFL> outtieBySsn = outties.ToDictionary(o => long.Parse(o.OUT_SSN), o => o);

        Stopwatch stopwatch = Stopwatch.StartNew();
        string actualText = await CreateTextReport();
        TestOutputHelper.WriteLine($"Took: {stopwatch.ElapsedMilliseconds} To create SMART QPAY066 report");
        actualText.ShouldNotBeNullOrEmpty();
        QPay066Report a = new(actualText);

        string expectedText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R3-QPAY066");
        QPay066Report e = new(expectedText);

        // Lets check the totals.
        a.ShouldBeEquivalentTo(e);

        // Flip == to !=  to see visual diff vs report 
        if (actualText.Length != 0)
        {
            //  ---- Use this for the Visual diff
            ProfitShareUpdateTests.AssertReportsAreEquivalent(expectedText, actualText);
        }
        else
        {
            // ----  Use this for the text report showing differences

            Dictionary<int, decimal> expectedMap = QPay066ReportParser.ParseBadgeToVestedBalance(expectedText);
            Dictionary<int, decimal> actualMap = QPay066ReportParser.ParseBadgeToVestedBalance(actualText);
            expectedMap.Count.ShouldBe(actualMap.Count);
            TestOutputHelper.WriteLine($"Total rows {actualMap.Count} (Benes are not parsed)");

            // Find mismatches
            List<(int Badge, decimal Ready, decimal Smart)> mismatches = new();
            foreach (KeyValuePair<int, decimal> kvp in expectedMap)
            {
                if (actualMap.TryGetValue(kvp.Key, out decimal actualValue) && kvp.Value != actualValue)
                {
                    mismatches.Add((kvp.Key, kvp.Value, actualValue));
                }
            }

            Dictionary<int, (int Ssn, decimal hours)> badge2Details = await
                DbFactory.UseReadOnlyContext(async ctx => await ctx.Demographics.Join(ctx.PayProfits, d => d.Id, pp => pp.DemographicId, (d, p) => new { d, p })
                    .Where(d => actualMap.Keys.Contains(d.d.BadgeNumber) && d.p.ProfitYear == 2025)
                    .ToDictionaryAsync(d => d.d.BadgeNumber, d => (d.d.Ssn, d.p.CurrentHoursYear + d.p.HoursExecutive)));

            TestOutputHelper.WriteLine($"\nMismatches: {mismatches.Count}");
            if (mismatches.Count > 0)
            {
                Dictionary<int, decimal> readyBalancesPsCalcView = await GetReadyVestedBalancesByBadge(mismatches.Select(f => f.Badge).ToHashSet(), DbFactory.ConnectionString);

                AsciiTable table = new("BADGE", "READY QPAY066", "SMART QPAY066", "READY MstrInqry", "READY MstrInqry Hours", "SMART QPAY066 and READY MstrInqry agree?");

                foreach ((int Badge, decimal Ready, decimal Smart) mismatch in mismatches)
                {
                    OutFL masterInquiryData = outtieBySsn[badge2Details[mismatch.Badge].Ssn];
                    decimal? masterInquiryVestedBalance = masterInquiryData.OUT_VESTING_AMT; // CURRENT VESTING AMOUNT

                    // We ignore QPAY066 in favor of the MasterInquiry as being authoritative
                    if (mismatch.Smart == masterInquiryVestedBalance)
                    {
                        continue;
                    }

                    decimal horus = badge2Details[mismatch.Badge].hours;

                    table.Add(
                        mismatch.Badge,
                        mismatch.Ready,
                        mismatch.Smart,
                        masterInquiryVestedBalance,
                        horus,
                        mismatch.Smart == masterInquiryVestedBalance ? "Y" : "<------ No"
                    );
                }

                TestOutputHelper.WriteLine(table.ToString());
                table.RowCount.ShouldBeLessThan(3); // Is there a problem in THA008-10 ?
            }
        }
    }

    public static string ReadEmbeddedResource(string resourceName)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using StreamReader reader = new(stream!);
        return reader.ReadToEnd();
    }


    /// <summary>
    ///     Queries the READY schema's pscalcview2 view to get vested balances for a set of badge numbers.
    /// </summary>
    /// <param name="badges">HashSet of badge numbers to query</param>
    /// <param name="connectionString">Connection string to the database</param>
    /// <returns>Dictionary mapping badge number to vested balance</returns>
    private static async Task<Dictionary<int, decimal>> GetReadyVestedBalancesByBadge(HashSet<int> badges, string connectionString)
    {
        if (badges == null || badges.Count == 0)
        {
            return new Dictionary<int, decimal>();
        }

        await using OracleConnection connection = new(connectionString);
        await connection.OpenAsync();

        // Query the READY schema's pscalcview2 view
        // This view returns SSN, VESTED_PERCENT, TOTAL_BALANCE, VESTED_BALANCE
        string query = @"
            SELECT d.DEM_BADGE, v.VESTED_BALANCE 
            FROM tbherrmann.pscalcview2 v
            INNER JOIN tbherrmann.DEMOGRAPHICS d ON v.SSN = d.DEM_SSN
            WHERE d.DEM_BADGE IN (" + string.Join(",", badges) + ")";

        Dictionary<int, decimal> result = new();

        await using OracleCommand command = new(query, connection);
        await using OracleDataReader? reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int badge = reader.GetInt32(0);
            decimal vestedBalance = reader.GetDecimal(1);
            result[badge] = vestedBalance;
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
                textReportGenerator.PrintDetails(ms.BadgePSn, ms.Name, yd.BeginningBalance,
                    yd.BeneficiaryAllocation, yd.DistributionAmount, yd.Forfeit,
                    yd.EndingBalance, yd.VestedBalance, yd.DateTerm, yd.YtdPsHours, yd.VestedPercent, yd.Age,
                    yd.HasForfeited ? (byte)4 : (byte)0);
            }
        }

        textReportGenerator.PrintTotals(report.TotalEndingBalance, report.TotalVested, report.TotalForfeit, report.TotalBeneficiaryAllocation);
        return textReportGenerator.GetReport();
    }
}
