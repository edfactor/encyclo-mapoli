using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public sealed class UpdateSummaryReportEndpoint : EndpointWithCsvTotalsBase<FrozenProfitYearRequest, UpdateSummaryReportResponse, UpdateSummaryReportDetail, UpdateSummaryReportEndpoint.UpdateSummaryReportMapper>
{
    private readonly IFrozenReportService _frozenReportService;
    private readonly IAuditService _auditService;
    private readonly ILogger<UpdateSummaryReportEndpoint> _logger;

    public UpdateSummaryReportEndpoint(IFrozenReportService frozenReportService, IAuditService auditService, ILogger<UpdateSummaryReportEndpoint> logger)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _frozenReportService = frozenReportService;
        _auditService = auditService;
        _logger = logger;
    }

    public override string ReportFileName => "UPDATE SUMMARY FOR PROFIT SHARING";

    public override void Configure()
    {
        Get("frozen/updatesummary");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description = "This report produces a list of members showing last year's balance, compared to this years";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>()
            {
                {
                    200, UpdateSummaryReportResponse.ResponseExample()
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<UpdateSummaryReportResponse> GetResponse(FrozenProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "update_summary_report"),
                new("profit_year", req.ProfitYear.ToString()));

            var result = await _auditService.ArchiveCompletedReportAsync(ReportFileName, req.ProfitYear, req,
                (audit, _, cancellationToken) => _frozenReportService.GetUpdateSummaryReport(audit, cancellationToken),
                ct);

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "frozen-update-summary"),
                new("endpoint", "UpdateSummaryReportEndpoint"));

            _logger.LogInformation("Year-end frozen update summary report generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            // Create empty result if needed (though audit service should always return data)
            var emptyResult = new UpdateSummaryReportResponse
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] }
            };

            this.RecordResponseMetrics(HttpContext, _logger, emptyResult);
            return emptyResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }


    public class UpdateSummaryReportMapper : ClassMap<UpdateSummaryReportDetail>
    {
        public UpdateSummaryReportMapper()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE#");
            Map(m => m.Name).Index(2).Name("NAME");
            Map(m => m.StoreNumber).Index(3).Name("STR");

            Map(m => m.Before.ProfitSharingAmount).Index(4).Name("BEFORE_P/S_AMOUNT");
            Map(m => m.Before.VestedProfitSharingAmount).Index(5).Name("BEFORE_P/S_VESTED");
            Map(m => m.Before.YearsInPlan).Index(6).Name("YEARS");
            Map(m => m.Before.EnrollmentId).Index(7).Name("ENROLL");

            Map(m => m.After.ProfitSharingAmount).Index(4).Name("AFTER_P/S_AMOUNT");
            Map(m => m.After.VestedProfitSharingAmount).Index(5).Name("AFTER_P/S_VESTED");
            Map(m => m.After.YearsInPlan).Index(6).Name("YEARS");
            Map(m => m.After.EnrollmentId).Index(7).Name("ENROLL");
        }
    }
}
