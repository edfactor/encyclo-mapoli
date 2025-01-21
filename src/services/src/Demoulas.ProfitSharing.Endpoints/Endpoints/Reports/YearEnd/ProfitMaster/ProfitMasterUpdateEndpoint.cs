using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitMaster;

public class ProfitMasterUpdateEndpoint : Endpoint<ProfitShareUpdateRequest, ProfitMasterResponse>
{
    private readonly IProfitMasterService _profitMasterService;

    public ProfitMasterUpdateEndpoint(IProfitMasterService profitMasterUpdate)
    {
        _profitMasterService = profitMasterUpdate;
    }

    public override void Configure()
    {
        Post("profit-master-update");
        Summary(s =>
        {
            s.Summary = "applies YE updates to members";
            s.ResponseExamples = new Dictionary<int, object> { { 200, ProfitMasterResponse.Example() } };
        });
        Group<YearEndGroup>();
    }

    public override Task<ProfitMasterResponse> ExecuteAsync(ProfitShareUpdateRequest req, CancellationToken ct)
    {
        return _profitMasterService.Update(req, ct);
    }

}
