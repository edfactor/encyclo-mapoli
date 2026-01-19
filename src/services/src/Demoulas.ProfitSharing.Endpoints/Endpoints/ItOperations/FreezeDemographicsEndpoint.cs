using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;

public class FreezeDemographicsEndpoint : ProfitSharingEndpoint<SetFrozenStateRequest, FrozenStateResponse>
{
    private readonly IFrozenService _frozenService;
    private readonly IAppUser _appUser;
    private readonly ILogger<FreezeDemographicsEndpoint> _logger;

    public FreezeDemographicsEndpoint(
        IFrozenService frozenService,
        IAppUser appUser,
        ILogger<FreezeDemographicsEndpoint> logger) : base(Navigation.Constants.DemographicFreeze)
    {
        _frozenService = frozenService;
        _appUser = appUser;
        _logger = logger;
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

    protected override async Task<FrozenStateResponse> HandleRequestAsync(SetFrozenStateRequest req, CancellationToken ct)
    {
        var result = await _frozenService.FreezeDemographicsAsync(req.ProfitYear, req.AsOfDateTime, _appUser.UserName, ct);
        _logger.LogInformation("Freeze demographics executed for profit year {ProfitYear}", req.ProfitYear);
        return result ?? new FrozenStateResponse { Id = 0 };
    }
}
