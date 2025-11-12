using System.Text.RegularExpressions;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ReadyParserHelpers;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426;

/// <summary>
///     Parses the main PAY426 report format
/// </summary>
public static class Pay426Parser
{
    /// <summary>
    ///     Parses the PAY426 report and returns a fully populated YearEndProfitSharingReportResponse.
    ///     Includes individual employee records, summary totals, and validates that line items match totals.
    ///     Throws InvalidOperationException if validation fails.
    /// </summary>
    public static YearEndProfitSharingReportResponse Parse(string reportText, short profitYear = 2025)
    {
        // Parse individual employee records
        List<EmployeeRecord> employees = ParseEmployeeRecords(reportText);

        // Parse summary totals
        (decimal wages, decimal hours, decimal points, int allEmp, int newEmp, int under21, int inPlan) = ParseReportTotals(reportText);

        // Map to YearEndProfitSharingReportDetail records
        List<YearEndProfitSharingReportDetail> details = employees.Select(emp => MapToReportDetail(emp, profitYear)).ToList();

        // Validate wages and hours
        // PAY426 SECTION TOTAL excludes under-21 employees
        // NOTE: Hours must be truncated to match SMART's behavior (COBOL uses integer part)
        decimal summedWages = employees.Where(e => !e.IsUnder21).Sum(e => e.Wages);
        decimal summedHours = employees.Where(e => !e.IsUnder21).Sum(e => Math.Truncate(e.Hours));

        if (summedWages != wages)
        {
            throw new InvalidOperationException(
                $"PAY426 validation failed: Summed wages ({summedWages:N2}) do not match total wages ({wages:N2})");
        }

        if (summedHours != hours)
        {
            throw new InvalidOperationException(
                $"PAY426 validation failed: Summed hours ({summedHours:N0}) do not match total hours ({hours:N0})");
        }

        // Validate employee counts
        if (employees.Count != allEmp)
        {
            throw new InvalidOperationException(
                $"PAY426 validation failed: Parsed employee count ({employees.Count}) does not match ALL-EMP ({allEmp})");
        }

        if (employees.Count(e => e.IsNew) != newEmp)
        {
            throw new InvalidOperationException(
                $"PAY426 validation failed: NEW employee count ({employees.Count(e => e.IsNew)}) does not match NEW-EMP ({newEmp})");
        }

        if (employees.Count(e => e.IsUnder21) != under21)
        {
            throw new InvalidOperationException(
                $"PAY426 validation failed: Under-21 count ({employees.Count(e => e.IsUnder21)}) does not match EMP<21 ({under21})");
        }

        if (employees.Count(e => !e.IsNew && !e.IsUnder21) != inPlan)
        {
            throw new InvalidOperationException(
                $"PAY426 validation failed: IN-PLAN count ({employees.Count(e => !e.IsNew && !e.IsUnder21)}) does not match IN-PLAN ({inPlan})");
        }

        // Create a response with all data
        YearEndProfitSharingReportResponse response = new()
        {
            ReportName = "PAY426 - Year-End Profit Sharing Report",
            StartDate = new DateOnly(profitYear, 1, 1),
            EndDate = new DateOnly(profitYear, 12, 31),
            WagesTotal = wages,
            HoursTotal = hours,
            PointsTotal = points,
            NumberOfEmployees = allEmp,
            NumberOfNewEmployees = newEmp,
            NumberOfEmployeesUnder21 = under21,
            NumberOfEmployeesInPlan = inPlan,
            BalanceTotal = 0, // Not available in PAY426 report
            Response = new PaginatedResponseDto<YearEndProfitSharingReportDetail>(new PaginationRequestDto()) { Total = details.Count, Results = details }
        };

        return response;
    }

