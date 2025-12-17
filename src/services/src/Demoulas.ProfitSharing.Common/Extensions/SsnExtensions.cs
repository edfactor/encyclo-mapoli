using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Demoulas.ProfitSharing.Common.Extensions;

public static class SsnExtensions
{
    /// <summary>
    /// Masks the given Social Security Number (SSN) by replacing the first five digits with 'X'
    /// and formatting it as "XXX-XX-####".
    /// </summary>
    /// <param name="ssn">The Social Security Number to be masked.</param>
    /// <returns>A masked SSN string in the format "XXX-XX-####".</returns>
    public static string MaskSsn(this int ssn)
    {
        Span<char> ssnSpan = stackalloc char[9];
        ssn.ToString().AsSpan().CopyTo(ssnSpan[(9 - ssn.ToString().Length)..]);
        ssnSpan[..(9 - ssn.ToString().Length)].Fill('0');

        Span<char> resultSpan = stackalloc char[11];
        "XXX-XX-".AsSpan().CopyTo(resultSpan);
        ssnSpan.Slice(5, 4).CopyTo(resultSpan[7..]);

        return new string(resultSpan);
    }

    public static string MaskSsn(this object ssn)
    {
        if (ssn is null)
        {
            return string.Empty;
        }

        string raw = ssn as string ?? ssn.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        // Normalize: keep only digits. This supports formatted inputs like "123-45-6789".
        string digitsOnly = new string(raw.Where(char.IsDigit).ToArray());
        if (digitsOnly.Length == 0)
        {
            return string.Empty;
        }

        // Keep the last 9 digits (handles cases where the value includes extra digits/characters).
        if (digitsOnly.Length > 9)
        {
            digitsOnly = digitsOnly[^9..];
        }

        Span<char> ssnSpan = stackalloc char[9];
        digitsOnly.AsSpan().CopyTo(ssnSpan[(9 - digitsOnly.Length)..]);
        ssnSpan[..(9 - digitsOnly.Length)].Fill('0');

        Span<char> resultSpan = stackalloc char[11];
        "XXX-XX-".AsSpan().CopyTo(resultSpan);
        ssnSpan.Slice(5, 4).CopyTo(resultSpan[7..]);

        return new string(resultSpan);
    }

    /// <summary>
    /// Converts a formatted Social Security Number (SSN) string into an integer representation.
    /// </summary>
    /// <param name="formattedSsn">
    /// The formatted SSN string in the format "###-##-####" or similar.
    /// Non-numeric characters will be removed during conversion.
    /// </param>
    /// <returns>
    /// An integer representation of the numeric portion of the SSN.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the input <paramref name="formattedSsn"/> is null, empty, or does not meet the expected format.
    /// </exception>
    public static int ConvertSsnToInt([StringSyntax("###-##-####")] this string formattedSsn)
    {
        if (string.IsNullOrWhiteSpace(formattedSsn) || formattedSsn.Length < 9)
        {
            Debug.WriteLine($"SSN :{formattedSsn}");
            throw new ArgumentException("Invalid SSN format. Expected format: ###-##-1234 or ######1234.");
        }

        // Remove non-numeric characters if any (e.g., dashes)
        string numericSsn = new string(formattedSsn.Where(char.IsDigit).ToArray());

        // Convert to int
        return int.Parse(numericSsn);
    }
}
