namespace Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;

/// <summary>
/// Request to check which years have complete annuity rate data.
/// </summary>
public sealed record GetMissingAnnuityYearsRequest
{
    /// <summary>
    /// Gets or sets the starting year (inclusive) for the range to check.
    /// Defaults to current year minus 5.
    /// </summary>
    public short? StartYear { get; set; }

    /// <summary>
    /// Gets or sets the ending year (inclusive) for the range to check.
    /// Defaults to current year.
    /// </summary>
    public short? EndYear { get; set; }

    /// <summary>
    /// Sample request for API documentation.
    /// </summary>
    public static GetMissingAnnuityYearsRequest RequestExample() => new()
    {
        StartYear = 2020,
        EndYear = 2026
    };
}
