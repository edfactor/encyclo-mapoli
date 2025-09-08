using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class FreezeDemographicsEndpoint : ProfitSharingEndpoint<SetFrozenStateRequest, FrozenStateResponse>
{
    private readonly IFrozenService _frozenService;
    private readonly IAppUser _appUser;

    public FreezeDemographicsEndpoint(IFrozenService frozenService, IAppUser appUser) : base(Navigation.Constants.DemographicFreeze)
    {
        _frozenService = frozenService;
        _appUser = appUser;
    }

    public override void Configure()
    {
        Post("freeze");
        Summary(s =>
        {
            s.Summary = "Freezes demographics for a specific profit year";
            s.ResponseExamples = new Dictionary<int, object> { { 200, new FrozenStateResponse() { Id = 2, ProfitYear = Convert.ToInt16(DateTime.Now.Year) } } };
            s.ExampleRequest =
                new SetFrozenStateRequest { AsOfDateTime = DateTime.Today, ProfitYear = (short)DateTime.Today.Year };
        });
    Group<ItDevOpsGroup>();
    }

    public override Task<FrozenStateResponse> ExecuteAsync(SetFrozenStateRequest req, CancellationToken ct)
    {
        return _frozenService.FreezeDemographics(req.ProfitYear, req.AsOfDateTime, _appUser.UserName, ct);
    }
}
