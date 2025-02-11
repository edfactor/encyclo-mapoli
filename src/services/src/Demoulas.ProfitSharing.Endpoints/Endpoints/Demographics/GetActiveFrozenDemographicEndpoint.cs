using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;

public class GetActiveFrozenDemographicEndpoint : EndpointWithoutRequest<SetFrozenStateResponse>
{
    private readonly IFrozenService _frozenService;

    public GetActiveFrozenDemographicEndpoint(IFrozenService frozenService)
    {
        _frozenService = frozenService;
    }

    public override void Configure()
    {
        Get("frozen/active");
        Summary(s =>
        {
            s.Summary = "Gets frozen demographic meta data";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new SetFrozenStateResponse { Id = 2, ProfitYear = Convert.ToInt16(DateTime.Now.Year) } } };
        });
        Group<DemographicsGroup>();
    }

    public override Task<SetFrozenStateResponse> ExecuteAsync(CancellationToken ct)
    {
        return _frozenService.GetActiveFrozenDemographic(ct);
    }
}
