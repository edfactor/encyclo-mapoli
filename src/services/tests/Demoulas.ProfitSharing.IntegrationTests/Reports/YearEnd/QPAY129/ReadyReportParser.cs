using System.Globalization;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.QPAY129;

/// <summary>
/// Parser for READY QPAY129 fixed-field report format to extract totals
/// </summary>
internal static class ReadyReportParser
{
    /// <summary>
    /// Parses READY QPAY129 report and extracts totals from the last section
    /// </summary>
    /// <param name="reportText">Full report text</param>
    /// <returns>Response object with parsed totals</returns>
    public static DistributionsAndForfeitureTotalsResponse ParseTotals(string reportText)
    {
        if (string.IsNullOrWhiteSpace(reportText))
        {
            throw new ArgumentException("Report text cannot be null or empty", nameof(reportText));
        }

        var lines = reportText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.TrimEnd())
            .ToArray();

        // Parse COMPANY TOTAL line
        // Format: "                       COMPANY TOTAL             32413950.14    429484.90 2637281.70    82184.28"
        var companyTotalLine = lines.FirstOrDefault(l => l.Contains("COMPANY TOTAL"));
        if (companyTotalLine == null)
        {
            throw new InvalidOperationException("COMPANY TOTAL line not found in report");
        }

        // Extract the part after "COMPANY TOTAL" and split by whitespace
        var totalsPart = companyTotalLine.Substring(companyTotalLine.IndexOf("COMPANY TOTAL") + "COMPANY TOTAL".Length);
        var companyTotals = ParseNumbersFromFixedField(totalsPart);

        if (companyTotals.Count != 4)
        {
            throw new InvalidOperationException(
                $"Expected 4 numbers on COMPANY TOTAL line, found {companyTotals.Count}");
        }

        // Parse forfeit breakdown lines
        var forfeitRegular = ExtractNumberAfterLabel(lines, "FORFEIT");
        var forfeitAdministrative = ExtractNumberAfterLabel(lines, "ADMINISTRATIVE");
        var forfeitClassAction = ExtractNumberAfterLabel(lines, "CLASS ACTION");

        // Return response object matching service output
        return new DistributionsAndForfeitureTotalsResponse
        {
            ReportName = "Distributions and Forfeitures",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = new DateOnly(2025, 1, 1), // From test request
            EndDate = new DateOnly(2025, 12, 31), // From test request
            DistributionTotal = companyTotals[0],
            StateTaxTotal = companyTotals[1],      // Swapped: position 1 is State Tax
            FederalTaxTotal = companyTotals[2],    // Swapped: position 2 is Federal Tax
            ForfeitureTotal = companyTotals[3],
            ForfeitureRegularTotal = forfeitRegular,
            ForfeitureAdministrativeTotal = forfeitAdministrative,
            ForfeitureClassActionTotal = forfeitClassAction,
            StateTaxTotals = new Dictionary<string, decimal>(), // Not available in READY report
            Response = new PaginatedResponseDto<DistributionsAndForfeitureResponse>
            {
                Results = new List<DistributionsAndForfeitureResponse>()
            }
        };
    }

    /// <summary>
    /// Parses numbers from a fixed-field format, handling trailing minus signs
    /// </summary>
    /// <param name="text">Text containing numbers separated by whitespace</param>
    /// <returns>List of parsed decimal numbers</returns>
    private static List<decimal> ParseNumbersFromFixedField(string text)
    {
        var numbers = new List<decimal>();
        var tokens = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            var cleanedToken = token.Trim();
            if (string.IsNullOrEmpty(cleanedToken))
            {
                continue;
            }

            // Handle trailing minus sign (e.g., "1000.00-" becomes "-1000.00")
            bool isNegative = cleanedToken.EndsWith('-');
            var numberPart = isNegative ? cleanedToken[..^1] : cleanedToken;

            if (decimal.TryParse(numberPart, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
            {
                numbers.Add(isNegative ? -number : number);
            }
        }

        return numbers;
    }

    /// <summary>
    /// Finds a line containing the label and extracts the number after it
    /// </summary>
    /// <param name="lines">All report lines</param>
    /// <param name="label">Label to search for (e.g., "FORFEIT", "ADMINISTRATIVE")</param>
    /// <returns>The decimal number found after the label</returns>
    private static decimal ExtractNumberAfterLabel(string[] lines, string label)
    {
        // Search from the end of the file (totals section is at the bottom)
        // Skip lines that are just dashes or equals (separator lines)
        // For "FORFEIT", we want one that's NOT "FORFEIT TOTAL"
        string? targetLine = null;

        for (int i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i];

            // Skip separator lines
            if (line.Trim().All(c => c == '-' || c == '=' || char.IsWhiteSpace(c)))
            {
                continue;
            }

            // Check if this line contains our label
            if (label == "FORFEIT")
            {
                // For FORFEIT, find one that doesn't say "FORFEIT TOTAL" and isn't a column header
                if (line.Contains(label) &&
                    !line.Contains("FORFEIT TOTAL") &&
                    !line.Contains("DISTRIBUTION") &&
                    line.Trim().StartsWith(label))
                {
                    targetLine = line;
                    break;
                }
            }
            else if (line.Contains(label) && line.Trim().StartsWith(label))
            {
                targetLine = line;
                break;
            }
        }

        if (targetLine == null)
        {
            throw new InvalidOperationException($"{label} line not found in report");
        }

        // Extract the part after the label
        var numberPart = targetLine.Substring(targetLine.IndexOf(label) + label.Length);
        var numbers = ParseNumbersFromFixedField(numberPart);

        if (numbers.Count == 0)
        {
            throw new InvalidOperationException($"No number found on {label} line");
        }

        // Return the first (and typically only) number after the label
        return numbers[0];
    }
}
