using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class GetForfeitureAdjustmentsEndpoint : ProfitSharingRequestEndpoint<SuggestedForfeitureAdjustmentRequest>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;

    public GetForfeitureAdjustmentsEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService) : base(Navigation.Constants.Forfeitures)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
    }

    public override void Configure()
    {
        Get("forfeiture-adjustments");
        Summary(s =>
        {
            s.Summary = "Get forfeiture suggested adjustments for  badge number or ssn.";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Description = "This endpoint is used to get a suggested forfeiture adjustment for a badge number or ssn.";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(SuggestedForfeitureAdjustmentRequest req, CancellationToken ct)
    {
        SuggestedForfeitureAdjustmentResponse r = await _forfeitureAdjustmentService.GetSuggestedForfeitureAmount(req, ct);
        await Send.OkAsync(r, ct);
    }
}
