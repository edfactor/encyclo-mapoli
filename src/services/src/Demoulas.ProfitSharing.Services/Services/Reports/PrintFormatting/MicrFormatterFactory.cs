using Microsoft.Extensions.Configuration;

namespace Demoulas.ProfitSharing.Services.Services.Reports.PrintFormatting;

/// <summary>
/// Factory implementation for creating bank-specific MICR formatters.
/// </summary>
public sealed class MicrFormatterFactory : IMicrFormatterFactory
{
    private const string NewtekRoutingNumber = "026004297";
    private readonly IConfiguration _configuration;

    public MicrFormatterFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

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
            NewtekRoutingNumber => new NewtekCheckMicrFormatter(GetRequiredNewtekAccountNumber()),
            _ => throw new NotSupportedException($"Bank routing number '{bankRoutingNumber}' is not supported. Only Newtek Bank ({NewtekRoutingNumber}) is currently configured.")
        };
    }

    private string GetRequiredNewtekAccountNumber()
    {
        var accountNumber = _configuration["Printing:Micr:Newtek:AccountNumber"];
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new InvalidOperationException("Newtek MICR account number is not configured. Set 'Printing:Micr:Newtek:AccountNumber' via user secrets or environment variables.");
        }

        accountNumber = new string(accountNumber.Where(char.IsDigit).ToArray());
        if (accountNumber.Length == 0)
        {
            throw new InvalidOperationException("Newtek MICR account number is not configured (no digits found). Set 'Printing:Micr:Newtek:AccountNumber'.");
        }

        return accountNumber;
    }
}
