using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class UpdateForfeitureAdjustmentEndpoint : ProfitSharingEndpoint<ForfeitureAdjustmentUpdateRequest, Results<NoContent, ProblemHttpResult>>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;
    private readonly ILogger<UpdateForfeitureAdjustmentEndpoint> _logger;

    public UpdateForfeitureAdjustmentEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService, ILogger<UpdateForfeitureAdjustmentEndpoint> logger)
        : base(Navigation.Constants.ProfitShareForfeit)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("forfeiture-adjustments/update");
        Summary(s =>
        {
            s.Summary = "Update forfeiture adjustment for a badge number";
            s.Description = "This endpoint updates the forfeiture adjustment for a specific badge number";
            s.ExampleRequest = ForfeitureAdjustmentUpdateRequest.RequestExample();
            s.Responses[204] = "Successfully updated the forfeiture adjustment";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "Badge number not found";
        });
        Group<YearEndGroup>();
    }
    public override async Task<Results<NoContent, ProblemHttpResult>> ExecuteAsync(ForfeitureAdjustmentUpdateRequest req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            await _forfeitureAdjustmentService.UpdateForfeitureAdjustmentAsync(req, ct);
            return TypedResults.NoContent();
        }, "operation:year-end-forfeiture-adjustment-update", $"badge_number:{req.BadgeNumber}", $"forfeiture_amount:{req.ForfeitureAmount}", $"class_action:{req.ClassAction}", $"profit_year:{req.ProfitYear}");
    }
}
