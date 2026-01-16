using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Navigations;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;

public class UpdateNavigationStatusEndpoint : ProfitSharingEndpoint<UpdateNavigationRequestDto, Results<Ok<UpdateNavigationStatusResponseDto>, NotFound, ProblemHttpResult>>
{

    private readonly INavigationService _navigationService;

    public UpdateNavigationStatusEndpoint(INavigationService navigationService) : base(Navigation.Constants.Unknown)
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

    protected override async Task<Results<Ok<UpdateNavigationStatusResponseDto>, NotFound, ProblemHttpResult>> HandleRequestAsync(UpdateNavigationRequestDto req, CancellationToken ct)
    {
        var isSuccessful = await _navigationService.UpdateNavigation(req.NavigationId, req.StatusId, cancellationToken: ct);
        var response = new UpdateNavigationStatusResponseDto { IsSuccessful = isSuccessful };
        return Result<UpdateNavigationStatusResponseDto>.Success(response).ToHttpResult();
    }

}
