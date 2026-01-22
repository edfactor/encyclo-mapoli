namespace Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;

/// <summary>
/// Request object for filtering store lookups.
/// </summary>
public sealed record StoreListRequest
{
    /// <summary>
    /// Filter for store status.
    /// </summary>
    public StoreStatusFilter? Status { get; init; } = StoreStatusFilter.Active;

    /// <summary>
    /// Filter for store type (retail vs all).
    /// </summary>
    public StoreTypeFilter StoreType { get; init; } = StoreTypeFilter.All;

    public bool IncludeVirtualStores { get; set; } = false;

    /// <summary>
    /// Creates an example request for API documentation.
    /// </summary>
    public static StoreListRequest RequestExample() => new()
    {
        Status = StoreStatusFilter.Active,
        StoreType = StoreTypeFilter.Retail
    };
}
