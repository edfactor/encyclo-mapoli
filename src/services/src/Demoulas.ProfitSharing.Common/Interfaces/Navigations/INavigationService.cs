using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;

namespace Demoulas.ProfitSharing.Common.Interfaces.Navigations;

public interface INavigationService
{
    Task<List<NavigationDto>> GetNavigation(CancellationToken cancellationToken);
    NavigationDto GetNavigation(short navigationId);
    Task<List<NavigationStatusDto>> GetNavigationStatus(CancellationToken cancellationToken);
    Task<bool> UpdateNavigation(short navigationId, byte statusId, CancellationToken cancellationToken);

    /// <summary>
    /// Resets all navigation statuses to 'Not Started' and invalidates all navigation tree caches.
    /// This is typically called when a new demographics freeze point is created to restart workflows.
    /// </summary>
    Task ResetAllStatusesToNotStartedAsync(CancellationToken cancellationToken = default);
}
