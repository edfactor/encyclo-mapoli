using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using FluentValidation;

namespace Demoulas.ProfitSharing.Services.Navigations;

public class NavigationPrerequisiteValidator : INavigationPrerequisiteValidator
{
    private readonly INavigationService _navigationService;

    public NavigationPrerequisiteValidator(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public async Task ValidateAllCompleteAsync(short navigationId, CancellationToken cancellationToken)
    {
        // Fetch full navigation (by roles) and locate the requested node.
        var tree = await _navigationService.GetNavigation(cancellationToken);
        var node = FindNode(tree, navigationId);
        if (node == null)
        {
            throw new ValidationException($"Navigation {navigationId} not found or not accessible for current user.");
        }

        var prereqs = node.PrerequisiteNavigations ?? new();
        var incomplete = prereqs.Where(p => p.StatusName is null || !string.Equals(p.StatusName, "Complete", System.StringComparison.OrdinalIgnoreCase)).ToList();
        if (incomplete.Count > 0)
        {
            var list = string.Join(", ", incomplete.Select(p => $"{p.Id}:{p.Title}"));
            throw new ValidationException($"Prerequisites not complete for navigation {navigationId}: {list}");
        }
    }

    private static NavigationDto? FindNode(System.Collections.Generic.IEnumerable<NavigationDto> nodes, short id)
    {
        foreach (var n in nodes)
        {
            if (n.Id == id)
            {
                return n;
            }
            var found = FindNode(n.Items ?? [], id);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
}
