using System.Diagnostics;
using System.Reflection;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Service;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
using Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports;

public class TerminatedEmployeeAndBeneficiaryReportIntegrationTests : PristineBaseTest
{
    public TerminatedEmployeeAndBeneficiaryReportIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task EnsureSmartReportMatchesReadyReport()
    {
        // These are arguments to the program/rest endpoint
        // Plan admin may choose a range of dates (ie. Q2 ?)
        DateOnly startDate = new DateOnly(2025, 01, 4);
        DateOnly endDate = new DateOnly(2025, 12, 27);

        var distributedCache = new MemoryDistributedCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        var calendarService = new CalendarService(DbFactory, new AccountingPeriodsService(), distributedCache);
        var totalService = new TotalService(DbFactory,
            calendarService, new EmbeddedSqlService(),
            new DemographicReaderService(new FrozenService(DbFactory, new Mock<ICommitGuardOverride>().Object, new Mock<IServiceProvider>().Object), new HttpContextAccessor()));
        DemographicReaderService demographicReaderService = new(new FrozenService(DbFactory, new Mock<ICommitGuardOverride>().Object, new Mock<IServiceProvider>().Object), new HttpContextAccessor());
        TerminatedEmployeeService mockService =
            new TerminatedEmployeeService(DbFactory, totalService, demographicReaderService);

        Stopwatch stopwatch = Stopwatch.StartNew();
        stopwatch.Start();
        var actualData = await mockService.GetReportAsync(new StartAndEndDateRequest { BeginningDate = startDate, EndingDate = endDate, Take = int.MaxValue, SortBy = "name" }, CancellationToken.None);
        stopwatch.Stop();
        TestOutputHelper.WriteLine($"Took: {stopwatch.ElapsedMilliseconds} Rows: {actualData.Response.Results.Count()}");

        actualData.ShouldNotBeNull();
        actualData.Response.Results.ShouldNotBeEmpty();

        string expectedText = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.golden.R3-QPAY066");
        var expectedData = ParseGoldenFileToDto(expectedText);

        // Collect all differences instead of failing immediately
        var differences = new List<string>();

        // Compare report totals
        CompareValues(differences, "Report Totals", "TotalEndingBalance", expectedData.TotalEndingBalance, actualData.TotalEndingBalance);
        CompareValues(differences, "Report Totals", "TotalVested", expectedData.TotalVested, actualData.TotalVested);
        CompareValues(differences, "Report Totals", "TotalForfeit", expectedData.TotalForfeit, actualData.TotalForfeit);
        CompareValues(differences, "Report Totals", "TotalBeneficiaryAllocation", expectedData.TotalBeneficiaryAllocation, actualData.TotalBeneficiaryAllocation);

        // Compare employee counts
        var actualEmployees = actualData.Response.Results.ToList();
        var expectedEmployees = expectedData.Response.Results.ToList();
        CompareValues(differences, "Report Structure", "Employee Count", expectedEmployees.Count, actualEmployees.Count);

        // Compare each employee's properties
        int maxEmployees = Math.Max(actualEmployees.Count, expectedEmployees.Count);
        for (int i = 0; i < maxEmployees; i++)
        {
            var employeePrefix = $"Employee[{i}]";

            if (i >= expectedEmployees.Count)
            {
                differences.Add($"{employeePrefix}: Extra employee in actual data - {actualEmployees[i].BadgePSn} ({actualEmployees[i].Name})");
                continue;
            }

            if (i >= actualEmployees.Count)
            {
                differences.Add($"{employeePrefix}: Missing employee in actual data - {expectedEmployees[i].BadgePSn} ({expectedEmployees[i].Name})");
                continue;
            }

            var actual = actualEmployees[i];
            var expected = expectedEmployees[i];
            var employeeContext = $"{employeePrefix} [{actual.BadgePSn}]";

            // Employee-level properties
            CompareValues(differences, employeeContext, "BadgeNumber", expected.BadgeNumber, actual.BadgeNumber);
            CompareValues(differences, employeeContext, "PsnSuffix", expected.PsnSuffix, actual.PsnSuffix);
            CompareValues(differences, employeeContext, "Name", expected.Name, actual.Name);
            CompareValues(differences, employeeContext, "IsExecutive", expected.IsExecutive, actual.IsExecutive);
            CompareValues(differences, employeeContext, "BadgePSn", expected.BadgePSn, actual.BadgePSn);

            // Year details count
            CompareValues(differences, employeeContext, "YearDetails.Count", expected.YearDetails.Count, actual.YearDetails.Count);

            // Compare each year detail's properties
            int maxYears = Math.Max(actual.YearDetails.Count, expected.YearDetails.Count);
            for (int j = 0; j < maxYears; j++)
            {
                var yearPrefix = $"{employeeContext}.YearDetails[{j}]";

                if (j >= expected.YearDetails.Count)
                {
                    differences.Add($"{yearPrefix}: Extra year detail in actual data");
                    continue;
                }

                if (j >= actual.YearDetails.Count)
                {
                    differences.Add($"{yearPrefix}: Missing year detail in actual data");
                    continue;
                }

                var actualYear = actual.YearDetails[j];
                var expectedYear = expected.YearDetails[j];

                CompareValues(differences, yearPrefix, "ProfitYear", expectedYear.ProfitYear, actualYear.ProfitYear);
                CompareValues(differences, yearPrefix, "BeginningBalance", expectedYear.BeginningBalance, actualYear.BeginningBalance);
                CompareValues(differences, yearPrefix, "BeneficiaryAllocation", expectedYear.BeneficiaryAllocation, actualYear.BeneficiaryAllocation);
                CompareValues(differences, yearPrefix, "DistributionAmount", expectedYear.DistributionAmount, actualYear.DistributionAmount);
                CompareValues(differences, yearPrefix, "Forfeit", expectedYear.Forfeit, actualYear.Forfeit);
                CompareValues(differences, yearPrefix, "EndingBalance", expectedYear.EndingBalance, actualYear.EndingBalance);
                CompareValues(differences, yearPrefix, "VestedBalance", expectedYear.VestedBalance, actualYear.VestedBalance);
                CompareValues(differences, yearPrefix, "DateTerm", expectedYear.DateTerm, actualYear.DateTerm);
                CompareValues(differences, yearPrefix, "YtdPsHours", expectedYear.YtdPsHours, actualYear.YtdPsHours);
                CompareValues(differences, yearPrefix, "VestedPercent", expectedYear.VestedPercent, actualYear.VestedPercent);
                CompareValues(differences, yearPrefix, "Age", expectedYear.Age, actualYear.Age);
                CompareValues(differences, yearPrefix, "HasForfeited", expectedYear.HasForfeited, actualYear.HasForfeited);
                CompareValues(differences, yearPrefix, "IsExecutive", expectedYear.IsExecutive, actualYear.IsExecutive);
                // Note: SuggestedForfeit may not be in the golden file format, so we'll skip asserting on it
            }
        }

        // Generate comprehensive difference report
        if (differences.Count > 0)
        {
            var report = GenerateComprehensiveDifferenceReport(differences, actualEmployees.Count, expectedEmployees.Count);
            TestOutputHelper.WriteLine(report);
            differences[0].ShouldBeNull($"Found {differences.Count} differences between actual and expected data. See test output for comprehensive report.");
        }
        else
        {
            TestOutputHelper.WriteLine("✅ All data matches perfectly between actual and expected results!");
        }
    }

