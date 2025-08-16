using Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;
public class GetNavigationStatusEndpoint : Endpoint<GetNavigationStatusRequestDto, GetNavigationStatusResponseDto>
{

    private readonly INavigationService _navigationService;

    public GetNavigationStatusEndpoint(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public override void Configure()
    {
        Get("/status");
        Summary(m =>
        {
            m.Summary = "Get all navigation status";
            m.Description = "Fetch List of navigation status objects.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new GetNavigationStatusResponseDto() } };
        });
        Group<NavigationGroup>();
    }

    public override async Task<GetNavigationStatusResponseDto> ExecuteAsync(GetNavigationStatusRequestDto req, CancellationToken ct)
    {
        var navigationStatusList = await _navigationService.GetNavigationStatus(cancellationToken: ct);
        var response = new GetNavigationStatusResponseDto { NavigationStatusList = navigationStatusList };
        return response;
    }

}
