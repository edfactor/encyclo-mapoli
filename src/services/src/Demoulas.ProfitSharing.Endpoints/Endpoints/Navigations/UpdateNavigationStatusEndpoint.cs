using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.Naviations;
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
        AllowAnonymous();
        Get($"/update");
        Summary(m =>
        {
            m.Summary = "Get all navigation status";
            m.Description = "Fetch List of navigation status objects.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new UpdateNavigationStatusResponseDto() } };
        });
        Group<NavigationGroup>();
    }

    public override async Task<UpdateNavigationStatusResponseDto> ExecuteAsync(UpdateNavigationRequestDto req, CancellationToken ct)
    {
        var isSuccessful  = await this._navigationService.UpdateNavigation(req.NavigationId, req.StatusId, cancellationToken: ct);
        var response = new UpdateNavigationStatusResponseDto { IsSuccessful = isSuccessful };
        return response;
    }

}
