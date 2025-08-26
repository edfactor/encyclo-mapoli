using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public class ProfitSharingUnder21TotalsEndpoint: ProfitSharingEndpoint<ProfitYearRequest, ProfitSharingUnder21TotalsResponse>
{
    private readonly IPostFrozenService _postFrozenService;

    public ProfitSharingUnder21TotalsEndpoint(IPostFrozenService postFrozenService)
        : base(Navigation.Constants.QPAY066TAUnder21)
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

    public override Task<ProfitSharingUnder21TotalsResponse> ExecuteAsync(ProfitYearRequest req, CancellationToken ct)
    {
        return _postFrozenService.GetUnder21Totals(req, ct);
    }
}
