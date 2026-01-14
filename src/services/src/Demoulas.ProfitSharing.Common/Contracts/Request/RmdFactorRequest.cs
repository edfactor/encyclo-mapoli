namespace Demoulas.ProfitSharing.Common.Contracts.Request;

/// <summary>
/// Request for adding or updating RMD factor by age.
/// </summary>
public sealed record RmdsFactorRequest
{
    /// <summary>
    /// Age in years (73-120).
    /// </summary>
    public required byte Age { get; init; }

    /// <summary>
    /// IRS life expectancy divisor for this age (e.g., 26.5 years for age 73).
    /// Must be greater than 0.
    /// </summary>
    public required decimal Factor { get; init; }

    /// <summary>
    /// Example request for API documentation.
    /// </summary>
    public static RmdsFactorRequest RequestExample() => new()
    {
        Age = 73,
        Factor = 26.5m
    };
}
