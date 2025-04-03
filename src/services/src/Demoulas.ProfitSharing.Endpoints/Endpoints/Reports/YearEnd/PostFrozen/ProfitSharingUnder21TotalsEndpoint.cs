using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public class ProfitSharingUnder21TotalsEndpoint: Endpoint<ProfitYearRequest, ProfitSharingUnder21TotalsResponse>
{
    private readonly IPostFrozenService _postFrozenService;

    public ProfitSharingUnder21TotalsEndpoint(IPostFrozenService postFrozenService): base()
    {
        _postFrozenService = postFrozenService;
    }

    public override void Configure()
    {
        Get("post-frozen/totals");
        Summary(s =>
        {
            s.Summary = "Totals lines for under 21 reports";
            s.Description = "Produces a series of totals related to participants under 21";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {200,  ProfitSharingUnder21TotalsResponse.SampleResponse()}
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public async override Task<ProfitSharingUnder21TotalsResponse> ExecuteAsync(ProfitYearRequest req, CancellationToken ct)
    {
        var response = await _postFrozenService.GetUnder21Totals(req, ct);

        return response;
    }
}