    /// <summary>
    ///     Parses the PAY426 report totals from SECTION TOTAL and EMPLOYEE TOTALS lines.
    ///     Returns: (Wages, Hours, Points, AllEmployeesCount, NewEmployeesCount, EmployeeUnder21Count, InPlanCount)
    /// </summary>
    public static (decimal Wages, decimal Hours, decimal Points, int AllEmployeesCount, int NewEmployeesCount, int EmployeeUnder21Count, int InPlanCount)
        ParseReportTotals(string report)
    {
        string[] lines = report.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        decimal wages = 0;
        decimal hours = 0;
        decimal points = 0;
        int allEmployeesCount = 0;
        int newEmployeesCount = 0;
        int employeeUnder21Count = 0;
        int inPlanCount = 0;

        bool foundSectionTotal = false;
        bool foundEmployeeTotalHeader = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            // Look for the SECTION TOTAL line: "-SECTION TOTAL-    211,191,778.18   7,732,647 2,111,925"
            if (line.Contains("-SECTION TOTAL-") && !foundSectionTotal)
            {
                foundSectionTotal = true;

                // Extract everything after "-SECTION TOTAL-"
                Match match = Regex.Match(line, @"-SECTION TOTAL-\s+(.+)");
                if (match.Success)
                {
                    string[] parts = match.Groups[1].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        wages = ParseDecimal(parts[0]);
                        hours = ParseDecimal(parts[1]);
                        points = ParseDecimal(parts[2]);
                    }
                }
            }

            // Look for the EMPLOYEE TOTALS header
            if (line.Contains("EMPLOYEE TOTALS") && line.Contains("ALL-EMP"))
            {
                foundEmployeeTotalHeader = true;
                continue;
            }

            // If we found the header, the next non-empty line should have the counts
            if (foundEmployeeTotalHeader && Regex.IsMatch(line, @"\d"))
            {
                // Extract numbers from line like: "                                                                                   4,939         62        205      4,672"
                string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4)
                {
                    allEmployeesCount = ParseInt(parts[0]);
                    newEmployeesCount = ParseInt(parts[1]);
                    employeeUnder21Count = ParseInt(parts[2]);
                    inPlanCount = ParseInt(parts[3]);
                }

