using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.Util.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;
public class GetNavigationStatusEndpoint : ProfitSharingEndpoint<GetNavigationStatusRequestDto, Results<Ok<GetNavigationStatusResponseDto>, NotFound, ProblemHttpResult>>
{

    private readonly INavigationService _navigationService;

    public GetNavigationStatusEndpoint(INavigationService navigationService)
        : base(Navigation.Constants.Unknown)
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

        if (!Env.IsTestEnvironment())
        {
            // Specify caching duration and store it in metadata
            TimeSpan cacheDuration = TimeSpan.FromMinutes(15);
            Options(x => x.CacheOutput(p => p.Expire(cacheDuration)));
        }
    }

    public override async Task<Results<Ok<GetNavigationStatusResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(GetNavigationStatusRequestDto req, CancellationToken ct)
    {
        var navigationStatusList = await _navigationService.GetNavigationStatus(cancellationToken: ct);
        var response = new GetNavigationStatusResponseDto { NavigationStatusList = navigationStatusList };
        return Result<GetNavigationStatusResponseDto>.Success(response).ToHttpResult();
    }

}
