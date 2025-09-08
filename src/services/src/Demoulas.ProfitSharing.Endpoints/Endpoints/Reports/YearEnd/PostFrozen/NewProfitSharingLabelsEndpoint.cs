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
public class NewProfitSharingLabelsEndpoint: ProfitSharingEndpoint<ProfitYearRequest, PaginatedResponseDto<NewProfitSharingLabelResponse>>
{
    private readonly IPostFrozenService _postFrozenService;

    public NewProfitSharingLabelsEndpoint(IPostFrozenService postFrozenService)
        : base(Navigation.Constants.QNEWPROFLBL)
    {
        _postFrozenService = postFrozenService;
    }
    public override void Configure()
    {
        Get("post-frozen/new-profit-sharing-labels");
        Summary(s => 
        {
            s.Summary = "Returns the new profit sharing labels as a file";
            s.Description = "Returns either the JSON needed for the report showing which labels will be produced";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples= new Dictionary<int, object>()
            {
                {200, NewProfitSharingLabelResponse.SampleResponse() }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public sealed override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        var response = await _postFrozenService.GetNewProfitSharingLabels(req, ct);
        
        await Send.OkAsync(response, ct);

    }
}
