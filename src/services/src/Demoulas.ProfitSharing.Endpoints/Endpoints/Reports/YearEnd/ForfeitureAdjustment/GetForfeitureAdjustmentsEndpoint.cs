using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public class GetForfeitureAdjustmentsEndpoint : ProfitSharingEndpoint<SuggestedForfeitureAdjustmentRequest, Results<Ok<SuggestedForfeitureAdjustmentResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IForfeitureAdjustmentService _forfeitureAdjustmentService;
    private readonly ILogger<GetForfeitureAdjustmentsEndpoint> _logger;

    public GetForfeitureAdjustmentsEndpoint(IForfeitureAdjustmentService forfeitureAdjustmentService, ILogger<GetForfeitureAdjustmentsEndpoint> logger) : base(Navigation.Constants.Forfeitures)
    {
        _forfeitureAdjustmentService = forfeitureAdjustmentService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("forfeiture-adjustments");
        Summary(s =>
        {
            s.Summary = "Get forfeiture suggested adjustments for badge number or SSN.";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Description = "This endpoint is used to get a suggested forfeiture adjustment for a badge number or SSN. Uses POST to keep SSN out of the URL.";
        });
        Group<AdhocReportsGroup>();
    }
    public override Task<Results<Ok<SuggestedForfeitureAdjustmentResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(SuggestedForfeitureAdjustmentRequest req, CancellationToken ct)
    {
        var sensitiveFields = req.Ssn.HasValue
            ? new[] { "Ssn" }
            : Array.Empty<string>();

        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _forfeitureAdjustmentService.GetSuggestedForfeitureAmount(req, ct);
            return result.ToHttpResult(Error.EmployeeNotFound);
        }, sensitiveFields);
    }
}
