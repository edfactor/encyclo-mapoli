namespace Demoulas.ProfitSharing.Services.PrintFormatting;

/// <summary>
/// Utility for masking SSN values in MICR and print output for privacy compliance.
/// </summary>
public static class SsnMaskingUtility
{
    /// <summary>
    /// Masks SSN by showing only last 4 digits with leading zeros.
    /// Example: 123-45-6789 -> 000006789
    /// </summary>
    /// <param name="ssn">Full SSN in any format (with or without dashes).</param>
    /// <returns>Masked SSN with format 000006789 (last 4 digits only).</returns>
    public static string MaskSsn(string ssn)
    {
        if (string.IsNullOrWhiteSpace(ssn))
        {
            return "000000000";
        }

        // Remove all non-digit characters
        var digitsOnly = new string(ssn.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length < 4)
        {
            return "000000000";
        }

        // Get last 4 digits and pad with leading zeros to 9 digits
        var lastFour = digitsOnly[^4..];
        return $"00000{lastFour}";
    }
}
