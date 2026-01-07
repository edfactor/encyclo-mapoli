using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces.ItOperations;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;

/// <summary>
/// Endpoint to check which years have complete annuity rate data.
/// Returns completeness status for a range of years (default: current year and previous 5 years).
/// </summary>
public sealed class GetMissingAnnuityYearsEndpoint : ProfitSharingEndpoint<GetMissingAnnuityYearsRequest, Results<Ok<MissingAnnuityYearsResponse>, NotFound, BadRequest, ProblemHttpResult>>
{
    private readonly IAnnuityRatesService _annuityRatesService;
    private readonly ILogger<GetMissingAnnuityYearsEndpoint> _logger;

    public GetMissingAnnuityYearsEndpoint(
        IAnnuityRatesService annuityRatesService,
        ILogger<GetMissingAnnuityYearsEndpoint> logger)
        : base(Navigation.Constants.ManageAnnuityRates)
    {
        _annuityRatesService = annuityRatesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("annuity-rates/missing-years");
        Summary(s =>
        {
            s.Summary = "Checks which years have complete annuity rate data (all required ages defined).";
            s.Description = "Returns completeness status for a range of years. Defaults to current year and previous 5 years if no range specified.";
            s.ExampleRequest = GetMissingAnnuityYearsRequest.RequestExample();
        });
        Group<AdministrationGroup>();
    }

    public override async Task<Results<Ok<MissingAnnuityYearsResponse>, NotFound, BadRequest, ProblemHttpResult>> ExecuteAsync(GetMissingAnnuityYearsRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _annuityRatesService.GetMissingAnnuityYearsAsync(req, ct);

            if (!result.IsSuccess)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result, isSuccess: false);
                return result.ToHttpResultWithValidation();
            }

            var response = result.Value!;

            try
            {
                EndpointTelemetry.BusinessOperationsTotal?.Add(1,
                    new("operation", "check-missing-annuity-years"),
                    new("endpoint", nameof(GetMissingAnnuityYearsEndpoint)));

                var incompleteCount = response.Years.Count(y => !y.IsComplete);
                EndpointTelemetry.RecordCountsProcessed?.Record(incompleteCount,
                    new("record_type", "incomplete-years"),
                    new("endpoint", nameof(GetMissingAnnuityYearsEndpoint)));
            }
            catch
            {
                // Ignore telemetry errors in unit tests
            }

            // Record successful response metrics
            this.RecordResponseMetrics(HttpContext, _logger, response, isSuccess: true);

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            // Record exception and error metrics
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
