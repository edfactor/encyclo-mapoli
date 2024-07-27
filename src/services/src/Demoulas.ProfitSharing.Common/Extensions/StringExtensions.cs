namespace Demoulas.ProfitSharing.Common.Extensions;

public static class StringExtensions
{
    public static long? ConvertSsnToLong(this string ssn)
    {
        if (string.IsNullOrWhiteSpace(ssn))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(ssn));
        }

        // Remove non-numeric characters if any (e.g., dashes)
        string numericSsn = new string(ssn.Where(char.IsDigit).ToArray());

        // Convert to long
        return long.Parse(numericSsn);
    }
}
