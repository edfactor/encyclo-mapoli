namespace Demoulas.ProfitSharing.Services.PrintFormatting;

/// <summary>
/// Factory for creating bank-specific MICR formatters based on routing number.
/// </summary>
public interface IMicrFormatterFactory
{
    /// <summary>
    /// Gets the appropriate MICR formatter for the specified bank routing number.
    /// </summary>
    /// <param name="bankRoutingNumber">The bank's routing number (e.g., "026004297" for Newtek).</param>
    /// <returns>MICR formatter implementation for the specified bank.</returns>
    IMicrFormatter GetFormatter(string bankRoutingNumber);
}
