namespace Demoulas.ProfitSharing.Common.Extensions;

/// <summary>
/// Extension methods for masking sensitive bank account information.
/// </summary>
public static class BankAccountExtensions
{
    /// <summary>
    /// Masks a bank account number by showing only the last 4 digits.
    /// Example: "1234567890" becomes "******7890"
    /// </summary>
    /// <param name="accountNumber">The account number to mask.</param>
    /// <returns>A masked account number showing only the last 4 digits.</returns>
    public static string MaskAccountNumber(this string? accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            return string.Empty;
        }

        // Remove any spaces or dashes for consistent masking
        var cleanedAccount = new string(accountNumber.Where(char.IsLetterOrDigit).ToArray());

        if (cleanedAccount.Length <= 4)
        {
            // If 4 or fewer characters, mask all but last character
            return cleanedAccount.Length <= 1
                ? new string('*', cleanedAccount.Length)
                : new string('*', cleanedAccount.Length - 1) + cleanedAccount[^1];
        }

        // Show last 4 digits, mask the rest
        var lastFour = cleanedAccount[^4..];
        var maskedPortion = new string('*', cleanedAccount.Length - 4);

        return maskedPortion + lastFour;
    }
}
