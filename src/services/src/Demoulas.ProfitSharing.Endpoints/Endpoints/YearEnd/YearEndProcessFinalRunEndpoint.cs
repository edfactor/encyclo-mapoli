using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.YearEnd;

public class YearEndProcessFinalRunEndpoint : ProfitSharingRequestEndpoint<YearRequestWithRebuild>
{
    private readonly IYearEndService _yearEndService;
    private readonly ILogger<YearEndProcessFinalRunEndpoint> _logger;

    public YearEndProcessFinalRunEndpoint(IYearEndService yearEndService, ILogger<YearEndProcessFinalRunEndpoint> logger)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _yearEndService = yearEndService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("final");
        Summary(s =>
        {
            s.Summary = "Commits year-end profit sharing calculations (FINAL RUN)";
            s.Description = @"Finalizes and commits the profit sharing calculations for the specified year by updating critical employee records.

**Updates the following fields:**
- **Earn Points**: Contribution allocation amounts (how much goes toward each employee's contribution)
- **ZeroContributionReason**: Why an employee received zero contribution (Normal, Under21, Terminated (Vest Only), Retired, Soon to be Retired)
- **EmployeeType**: Identifies new employees in the plan (first year >21 with >1000 hours)
- **PsCertificateIssuedDate**: Indicates employee should receive a printed certificate (proxy for Earn Points > 0)

**IMPORTANT USAGE NOTES:**
- ✅ **Safe to run multiple times** BEFORE Master Update is saved
- ⚠️ **DO NOT run** AFTER Master Update - this will cause incorrect earnings and contribution calculations
- This operation is idempotent until Master Update is executed

**Typical Workflow:**
1. Run year-end enrollment updates
2. Review Profit Share Summary report
3. Run this COMMIT/Final Run endpoint
4. Verify calculations
5. Execute Master Update (point of no return)

Use the 'rebuild' parameter to force recalculation even if already committed.";

            s.ExampleRequest = new YearRequestWithRebuild
            {
                ProfitYear = 2024,
                Rebuild = false
            };

            s.Responses[204] = "Success. Year-end final calculations committed.";
            s.Responses[400] = "Bad Request. Invalid profit year or request parameters.";
            s.Responses[500] = "Internal Server Error. Failed to process final run updates.";
        });
        Group<YearEndGroup>();
    }

    public override async Task HandleAsync(YearRequestWithRebuild req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics
            this.RecordRequestMetrics(HttpContext, _logger, req);

            await _yearEndService.RunFinalYearEndUpdates(req.ProfitYear, req.Rebuild, ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "final-year-end-updates"),
                new KeyValuePair<string, object?>("endpoint.category", "year-end"));

            // Log final run processing (this is a significant operation)
            _logger.LogInformation("Year-end final run processing completed for profit year {ProfitYear}, rebuild: {Rebuild} (correlation: {CorrelationId})",
                req.ProfitYear, req.Rebuild, HttpContext.TraceIdentifier);

            await Send.NoContentAsync(ct);

            // Record successful response
            this.RecordResponseMetrics(HttpContext, _logger, new { Success = true }, isSuccess: true);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
