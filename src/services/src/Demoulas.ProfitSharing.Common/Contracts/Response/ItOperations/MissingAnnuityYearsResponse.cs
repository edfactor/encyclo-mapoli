namespace Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;

/// <summary>
/// Response indicating which years have complete/incomplete annuity rates.
/// </summary>
public sealed record MissingAnnuityYearsResponse
{
    /// <summary>
    /// Gets or sets the list of year completeness statuses.
    /// </summary>
    public required IReadOnlyList<AnnuityYearStatus> Years { get; set; }

    /// <summary>
    /// Sample response for API documentation.
    /// </summary>
    public static MissingAnnuityYearsResponse ResponseExample() => new()
    {
        Years = new List<AnnuityYearStatus>
        {
            new() { Year = 2026, IsComplete = true, MissingAges = Array.Empty<byte>() },
            new() { Year = 2025, IsComplete = false, MissingAges = new byte[] { 67, 68, 69 } },
            new() { Year = 2024, IsComplete = true, MissingAges = Array.Empty<byte>() }
        }
    };
}

/// <summary>
/// Represents the completeness status of annuity rates for a single year.
/// </summary>
public sealed record AnnuityYearStatus
{
    /// <summary>
    /// Gets or sets the profit year.
    /// </summary>
    public required short Year { get; set; }

    /// <summary>
    /// Gets or sets whether all required ages have annuity rates defined for this year.
    /// </summary>
    public required bool IsComplete { get; set; }

    /// <summary>
    /// Gets or sets the list of missing ages (if any).
    /// Empty if IsComplete is true.
    /// </summary>
    public required byte[] MissingAges { get; set; }
}
