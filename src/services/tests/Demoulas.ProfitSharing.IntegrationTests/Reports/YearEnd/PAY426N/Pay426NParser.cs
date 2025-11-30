using System.Text.RegularExpressions;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ReadyParserHelpers;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426N;

/// <summary>
///     Parses the PAY426N sub-report from READY and extracts summary totals.
/// </summary>
public static class Pay426NParser
{
    /// <summary>
    ///     Parses the PAY426N sub-report and returns a fully populated YearEndProfitSharingReportResponse.
    ///     Includes individual employee records, summary totals, and validates that line items match totals.
    ///     Throws InvalidOperationException if validation fails.
    /// </summary>
    public static YearEndProfitSharingReportResponse Parse(string reportText, short profitYear = 2024)
    {
        // Parse individual employee records
        List<EmployeeRecord> employees = ParseEmployeeRecords(reportText, profitYear);

        // Parse summary totals
        (decimal wages, decimal hours, decimal points, int allEmp, int newEmp, int under21, int inPlan) = ParseReportTotals(reportText, profitYear);

        // Map to YearEndProfitSharingReportDetail records
        List<YearEndProfitSharingReportDetail> details = employees.Select(emp => MapToReportDetail(emp, profitYear)).ToList();

        // Validate wages and hours
        // PAY426N sub-reports: TOTAL EMPS includes ALL employees (not excluding under-21)
        // NOTE: Hours must be truncated to match SMART's behavior (COBOL uses integer part)
        decimal summedWages = employees.Sum(e => e.Wages);
        decimal summedHours = employees.Sum(e => Math.Truncate(e.Hours));

        if (summedWages != wages)
        {
            throw new InvalidOperationException(
                $"PAY426N validation failed: Summed wages ({summedWages:N2}) do not match total wages ({wages:N2})");
        }

        if (summedHours != hours)
        {
            throw new InvalidOperationException(
                $"PAY426N validation failed: Summed hours ({summedHours:N0}) do not match total hours ({hours:N0})");
        }

        // Validate employee counts
        if (employees.Count != allEmp)
        {
            throw new InvalidOperationException(
                $"PAY426N validation failed: Parsed employee count ({employees.Count}) does not match ALL-EMP ({allEmp})");
        }

        if (employees.Count(e => e.IsNew) != newEmp)
        {
            throw new InvalidOperationException(
                $"PAY426N validation failed: NEW employee count ({employees.Count(e => e.IsNew)}) does not match NEW-EMP ({newEmp})");
        }

        if (employees.Count(e => e.IsUnder21) != under21)
        {
            throw new InvalidOperationException(
                $"PAY426N validation failed: Under-21 count ({employees.Count(e => e.IsUnder21)}) does not match EMP<21 ({under21})");
        }

        if (employees.Count(e => !e.IsNew && !e.IsUnder21) != inPlan)
        {
            throw new InvalidOperationException(
                $"PAY426N validation failed: IN-PLAN count ({employees.Count(e => !e.IsNew && !e.IsUnder21)}) does not match IN-PLAN ({inPlan})");
        }

        // Create response with all data
        YearEndProfitSharingReportResponse response = new()
        {
            ReportName = "PAY426N - Year-End Profit Sharing Sub-Report",
            StartDate = new DateOnly(profitYear, 1, 1),
            EndDate = new DateOnly(profitYear, 12, 31),
            WagesTotal = wages,
            HoursTotal = hours,
            PointsTotal = points,
            NumberOfEmployees = allEmp,
            NumberOfNewEmployees = newEmp,
            NumberOfEmployeesUnder21 = under21,
            NumberOfEmployeesInPlan = inPlan,
            BalanceTotal = 0, // Not available in PAY426N sub-report totals
            Response = new PaginatedResponseDto<YearEndProfitSharingReportDetail>(new PaginationRequestDto()) { Total = details.Count, Results = details }
        };

        return response;
    }