                break; // We've found everything we need
            }
        }

        return (wages, hours, points, allEmployeesCount, newEmployeesCount, employeeUnder21Count, inPlanCount);
    }

    /// <summary>
    ///     PAY426 report field specifications (fixed-column positions).
    /// </summary>
    private static readonly FieldSpec[] Pay426Fields = new[]
    {
        new FieldSpec("Prefix", 0, 3),          // spaces or "I  " for inactive
        new FieldSpec("Badge", 3, 7),           // 7 digits with leading zeros
        new FieldSpec("Spaces1", 10, 3),        // spaces
        new FieldSpec("Name", 13, 28),          // employee name
        new FieldSpec("Store", 41, 6),          // store number (right-aligned)
        new FieldSpec("TypeCode", 47, 1),       // employee type code (H, P, G, F)
        new FieldSpec("Spaces2", 48, 4),        // spaces
        new FieldSpec("DateOfBirth", 52, 8),    // MM/DD/YY format
        new FieldSpec("Age", 60, 7),            // age (with spaces)
        new FieldSpec("Ssn", 67, 12),           // format: "000 00 8004" with spaces
        new FieldSpec("Spaces3", 79, 1),        // space
        new FieldSpec("Wages", 80, 14),         // right-aligned decimal
        new FieldSpec("Spaces4", 94, 7),        // spaces
        new FieldSpec("Hours", 101, 8),         // right-aligned decimal
        new FieldSpec("Spaces5", 109, 2),       // spaces
        new FieldSpec("Points", 111, 4),        // points value (right-aligned, 1-4 digits)
        new FieldSpec("Marker", 115, 5)         // marker: "(NEW)" or "(<21)" at index 115
    };


    /// <summary>
    ///     Parses all individual employee records from the PAY426 report.
    ///     Uses fixed-column positions for PAY426 format (different from PAY426N).
    /// </summary>
    public static List<EmployeeRecord> ParseEmployeeRecords(string report)
    {
        string[] lines = report.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        List<EmployeeRecord> employees = new();

        // Minimum line length needed (up to hours field; points and marker are optional for some lines)
        FieldSpec hoursField = GetField(Pay426Fields, "Hours");
        int minLineLength = hoursField.StartPos + hoursField.Length;

        foreach (string line in lines)
        {
            // Stop parsing when we hit the section total
            if (line.Contains("-SECTION TOTAL-"))
            {
                break;
            }

            // Skip if line is too short
            if (line.Length < minLineLength)
            {
                continue;
            }

            // Check for badge number at position 3-10
            string badgeArea = line.Substring(0, Math.Min(11, line.Length));
            if (!Regex.IsMatch(badgeArea, @"^(I\s+|\s+)\d{6,7}"))
            {
                continue;
            }

            try
            {
                // Extract badge number
                string badgeStr = ExtractFieldTrimmed(Pay426Fields, line, "Badge");
                if (!int.TryParse(badgeStr, out int badge))
                {
                    continue;
                }

                // Extract name
                string name = ExtractFieldTrimmed(Pay426Fields, line, "Name");

                // Extract store number
                string storeStr = ExtractFieldTrimmed(Pay426Fields, line, "Store");
                if (!short.TryParse(storeStr, out short store))
                {
                    continue;
                }

                // Extract employee type code
                string typeCodeStr = ExtractField(line, GetField(Pay426Fields, "TypeCode"));
                char typeCode = typeCodeStr.Length > 0 ? typeCodeStr[0] : ' ';

                // Extract date of birth
                string dobStr = ExtractFieldTrimmed(Pay426Fields, line, "DateOfBirth");
                DateOnly dob = ParseDateOfBirth(dobStr);

                // Extract age (already calculated by READY report)
                string ageStr = ExtractFieldTrimmed(Pay426Fields, line, "Age");
                if (!byte.TryParse(ageStr, out byte age))
                {
                    continue; // Skip if age is invalid
                }

                // Extract SSN
                string ssnStr = ExtractFieldTrimmed(Pay426Fields, line, "Ssn").Replace(" ", "");

                // Extract wages
                string wagesStr = ExtractFieldTrimmed(Pay426Fields, line, "Wages");
                decimal wages = ParseDecimal(wagesStr);

                // Extract hours
                string hoursStr = ExtractFieldTrimmed(Pay426Fields, line, "Hours");
                if (string.IsNullOrWhiteSpace(hoursStr))
                {
                    continue;
                }

                decimal hours = ParseDecimal(hoursStr);

                // Extract points (fixed-width field, right-aligned)
                decimal points = 0;
                string pointsStr = ExtractFieldTrimmed(Pay426Fields, line, "Points");
                if (!string.IsNullOrWhiteSpace(pointsStr))
                {
                    points = ParseDecimal(pointsStr);
                }

                // Extract marker (fixed-width field)
                string marker = ExtractFieldTrimmed(Pay426Fields, line, "Marker");
                bool isUnder21 = marker.Contains("<21");
                bool isNew = marker.Contains("NEW");

                employees.Add(new EmployeeRecord(badge, name, store, typeCode, dob, age, ssnStr, wages, hours, points, isNew, isUnder21));
            }
            catch
            {
                // Skip malformed lines (silently ignore parsing errors)
                // This is expected for header lines, page markers, etc.
            }
        }

        return employees;
    }

    /// <summary>
    ///     Maps an EmployeeRecord from the PAY426 report to a YearEndProfitSharingReportDetail.
    /// </summary>
    private static YearEndProfitSharingReportDetail MapToReportDetail(EmployeeRecord emp, short profitYear)
    {
        return new YearEndProfitSharingReportDetail
        {
            BadgeNumber = emp.BadgeNumber,
            ProfitYear = profitYear,
            EmployeeName = emp.Name,
            StoreNumber = emp.StoreNumber,
            EmployeeTypeCode = emp.EmployeeTypeCode,
            EmployeeTypeName = MapEmployeeTypeName(emp.EmployeeTypeCode),
            DateOfBirth = emp.DateOfBirth,
            Age = emp.Age,
            Ssn = emp.Ssn,
            Wages = emp.Wages,
            // Truncate hours to match SMART's behavior (COBOL uses integer part)
            Hours = Math.Truncate(emp.Hours),
            Points = (short)emp.Points,
            IsUnder21 = emp.IsUnder21, // Use from marker field
            IsNew = emp.IsNew,
            
            // Reusing the DTO between PAY426 and PAY426N reports
            EmployeeStatus = null, // Not available in PAY426 report
            Balance = 0, // Not available in PAY426 report
            YearsInPlan = 0, // Not available in PAY426 report
            TerminationDate = null, // Not available in PAY426 report
            FirstContributionYear = null, // Not available in PAY426 report
            IsExecutive = false // Not available in PAY426 report
        };
    }

    public record EmployeeRecord(
        int BadgeNumber,
        string Name,
        short StoreNumber,
        char EmployeeTypeCode,
        DateOnly DateOfBirth,
        byte Age,
        string Ssn,
        decimal Wages,
        decimal Hours,
        decimal Points,
        bool IsNew,
        bool IsUnder21);
}
