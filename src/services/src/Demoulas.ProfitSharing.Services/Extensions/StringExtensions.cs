using System.Text.RegularExpressions;

namespace Demoulas.ProfitSharing.Services.Extensions;

internal static class StringExtensions
{
    public static string ReplaceNamedParams(this string template, params object?[] args)
    {
        // List of placeholders in the order they appear in the template
        var placeholders = Regex.Matches(template, @"\{(\w+)\}")
            .Cast<Match>()
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();

        return Regex.Replace(template, @"\{(\w+)\}", match =>
        {
            string placeholder = match.Groups[1].Value;

            // Find the index of the placeholder in the placeholders list
            int index = placeholders.IndexOf(placeholder);

            // If the index is valid and within bounds, replace it with the corresponding argument value
            if (index >= 0 && index < args.Length)
            {
                return args[index]?.ToString() ?? string.Empty;
            }

            // If no match is found, leave the placeholder unchanged
            return match.Value;
        });
    }
}
