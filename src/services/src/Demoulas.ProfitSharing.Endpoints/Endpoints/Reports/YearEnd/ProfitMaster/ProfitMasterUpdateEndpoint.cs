using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitMaster;

public class ProfitMasterUpdateEndpoint : Endpoint<ProfitShareUpdateRequest, ProfitMasterUpdateResponse>
{
    private readonly IProfitMasterService _profitMasterService;
    private readonly INavigationService _navigationService;
    private readonly IAuditService _auditService;

    public ProfitMasterUpdateEndpoint(IProfitMasterService profitMasterUpdate,
        INavigationService navigationService,
        IAuditService auditService)
    {
        _profitMasterService = profitMasterUpdate;
        _navigationService = navigationService;
        _auditService = auditService;
    }

    public override void Configure()
    {
        // If I use Post(), swagger shows no documentation :-(
        Get("profit-master-update");
        Summary(s =>
        {
            s.Summary = "Applies YE updates to members";
            s.ExampleRequest = ProfitShareUpdateRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, ProfitMasterUpdateResponse.Example() } };
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(ProfitShareUpdateRequest req, CancellationToken ct)
    {
        ProfitMasterUpdateResponse response = await _auditService.ArchiveCompletedReportAsync("PAY444|PAY447",
            req.ProfitYear,
            req, 
            isArchiveRequest: true,
            async (arditReq, _, cancellationToken) =>
            {
                ProfitMasterUpdateResponse response = await _profitMasterService.Update(arditReq, cancellationToken);
                await _navigationService.UpdateNavigation(Navigation.Constants.MasterUpdate, NavigationStatus.Constants.Complete, cancellationToken);
                return response;
            },
            ct);
        await Send.OkAsync(response, ct);
    }
}
