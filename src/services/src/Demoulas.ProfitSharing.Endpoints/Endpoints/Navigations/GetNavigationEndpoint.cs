using Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;

public class GetNavigationEndpoint : ProfitSharingEndpoint<NavigationRequestDto, NavigationResponseDto>
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

    protected override Task<NavigationResponseDto> HandleRequestAsync(NavigationRequestDto req, CancellationToken ct)
    {
        return _navigationService.GetNavigation(cancellationToken: ct);
    }

}
