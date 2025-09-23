using Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;
public class GetNavigationEndpoint: ProfitSharingEndpoint<NavigationRequestDto,NavigationResponseDto>
{

    private readonly INavigationService _navigationService;

    public GetNavigationEndpoint(INavigationService navigationService)
        : base(Navigation.Constants.Unknown)
    {
        _navigationService = navigationService;
    }

    public override void Configure()
    {
      
        Get("");
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
        var navigationList = await  _navigationService.GetNavigation(cancellationToken: ct);
        var response = new NavigationResponseDto { Navigation = navigationList };
        return response;
    }

}