    /// <summary>
    ///     Parses the PAY426N sub-report and extracts the 7 key summary numbers:
    ///     - Wages (from TOTAL EMPS line)
    ///     - Hours (calculated from employee records, not in TOTAL EMPS line)
    ///     - Points (calculated from employee records, not in TOTAL EMPS line)
    ///     - AllEmployeesCount (from TOTAL EMPS line)
    ///     - NewEmployeesCount (calculated from employee records)
    ///     - EmployeeUnder21Count (calculated from employee records)
    ///     - InPlanCount (calculated from employee records)
    /// </summary>
    public static (decimal Wages, decimal Hours, decimal Points, int AllEmployeesCount, int NewEmployeesCount, int EmployeeUnder21Count, int InPlanCount)
        ParseReportTotals(string report, short profitYear)
    {
        string[] lines = report.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        decimal wages = 0;
        int allEmployeesCount = 0;

        // Look for TOTAL EMPS line: "TOTAL EMPS:     205                                                   5,198,074.91                                       54,116.75"
        // Or for beneficiaries: "TOTAL NON-EMP BENEFICIAIRIES:     172                                          .00                                    2,661,010.13"
        foreach (string line in lines)
        {
            if (line.Contains("TOTAL EMPS:") || line.Contains("TOTAL NON-EMP BENEFICIAIRIES:"))
            {
                // Extract everything after "TOTAL EMPS:" or "TOTAL NON-EMP BENEFICIAIRIES:"
                Match match = Regex.Match(line, @"(?:TOTAL EMPS:|TOTAL NON-EMP BENEFICIAIRIES:)\s+(.+)");
                if (match.Success)
                {
                    string[] parts = match.Groups[1].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 1)
                    {
                        allEmployeesCount = ParseInt(parts[0]); // Employee count
                    }

                    if (parts.Length >= 2)
                    {
                        wages = ParseDecimal(parts[1]); // Total wages
                    }
                    // Note: Hours total not present in PAY426N sub-reports; will be calculated from employee records
                    // Note: parts[2] is balance, not needed for report comparison
                }

                break;
            }
        }

        // Calculate employee breakdown from individual records
        List<EmployeeRecord> employees = ParseEmployeeRecords(report, profitYear);
        int newEmployeesCount = employees.Count(e => e.IsNew);
        int employeeUnder21Count = employees.Count(e => e.IsUnder21);
        int inPlanCount = employees.Count(e => !e.IsNew && !e.IsUnder21);

        // Truncate hours to match SMART's behavior (COBOL uses integer part)
        // For PAY426N, hours total includes ALL employees
        decimal hours = employees.Sum(e => Math.Truncate(e.Hours));

        // Calculate points from employee records (PAY426N TOTAL EMPS line doesn't include points)
        decimal points = employees.Sum(e => e.Points);

