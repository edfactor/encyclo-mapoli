using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitMaster;

public class ProfitMasterStatusEndpoint : ProfitSharingEndpoint<ProfitYearRequest, ProfitMasterUpdateResponse>
{
    private readonly IProfitMasterService _profitMasterService;

    public ProfitMasterStatusEndpoint(IProfitMasterService profitMasterService)
        : base(Navigation.Constants.MasterUpdate)
    {
        _profitMasterService = profitMasterService;
    }

    public override void Configure()
    {
        Get("profit-master-status");
        Summary(s =>
        {
            s.Summary = "Shows a summary of the current profit share update status";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, ProfitMasterUpdateResponse.Example() } };
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        var response = await _profitMasterService.Status(req, ct);
        if (response == null)
        {
            await Send.NoContentAsync(ct);
        }
        else
        {
            await Send.OkAsync(response, ct);
        }
    }
}
