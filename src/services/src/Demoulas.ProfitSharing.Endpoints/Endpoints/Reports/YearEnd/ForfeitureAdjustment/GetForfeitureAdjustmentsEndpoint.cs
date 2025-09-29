using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
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
        Get("forfeiture-adjustments");
        Summary(s =>
        {
            s.Summary = "Get forfeiture suggested adjustments for  badge number or ssn.";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
            s.Description = "This endpoint is used to get a suggested forfeiture adjustment for a badge number or ssn.";
        });
        Group<YearEndGroup>();
    }
    public override Task<Results<Ok<SuggestedForfeitureAdjustmentResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(SuggestedForfeitureAdjustmentRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var response = await _forfeitureAdjustmentService.GetSuggestedForfeitureAmount(req, ct);
            return Result<SuggestedForfeitureAdjustmentResponse>.Success(response).ToHttpResult();
        }, "operation:year-end-forfeiture-adjustments-get", $"has_ssn:{req.Ssn.HasValue}", $"has_badge:{req.Badge.HasValue}", $"ssn:{req.Ssn}", $"badge:{req.Badge}");
    }
}
