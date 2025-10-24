using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;

public sealed class YearEndSetEnrollmentEndpoint : ProfitSharingRequestEndpoint<ProfitYearRequest>
{
    private readonly IYearEndService _yearEndService;
    private readonly ILogger<YearEndSetEnrollmentEndpoint> _logger;

    public YearEndSetEnrollmentEndpoint(IYearEndService yearEndService, ILogger<YearEndSetEnrollmentEndpoint> logger)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _yearEndService = yearEndService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("update-enrollment/{profitYear?}");
        Summary(s =>
        {
            s.Summary = "Updates the enrollment id of all members for the year";
            s.Description = "Accepts profit year as optional route parameter or in request body. Route parameter takes precedence if provided.";
        });
        Group<YearEndGroup>();

        // Wire up FluentValidation validator for profit year validation
        Validator<ProfitYearRequestValidator>();
    }

    public override async Task HandleAsync(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics
            this.RecordRequestMetrics(HttpContext, _logger, req);

            await _yearEndService.UpdateEnrollmentId(req.ProfitYear, ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "update-enrollment"),
                new KeyValuePair<string, object?>("endpoint.category", "year-end"),
                new KeyValuePair<string, object?>("profit_year", req.ProfitYear.ToString()));

            // Log enrollment update (this is a bulk operation)
            _logger.LogInformation("Year-end enrollment updated for profit year {ProfitYear} (correlation: {CorrelationId})",
                req.ProfitYear, HttpContext.TraceIdentifier);

            await Send.NoContentAsync(ct);

            // Record successful response
            this.RecordResponseMetrics(HttpContext, _logger, new { Success = true, ProfitYear = req.ProfitYear }, isSuccess: true);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
