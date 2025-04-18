using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.Naviations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;
public class GetNavigation: Endpoint<NavigationRequestDto,NavigationResponseDto>
{

    private readonly INavigationService _navigationService;

    public GetNavigation(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get($"/navigation/list");
        Summary(m =>
        {
            m.Summary = "Get all navigation";
            m.Description = "Fetch List of navigation object.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new NavigationResponseDto() } };
        });
        Group<NavigationGroup>();
    }

    public override async Task<NavigationResponseDto> ExecuteAsync(NavigationRequestDto req, CancellationToken ct)
    {
        var navigationList = await  this._navigationService.GetNavigation();
        var response = new NavigationResponseDto { Navigation = navigationList };
        return response;
    }

}
