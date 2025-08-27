using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
public sealed class ProfitSharingLabelsEndpoint : ProfitSharingEndpoint<FrozenProfitYearRequest, PaginatedResponseDto<ProfitSharingLabelResponse>>
{
    private readonly IPostFrozenService _postFrozenService;

    public ProfitSharingLabelsEndpoint(IPostFrozenService postFrozenService)
        : base(Navigation.Constants.PROFALL)
    {
        _postFrozenService = postFrozenService;
    }
    public override void Configure()
    {
        Get("post-frozen/profit-sharing-labels");
        Summary(s =>
        {
            s.Summary = "Returns data for the profit sharing labels";
            s.Description = "Returns the JSON needed for the report showing which labels will be produced";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>()
            {
                {200, ProfitSharingLabelResponse.SampleResponse() }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(FrozenProfitYearRequest req, CancellationToken ct)
    {
        var response = await _postFrozenService.GetProfitSharingLabels(req, ct);

        await Send.OkAsync(response, ct);
    }
}
