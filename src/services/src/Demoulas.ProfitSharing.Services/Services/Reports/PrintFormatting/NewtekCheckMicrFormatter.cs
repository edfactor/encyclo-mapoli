namespace Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;

/// <summary>
/// MICR formatter for Newtek Bank checks.
/// Routing: 026004297
/// Account: configured externally
/// Format: <{checkNumber}<!: !02!60!04!29!7:! {accountDigits}!<
/// </summary>
public sealed class NewtekCheckMicrFormatter : IMicrFormatter
{
    private readonly string _accountNumber;

    public NewtekCheckMicrFormatter(string accountNumber)
    {
        _accountNumber = accountNumber;
    }

    /// <summary>
    /// Formats a MICR line for Newtek Bank checks using their specific spacing requirements.
    /// </summary>
    /// <param name="checkNumber">The check number (e.g., 10001 from random 5K-10K initialization).</param>
    /// <param name="amount">The check amount (not used in Newtek MICR format).</param>
    /// <returns>Formatted MICR line with '!' spacing delimiters.</returns>
    public string FormatMicrLine(int checkNumber, decimal amount)
    {
        // Newtek MICR format: <checkNumber<!: !routing:! account!<
        // '!' characters represent spacing for MICR font alignment
        // < and > are MICR control characters for field boundaries

        var accountDigits = ToMicrDigits(_accountNumber);
        return $"<{checkNumber}<!: !0!2!6!0!0!4!2!9!7:! {accountDigits}!<";
    }

    private static string ToMicrDigits(string digits)
    {
        var builder = new System.Text.StringBuilder(digits.Length * 2);
        for (var i = 0; i < digits.Length; i++)
        {
            if (i > 0)
            {
                _ = builder.Append('!');
            }

            _ = builder.Append(digits[i]);
        }

        return builder.ToString();
    }
}
