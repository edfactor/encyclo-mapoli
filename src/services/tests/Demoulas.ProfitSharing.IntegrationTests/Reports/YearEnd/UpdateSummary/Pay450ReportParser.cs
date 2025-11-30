using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ReadyParserHelpers;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.UpdateSummary;

// Parses a line of the READY Pay450 report
public static class Pay450ReportParser
{
    // Field specifications for PAY450 report format
    private static readonly FieldSpec[] Fields =
    [
        new("BadgeAndStore", 0, 12),
        new("Name", 12, 29),
        new("BeforeAmount", 42, 13),      // 12 digits + 1 sign
        new("BeforeVested", 58, 13),      // 12 digits + 1 sign
        new("BeforeYears", 72, 5),
        new("BeforeEnroll", 79, 5),
        new("AfterAmount", 92, 13),       // 12 digits + 1 sign
        new("AfterVested", 109, 13),      // 12 digits + 1 sign
        new("AfterYears", 126, 5),
        new("AfterEnroll", 131, -1)       // Variable length to end of line
    ];

    public static Pay450Record ParseLine(string line)
    {
        return new Pay450Record
        {
            BadgeAndStore = ExtractFieldTrimmed(Fields, line, "BadgeAndStore"),
            Name = ExtractFieldTrimmed(Fields, line, "Name"),
            BeforeAmount = ParseSignedDecimal(line, "BeforeAmount"),
            BeforeVested = ParseSignedDecimal(line, "BeforeVested"),
            BeforeYears = ParseNullableInt(line, "BeforeYears"),
            BeforeEnroll = ParseNullableInt(line, "BeforeEnroll"),
            AfterAmount = ParseSignedDecimal(line, "AfterAmount"),
            AfterVested = ParseSignedDecimal(line, "AfterVested"),
            AfterYears = ParseNullableInt(line, "AfterYears"),
            AfterEnroll = ParseNullableInt(line, "AfterEnroll")
        };
    }

    /// <summary>
    /// Parses a nullable integer field from the line.
    /// Returns null if the field is empty or whitespace.
    /// </summary>
    private static int? ParseNullableInt(string line, string fieldName)
    {
        string value = ExtractFieldTrimmed(Fields, line, fieldName);
        return string.IsNullOrWhiteSpace(value) ? null : ParseInt(value);
    }

    /// <summary>
    /// Parses a decimal field with trailing sign (PAY450 format).
    /// Format: 12 digits followed by optional sign character ('-' for negative).
    /// Example: "    1,234.56-" => -1234.56
    /// </summary>
    private static decimal ParseSignedDecimal(string line, string fieldName)
    {
        FieldSpec field = GetField(Fields, fieldName);

        if (line.Length < field.StartPos)
        {
            return 0;
        }

        // Extract the numeric portion (12 characters)
        int length = Math.Min(12, line.Length - field.StartPos);
        string digits = line.Substring(field.StartPos, length).Trim();

        // Extract the sign character (13th character if present)
        string sign = (line.Length > field.StartPos + 12)
            ? line.Substring(field.StartPos + 12, 1).Trim()
            : "";

        if (string.IsNullOrWhiteSpace(digits) && string.IsNullOrWhiteSpace(sign))
        {
            return 0;
        }

        // Build the signed number string
        string signedValue = sign + digits;
        return ParseDecimal(signedValue);
    }
}
