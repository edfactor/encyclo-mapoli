using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;
public sealed class ProfitSharingLabelsEndpoint : Endpoint<ProfitYearRequest, PaginatedResponseDto<ProfitSharingLabelResponse>>
{
    private readonly IPostFrozenService _postFrozenService;

    public ProfitSharingLabelsEndpoint(IPostFrozenService postFrozenService)
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

    public sealed override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        var response = await _postFrozenService.GetProfitSharingLabels(req, ct);

        await SendOkAsync(response, ct);

    }
}
