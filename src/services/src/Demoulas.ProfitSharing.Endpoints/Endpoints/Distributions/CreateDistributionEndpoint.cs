using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Distributions;

public sealed class CreateDistributionEndpoint : ProfitSharingEndpoint<CreateDistributionRequest, Results<Ok<CreateOrUpdateDistributionResponse>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IDistributionService _distributionService;
    private readonly ILogger<CreateDistributionEndpoint> _logger;

    public CreateDistributionEndpoint(IDistributionService distributionService, ILogger<CreateDistributionEndpoint> logger)
        : base(Navigation.Constants.Distributions)
    {
        _distributionService = distributionService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/");
        Group<DistributionGroup>();
        Policies(Security.Policy.CanManageDistributions); // Override group policy - create requires manage permission
        Summary(s =>
        {
            s.Summary = "Create a new profit sharing distribution in the current profit year.";
            s.Description = "Creates a new profit sharing distribution record for the current profit year. ";
            s.ExampleRequest = CreateDistributionRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new CreateOrUpdateDistributionResponse
                    {
                        Id = 1,
                        BadgeNumber = 12345,
                        StatusId = 'A',
                        FrequencyId = 'M',
                        FederalTaxPercentage = 15.0m,
                        FederalTaxAmount = 150.00m,
                        StateTaxPercentage = 5.0m,
                        StateTaxAmount = 50.00m,
                        GrossAmount = 1000.00m,
                        CheckAmount = 800.00m,
                        TaxCodeId = '1',
                        MaskSsn = "XXX-XX-6789",
                        PaymentSequence = 1,
                        CreatedAt = DateTime.UtcNow,
                        Memo = "Distribution for June 2024"
                    }
                }
            };
        });
    }

    protected override async Task<Results<Ok<CreateOrUpdateDistributionResponse>, NotFound, BadRequest, ProblemHttpResult>> HandleRequestAsync(CreateDistributionRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _distributionService.CreateDistributionAsync(req, ct);

            // Business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distribution-create"),
                new("endpoint", "CreateDistributionEndpoint"));

            if (result.IsSuccess)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(1,
                    new("record_type", "distribution-created"),
                    new("endpoint", "CreateDistributionEndpoint"));

                _logger.LogInformation("Distribution created with ID: {Id}, Badge: {BadgeNumber}, Gross Amount: {GrossAmount} (correlation: {CorrelationId})",
                    result.Value?.Id, result.Value?.BadgeNumber, result.Value?.GrossAmount, HttpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning("Distribution creation failed - {Error} (correlation: {CorrelationId})",
                    result.Error, HttpContext.TraceIdentifier);
            }

            var httpResult = result.ToHttpResultWithValidation(Error.BadgeNumberNotFound);
            this.RecordResponseMetrics(HttpContext, _logger, httpResult);

            return httpResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
