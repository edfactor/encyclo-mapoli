using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Shouldly;

namespace YEMatch.AssertActivities;

public class UpdateNavigation : BaseActivity
{
    public required ApiClient ApiClient { get; init; }

    public override async Task<Outcome> Execute()
    {
        await MarkComplete(Navigation.Constants.CleanupReports);
        await MarkComplete(Navigation.Constants.Unforfeit);
        await MarkComplete(Navigation.Constants.MilitaryContributions);
        await MarkComplete(Navigation.Constants.Terminations);
        await MarkComplete(Navigation.Constants.DistributionsAndForfeitures);
        await MarkComplete(Navigation.Constants.ProfitShareReport);
        await MarkComplete(Navigation.Constants.Forfeitures);
        await MarkComplete(Navigation.Constants.ManageExecutiveHours);

        return new Outcome(Name(), Name(), "", OutcomeStatus.Ok, "", null, false);
    }

    private async Task MarkComplete(short page)
    {
        ApiClient apiClient = ApiClient;
        UpdateNavigationRequestDto req = new() { NavigationId = page, StatusId = 4 /* Complete */ };
        UpdateNavigationStatusResponseDto res = await apiClient.NavigationsUpdateNavigationStatusEndpointAsync(null, req);
        res.IsSuccessful.ShouldBeTrue();
    }
}
