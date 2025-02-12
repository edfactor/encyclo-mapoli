using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;

public class GetFrozenDemographicsEndpoint : EndpointWithoutRequest<List<FrozenStateResponse>>
{
    private readonly IFrozenService _frozenService;

    public GetFrozenDemographicsEndpoint(IFrozenService frozenService)
    {
        _frozenService = frozenService;
    }

    public override void Configure()
    {
        Get("frozen");
        Summary(s =>
        {
            s.Summary = "Gets frozen demographic meta data";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new FrozenStateResponse() { Id = 2, ProfitYear = Convert.ToInt16(DateTime.Now.Year) } } };
        });
        Group<DemographicsGroup>();
    }

    public override Task<List<FrozenStateResponse>> ExecuteAsync(CancellationToken ct)
    {
        return _frozenService.GetFrozenDemographics(ct);
    }
}
