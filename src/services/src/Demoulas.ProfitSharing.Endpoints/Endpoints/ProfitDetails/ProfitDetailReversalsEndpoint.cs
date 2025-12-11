using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.ProfitDetails;

public sealed class ProfitDetailReversalsEndpoint : ProfitSharingEndpoint<IdsRequest, Results<Ok<IdsResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IProfitDetailReversalsService _profitDetailReversalsService;
    private readonly ILogger<ProfitDetailReversalsEndpoint> _logger;

    public ProfitDetailReversalsEndpoint(
        IProfitDetailReversalsService profitDetailReversalsService,
        ILogger<ProfitDetailReversalsEndpoint> logger) : base(Navigation.Constants.ProfitDetailReversals)
    {
        _profitDetailReversalsService = profitDetailReversalsService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/reversals");
        Group<ProfitDetailsGroup>();
        Policies(Security.Policy.CanReverseProfitDetails);
        Validator<IdsRequestValidator>();
        Summary(s =>
        {
            s.Description = "Reverses profit detail entries based on provided IDs. Creates reversal entries for eligible profit codes and updates ETVA balances where applicable.";
            s.Summary = "Reverse Profit Detail Entries";
            s.ExampleRequest = IdsRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 200, IdsResponse.ResponseExample() },
                { 400, new { detail = "Validation error", title = "Validation Failed" } },
                { 404, new { detail = "Profit details not found" } }
            };
        });
    }

    public override Task<Results<Ok<IdsResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(IdsRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _profitDetailReversalsService.ReverseProfitDetailsAsync(req.Ids, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit-detail-reversal"),
                new("endpoint", nameof(ProfitDetailReversalsEndpoint)),
                new("batch_size", req.Ids?.Length.ToString() ?? "0"));

            // Convert Result<bool> to proper HTTP response
            // Use implicit cast to ensure validation errors are properly included in the response
            return result.IsSuccess
                ? TypedResults.Ok(new IdsResponse { Ids = req.Ids ?? Array.Empty<int>() })
                : (Results<Ok<IdsResponse>, NotFound, ProblemHttpResult>)TypedResults.Problem(result.Error!);
        });
    }
}
