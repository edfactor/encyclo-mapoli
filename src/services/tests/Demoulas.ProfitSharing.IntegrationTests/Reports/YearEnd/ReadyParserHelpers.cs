namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;

/// <summary>
///     Shared helper methods and types for parsing READY PAY426 reports.
///     Used by both Pay426Parser (main report) and Pay426NParser (sub-reports).
/// </summary>
internal static class ReadyParserHelpers
{
    /// <summary>
    ///     Field specification for fixed-width parsing of PAY426 report lines.
    /// </summary>
    internal sealed record FieldSpec(string Name, int StartPos, int Length);

    /// <summary>
    ///     Extracts a field value from a line using the field specification.
    ///     Returns empty string if the field position is beyond the line length.
    /// </summary>
    /// <param name="line">The line to extract from</param>
    /// <param name="field">Field specification</param>
    /// <param name="customExtractor">Optional custom extractor for special field handling (e.g., Points field with variable length).
    ///     Custom extractor should return null to fall back to default extraction.</param>
    internal static string ExtractField(string line, FieldSpec field, Func<string, FieldSpec, string?>? customExtractor = null)
    {
        if (line.Length < field.StartPos)
        {
            return string.Empty;
        }

        // Use custom extractor if provided and it returns a non-null value
        if (customExtractor != null)
        {
            string? customResult = customExtractor(line, field);
            if (customResult != null)
            {
                return customResult;
            }
            // Fall through to default extraction if custom extractor returned null
        }

        if (field.Length == -1)
        {
            // Variable length field - extract to end of line
            return line.Substring(field.StartPos);
        }

        int length = Math.Min(field.Length, line.Length - field.StartPos);
        return line.Substring(field.StartPos, length);
    }

    /// <summary>
    ///     Gets a field specification by name from an array of field specifications.
    /// </summary>
    internal static FieldSpec GetField(FieldSpec[] fields, string name)
    {
        return fields.First(f => f.Name == name);
    }

    /// <summary>
    ///     Extracts a field value by name and trims whitespace.
    ///     Common pattern for most field extractions.
    /// </summary>
    internal static string ExtractFieldTrimmed(FieldSpec[] fields, string line, string fieldName, Func<string, FieldSpec, string?>? customExtractor = null)
    {
        return ExtractField(line, GetField(fields, fieldName), customExtractor).Trim();
    }

    /// <summary>
    ///     Calculates age as of a given date.
    /// </summary>
    internal static int CalculateAge(DateOnly birthDate, DateOnly asOfDate)
    {
        int age = asOfDate.Year - birthDate.Year;
        if (asOfDate.Month < birthDate.Month || asOfDate.Month == birthDate.Month && asOfDate.Day < birthDate.Day)
        {
            age--;
        }

        return age;
    }

    /// <summary>
    ///     Parses date of birth from PAY426 format (MM/DD/YY).
    ///     Assumes years 00-25 are 2000s, 26-99 are 1900s (pivot at current year 2025).
    ///     Returns DateOnly.MinValue for invalid/unknown dates (e.g., "00/00/00" for beneficiaries with unknown DOB).
    /// </summary>
    internal static DateOnly ParseDateOfBirth(string dobStr)
    {
        string[] parts = dobStr.Split('/');
        if (parts.Length != 3)
        {
            return DateOnly.MinValue; // Invalid format
        }

        if (!int.TryParse(parts[0], out int month) ||
            !int.TryParse(parts[1], out int day) ||
            !int.TryParse(parts[2], out int year))
        {
            return DateOnly.MinValue; // Invalid number format
        }

        // Handle unknown dates (00/00/00) - common for beneficiaries
        if (month == 0 || day == 0)
        {
            return DateOnly.MinValue;
        }

        // Convert 2-digit year to 4-digit year
        // Pivot at current year (2025): 00-25 => 2000-2025, 26-99 => 1926-1999
        if (year <= 25)
        {
            year += 2000;
        }
        else
        {
            year += 1900;
        }

        // Validate the date is constructable
        try
        {
            return new DateOnly(year, month, day);
        }
        catch
        {
            return DateOnly.MinValue; // Invalid date (e.g., Feb 30)
        }
    }

    /// <summary>
    ///     Parses termination date from PAY426 format (MM/DD/YY).
    ///     Uses the report profit year as context - termination dates are always within
    ///     a few years of the report year (not decades like birth dates).
    /// </summary>
    internal static DateOnly ParseTerminationDate(string termDateStr, short profitYear)
    {
        string[] parts = termDateStr.Split('/');
        if (parts.Length != 3)
        {
            throw new FormatException($"Invalid date format: {termDateStr}");
        }

        int month = int.Parse(parts[0]);
        int day = int.Parse(parts[1]);
        int year = int.Parse(parts[2]);

        // For termination dates, use the report year's century
        // Example: For 2025 report, "10/13/25" => 2025, not 1925
        int century = (profitYear / 100) * 100; // 2025 => 2000
        year += century;

        return new DateOnly(year, month, day);
    }

    /// <summary>
    ///     Maps employee type code to employee type name.
    ///     Supports both PAY426 and PAY426N report formats.
    /// </summary>
    internal static string MapEmployeeTypeName(char typeCode)
    {
        return typeCode switch
        {
            'P' => "Part Time",
            'H' => "Hourly",
            'G' => "Grocery Manager",
            'F' => "Full Time", // F appears in PAY426 reports
            _ => "Unknown"
        };
    }

    /// <summary>
    ///     Parses a decimal value from a string, removing commas and trimming whitespace.
    /// </summary>
    internal static decimal ParseDecimal(string value)
    {
        return decimal.Parse(value.Replace(",", "").Trim());
    }

    /// <summary>
    ///     Parses an integer value from a string, removing commas and trimming whitespace.
    /// </summary>
    internal static int ParseInt(string value)
    {
        return int.Parse(value.Replace(",", "").Trim());
    }
}
