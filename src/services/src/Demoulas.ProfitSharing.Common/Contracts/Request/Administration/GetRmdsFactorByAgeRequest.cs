namespace Demoulas.ProfitSharing.Common.Contracts.Request.Administration;

/// <summary>
/// Request for getting RMD factor by age.
/// </summary>
public sealed record GetRmdsFactorByAgeRequest
{
    /// <summary>
    /// Age to retrieve RMD factor for.
    /// </summary>
    public required byte Age { get; init; }

    /// <summary>
    /// Example request for documentation and testing.
    /// </summary>
    public static GetRmdsFactorByAgeRequest RequestExample() => new()
    {
        Age = 73
    };
}
