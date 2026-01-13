using System.Text.RegularExpressions;
using Serilog.Enrichers.Sensitive;

namespace Demoulas.ProfitSharing.Services.LogMasking;

public sealed partial class UnformattedSocialSecurityNumberMaskingOperator : RegexMaskingOperator
{
    // Define the SSN pattern (matches SSNs in the format XXX-XX-XXXX or XXXXXXXXX)
    private const string SsnPattern = @"(?<!\d)(\d{3}-\d{2}-\d{4}|\d{9})(?!\d)";

    [GeneratedRegex(SsnPattern, RegexOptions.Compiled)]
    private static partial Regex SsnRegex();

    public UnformattedSocialSecurityNumberMaskingOperator()
        : base(SsnPattern, RegexOptions.Compiled)
    {
    }

    /// <summary>
    /// Preprocesses the input before applying the masking.
    /// </summary>
    /// <param name="input">The input string that may contain an SSN.</param>
    /// <returns>The processed input string.</returns>
    protected override string PreprocessInput(string input)
    {
        // No special preprocessing required for SSNs
        return input;
    }

    /// <summary>
    /// Determines if the input should be masked.
    /// </summary>
    /// <param name="input">The input string that may contain an SSN.</param>
    /// <returns>True if the input contains an SSN and should be masked.</returns>
    protected override bool ShouldMaskInput(string input)
    {
        // Mask input if it contains a valid SSN pattern
        return SsnRegex().IsMatch(input);
    }
}
