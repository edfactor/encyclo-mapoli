using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ProfitDetails;
public sealed class ProfitDetailReversalsEndpoint: ProfitSharingRequestEndpoint<IdsRequest>
{
    private readonly IProfitDetailReversalsService _profitDetailReversalsService;

    public ProfitDetailReversalsEndpoint(IProfitDetailReversalsService profitDetailReversalsService) : base(Navigation.Constants.ProfitDetailReversals)
    {
        _profitDetailReversalsService = profitDetailReversalsService;
    }

    public override void Configure()
    {
        Post("/reversals");
        Group<ProfitDetailsGroup>();
        Policies(Security.Policy.CanReverseProfitDetails); // <-- This line is incorrect usage
        Summary(s =>
        {
            s.Description = "Reverses profit detail entries based on provided IDs.";
            s.Summary = "Reverse Profit Detail Entries";
            s.ExampleRequest = IdsRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 204, "success" }
            };
        });
    }

    public async override Task HandleAsync(IdsRequest req, CancellationToken ct)
    {
        await _profitDetailReversalsService.ReverseProfitDetailsAsync(req.Ids, ct);
        await Send.NoContentAsync(ct);
    }
}
