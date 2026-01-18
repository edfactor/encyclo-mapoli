using Demoulas.Common.Contracts.Contracts.Response.Navigation;
using Demoulas.Common.Contracts.Interfaces;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Null object pattern implementation of INavigationService for console/background service scenarios
/// where navigation concepts don't apply (no web UI context).
/// </summary>
internal sealed class NullNavigationService : INavigationService
{
    public Task<NavigationResponseResponse> GetNavigationAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new NavigationResponseResponse
        {
            Navigation = new List<NavigationResponse>(),
            CustomSettings = null
        });
    }

    public NavigationResponse GetNavigation(short navigationId)
    {
        return new NavigationResponse
        {
            Id = navigationId,
            Title = "N/A",
            StatusId = 0,
            StatusName = "N/A",
            ParentId = null,
            Items = new List<NavigationResponse>(),
            IsReadOnly = false
        };
    }

    public Task<List<NavigationStatusResponse>> GetNavigationStatusAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<NavigationStatusResponse>());
    }

    public Task<bool> UpdateNavigationAsync(short navigationId, byte statusId, CancellationToken cancellationToken)
    {
        // Always return success for background services - navigation status updates don't apply
        return Task.FromResult(true);
    }

    public Task ResetAllStatusesToNotStartedAsync(CancellationToken cancellationToken = default)
    {
        // No-op for background services - navigation status doesn't apply
        return Task.CompletedTask;
    }
}
