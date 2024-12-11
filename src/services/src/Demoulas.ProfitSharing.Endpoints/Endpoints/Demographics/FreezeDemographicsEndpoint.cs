using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Demographics;
public class FreezeDemographicsEndpoint:Endpoint<SetFrozenStateRequest, SetFrozenStateResponse>
{
    private readonly IFrozenService _frozenService;

    public FreezeDemographicsEndpoint(IFrozenService frozenService)
    {
        _frozenService = frozenService;
    }
    public override void Configure()
    {
        Post("freeze");
        Summary(s =>
        {
            s.Summary = "Freezes demographics for a specific profit year";
            s.ResponseExamples = new Dictionary<int, object>
            {
                {200, new SetFrozenStateResponse() {Id=2, ProfitYear = Convert.ToInt16(DateTime.Now.Year)} }
            };
        });
        Policies(Security.Policy.CanFreezeDemographics);
        Group<DemographicsGroup>();
    }

    public override Task<SetFrozenStateResponse> ExecuteAsync(SetFrozenStateRequest req, CancellationToken ct)
    {
        return _frozenService.FreezeDemographics(req.ProfitYear, req.AsOfDateTime, ct);
    }
}
