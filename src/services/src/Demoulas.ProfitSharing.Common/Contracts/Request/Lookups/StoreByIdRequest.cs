namespace Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;

/// <summary>
/// Request object for looking up a store by ID.
/// </summary>
public sealed record StoreByIdRequest
{
    /// <summary>
    /// The unique identifier for the store.
    /// </summary>
    public int StoreId { get; set; }
}



