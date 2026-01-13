using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using FluentValidation;
using FluentValidation.Results;

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
        var response = await _navigationService.GetNavigation(cancellationToken);
        var tree = response.Navigation ?? [];
        var node = FindNode(tree, navigationId);
        if (node == null)
        {
            throw new ValidationException($"Navigation {navigationId} not found or not accessible for current user.");
        }

        var prereqs = node.PrerequisiteNavigations ?? new();
        var incomplete = prereqs.Where(p => p.StatusId != NavigationStatus.Constants.Complete).ToList();
        if (incomplete.Count > 0)
        {
            var failures = incomplete.Select(p => new ValidationFailure(p.Title, $"{p.Title} is '{p.StatusName}' but must be '{nameof(NavigationStatus.Constants.Complete)}'"));
            throw new ValidationException($"Prerequisites not {nameof(NavigationStatus.Constants.Complete)} for {node.Title}", failures);
        }
    }

    private static NavigationDto? FindNode(IEnumerable<NavigationDto> nodes, short id)
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
