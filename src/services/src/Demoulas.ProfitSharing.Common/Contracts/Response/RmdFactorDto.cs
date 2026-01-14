using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

/// <summary>
/// Response DTO for RMD factor by age data.
/// </summary>
public sealed record RmdsFactorDto
{
    /// <summary>
    /// Age in years (73-120).
    /// </summary>
    [MaskSensitive]
    public required byte Age { get; init; }

    /// <summary>
    /// IRS life expectancy divisor for this age (e.g., 26.5 years for age 73).
    /// Used in RMD calculation: RMD Amount = Account Balance ÷ Factor
    /// </summary>
    public required decimal Factor { get; init; }
}
