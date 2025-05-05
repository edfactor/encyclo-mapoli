using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request.Naviations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;
public class GetNavigationEndpoint: Endpoint<NavigationRequestDto,NavigationResponseDto>
{

    private readonly INavigationService _navigationService;

    public GetNavigationEndpoint(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get($"/list");
        Summary(m =>
        {
            m.Summary = "Get all navigation";
            m.Description = "Fetch List of navigation object.";
            m.ResponseExamples = new Dictionary<int, object> { { 200, new NavigationResponseDto() } };
        });
        Group<NavigationGroup>();

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(15);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<NavigationResponseDto> ExecuteAsync(NavigationRequestDto req, CancellationToken ct)
    {
        var navigationList = await  _navigationService.GetNavigation(cancellationToken: ct);
        var response = new NavigationResponseDto { Navigation = navigationList };
        return response;
    }

}
