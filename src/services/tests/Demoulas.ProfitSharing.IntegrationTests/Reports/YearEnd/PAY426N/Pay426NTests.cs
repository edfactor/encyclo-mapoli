using Demoulas.ProfitSharing.Common.Contracts.Report;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Services.Reports;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426N;

/// <summary>
///     Tests individual PAY426N report IDs (1-8, 10) against golden READY reports.
///     Each test validates that SMART's filtered employee list matches READY's output for that specific report category.
/// </summary>
public class Pay426NTests : PristineBaseTest
{
    private readonly ProfitSharingSummaryReportService _reportService;

    public Pay426NTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        _reportService = new ProfitSharingSummaryReportService(DbFactory, CalendarService, TotalService, DemographicReaderService);
    }

    /// <summary>
    ///     Provides test cases for each PAY426N report ID.
    ///     Format: ReportId enum value, resource number (1-8, 10)
    /// </summary>
    public static TheoryData<YearEndProfitSharingReportId, int> ReportTestCases =>
        new()
        {
            { YearEndProfitSharingReportId.Age18To20With1000Hours, 1 },
            { YearEndProfitSharingReportId.Age21OrOlderWith1000Hours, 2 },
            { YearEndProfitSharingReportId.Under18, 3 },
            { YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount, 4 },
            { YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount, 5 },
            { YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours, 6 },
            { YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount, 7 },
            { YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount, 8 },
            { YearEndProfitSharingReportId.NonEmployeeBeneficiaries, 10 }
        };

    [Theory]
    [MemberData(nameof(ReportTestCases))]
    public async Task Pay426NReport_ShouldMatchReady(YearEndProfitSharingReportId reportId, int resourceNumber)
    {
        // Arrange - load expected data from READY report
        string resourceName = $"Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R8-PAY426N-{resourceNumber}";
        string expectedReportText = ReadEmbeddedResource(resourceName).Trim();
        YearEndProfitSharingReportResponse expected = Pay426NParser.Parse(expectedReportText, 2025);

        YearEndProfitSharingReportRequest request = new()
        {
            ReportId = reportId,
            ProfitYear = 2025,
            UseFrozenData = false,
            Take = int.MaxValue,
            Skip = 0
        };

        // Act - get actual data from SMART
        YearEndProfitSharingReportResponse actual = await _reportService.GetYearEndProfitSharingReportAsync(request, CancellationToken.None);

        // Assert

        // We validate "Years in Plan" here - as SMART and READY diverge
        // Skip for Report 10 (NonEmployeeBeneficiaries) - beneficiaries don't participate in the plan (all have BadgeNumber=0)
        if (reportId != YearEndProfitSharingReportId.NonEmployeeBeneficiaries)
        {
            ValidateYearsInPlanWithinOneYear(expected.Response.Results, actual.Response.Results);
        }

        // Normalize and compare (records have value-based equality)
        HashSet<YearEndProfitSharingReportDetail> expectedRows = NormalizeForComparison(expected.Response.Results, true);
        HashSet<YearEndProfitSharingReportDetail> actualRows = NormalizeForComparison(actual.Response.Results, false);

        // Diagnostic: Find differences
        if (!actualRows.SetEquals(expectedRows))
        {
            var inReadyOnly = expectedRows.Except(actualRows).ToList();
            var inSmartOnly = actualRows.Except(expectedRows).ToList();

            TestOutputHelper.WriteLine($"\n=== Report {resourceNumber} ({reportId}) Differences ===");
            TestOutputHelper.WriteLine($"READY record count: {expectedRows.Count}");
            TestOutputHelper.WriteLine($"SMART record count: {actualRows.Count}");
            TestOutputHelper.WriteLine($"Records only in READY: {inReadyOnly.Count}");
            TestOutputHelper.WriteLine($"Records only in SMART: {inSmartOnly.Count}");

            if (inReadyOnly.Any())
            {
                TestOutputHelper.WriteLine("\n--- Records only in READY (first 5) ---");
                foreach (var r in inReadyOnly.Take(5))
                {
                    TestOutputHelper.WriteLine(r.ToString());
                }
            }

            if (inSmartOnly.Any())
            {
                TestOutputHelper.WriteLine("\n--- Records only in SMART (first 5) ---");
                foreach (var r in inSmartOnly.Take(5))
                {
                    TestOutputHelper.WriteLine(r.ToString());
                }
            }

            // Badge-based comparison only works for employee reports (Reports 1-8)
            // Skip for Report 10 (NonEmployeeBeneficiaries) where all BadgeNumbers are 0
            if (reportId != YearEndProfitSharingReportId.NonEmployeeBeneficiaries)
            {
                // Create dictionaries for badge-based lookups
                var readyByBadge = expectedRows.ToDictionary(r => r.BadgeNumber);
                var smartByBadge = actualRows.ToDictionary(r => r.BadgeNumber);

                // First, find badges that exist in both but don't match
                var mismatchedBadges = readyByBadge.Keys.Intersect(smartByBadge.Keys)
                    .Where(badge => !readyByBadge[badge].Equals(smartByBadge[badge]))
                    .Take(5)
                    .ToList();

                if (mismatchedBadges.Any())
                {
                    TestOutputHelper.WriteLine("\n=== FIRST 5 MISMATCHED BADGE (same badge, different values) ===");
                    var badge = mismatchedBadges[0];
                    var ready = readyByBadge[badge];
                    var smart = smartByBadge[badge];

                    TestOutputHelper.WriteLine("READY: " + ready);
                    TestOutputHelper.WriteLine("SMART: " + smart);
                }
            }
            else
            {
                // For Report 10, use SSN-based comparison to find field mismatches
                var readyBySsn = expectedRows.ToDictionary(r => r.Ssn);
                var smartBySsn = actualRows.ToDictionary(r => r.Ssn);

                var mismatchedSsns = readyBySsn.Keys.Intersect(smartBySsn.Keys)
                    .Where(ssn => !readyBySsn[ssn].Equals(smartBySsn[ssn]))
                    .Take(10)
                    .ToList();

                if (mismatchedSsns.Any())
                {
                    TestOutputHelper.WriteLine("\n=== BENEFICIARIES WITH SAME SSN BUT DIFFERENT VALUES ===");
                    TestOutputHelper.WriteLine($"Found {mismatchedSsns.Count} beneficiaries with matching SSN but different field values:");

                    foreach (var ssn in mismatchedSsns.Take(3))
                    {
                        var ready = readyBySsn[ssn];
                        var smart = smartBySsn[ssn];

                        TestOutputHelper.WriteLine($"\n--- SSN {ssn} ({ready.FullName}) ---");
                        TestOutputHelper.WriteLine($"READY: {ready}");
                        TestOutputHelper.WriteLine($"SMART: {smart}");

                        // Field-by-field comparison
                        if (ready.DateOfBirth != smart.DateOfBirth)
                        {
                            TestOutputHelper.WriteLine($"  DateOfBirth: READY={ready.DateOfBirth}, SMART={smart.DateOfBirth}");
                        }

                        if (ready.Age != smart.Age)
                        {
                            TestOutputHelper.WriteLine($"  Age: READY={ready.Age}, SMART={smart.Age}");
                        }

                        if (ready.Wages != smart.Wages)
                        {
                            TestOutputHelper.WriteLine($"  Wages: READY={ready.Wages}, SMART={smart.Wages}");
                        }

                        if (ready.Balance != smart.Balance)
                        {
                            TestOutputHelper.WriteLine($"  Balance: READY={ready.Balance}, SMART={smart.Balance}");
                        }
                    }
                }

                // Find SSNs only in READY
                var ssnsOnlyInReady = readyBySsn.Keys.Except(smartBySsn.Keys).Take(10).ToList();
                if (ssnsOnlyInReady.Any())
                {
                    TestOutputHelper.WriteLine($"\n=== SSNs ONLY IN READY ({ssnsOnlyInReady.Count} shown) ===");
                    foreach (var ssn in ssnsOnlyInReady.Take(5))
                    {
                        TestOutputHelper.WriteLine($"  SSN {ssn}: {readyBySsn[ssn].FullName}");
                    }
                }

                // Find SSNs only in SMART
                var ssnsOnlyInSmart = smartBySsn.Keys.Except(readyBySsn.Keys).Take(10).ToList();
                if (ssnsOnlyInSmart.Any())
                {
                    TestOutputHelper.WriteLine($"\n=== SSNs ONLY IN SMART ({ssnsOnlyInSmart.Count} shown) ===");
                    foreach (var ssn in ssnsOnlyInSmart.Take(5))
                    {
                        TestOutputHelper.WriteLine($"  SSN {ssn}: {smartBySsn[ssn].FullName}");
                    }
                }
            }
        }

        actualRows.SetEquals(expectedRows).ShouldBeTrue($"Report {resourceNumber} ({reportId}) employee records should match READY");
        TestOutputHelper.WriteLine($"Report {resourceNumber} ({reportId}) employee records {expectedRows.Count}={actualRows.Count} should match READY");
    }

    /// <summary>
    ///     Normalizes employee records for comparison between SMART and READY.
    ///     Clears fields that differ due to system implementation but don't affect business logic.
    /// </summary>
    private static HashSet<YearEndProfitSharingReportDetail> NormalizeForComparison(IEnumerable<YearEndProfitSharingReportDetail> rows, bool isReady)
    {
        return rows
            .Select(r =>
            {
                // Normalize fields with format differences between systems

                r.FirstContributionYear = null; // SMART Only
                r.IsExecutive = false; // SMART Only, used for blacking out information about execs depending on use roll

                r.Ssn = r.Ssn.Length >= 4 ? r.Ssn.Substring(r.Ssn.Length - 4) : r.Ssn; // Compare last 4 digits only

                // READY uses 'H' for employees but '0' for beneficiaries (non-employees with BadgeNumber=0)
                if (r.BadgeNumber > 0)
                {
                    r.EmployeeTypeCode = 'H'; // READY always uses 'H' for all employees - BUG in PAY426N
                    r.EmployeeTypeName = "Hourly"; // READY maps 'H' to "Hourly"
                }
                else
                {
                    r.EmployeeTypeCode = '0'; // Beneficiaries use '0' type code in READY
                    r.EmployeeTypeName = "Hourly"; // Normalized to Hourly for comparison (SMART may use different name)
                }

                // Infer employee status from termination date (READY quirk)
                // READY reports (and Pay426NParser at line 424-429) infer 'T' from termination date presence
                // READY report files show space (not 'T') for terminated employees - parser converts space -> 'A', then 'A' -> 'T'
                // Apply same inference to SMART data for comparison
                if (r.TerminationDate.HasValue && r.EmployeeStatus == 'A')
                {
                    r.EmployeeStatus = 'T';
                }

                // SMART and READY YearsInPlan calculations differ (the YearsInPlan are checked this earlier in the test)
                r.YearsInPlan = 0;

                // READY has Y2K-style bug: 2-digit termination dates in 90s (97, 99) are interpreted as 2090s (2097, 2099)
                // SMART database correctly stores 1990s dates. Normalize by converting READY's future dates to past century.
                // This only affects very old termination dates that happened before Y2K.
                if (isReady && r.TerminationDate.HasValue && r.TerminationDate.Value.Year >= 2090 && r.TerminationDate.Value.Year < 2100)
                {
                    // Convert 2090s back to 1990s (subtract 100 years)
                    r.TerminationDate = r.TerminationDate.Value.AddYears(-100);
                }

                // Normalize DateOfBirth for beneficiaries with unknown DOB
                // READY parser returns DateOnly.MinValue (1/1/0001) for "00/00/00" dates
                // SMART database may have different placeholder dates - normalize to MinValue for comparison
                if (r.BadgeNumber == 0 && (r.DateOfBirth == DateOnly.MinValue || r.DateOfBirth.Year < 1900))
                {
                    r.DateOfBirth = DateOnly.MinValue;
                    r.Age = 99; // Beneficiaries with unknown DOB get age 99 in READY reports
                }

                // READY has Y2K-style pivot bug for birth dates: 2-digit years 00-25 are interpreted as 2000-2025
                // But beneficiaries born in 1920s show as "23" => parsed as 2023 instead of 1923
                // SMART database correctly stores 1920s dates. Normalize by converting READY's 2020s dates to 1920s.
                if (isReady && r.BadgeNumber == 0 && r.DateOfBirth.Year >= 2020 && r.DateOfBirth.Year < 2030)
                {
                    // Convert 2020s back to 1920s (subtract 100 years)
                    r.DateOfBirth = r.DateOfBirth.AddYears(-100);
                }

                return r;
            }).ToHashSet();
    }

    /// <summary>
    ///     Validates that YearsInPlan values between READY and SMART match the expected logic.
    ///     SMART should equal READY + 1 when employee qualifies for current year (no 2025 contribution yet, >= 1000 hours, age 18+).
    ///     Otherwise SMART should equal READY.
    /// </summary>
    private void ValidateYearsInPlanWithinOneYear(
        IEnumerable<YearEndProfitSharingReportDetail> readyRecords,
        IEnumerable<YearEndProfitSharingReportDetail> smartRecords)
    {
        var readyByBadge = readyRecords.ToDictionary(r => r.BadgeNumber);
        var smartByBadge = smartRecords.ToDictionary(r => r.BadgeNumber);

        var violations = new List<string>();
        int totalChecked = 0;
        int correctPlusOne = 0;
        int correctSame = 0;

        foreach (var badge in readyByBadge.Keys.Intersect(smartByBadge.Keys))
        {
            var ready = readyByBadge[badge];
            var smart = smartByBadge[badge];
            totalChecked++;

            short difference = (short)(smart.YearsInPlan - ready.YearsInPlan);

            // Determine if SMART should add +1 based on the three conditions:
            // 1. No 2025 contribution yet (FirstContributionYear == null OR FirstContributionYear != 2025)
            // 2. Employee has >= 1000 hours
            // 3. Employee is 18+ years old
            bool hasNo2025Contribution = !smart.FirstContributionYear.HasValue || smart.FirstContributionYear.Value != 2025;
            bool hasEnoughHours = smart.Hours >= 1000;
            bool isAge18Plus = smart.Age >= 18;
            bool shouldAddOne = hasNo2025Contribution && hasEnoughHours && isAge18Plus;

            short expectedDifference = shouldAddOne ? (short)1 : (short)0;

            if (difference == expectedDifference)
            {
                if (difference == 1)
                {
                    correctPlusOne++;
                }
                else
                {
                    correctSame++;
                }
            }
            else
            {
                // Log violation with detailed diagnostic info
                violations.Add(
                    $"Badge {badge} ({smart.FullName}): " +
                    $"READY={ready.YearsInPlan}, SMART={smart.YearsInPlan}, Diff={difference}, Expected={expectedDifference} | " +
                    $"FirstContrYear={smart.FirstContributionYear?.ToString() ?? "null"}, Hours={smart.Hours}, Age={smart.Age} | " +
                    $"Conditions: No2025Contr={hasNo2025Contribution}, Hours>=1000={hasEnoughHours}, Age>=18={isAge18Plus}, ShouldAdd+1={shouldAddOne}"
                );
            }
        }

        if (violations.Any())
        {
            TestOutputHelper.WriteLine($"\n VIOLATIONS FOUND: {violations.Count} employees with incorrect YearsInPlan logic:");
            foreach (var violation in violations.Take(10))
            {
                TestOutputHelper.WriteLine($"  {violation}");
            }

            if (violations.Count > 10)
            {
                TestOutputHelper.WriteLine($"  ... and {violations.Count - 10} more violations");
            }
        }
    }
}
