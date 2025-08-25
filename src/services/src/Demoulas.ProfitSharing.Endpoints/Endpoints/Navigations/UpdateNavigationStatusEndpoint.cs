using Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;
public class UpdateNavigationStatusEndpoint : Endpoint<UpdateNavigationRequestDto, UpdateNavigationStatusResponseDto>
{

    private readonly INavigationService _navigationService;

    public UpdateNavigationStatusEndpoint(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public override void Configure()
    {
        Put("");
        Summary(m =>
        {
            m.Summary = "Update navigation Status";
            m.Description = "Get the navigationId and statusId and update navigation status.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new UpdateNavigationStatusResponseDto() } };
        });
        Group<NavigationGroup>();
    }

    public override async Task<UpdateNavigationStatusResponseDto> ExecuteAsync(UpdateNavigationRequestDto req, CancellationToken ct)
    {
    var isSuccessful  = await _navigationService.UpdateNavigation(req.NavigationId, req.StatusId, cancellationToken: ct);
        var response = new UpdateNavigationStatusResponseDto { IsSuccessful = isSuccessful };
        return response;
    }

}
