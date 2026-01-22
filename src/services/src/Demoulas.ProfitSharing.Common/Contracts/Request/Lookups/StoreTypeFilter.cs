namespace Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;

/// <summary>
/// Enum for filtering stores by type.
/// </summary>
public enum StoreTypeFilter
{
    /// <summary>
    /// Return all store types.
    /// </summary>
    All = 0,

    /// <summary>
    /// Return only retail stores (store number &lt; 899).
    /// </summary>
    Retail = 1
}
