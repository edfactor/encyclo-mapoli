using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.PayBen;
using Demoulas.ProfitSharing.Common.Contracts.Response.PayBen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.PayBen;

public class PayBenReportEndpoint : ProfitSharingEndpoint<PayBenReportRequest,
    PaginatedResponseDto<PayBenReportResponse>>
{
    private readonly IPayBenReportService _reportService;
    private readonly ILogger<PayBenReportEndpoint> _logger;

    public PayBenReportEndpoint(IPayBenReportService reportService, ILogger<PayBenReportEndpoint> logger)
        : base(Navigation.Constants.PayBenReport)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("payben-report");
        Summary(s =>
        {
            s.Summary = "PayBen Report";
            s.Description = "Returns a report of beneficiary and their percentage";
            s.ExampleRequest = new PayBenReportRequest();
            s.ResponseExamples = new Dictionary<int, object> { { 200, new PaginatedResponseDto<PayBenReportResponse>() } };
        });
        Group<YearEndGroup>();
    }


    public override Task<PaginatedResponseDto<PayBenReportResponse>> ExecuteAsync(PayBenReportRequest req, CancellationToken ct)
    {
        return this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            var result = await _reportService.GetPayBenReport(req, ct);

            // Record business metrics
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new KeyValuePair<string, object?>("operation", "payben-report"),
                new KeyValuePair<string, object?>("endpoint.category", "reports"));

            // Record result count
            var resultCount = result?.Results?.Count() ?? 0;
            Demoulas.ProfitSharing.Common.Telemetry.EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new KeyValuePair<string, object?>("operation", "payben-report"),
                new KeyValuePair<string, object?>("endpoint.category", "reports"));

            _logger.LogInformation("PayBen report generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            return result ?? new PaginatedResponseDto<PayBenReportResponse>();
        });
    }
}
