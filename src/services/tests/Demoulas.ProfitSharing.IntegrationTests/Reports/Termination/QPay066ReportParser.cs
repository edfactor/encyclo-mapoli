namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

/// <summary>
///     Parser for QPAY066 Termination - Profit Sharing report.
///     Extracts all fields from the fixed-width column format.
/// </summary>
public static class QPay066ReportParser
{
    /// <summary>
    ///     Parses the QPAY066 report text and returns all data rows as QPay066Record objects.
    /// </summary>
    public static List<QPay066Record> ParseRecords(string reportText)
    {
        List<QPay066Record> records = new();

        // Split into lines
        string[] lines = reportText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            // Skip lines that are too short or are headers
            // Minimum length should include at least up to vested balance field
            if (line.Length < 108)
            {
                continue;
            }

            // Check if line starts with spaces followed by digits (data row)
            string trimStart = line.TrimStart();
            if (string.IsNullOrWhiteSpace(trimStart) || !char.IsDigit(trimStart[0]))
            {
                continue;
            }

            // Based on the fixed column format (right-aligned fields with variable spacing):
            // Columns 0-11: Badge/PSN # (right-aligned, 12 chars)
            // Columns 12-30: Employee Name (19 chars)
            // Columns 31-43: Beginning Balance (right-aligned, 13 chars including possible trailing minus)
            // Columns 44-57: Beneficiary Allocation (right-aligned, 14 chars including possible trailing minus)
            // Columns 58-70: Distribution Amount (right-aligned, 13 chars including possible trailing minus)
            // Columns 70-82: Forfeit (right-aligned, 13 chars including possible trailing minus)
            // Columns 83-95: Ending Balance (right-aligned, 13 chars including possible trailing minus)
            // Columns 96-109: Vested Balance (right-aligned, 14 chars including possible trailing minus)
            // Columns 110-115: Date Term (6 chars, YYMMDD format or empty)
            // Columns 118-124: YTD VST PS HRS (7 chars)
            // Columns 125+: PCT AGE E_C (space-delimited trailing fields)

            string badgeOrPSN = line.Substring(0, Math.Min(12, line.Length)).Trim();

            // Parse badge and PSN suffix
            int badge;
            short psnSuffix = 0;
            if (badgeOrPSN.Length > 7)
            {
                // PSN format: badge number followed by 4-digit suffix (e.g., "7039171000")
                badge = int.Parse(badgeOrPSN.Substring(0, badgeOrPSN.Length - 4));
                psnSuffix = short.Parse(badgeOrPSN.Substring(badgeOrPSN.Length - 4));
            }
            else
            {
                badge = int.Parse(badgeOrPSN);
            }

            // Parse employee name (positions 12-30, 19 chars)
            string employeeName = line.Substring(12, 19).Trim();

            // Parse monetary amounts
            // Note: Fields are right-aligned with variable spacing between them
            // Each field width includes space for a potential trailing minus sign
            decimal beginningBalance = ParseMonetaryAmount(line, 31, 13);
            decimal beneficiaryAllocation = ParseMonetaryAmount(line, 44, 14);
            decimal distributionAmount = ParseMonetaryAmount(line, 58, 13);
            decimal forfeit = ParseMonetaryAmount(line, 70, 13);
            decimal endingBalance = ParseMonetaryAmount(line, 83, 13);
            decimal vestedBalance = ParseMonetaryAmount(line, 96, 14);

            // Parse date term (YYMMDD format or empty) - starts at position 110
            DateOnly? dateTerm = null;
            if (line.Length > 110)
            {
                string dateTermStr = line.Substring(110, Math.Min(6, line.Length - 110)).Trim();
                if (dateTermStr.Length == 6 && int.TryParse(dateTermStr, out _))
                {
                    int year = int.Parse(dateTermStr.Substring(0, 2));
                    int month = int.Parse(dateTermStr.Substring(2, 2));
                    int day = int.Parse(dateTermStr.Substring(4, 2));

                    // Convert 2-digit year to 4-digit (assume 20xx for years 00-99)
                    year += 2000;

                    dateTerm = new DateOnly(year, month, day);
                }
            }

            // Parse YTD VST PS HRS - starts at position 118
            decimal ytdVstPsHours = 0m;
            if (line.Length > 118)
            {
                string hoursStr = line.Substring(118, Math.Min(7, line.Length - 118)).Trim();
                if (decimal.TryParse(hoursStr, out decimal hours))
                {
                    ytdVstPsHours = hours;
                }
            }

            // Parse vested percent (PCT), AGE, and executive code from remaining fields
            // These fields are space-delimited at the end of the line
            decimal vestedPercent = 0m;
            int? age = null;
            char? executiveCode = null;

            if (line.Length > 125)
            {
                string remainingFields = line.Substring(125).Trim();
                string[] parts = remainingFields.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // PCT is first (2-3 digits)
                if (parts.Length > 0 && decimal.TryParse(parts[0], out decimal pct))
                {
                    vestedPercent = pct;
                }

                // AGE is second (2-3 digits)
                if (parts.Length > 1 && int.TryParse(parts[1], out int ageValue))
                {
                    age = ageValue;
                }

                // Executive code is third (single character), optional
                if (parts.Length > 2 && parts[2].Length > 0)
                {
                    executiveCode = parts[2][0];
                }
            }

            records.Add(new QPay066Record
            {
                BadgeNumber = badge,
                PsnSuffix = psnSuffix,
                EmployeeName = employeeName,
                BeginningBalance = beginningBalance,
                BeneficiaryAllocation = beneficiaryAllocation,
                DistributionAmount = distributionAmount,
                Forfeit = forfeit,
                EndingBalance = endingBalance,
                VestedBalance = vestedBalance,
                DateTerm = dateTerm,
                YtdVstPsHours = ytdVstPsHours,
                VestedPercent = vestedPercent,
                Age = age,
                ExecutiveCode = executiveCode
            });
        }

