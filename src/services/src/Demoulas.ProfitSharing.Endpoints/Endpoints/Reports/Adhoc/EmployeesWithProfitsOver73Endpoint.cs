using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

/// <summary>
/// PROF-LETTER73 - Adhoc report for employees with profits over age 73.
/// Returns JSON only (CSV export removed for simplification).
/// </summary>
public sealed class EmployeesWithProfitsOver73Endpoint
    : ProfitSharingEndpoint<EmployeesWithProfitsOver73Request, Results<Ok<PaginatedResponseDto<EmployeesWithProfitsOver73DetailDto>>, ProblemHttpResult>>
{
    private readonly IEmployeesWithProfitsOver73Service _reportService;
    private readonly ILogger<EmployeesWithProfitsOver73Endpoint> _logger;

    public EmployeesWithProfitsOver73Endpoint(
        IEmployeesWithProfitsOver73Service reportService,
        ILogger<EmployeesWithProfitsOver73Endpoint> logger)
        : base(Navigation.Constants.AdhocProfLetter73)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("prof-letter73");

        Summary(s =>
        {
            s.Summary = "PROF-LETTER73: Employees with Profits Over Age 73";
            s.Description = "Returns employees over age 73 who have profit sharing balances. JSON format only.";
            s.ExampleRequest = EmployeesWithProfitsOver73Request.RequestExample();
            s.Responses[200] = "Success - Returns paginated list of employees over 73";
            s.Responses[403] = $"Forbidden. Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
    }

    protected override async Task<Results<Ok<PaginatedResponseDto<EmployeesWithProfitsOver73DetailDto>>, ProblemHttpResult>> HandleRequestAsync(
        EmployeesWithProfitsOver73Request req,
        CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _reportService.GetEmployeesWithProfitsOver73Async(req, ct);

            // Record standardized business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "employees-profits-over-73-report"),
                new("endpoint", "EmployeesWithProfitsOver73Endpoint"),
                new("report_type", "prof-letter73"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "employees-over-73"),
                new("endpoint", "EmployeesWithProfitsOver73Endpoint"));

            _logger.LogInformation(
                "PROF-LETTER73 report generated for year {ProfitYear}, returned {Count} employees over 73 with balances (correlation: {CorrelationId})",
                req.ProfitYear, resultCount, HttpContext.TraceIdentifier);

            if (result?.Response != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result.Response);
                return TypedResults.Ok(result.Response);
            }

            var emptyResult = new PaginatedResponseDto<EmployeesWithProfitsOver73DetailDto>
            {
                Results = [],
                Total = 0
            };

            this.RecordResponseMetrics(HttpContext, _logger, emptyResult);
            return TypedResults.Ok(emptyResult);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
