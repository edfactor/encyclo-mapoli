using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class CreateMilitaryContributionRecord : ProfitSharingEndpoint<CreateMilitaryContributionRequest, Results<Created<MilitaryContributionResponse>, NotFound, ProblemHttpResult>>
{
    private readonly IMilitaryService _militaryService;
    private readonly ILogger<CreateMilitaryContributionRecord> _logger;

    public CreateMilitaryContributionRecord(IMilitaryService militaryService, ILogger<CreateMilitaryContributionRecord> logger) : base(Navigation.Constants.MilitaryContributions)
    {
        _militaryService = militaryService ?? throw new ArgumentNullException(nameof(militaryService));
        _logger = logger;
    }

    public override void Configure()
    {
        Post(string.Empty);
        Summary(s =>
        {
            s.Summary = "Create Military Contribution Record";
            s.ResponseExamples = new Dictionary<int, object>
            {
                { 201, new MilitaryContributionResponse { ProfitYear = Convert.ToInt16(DateTime.Now.Year), BadgeNumber = 1234567 } }
            };
            s.ExampleRequest = CreateMilitaryContributionRequest.RequestExample();
            s.Responses[404] = "Not Found. Employee not found.";
        });
        Group<MilitaryGroup>();
    }

    public override Task<Results<Created<MilitaryContributionResponse>, NotFound, ProblemHttpResult>> ExecuteAsync(CreateMilitaryContributionRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _militaryService.CreateMilitaryServiceRecordAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "military-contribution-create"),
                new("endpoint", nameof(CreateMilitaryContributionRecord)));

            if (result.IsSuccess)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(1,
                    new("record_type", "military-contribution"),
                    new("endpoint", nameof(CreateMilitaryContributionRecord)));

                _logger.LogInformation("Military contribution record created for Badge: {BadgeNumber}, ProfitYear: {ProfitYear} (correlation: {CorrelationId})",
                    req.BadgeNumber, req.ProfitYear, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Military contribution record creation failed for Badge: {BadgeNumber}, ProfitYear: {ProfitYear} - {Error} (correlation: {CorrelationId})",
                    req.BadgeNumber, req.ProfitYear, result.Error?.Description, HttpContext.TraceIdentifier);
            }

            return result.Match<Results<Created<MilitaryContributionResponse>, NotFound, ProblemHttpResult>>(
                success => TypedResults.Created($"/military-contributions/{success.BadgeNumber}/{success.ProfitYear}", success),
                error => error.Detail == Error.EmployeeNotFound.Description
                    ? TypedResults.NotFound()
                    : TypedResults.Problem(error.Detail));
        }, "BadgeNumber");
    }
}
