using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Provides services for calculating and retrieving various profit-sharing totals and related data.
/// </summary>
public interface ITotalService
{
    /// <summary>
    /// Retrieves the vesting balance for a single member based on the specified search criteria.
    /// </summary>
    /// <param name="searchBy">Specifies the search criteria, either by SSN or Employee ID.</param>
    /// <param name="badgeNumberOrSsn">The identifier used for the search.</param>
    /// <param name="profitYear">The profit year for which the vesting balance is being retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The vesting balance details as a BalanceEndpointResponse, or null if not found.</returns>
    Task<BalanceEndpointResponse?> GetVestingBalanceForSingleMemberAsync(SearchBy searchBy, int badgeNumberOrSsn, short profitYear,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the vesting balance for multiple members based on the specified search criteria.
    /// </summary>
    /// <param name="searchBy">Specifies the search criteria, either by SSN or Employee ID.</param>
    /// <param name="badgeNumberOrSsnCollection">The collection of identifiers used for the search.</param>
    /// <param name="profitYear">The profit year for which the vesting balances are being retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of vesting balance details as BalanceEndpointResponse objects.</returns>
    Task<List<BalanceEndpointResponse>> GetVestingBalanceForMembersAsync(SearchBy searchBy,
        ISet<int> badgeNumberOrSsnCollection, short profitYear, CancellationToken cancellationToken);
}
