using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using static Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.ReadyParserHelpers;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.PAY426N;

/// <summary>
///     Parses the PAY426N-9 summary report (R8-PAY426N-9) from READY and extracts line item totals.
///     This is the summary page that aggregates counts and totals across all PAY426N sub-reports.
/// </summary>
public static class Pay426N9Parser
{
    /// <summary>
    ///     Parses the PAY426N-9 summary report and returns a YearEndProfitSharingReportSummaryResponse
    ///     with line items for each category (1-8, E, X, N).
    /// </summary>
    public static YearEndProfitSharingReportSummaryResponse Parse(string reportText)
    {
        var lineItems = new List<YearEndProfitSharingReportSummaryLineItem>();
        var lines = reportText.Split('\n');

        string? currentSubgroup = null;

        foreach (var line in lines)
        {
            // Track subgroup headers
            if (line.Contains("ACTIVE AND INACTIVE:"))
            {
                currentSubgroup = "ACTIVE AND INACTIVE";
                continue;
            }
            if (line.Contains("TERMINATED:"))
            {
                currentSubgroup = "TERMINATED";
                continue;
            }

            // Skip empty or short lines
            if (string.IsNullOrWhiteSpace(line) || line.Length < 10)
            {
                continue;
            }

            // Check if line starts with a valid prefix (E, 1-8, X, N)
            char firstChar = line[0];
            if (!IsValidPrefix(firstChar))
            {
                continue;
            }

            // Verify the " - " separator exists at position 1-3
            if (line.Length < 4 || line.Substring(1, 3) != " - ")
            {
                continue;
            }

            try
            {
                // Parse line items using whitespace-delimited approach with ReadyParserHelpers
                // Format: "PREFIX - DESCRIPTION    COUNT   WAGES   BALANCE"
                // Example: "1 -    AGE 18-20 WITH >= 1000 PS HOURS                          205   5,198,074.91        54,116.75"
                // Note: Numbers are right-aligned with variable spacing, so fixed-width parsing doesn't work

                string prefix = firstChar.ToString();

                // Extract the remainder after " - " prefix
                string remainder = line.Substring(4);

                // Split by all whitespace and filter empty entries
                string[] allTokens = remainder.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (allTokens.Length < 3)
                {
                    continue; // Need at least: count, wages, balance
                }

                // Last 3 tokens are: COUNT, WAGES, BALANCE
                string countStr = allTokens[^3];
                string wagesStr = allTokens[^2];
                string balanceStr = allTokens[^1];

                // Everything before the last 3 tokens is the description
                string description = string.Join(" ", allTokens.Take(allTokens.Length - 3)).Trim();

                // Parse numeric values using ReadyParserHelpers for consistency
                int count = ParseInt(countStr);
                decimal wages = ParseDecimal(wagesStr);
                decimal balance = ParseDecimal(balanceStr);

                // Determine subgroup - E and X don't belong to a subgroup, but N appears under TERMINATED
                string subgroup = prefix is "E" or "X" ? string.Empty : currentSubgroup ?? string.Empty;

                // Hours and points are only meaningful for Line 2 (>= AGE 21 WITH >= 1000 PS HOURS)
                // R8-PAY426N-9 doesn't provide hours/points; they come from R8-PAY426-TOT detail report
                // For non-Line-2 items, set to null (displayed as blank in UI)
                bool isLine2 = prefix == "2";

                lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
                {
                    Subgroup = subgroup,
                    LineItemPrefix = prefix,
                    LineItemTitle = description,
                    NumberOfMembers = count,
                    TotalWages = wages,
                    TotalBalance = balance,
                    TotalHours = isLine2 ? 0 : null, // Only Line 2 has hours (populated from backend service)
                    TotalPoints = isLine2 ? 0 : null // Only Line 2 has points (populated from backend service)
                });
            }
            catch
            {
                // Skip malformed lines (expected for headers, page markers, etc.)
            }
        }

        if (lineItems.Count == 0)
        {
            throw new InvalidOperationException("Failed to parse any line items from PAY426N-9 summary report");
        }

        return new YearEndProfitSharingReportSummaryResponse
        {
            LineItems = lineItems
        };
    }

    /// <summary>
    ///     Checks if the character is a valid line item prefix.
    /// </summary>
    private static bool IsValidPrefix(char c)
    {
        return c == 'E' || c == 'X' || c == 'N' || (c >= '1' && c <= '8');
    }
}
