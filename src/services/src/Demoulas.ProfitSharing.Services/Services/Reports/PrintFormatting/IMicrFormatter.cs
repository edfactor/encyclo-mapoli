namespace Demoulas.ProfitSharing.Services.PrintFormatting;

/// <summary>
/// Formats check MICR (Magnetic Ink Character Recognition) lines for bank processing.
/// </summary>
public interface IMicrFormatter
{
    /// <summary>
    /// Formats a MICR line for printing on a check with bank routing and account information.
    /// </summary>
    /// <param name="checkNumber">The check number to include in the MICR line.</param>
    /// <param name="amount">The check amount (used by some banks, optional for Newtek).</param>
    /// <returns>Formatted MICR line string ready for printer output.</returns>
    string FormatMicrLine(int checkNumber, decimal amount);
}