    private static void CompareValues<T>(List<string> differences, string context, string propertyName, T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            differences.Add($"{context}.{propertyName}: Expected='{expected}', Actual='{actual}'");
        }
    }

    private static string GenerateComprehensiveDifferenceReport(List<string> differences, int actualCount, int expectedCount)
    {
        var report = new System.Text.StringBuilder();

        report.AppendLine("╔══════════════════════════════════════════════════════════════════════════════════════════╗");
        report.AppendLine("║                           COMPREHENSIVE DIFFERENCE REPORT                                   ║");
        report.AppendLine("╠══════════════════════════════════════════════════════════════════════════════════════════╣");
        report.AppendLine($"║ Total Differences Found: {differences.Count,-10} | Actual Employees: {actualCount,-10} | Expected: {expectedCount,-10} ║");
        report.AppendLine("╠══════════════════════════════════════════════════════════════════════════════════════════╣");

        // Group differences by category
        var groupedDifferences = differences
            .GroupBy(d => GetDifferenceCategory(d))
            .OrderBy(g => GetCategoryOrder(g.Key));

        foreach (var group in groupedDifferences)
        {
            report.AppendLine($"║ {group.Key.ToUpper()} ({group.Count()} differences)");
            report.AppendLine("╠──────────────────────────────────────────────────────────────────────────────────────────╣");

            foreach (var difference in group.Take(10)) // Limit to first 10 per category to avoid overwhelming output
            {
                var truncated = difference.Length > 82 ? difference.Substring(0, 79) + "..." : difference;
                report.AppendLine($"║ {truncated,-88} ║");
            }

            if (group.Count() > 10)
            {
                report.AppendLine($"║ ... and {group.Count() - 10} more {group.Key.ToLower()} differences                                      ║");
            }
            report.AppendLine("╠──────────────────────────────────────────────────────────────────────────────────────────╣");
        }

        report.AppendLine("║ ANALYSIS SUMMARY:                                                                           ║");
        report.AppendLine("╠──────────────────────────────────────────────────────────────────────────────────────────╣");

        var totalDiffs = differences.Count(d => d.Contains("Report Totals"));
        var employeeDiffs = differences.Count(d => d.Contains("Employee[") && !d.Contains("YearDetails"));
        var yearDetailDiffs = differences.Count(d => d.Contains("YearDetails"));

        report.AppendLine($"║ • Report Totals:     {totalDiffs,-3} differences                                                     ║");
        report.AppendLine($"║ • Employee Details:  {employeeDiffs,-3} differences                                                     ║");
        report.AppendLine($"║ • Year Details:      {yearDetailDiffs,-3} differences                                                     ║");
        report.AppendLine("╠──────────────────────────────────────────────────────────────────────────────────────────╣");
        report.AppendLine("║ RECOMMENDATIONS:                                                                            ║");
        report.AppendLine("║ 1. Review calculation logic if totals don't match                                          ║");
        report.AppendLine("║ 2. Check data parsing if employee details differ                                           ║");
        report.AppendLine("║ 3. Verify golden file format if many parsing errors                                       ║");
        report.AppendLine("╚══════════════════════════════════════════════════════════════════════════════════════════╝");

        return report.ToString();
    }

    private static string GetDifferenceCategory(string difference)
    {
        if (difference.Contains("Report Totals"))
        {
            return "Report Totals";
        }
        if (difference.Contains("Report Structure"))
        {
            return "Report Structure";
        }
        if (difference.Contains("YearDetails["))
        {
            return "Year Details";
        }
        if (difference.Contains("Employee["))
        {
            return "Employee Details";
        }
        return "Other";
    }

    private static int GetCategoryOrder(string category)
    {
        return category switch
        {
            "Report Structure" => 1,
            "Report Totals" => 2,
            "Employee Details" => 3,
            "Year Details" => 4,
            _ => 5
        };
    }

    public static string ReadEmbeddedResource(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Parses the golden QPAY066 text report back into the DTO structure for detailed comparison
    /// </summary>
    private static TerminatedEmployeeAndBeneficiaryResponse ParseGoldenFileToDto(string goldenFileContent)
    {
        var lines = goldenFileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var employees = new List<TerminatedEmployeeAndBeneficiaryDataResponseDto>();

        decimal totalEndingBalance = 0;
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalBeneficiaryAllocation = 0;

        bool inDataSection = false;
        bool inTotalsSection = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            // Skip header lines until we reach the data section
            if (trimmedLine.StartsWith("BADGE/PSN # EMPLOYEE NAME"))
            {
                inDataSection = true;
                continue;
            }

            // Check for totals section
            if (trimmedLine.StartsWith("TOTALS"))
            {
                inTotalsSection = true;
                continue;
            }

            if (inTotalsSection)
            {
                if (trimmedLine.StartsWith("AMOUNT IN PROFIT SHARING"))
                {
                    totalEndingBalance = ParseDecimalFromTotalLine(trimmedLine);
                }
                else if (trimmedLine.StartsWith("VESTED AMOUNT"))
                {
                    totalVested = ParseDecimalFromTotalLine(trimmedLine);
                }
                else if (trimmedLine.StartsWith("TOTAL FORFEITURES"))
                {
                    totalForfeit = ParseDecimalFromTotalLine(trimmedLine);
                }
                else if (trimmedLine.StartsWith("TOTAL BENEFICIARY ALLOCTIONS"))
                {
                    totalBeneficiaryAllocation = ParseDecimalFromTotalLine(trimmedLine);
                }
                continue;
            }

            if (!inDataSection || string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            // Parse employee data line
            var employee = ParseEmployeeDataLine(line);
            if (employee != null)
            {
                employees.Add(employee);
            }
        }

        return new TerminatedEmployeeAndBeneficiaryResponse
        {
            ReportName = "Terminated Employees",
            ReportDate = DateTimeOffset.Now,
            StartDate = new DateOnly(2025, 1, 4),
            EndDate = new DateOnly(2025, 12, 27),
            TotalEndingBalance = totalEndingBalance,
            TotalVested = totalVested,
            TotalForfeit = totalForfeit,
            TotalBeneficiaryAllocation = totalBeneficiaryAllocation,
            Response = new PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto>
            {
                Results = employees,
                Total = employees.Count
            }
        };
    }

    private static decimal ParseDecimalFromTotalLine(string line)
    {
        // Extract the decimal value from lines like "AMOUNT IN PROFIT SHARING          24,692,640.86"
        var parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            var valueStr = parts[^1]; // Last part

            // Handle negative values with trailing minus sign
            bool isNegative = valueStr.EndsWith('-');
            if (isNegative)
            {
                valueStr = valueStr.TrimEnd('-');
            }

            // Remove commas and parse
            valueStr = valueStr.Replace(",", "");
            if (decimal.TryParse(valueStr, out var value))
            {
                return isNegative ? -value : value;
            }
        }
        return 0;
    }

    private static TerminatedEmployeeAndBeneficiaryDataResponseDto? ParseEmployeeDataLine(string line)
    {
        // Expected format:
        // BADGE/PSN # EMPLOYEE NAME           BALANCE  ALLOCATION       AMOUNT       FORFEIT      BALANCE      BALANCE   TERM   PS HRS PCT AGE C
        //      707319 ACOSTA, THEO           6,045.93         0.00         0.00         0.00     6,045.93     2,418.37  251111  697.00  40  36

        if (string.IsNullOrWhiteSpace(line) || line.Length < 80) // Minimum expected length for a data line
        {
            return null;
        }

        try
        {
            // Safely parse badge/PSN (positions 0-10, right-aligned)
            var badgePsnStr = SafeSubstring(line, 0, 11).Trim();
            if (string.IsNullOrEmpty(badgePsnStr))
            {
                return null;
            }

            // Safely parse employee name (positions 11-30)
            var name = SafeSubstring(line, 11, 20).Trim();

            // Parse the numeric fields (fixed positions, right-aligned with spaces)
            var beginningBalance = ParseDecimalField(line, 31, 12);      // Beginning Balance
            var beneficiaryAllocation = ParseDecimalField(line, 44, 12); // Beneficiary Allocation
            var distributionAmount = ParseDecimalField(line, 57, 12);    // Distribution Amount
            var forfeit = ParseDecimalField(line, 70, 12);               // Forfeit
            var endingBalance = ParseDecimalField(line, 83, 12);         // Ending Balance
            var vestedBalance = ParseDecimalField(line, 96, 12);         // Vested Balance

            // Parse termination date (6 characters, YYMMDD format or empty)
            var termDateStr = SafeSubstring(line, 109, 6).Trim();
            DateOnly? termDate = null;
            if (!string.IsNullOrEmpty(termDateStr) && termDateStr.Length == 6 &&
                int.TryParse(termDateStr.Substring(0, 2), out var year) &&
                int.TryParse(termDateStr.Substring(2, 2), out var month) &&
                int.TryParse(termDateStr.Substring(4, 2), out var day) &&
                year >= 0 && month >= 1 && month <= 12 && day >= 1 && day <= 31)
            {
                try
                {
                    termDate = new DateOnly(2000 + year, month, day);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Invalid date, leave as null
                    termDate = null;
                }
            }

            // Parse YTD PS Hours (7 characters, decimal)
            var ytdPsHours = ParseDecimalField(line, 116, 7);

            // Parse vested percent (3 characters, integer)
            var vestedPercent = ParseIntField(line, 124, 3);

            // Parse age (3 characters, integer, can be empty)
            var age = ParseNullableIntField(line, 128, 3);

            // Parse badge number and PSN suffix from badgePsnStr with safer logic
            if (!TryParseBadgeAndSuffix(badgePsnStr, out int badgeNumber, out short psnSuffix))
            {
                return null;
            }

            return new TerminatedEmployeeAndBeneficiaryDataResponseDto
            {
                BadgeNumber = badgeNumber,
                PsnSuffix = psnSuffix,
                Name = name,
                IsExecutive = false, // Not directly available in the text format
                YearDetails = new List<TerminatedEmployeeAndBeneficiaryYearDetailDto>
                {
                    new TerminatedEmployeeAndBeneficiaryYearDetailDto
                    {
                        ProfitYear = 2025, // Assuming current year from context
                        BeginningBalance = beginningBalance,
                        BeneficiaryAllocation = beneficiaryAllocation,
                        DistributionAmount = distributionAmount,
                        Forfeit = forfeit,
                        EndingBalance = endingBalance,
                        VestedBalance = vestedBalance,
                        DateTerm = termDate,
                        YtdPsHours = ytdPsHours,
                        VestedPercent = vestedPercent,
                        Age = age,
                        HasForfeited = forfeit != 0,
                        IsExecutive = false // Not directly available in the text format
                    }
                }
            };
        }
        catch (Exception ex)
        {
            // Log the parsing error for debugging (can be removed in production)
            System.Diagnostics.Debug.WriteLine($"Failed to parse employee line: {line.Substring(0, Math.Min(50, line.Length))}... Error: {ex.Message}");
            return null;
        }
    }
    
    private static string SafeSubstring(string input, int startIndex, int length)
    {
        if (startIndex >= input.Length)
        {
            return string.Empty;
        }
        
        int actualLength = Math.Min(length, input.Length - startIndex);
        return actualLength > 0 ? input.Substring(startIndex, actualLength) : string.Empty;
    }
    
    private static bool TryParseBadgeAndSuffix(string badgePsnStr, out int badgeNumber, out short psnSuffix)
    {
        badgeNumber = 0;
        psnSuffix = 0;
        
        if (string.IsNullOrEmpty(badgePsnStr))
        {
            return false;
        }
        
        // Try to parse as a simple badge number first
        if (int.TryParse(badgePsnStr, out badgeNumber))
        {
            psnSuffix = 0;
            return true;
        }
        
        // If that fails, try to parse with suffix logic
        // Look for patterns like 7029671000 where the last 4 digits might be suffix
        if (badgePsnStr.Length > 6)
        {
            // Try different suffix lengths
            for (int suffixLength = 3; suffixLength <= 4; suffixLength++)
            {
                if (badgePsnStr.Length > suffixLength)
                {
                    var badgeStr = badgePsnStr.Substring(0, badgePsnStr.Length - suffixLength);
                    var suffixStr = badgePsnStr.Substring(badgePsnStr.Length - suffixLength);
                    
                    if (int.TryParse(badgeStr, out badgeNumber) && short.TryParse(suffixStr, out psnSuffix))
                    {
                        return true;
                    }
                }
            }
        }
        
        // Fallback: try to parse the whole thing as badge number
        if (int.TryParse(badgePsnStr, out badgeNumber))
        {
            psnSuffix = 0;
            return true;
        }
        
        return false;
    }

    private static decimal ParseDecimalField(string line, int startIndex, int length)
    {
        try
        {
            var fieldStr = SafeSubstring(line, startIndex, length).Trim();
            if (string.IsNullOrEmpty(fieldStr))
            {
                return 0;
            }

            // Handle negative values with trailing minus sign
            bool isNegative = fieldStr.EndsWith('-');
            if (isNegative)
            {
                fieldStr = fieldStr.TrimEnd('-');
            }

            // Remove commas and other formatting characters
            fieldStr = fieldStr.Replace(",", "").Replace("$", "").Trim();
            
            if (string.IsNullOrEmpty(fieldStr))
            {
                return 0;
            }

            if (decimal.TryParse(fieldStr, out var value))
            {
                return isNegative ? -value : value;
            }

            return 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    private static int ParseIntField(string line, int startIndex, int length)
    {
        try
        {
            var fieldStr = SafeSubstring(line, startIndex, length).Trim();
            if (string.IsNullOrEmpty(fieldStr))
            {
                return 0;
            }

            if (int.TryParse(fieldStr, out var value))
            {
                return value;
            }

            return 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    private static int? ParseNullableIntField(string line, int startIndex, int length)
    {
        try
        {
            var fieldStr = SafeSubstring(line, startIndex, length).Trim();
            if (string.IsNullOrEmpty(fieldStr))
            {
                return null;
            }

            if (int.TryParse(fieldStr, out var value))
            {
                return value;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }


}
