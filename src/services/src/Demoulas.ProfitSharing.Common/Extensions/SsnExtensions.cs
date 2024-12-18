using System.Diagnostics.CodeAnalysis;

namespace Demoulas.ProfitSharing.Common.Extensions;
public static class SsnExtensions
{
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

    public static int ConvertSsnToInt([StringSyntax("###-##-####")] this string formattedSsn)
    {
        if (string.IsNullOrWhiteSpace(formattedSsn) || formattedSsn.Length != 11)
        {
            throw new ArgumentException("Invalid SSN format. Expected format: ###-##-1234.");
        }

        // Remove non-numeric characters if any (e.g., dashes)
        string numericSsn = new string(formattedSsn.Where(char.IsDigit).ToArray());

        // Convert to int
        return int.Parse(numericSsn);
    }
}
