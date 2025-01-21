using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitMaster;

public class ProfitMasterRevertEndpoint : Endpoint<ProfitYearRequest, ProfitMasterResponse>
{
    private readonly IProfitMasterService _profitMasterService;

    public ProfitMasterRevertEndpoint(IProfitMasterService profitMasterService)
    {
        _profitMasterService = profitMasterService;
    }

    public override void Configure()
    {
        Post("profit-master-revert");
        Summary(s =>
        {
            s.Summary = "reverts YE updates to members";
            s.ResponseExamples = new Dictionary<int, object> { { 200, ProfitMasterResponse.Example() } };
        });
        Group<YearEndGroup>();
    }

    public override Task<ProfitMasterResponse> ExecuteAsync(ProfitYearRequest req, CancellationToken ct)
    {
        return _profitMasterService.Revert(req, ct);
    }
}