        return records;
    }

    /// <summary>
    ///     Parses a monetary amount from a fixed-width column.
    ///     Handles negative signs that appear after the number (e.g., "12,345.67-").
    /// </summary>
    private static decimal ParseMonetaryAmount(string line, int startIndex, int length)
    {
        if (line.Length < startIndex + length)
        {
            return 0m;
        }

        string amountStr = line.Substring(startIndex, length).Trim().Replace(",", "");

        if (string.IsNullOrWhiteSpace(amountStr))
        {
            return 0m;
        }

        // Check for trailing negative sign
        bool isNegative = amountStr.EndsWith('-');
        if (isNegative)
        {
            amountStr = amountStr.Substring(0, amountStr.Length - 1);
        }

        if (decimal.TryParse(amountStr, out decimal amount))
        {
            return isNegative ? -amount : amount;
        }

        throw new InvalidDataException($"Unable to parse monetary amount: '{amountStr}'");
    }

    /// <summary>
    ///     Backward-compatible method that returns a dictionary of badge number to vested balance.
    ///     For badge numbers that appear multiple times (employee + beneficiaries), uses the last occurrence.
    /// </summary>
    public static Dictionary<int, decimal> ParseBadgeToVestedBalance(string reportText)
    {
        List<QPay066Record> records = ParseRecords(reportText);
        return records
            .GroupBy(r => r.BadgeNumber)
            .ToDictionary(g => g.Key, g => g.Last().VestedBalance);
    }

    /// <summary>
    ///     Computes totals by summing the values from a list of records.
    ///     Note: All values are summed directly as they appear in the parsed records, including negative forfeitures.
    /// </summary>
    public static QPay066Totals ComputeTotals(List<QPay066Record> records)
    {
        return new QPay066Totals
        {
            AmountInProfitSharing = records.Sum(r => r.EndingBalance),
            VestedAmount = records.Sum(r => r.VestedBalance),
            TotalForfeitures = records.Sum(r => r.Forfeit), // Sum directly (forfeitures are already negative in records)
            TotalBeneficiaryAllocations = records.Sum(r => r.BeneficiaryAllocation)
        };
    }

    /// <summary>
    ///     Parses the totals section at the end of the QPAY066 report.
    /// </summary>
    public static QPay066Totals ParseTotals(string reportText)
    {
        string[] lines = reportText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        decimal? amountInProfitSharing = null;
        decimal? vestedAmount = null;
        decimal? totalForfeitures = null;
        decimal? totalBeneficiaryAllocations = null;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // Look for each total line by its label
            if (trimmedLine.StartsWith("AMOUNT IN PROFIT SHARING"))
            {
                amountInProfitSharing = ParseTotalLine(trimmedLine, "AMOUNT IN PROFIT SHARING");
            }
            else if (trimmedLine.StartsWith("VESTED AMOUNT"))
            {
                vestedAmount = ParseTotalLine(trimmedLine, "VESTED AMOUNT");
            }
            else if (trimmedLine.StartsWith("TOTAL FORFEITURES"))
            {
                totalForfeitures = ParseTotalLine(trimmedLine, "TOTAL FORFEITURES");
            }
            else if (trimmedLine.StartsWith("TOTAL BENEFICIARY ALLOCTIONS")) // Note: typo in original report
            {
                totalBeneficiaryAllocations = ParseTotalLine(trimmedLine, "TOTAL BENEFICIARY ALLOCTIONS");
            }
        }

        // Only return totals if all values were found
        if (amountInProfitSharing.HasValue && vestedAmount.HasValue &&
            totalForfeitures.HasValue && totalBeneficiaryAllocations.HasValue)
        {
            return new QPay066Totals
            {
                AmountInProfitSharing = amountInProfitSharing.Value,
                VestedAmount = vestedAmount.Value,
                TotalForfeitures = totalForfeitures.Value,
                TotalBeneficiaryAllocations = totalBeneficiaryAllocations.Value
            };
        }

        throw new Exception();
    }

    /// <summary>
    ///     Parses a total line by extracting the numeric value after the label.
    /// </summary>
    private static decimal ParseTotalLine(string line, string label)
    {
        // Remove the label and trim
        string valueStr = line.Substring(label.Length).Trim();

        // Handle trailing negative sign
        bool isNegative = valueStr.EndsWith('-');
        if (isNegative)
        {
            valueStr = valueStr.Substring(0, valueStr.Length - 1).Trim();
        }

        // Remove commas and parse
        valueStr = valueStr.Replace(",", "");

        if (decimal.TryParse(valueStr, out decimal value))
        {
            return isNegative ? -value : value;
        }

        throw new InvalidDataException($"Unable to parse total value from line: '{line}'");
    }
}
