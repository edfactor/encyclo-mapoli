using System.Diagnostics;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Contracts.Response.Military;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Military;

public class CreateMilitaryContributionRecord : ProfitSharingRequestEndpoint<CreateMilitaryContributionRequest>
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
                { 200, new CreateMilitaryContributionRequest { ProfitYear = Convert.ToInt16(DateTime.Now.Year) } }
            };
            s.ExampleRequest = CreateMilitaryContributionRequest.RequestExample();
        });
        Group<MilitaryGroup>();
    }

    public override async Task HandleAsync(CreateMilitaryContributionRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var response = await _militaryService.CreateMilitaryServiceRecordAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "military-contribution-create"),
                new("endpoint", "CreateMilitaryContributionRecord"));

            await response.Match(
                async success =>
                {
                    EndpointTelemetry.RecordCountsProcessed.Record(1,
                        new("record_type", "military-contribution-created"),
                        new("endpoint", "CreateMilitaryContributionRecord"));

                    _logger.LogInformation("Military contribution record created for Badge: {BadgeNumber}, ProfitYear: {ProfitYear} (correlation: {CorrelationId})",
                        req.BadgeNumber, req.ProfitYear, HttpContext.TraceIdentifier);

                    this.RecordResponseMetrics(HttpContext, _logger, success);

                    await Send.CreatedAtAsync<CreateMilitaryContributionRecord>(
                        routeValues: new MilitaryContributionResponse
                        {
                            BadgeNumber = req.BadgeNumber,
                            ProfitYear = req.ProfitYear,
                        },
                        responseBody: success,
                        cancellation: ct
                    );
                },
                async error =>
                {
                    _logger.LogWarning("Military contribution record creation failed for Badge: {BadgeNumber}, ProfitYear: {ProfitYear} - {Error} (correlation: {CorrelationId})",
                        req.BadgeNumber, req.ProfitYear, error, HttpContext.TraceIdentifier);

                    await Send.ResponseAsync(error, statusCode: 400, cancellation: ct);
                }
            );
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

}
