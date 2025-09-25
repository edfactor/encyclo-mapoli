using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using FastEndpoints;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class UpdateForfeitureAdjustmentBulkEndpoint : ProfitSharingEndpoint<List<ForfeitureAdjustmentUpdateRequest>, Results<NoContent, ProblemHttpResult>>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;
    private readonly ILogger<UpdateForfeitureAdjustmentBulkEndpoint> _logger;

    public UpdateForfeitureAdjustmentBulkEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService, ILogger<UpdateForfeitureAdjustmentBulkEndpoint> logger) : base(Navigation.Constants.Forfeitures)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("forfeiture-adjustments/bulk-update");
        Summary(s =>
        {
            s.Summary = "Update multiple forfeiture adjustments";
            s.Description = "This endpoint updates multiple forfeiture adjustments in a single request";
            s.ExampleRequest =
            [
                ForfeitureAdjustmentUpdateRequest.RequestExample()
            ];
            s.Responses[204] = "Successfully updated the forfeiture adjustments";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Responses[404] = "One or more badge numbers not found";
        });
        Group<YearEndGroup>();
    }
    public override async Task<Results<NoContent, ProblemHttpResult>> ExecuteAsync(List<ForfeitureAdjustmentUpdateRequest> req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            await _forfeitureAdjustmentService.UpdateForfeitureAdjustmentBulkAsync(req, ct);
            return TypedResults.NoContent();
        }, "operation:year-end-forfeiture-adjustment-bulk-update", $"request_count:{req.Count}", $"total_forfeiture_amount:{req.Sum(r => r.ForfeitureAmount)}");
    }
}
