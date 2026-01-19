using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.Common.Contracts.Interfaces;
using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitMaster;

public class ProfitMasterRevertEndpoint : ProfitSharingEndpoint<ProfitYearRequest, ProfitMasterRevertResponse>
{
    private readonly IProfitMasterService _profitMasterService;
    private readonly INavigationService _navigationService;

    public ProfitMasterRevertEndpoint(IProfitMasterService profitMasterService,
        INavigationService navigationService)
        : base(Navigation.Constants.MasterUpdate)
    {
        _profitMasterService = profitMasterService;
        _navigationService = navigationService;
    }

    public override void Configure()
    {
        // If I use Post(), swagger shows no documentation :-(
        Get("profit-master-revert");
        Summary(s =>
        {
            s.Summary = "Reverts YE updates to members";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object> { { 200, ProfitMasterRevertResponse.Example() } };
        });
        Group<YearEndGroup>();
    }

    protected override async Task<ProfitMasterRevertResponse> HandleRequestAsync(ProfitYearRequest req, CancellationToken ct)
    {
        var response = await _profitMasterService.RevertAsync(req, ct);
        await _navigationService.UpdateNavigationAsync(Navigation.Constants.MasterUpdate, NavigationStatusIds.InProgress, ct);

        return response;
    }
}
