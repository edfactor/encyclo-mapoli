using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;

namespace Demoulas.ProfitSharing.Common.Interfaces.Navigations;
public interface INavigationService
{
    Task<List<NavigationDto>> GetNavigation(CancellationToken cancellationToken);
    NavigationDto GetNavigation(int navigationId);
}
