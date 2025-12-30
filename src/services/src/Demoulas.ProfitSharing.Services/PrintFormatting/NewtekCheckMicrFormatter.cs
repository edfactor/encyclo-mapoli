namespace Demoulas.ProfitSharing.Services.PrintFormatting;

/// <summary>
/// MICR formatter for Newtek Bank checks.
/// Routing: 026004297
/// Account: 0375495656
/// Format: <{checkNumber}<!: !02!60!04!29!7:! 3!7 !54!95!65! 6!<
/// </summary>
public sealed class NewtekCheckMicrFormatter : IMicrFormatter
{
    /// <summary>
    /// Formats a MICR line for Newtek Bank checks using their specific spacing requirements.
    /// </summary>
    /// <param name="checkNumber">The check number (e.g., 10001 from random 5K-10K initialization).</param>
    /// <param name="amount">The check amount (not used in Newtek MICR format).</param>
    /// <returns>Formatted MICR line with '!' spacing delimiters: <{checkNumber}<!: !02!60!04!29!7:! 3!7 !54!95!65! 6!<</returns>
    public string FormatMicrLine(int checkNumber, decimal amount)
    {
        // Newtek MICR format: <checkNumber<!: !routing:! account!<
        // '!' characters represent spacing for MICR font alignment
        // < and > are MICR control characters for field boundaries

        return $"<{checkNumber}<!: !0!2!6!0!0!4!2!9!7:! 0!3!7!5!4!9!5!6!5!6!<";
    }
}
