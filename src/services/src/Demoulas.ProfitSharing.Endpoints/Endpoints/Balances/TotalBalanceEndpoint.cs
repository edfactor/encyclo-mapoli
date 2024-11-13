using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Balances;

public class TotalBalanceEndpoint:Endpoint<BalanceEndpointRequest, BalanceEndpointResponse?>
{
    private readonly ITotalService _totalService;

    public TotalBalanceEndpoint(ITotalService totalService)
    {
        _totalService = totalService;
    }
    public override void Configure()
    {
        Get("for/@ProfitYear/by/@SearchType/@Id");
        Summary(s =>
        {
            s.Summary = "Gets total vested balance, and supporting number for a participant in the profit sharing plan";
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, new List<BalanceEndpointResponse> {BalanceEndpointResponse.ResponseExample()}}
            };
        });
        Group<BalanceGroup>();
    }

    public override async Task<BalanceEndpointResponse?> ExecuteAsync(BalanceEndpointRequest req, CancellationToken ct)
    {
        return await _totalService.GetVestingBalanceForSingleMember(req.SearchType, req.Id, req.ProfitYear);
    }
}
