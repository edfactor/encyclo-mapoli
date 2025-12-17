namespace Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;

public sealed class GetAnnuityRatesRequest
{
    /// <summary>
    /// Requested sort column. Supported values: "Year", "Age".
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// When true, sorts descending.
    /// </summary>
    public bool? IsSortDescending { get; init; }
}
