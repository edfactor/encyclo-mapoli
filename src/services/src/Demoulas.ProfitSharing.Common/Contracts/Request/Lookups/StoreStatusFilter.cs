namespace Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
/// <summary>
/// Enum for filtering stores by their operational status.
/// </summary>
public enum StoreStatusFilter
{
    /// <summary>
    /// Return all stores regardless of status.
    /// </summary>
    All = 0,

    /// <summary>
    /// Return only currently active/open stores.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Return only unopened (closed) stores.
    /// </summary>
    Unopened = 2
}
