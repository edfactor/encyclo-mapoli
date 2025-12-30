namespace Demoulas.ProfitSharing.Services.PrintFormatting;

/// <summary>
/// Factory implementation for creating bank-specific MICR formatters.
/// </summary>
public sealed class MicrFormatterFactory : IMicrFormatterFactory
{
    private const string NewtekRoutingNumber = "026004297";

    /// <summary>
    /// Gets the appropriate MICR formatter for the specified bank routing number.
    /// </summary>
    /// <param name="bankRoutingNumber">The bank's routing number.</param>
    /// <returns>MICR formatter implementation for the specified bank.</returns>
    /// <exception cref="NotSupportedException">Thrown when the routing number is not supported.</exception>
    public IMicrFormatter GetFormatter(string bankRoutingNumber)
    {
        return bankRoutingNumber switch
        {
            NewtekRoutingNumber => new NewtekCheckMicrFormatter(),
            _ => throw new NotSupportedException($"Bank routing number '{bankRoutingNumber}' is not supported. Only Newtek Bank ({NewtekRoutingNumber}) is currently configured.")
        };
    }
}
