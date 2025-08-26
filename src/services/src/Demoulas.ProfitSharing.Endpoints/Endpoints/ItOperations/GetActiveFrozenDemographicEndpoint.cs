using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class GetActiveFrozenDemographicEndpoint : ProfitSharingResponseEndpoint<FrozenStateResponse>
{
    private readonly IFrozenService _frozenService;

    public GetActiveFrozenDemographicEndpoint(IFrozenService frozenService) : base(Navigation.Constants.DemographicFreeze)
    {
        _frozenService = frozenService;
    }

    public override void Configure()
    {
        Get("frozen/active");
        Summary(s =>
        {
            s.Summary = "Gets frozen demographic meta data";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new FrozenStateResponse { Id = 2, ProfitYear = Convert.ToInt16(DateTime.Now.Year), IsActive = true }
                }
            };
        });
        Group<ItDevOpsGroup>();
    }

    public override Task<FrozenStateResponse> ExecuteAsync(CancellationToken ct)
    {
        return _frozenService.GetActiveFrozenDemographic(ct);
    }
}
