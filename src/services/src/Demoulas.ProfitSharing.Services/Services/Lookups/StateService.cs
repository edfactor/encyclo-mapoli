using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Services.Lookups;

/// <summary>
/// Service for retrieving state information from the database, including states
/// that are referenced in profit sharing comments.
/// Provides efficient database queries for state lookups with proper filtering and ordering.
/// </summary>
public sealed class StateService : IStateService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public StateService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    /// <summary>
    /// Gets all states from the database with their full names ordered alphabetically by abbreviation.
    /// Queries both the State lookup table and ProfitDetail records to ensure all
    /// referenced states are included in the results.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Collection of StateListResponse objects ordered by abbreviation</returns>
    public async Task<ICollection<StateListResponse>> GetStatesAsync(CancellationToken ct = default)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Get distinct states from ProfitDetail.COMMENT_RELATED_STATE
            var profitDetailStates = await ctx.ProfitDetails
                .TagWith("GetStates-FromProfitDetail")
                .Where(pd => pd.CommentRelatedState != null && pd.CommentRelatedState != "")
                .Select(pd => pd.CommentRelatedState)
                .Distinct()
                .ToListAsync(ct);

            // Join with State lookup table to get full state names
            var states = await ctx.States
                .TagWith("GetStates-WithNames")
                .Where(s => profitDetailStates.Contains(s.Abbreviation))
                .Select(s => new StateListResponse
                {
                    Abbreviation = s.Abbreviation,
                    Name = s.Name
                })
                .OrderBy(s => s.Abbreviation)
                .ToListAsync(ct);

            return states;
        }, ct);
    }
}