        return (wages, hours, points, allEmployeesCount, newEmployeesCount, employeeUnder21Count, inPlanCount);
    }

    /// <summary>
    ///     PAY426N report field specifications (fixed-column positions).
    /// </summary>
    private static readonly FieldSpec[] Pay426NFields = new[]
    {
        new FieldSpec("Status", 0, 1),          // ' ' = Active, 'I' = Inactive, 'T' = Terminated
        new FieldSpec("Badge", 3, 7),           // 6-7 digits
        new FieldSpec("Name", 10, 26),          // employee name
        new FieldSpec("Store", 36, 4),          // store number (right-aligned 2-3 digits)
        new FieldSpec("TypeCode", 40, 1),       // employee type code (H, P, G)
        new FieldSpec("Spaces1", 41, 1),        // space
        new FieldSpec("DateOfBirth", 42, 9),    // MM/DD/YY format with optional spaces
        new FieldSpec("Age", 51, 5),            // age (formatted as "(20)")
        new FieldSpec("Ssn", 56, 12),           // format: "000-00-0000"
        new FieldSpec("Spaces2", 68, 4),        // spaces
        new FieldSpec("Wages", 72, 12),         // right-aligned decimal (handles wages over $100k)
        new FieldSpec("Hours", 84, 12),         // right-aligned decimal
        new FieldSpec("Points", 91, -1),        // variable length, extracted to "(" character
        new FieldSpec("Marker", 98, 6),         // NEW marker or <21 marker: "(   )", "(<21)", "(NEW)"
        new FieldSpec("TermDate", 104, 8),      // termination date: MM/DD/YY
        new FieldSpec("Spaces3", 112, 6),       // spaces
        new FieldSpec("Balance", 118, 14),      // current balance (right-aligned)
        new FieldSpec("YearsInPlan", 132, 3)    // service years (right-aligned)
    };

    /// <summary>
    ///     Custom field extractor for Points field - extracts variable-length points up to "(" character.
    /// </summary>
    private static readonly Func<string, FieldSpec, string?> PointsCustomExtractor = (line, field) =>
    {
        if (field.Length == -1 && field.Name == "Points")
        {
            if (line.Length < field.StartPos)
            {
                return string.Empty;
            }

            int parenPos = line.IndexOf('(', field.StartPos);
            if (parenPos > field.StartPos)
            {
                return line.Substring(field.StartPos, parenPos - field.StartPos);
            }
            return line.Substring(field.StartPos);
        }
        return null; // Let default extraction handle it
    };

    /// <summary>
    ///     Parses all individual employee records from the PAY426N sub-report.
    ///     Uses fixed-column positions for PAY426N format.
    /// </summary>
    public static List<EmployeeRecord> ParseEmployeeRecords(string report, short profitYear = 2024)
    {
        string[] lines = report.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        List<EmployeeRecord> employees = new();

        // Minimum line length needed (up to hours field)
        FieldSpec hoursField = GetField(Pay426NFields, "Hours");
        int minLineLength = hoursField.StartPos + hoursField.Length;

        foreach (string line in lines)
        {
            // Stop parsing when we hit TOTAL EMPS line (PAY426N format) or TOTAL NON-EMP BENEFICIAIRIES (Report 10)
            if (line.Contains("TOTAL EMPS:") || line.Contains("TOTAL NON-EMP BENEFICIAIRIES:"))
            {
                break;
            }

            // Skip if line is too short
            if (line.Length < minLineLength)
            {
                continue;
            }

            // Check for badge number at position 3-9 (allowing for "I" prefix for inactive employees)
            // For Report 10 (NonEmployeeBeneficiaries), badge number is 0 (single digit)
            string badgeArea = line.Substring(0, Math.Min(10, line.Length));
            if (!Regex.IsMatch(badgeArea, @"^(I\s+|\s+)\d{1,7}"))
            {
                continue;
            }

            try
            {
                // Extract employee status from line prefix
                // ' ' = Active, 'I' = Inactive, 'T' = Terminated
                string statusStr = ExtractField(line, GetField(Pay426NFields, "Status"));
                char? employeeStatus = statusStr.Length > 0 ? statusStr[0] : null;
                if (employeeStatus == ' ')
                {
                    employeeStatus = 'A'; // Active
                }

                // Extract badge number
                string badgeStr = ExtractFieldTrimmed(Pay426NFields, line, "Badge");
                if (!int.TryParse(badgeStr, out int badge))
                {
                    continue;
                }

                // Extract name
                string name = ExtractFieldTrimmed(Pay426NFields, line, "Name");

                // Extract store number
                string storeStr = ExtractFieldTrimmed(Pay426NFields, line, "Store");
                if (!short.TryParse(storeStr, out short store))
                {
                    continue;
                }

                // Extract employee type code
                string typeCodeStr = ExtractField(line, GetField(Pay426NFields, "TypeCode"));
                char typeCode = typeCodeStr.Length > 0 ? typeCodeStr[0] : ' ';

                // Extract date of birth
                string dobStr = ExtractFieldTrimmed(Pay426NFields, line, "DateOfBirth");
                DateOnly dob = ParseDateOfBirth(dobStr);

                // Extract age from report (format: "(99)")
                string ageStr = ExtractFieldTrimmed(Pay426NFields, line, "Age");
                byte age = 0;
                if (!string.IsNullOrWhiteSpace(ageStr))
                {
                    // Remove parentheses and parse
                    string ageNumStr = ageStr.Replace("(", "").Replace(")", "").Trim();
                    if (!string.IsNullOrEmpty(ageNumStr) && byte.TryParse(ageNumStr, out byte parsedAge))
                    {
                        age = parsedAge;
                    }
                }

                // Extract SSN
                string ssnStr = ExtractFieldTrimmed(Pay426NFields, line, "Ssn").Replace("-", "");

                // Extract wages
                string wagesStr = ExtractFieldTrimmed(Pay426NFields, line, "Wages");
                decimal wages = ParseDecimal(wagesStr);

                // Extract hours
                string hoursStr = ExtractFieldTrimmed(Pay426NFields, line, "Hours");

                // Split on whitespace and take first token to get just the hours value
                string hoursToken = hoursStr.Split([' '], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";

                if (string.IsNullOrWhiteSpace(hoursToken))
                {
                    continue;
                }

                decimal hours = ParseDecimal(hoursToken);

                // Extract points (variable length field, extracted to "(" character)
                decimal points = 0;
                string pointsStr = ExtractFieldTrimmed(Pay426NFields, line, "Points", PointsCustomExtractor);
                if (!string.IsNullOrWhiteSpace(pointsStr))
                {
                    try
                    {
                        points = ParseDecimal(pointsStr);
                    }
                    catch
                    {
                        points = 0;
                    }
                }

                // Extract NEW/Under21 markers
                bool isUnder21 = false;
                bool isNew = false;
                string newMarker = ExtractField(line, GetField(Pay426NFields, "Marker"));
                if (!string.IsNullOrEmpty(newMarker))
                {
                    isNew = newMarker.Contains("NEW");
                    isUnder21 = newMarker.Contains("<21");
                }

                // Extract termination date
                DateOnly? terminationDate = null;
                string termDateStr = ExtractFieldTrimmed(Pay426NFields, line, "TermDate");
                if (!string.IsNullOrWhiteSpace(termDateStr) && termDateStr.Contains('/'))
                {
                    try
                    {
                        terminationDate = ParseTerminationDate(termDateStr, profitYear);
                    }
                    catch
                    {
                        terminationDate = null;
                    }
                }

                // Extract balance
                decimal balance = 0;
                string balanceStr = ExtractFieldTrimmed(Pay426NFields, line, "Balance");
                if (!string.IsNullOrWhiteSpace(balanceStr))
                {
                    try
                    {
                        balance = ParseDecimal(balanceStr);
                    }
                    catch
                    {
                        balance = 0;
                    }
                }

                // Extract years in plan
                short yearsInPlan = 0;
                string svcStr = ExtractFieldTrimmed(Pay426NFields, line, "YearsInPlan");
                if (!string.IsNullOrWhiteSpace(svcStr) && short.TryParse(svcStr, out short svc))
                {
                    yearsInPlan = svc;
                }

                // Infer employee status from termination date if not explicitly marked
                // READY reports don't prefix terminated employee lines with 'T', but they do have TERM DTE
                if (terminationDate.HasValue && employeeStatus == 'A')
                {
                    employeeStatus = 'T'; // Override to Terminated if termination date present
                }

                employees.Add(new EmployeeRecord(badge, name, store, typeCode, dob, age, ssnStr, wages, hours, points, isNew, isUnder21, balance, yearsInPlan, employeeStatus, terminationDate));
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
    ///     Maps an EmployeeRecord from the PAY426N sub-report to a YearEndProfitSharingReportDetail.
    /// </summary>
    private static YearEndProfitSharingReportDetail MapToReportDetail(EmployeeRecord emp, short profitYear)
    {
        // Use age from report, not calculated - COBOL sets age to 99 for beneficiaries (PAY426N.cbl line 1610)
        byte age = emp.Age;

        // Convert DateOnly.MinValue (from "00/00/00" in READY) to 1/1/1900 to match SMART's representation
        DateOnly dateOfBirth = emp.DateOfBirth == DateOnly.MinValue
            ? new DateOnly(1900, 1, 1)
            : emp.DateOfBirth;

        return new YearEndProfitSharingReportDetail
        {
            BadgeNumber = emp.BadgeNumber,
            ProfitYear = profitYear,
//            PriorProfitYear = (short)(profitYear - 1),
            EmployeeName = emp.Name,
            StoreNumber = emp.StoreNumber,
            EmployeeTypeCode = emp.EmployeeTypeCode,
            EmployeeTypeName = MapEmployeeTypeName(emp.EmployeeTypeCode),
            DateOfBirth = dateOfBirth,
            Age = age,
            Ssn = emp.Ssn,
            Wages = emp.Wages,
   //         PriorBalance = 0, // Not available in PAY426 report
            // Truncate hours to match SMART's behavior (COBOL uses S-HRS integer part)
            Hours = Math.Truncate(emp.Hours),
            Points = (short)emp.Points,
            // Calculate IsUnder21 from age, not from marker (NEW employees don't have <21 marker)
            IsUnder21 = age < 21,
            IsNew = emp.IsNew,
            EmployeeStatus = emp.EmployeeStatus, // Extracted from line prefix (A/I/T)
            Balance = emp.Balance, // Extracted from CURR.BALANCE field (token 3)
            YearsInPlan = emp.YearsInPlan, // Extracted from SVC field (token 4)
            TerminationDate = emp.TerminationDate, // Extracted from TERM DTE field (token 2)
            FirstContributionYear = null, // Not available in PAY426N report
            IsExecutive = false // Not available in PAY426N report
        };
    }

    public record EmployeeRecord(
        int BadgeNumber,
        string Name,
        short StoreNumber,
        char EmployeeTypeCode,
        DateOnly DateOfBirth,
        byte Age,          // Age extracted from report (not calculated) - COBOL sets to 99 for beneficiaries
        string Ssn,
        decimal Wages,
        decimal Hours,
        decimal Points,
        bool IsNew,
        bool IsUnder21,
        decimal Balance,
        short YearsInPlan,
        char? EmployeeStatus,
        DateOnly? TerminationDate);
}
