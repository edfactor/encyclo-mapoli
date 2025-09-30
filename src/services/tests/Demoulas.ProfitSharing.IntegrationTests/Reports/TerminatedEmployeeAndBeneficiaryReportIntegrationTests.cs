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

        // Assert on report totals first
        actualData.TotalEndingBalance.ShouldBe(expectedData.TotalEndingBalance, "TotalEndingBalance should match");
        actualData.TotalVested.ShouldBe(expectedData.TotalVested, "TotalVested should match");
        actualData.TotalForfeit.ShouldBe(expectedData.TotalForfeit, "TotalForfeit should match");
        actualData.TotalBeneficiaryAllocation.ShouldBe(expectedData.TotalBeneficiaryAllocation, "TotalBeneficiaryAllocation should match");

        // Assert on employee count
        var actualEmployees = actualData.Response.Results.ToList();
        var expectedEmployees = expectedData.Response.Results.ToList();
        actualEmployees.Count.ShouldBe(expectedEmployees.Count, "Employee count should match");

        // Assert on each employee's properties
        for (int i = 0; i < actualEmployees.Count; i++)
        {
            var actual = actualEmployees[i];
            var expected = expectedEmployees[i];

            // Employee-level properties
            actual.BadgeNumber.ShouldBe(expected.BadgeNumber, $"Employee {i}: BadgeNumber should match");
            actual.PsnSuffix.ShouldBe(expected.PsnSuffix, $"Employee {i}: PsnSuffix should match");
            actual.Name.ShouldBe(expected.Name, $"Employee {i}: Name should match");
            actual.IsExecutive.ShouldBe(expected.IsExecutive, $"Employee {i}: IsExecutive should match");
            actual.BadgePSn.ShouldBe(expected.BadgePSn, $"Employee {i}: BadgePSn should match");

            // Year details count
            actual.YearDetails.Count.ShouldBe(expected.YearDetails.Count, $"Employee {i} ({actual.Name}): YearDetails count should match");

            // Assert on each year detail's properties
            for (int j = 0; j < actual.YearDetails.Count; j++)
            {
                var actualYear = actual.YearDetails[j];
                var expectedYear = expected.YearDetails[j];
                var employeeContext = $"Employee {i} ({actual.Name}), Year {j}";

                actualYear.ProfitYear.ShouldBe(expectedYear.ProfitYear, $"{employeeContext}: ProfitYear should match");
                actualYear.BeginningBalance.ShouldBe(expectedYear.BeginningBalance, $"{employeeContext}: BeginningBalance should match");
                actualYear.BeneficiaryAllocation.ShouldBe(expectedYear.BeneficiaryAllocation, $"{employeeContext}: BeneficiaryAllocation should match");
                actualYear.DistributionAmount.ShouldBe(expectedYear.DistributionAmount, $"{employeeContext}: DistributionAmount should match");
                actualYear.Forfeit.ShouldBe(expectedYear.Forfeit, $"{employeeContext}: Forfeit should match");
                actualYear.EndingBalance.ShouldBe(expectedYear.EndingBalance, $"{employeeContext}: EndingBalance should match");
                actualYear.VestedBalance.ShouldBe(expectedYear.VestedBalance, $"{employeeContext}: VestedBalance should match");
                actualYear.DateTerm.ShouldBe(expectedYear.DateTerm, $"{employeeContext}: DateTerm should match");
                actualYear.YtdPsHours.ShouldBe(expectedYear.YtdPsHours, $"{employeeContext}: YtdPsHours should match");
                actualYear.VestedPercent.ShouldBe(expectedYear.VestedPercent, $"{employeeContext}: VestedPercent should match");
                actualYear.Age.ShouldBe(expectedYear.Age, $"{employeeContext}: Age should match");
                actualYear.HasForfeited.ShouldBe(expectedYear.HasForfeited, $"{employeeContext}: HasForfeited should match");
                actualYear.IsExecutive.ShouldBe(expectedYear.IsExecutive, $"{employeeContext}: IsExecutive should match");
                // Note: SuggestedForfeit may not be in the golden file format, so we'll skip asserting on it
            }
        }
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

        if (line.Length < 100) // Minimum expected length for a data line
        {
            return null;
        }

        try
        {
            // Parse badge/PSN (positions 0-10, right-aligned)
            var badgePsnStr = line.Substring(0, 11).Trim();

            // Parse employee name (positions 11-30)
            var name = line.Substring(11, 20).Trim();

            // Parse the numeric fields (fixed positions, right-aligned with spaces)
            var beginningBalance = ParseDecimalField(line, 31, 12);      // Beginning Balance
            var beneficiaryAllocation = ParseDecimalField(line, 44, 12); // Beneficiary Allocation
            var distributionAmount = ParseDecimalField(line, 57, 12);    // Distribution Amount
            var forfeit = ParseDecimalField(line, 70, 12);               // Forfeit
            var endingBalance = ParseDecimalField(line, 83, 12);         // Ending Balance
            var vestedBalance = ParseDecimalField(line, 96, 12);         // Vested Balance

            // Parse termination date (6 characters, YYMMDD format or empty)
            var termDateStr = line.Length > 108 ? line.Substring(109, 6).Trim() : "";
            DateOnly? termDate = null;
            if (!string.IsNullOrEmpty(termDateStr) && termDateStr.Length == 6 &&
                int.TryParse(termDateStr.Substring(0, 2), out var year) &&
                int.TryParse(termDateStr.Substring(2, 2), out var month) &&
                int.TryParse(termDateStr.Substring(4, 2), out var day))
            {
                termDate = new DateOnly(2000 + year, month, day);
            }

            // Parse YTD PS Hours (7 characters, decimal)
            var ytdPsHours = ParseDecimalField(line, 116, 7);

            // Parse vested percent (3 characters, integer)
            var vestedPercent = ParseIntField(line, 124, 3);

            // Parse age (3 characters, integer, can be empty)
            var age = ParseNullableIntField(line, 128, 3);

            // Parse badge number and PSN suffix from badgePsnStr
            int badgeNumber;
            short psnSuffix;

            if (badgePsnStr.Length > 7) // Has suffix
            {
                var badgeStr = badgePsnStr.Substring(0, badgePsnStr.Length - 4);
                var suffixStr = badgePsnStr.Substring(badgePsnStr.Length - 4);
                badgeNumber = int.Parse(badgeStr);
                psnSuffix = short.Parse(suffixStr);
            }
            else
            {
                badgeNumber = int.Parse(badgePsnStr);
                psnSuffix = 0;
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
        catch (Exception)
        {
            // Skip malformed lines
            return null;
        }
    }

    private static decimal ParseDecimalField(string line, int startIndex, int length)
    {
        if (startIndex + length > line.Length)
        {
            return 0;
        }

        var fieldStr = line.Substring(startIndex, length).Trim();
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

        // Remove commas and parse
        fieldStr = fieldStr.Replace(",", "");
        if (decimal.TryParse(fieldStr, out var value))
        {
            return isNegative ? -value : value;
        }

        return 0;
    }

    private static int ParseIntField(string line, int startIndex, int length)
    {
        if (startIndex + length > line.Length)
        {
            return 0;
        }

        var fieldStr = line.Substring(startIndex, length).Trim();
        if (int.TryParse(fieldStr, out var value))
        {
            return value;
        }

        return 0;
    }

    private static int? ParseNullableIntField(string line, int startIndex, int length)
    {
        if (startIndex + length > line.Length)
        {
            return null;
        }

        var fieldStr = line.Substring(startIndex, length).Trim();
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


}
