using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;

namespace Demoulas.ProfitSharing.OracleHcm.Services;

/// <summary>
/// Null object pattern implementation of INavigationService for console/background service scenarios
/// where navigation concepts don't apply (no web UI context).
/// </summary>
internal sealed class NullNavigationService : INavigationService
{
    public Task<NavigationResponseDto> GetNavigation(CancellationToken cancellationToken)
    {
        return Task.FromResult(new NavigationResponseDto
        {
            Navigation = new List<NavigationDto>(),
            CustomSettings = new Dictionary<string, object?>
            {
                [NavigationCustomSettingsKeys.TrackPageStatus] = true,
                [NavigationCustomSettingsKeys.UseFrozenYear] = true
            }
        });
    }

    public NavigationDto GetNavigation(short navigationId)
    {
        return new NavigationDto
        {
            Id = navigationId,
            Title = "N/A",
            StatusId = 0,
            StatusName = "N/A",
            ParentId = null,
            Items = new List<NavigationDto>(),
            IsReadOnly = false
        };
    }

    public Task<List<NavigationStatusDto>> GetNavigationStatus(CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<NavigationStatusDto>());
    }

    public Task<bool> UpdateNavigation(short navigationId, byte statusId, CancellationToken cancellationToken)
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
