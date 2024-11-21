using System.Diagnostics.CodeAnalysis;

namespace Demoulas.ProfitSharing.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Converts a Social Security Number (SSN) string to an integer.
    /// </summary>
    /// <param name="ssn">The SSN string in the format "###-##-####".</param>
    /// <returns>
    /// An integer representation of the SSN if the conversion is successful; otherwise, <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="ssn"/> is null, empty, or contains non-numeric characters.
    /// </exception>
    public static int? ConvertSsnToInt([StringSyntax("###-##-####")]this string ssn)
    {
        if (string.IsNullOrWhiteSpace(ssn))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(ssn));
        }

        // Remove non-numeric characters if any (e.g., dashes)
        string numericSsn = new string(ssn.Where(char.IsDigit).ToArray());

        // Convert to int
        return int.Parse(numericSsn);
    }
}
