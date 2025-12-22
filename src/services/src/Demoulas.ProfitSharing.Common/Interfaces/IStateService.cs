using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for retrieving state information from the profit sharing database.
/// Provides lookups for states referenced in profit sharing comments and transactions.
/// </summary>
public interface IStateService
{
    /// <summary>
    /// Gets all states that are referenced in profit sharing comment records.
    /// Returns states from the State lookup table that have corresponding COMMENT_RELATED_STATE
    /// values in the PROFIT_DETAIL table, joined to provide full state names.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Collection of StateListResponse objects ordered by abbreviation</returns>
    Task<ICollection<StateListResponse>> GetStatesAsync(CancellationToken ct = default);
}
