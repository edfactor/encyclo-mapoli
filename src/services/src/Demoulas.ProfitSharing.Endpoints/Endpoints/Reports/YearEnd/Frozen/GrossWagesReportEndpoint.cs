using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public sealed class GrossWagesReportEndpoint : EndpointWithCsvTotalsBase<GrossWagesReportRequest, GrossWagesReportResponse, GrossWagesReportDetail, GrossWagesReportEndpoint.GrossReportMapper>
{
    private readonly IFrozenReportService _frozenReportService;
    private readonly ILogger<GrossWagesReportEndpoint> _logger;

    public override string ReportFileName => GrossWagesReportResponse.REPORT_NAME;

    public GrossWagesReportEndpoint(IFrozenReportService frozenReportService, ILogger<GrossWagesReportEndpoint> logger)
        : base(Navigation.Constants.ProfShareGrossRpt)
    {
        _frozenReportService = frozenReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("frozen/gross-wages");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of employees whose wages exceeded the input amount";
            s.ExampleRequest = GrossWagesReportRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, GrossWagesReportResponse.ResponseExample()
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<GrossWagesReportResponse> GetResponse(GrossWagesReportRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _frozenReportService.GetGrossWagesReport(req, ct);

            // Record year-end frozen gross wages report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-frozen-gross-wages-report"),
                new("endpoint", "GrossWagesReportEndpoint"),
                new("report_type", "frozen-wages"),
                new("report_code", "GROSS-WAGES-RPT"),
                new("minimum_gross_amount", req.MinGrossAmount.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "gross-wages-employees"),
                new("endpoint", "GrossWagesReportEndpoint"));

            _logger.LogInformation("Year-end frozen gross wages report generated with minimum gross amount {MinGrossAmount}, returned {Count} employees (correlation: {CorrelationId})",
                req.MinGrossAmount, resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new GrossWagesReportResponse
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] },
                TotalGrossWages = 0,
                TotalProfitSharingAmount = 0,
                TotalLoans = 0,
                TotalForfeitures = 0
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

    public class GrossReportMapper : ClassMap<GrossWagesReportDetail>
    {
        public GrossReportMapper()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE #");
            Map(m => m.EmployeeName).Index(1).Name("EMPLOYEE NAME");
            Map(m => m.Ssn).Index(2).Name("SSN NUM");
            Map(m => m.DateOfBirth).Index(3).Name("D-O-B");
            Map(m => m.GrossWages).Index(4).Name("PS WAGES");
            Map(m => m.ProfitSharingAmount).Index(5).Name("PS AMOUNT");
            Map(m => m.Loans).Index(6).Name("LOANS");
            Map(m => m.Forfeitures).Index(7).Name("FORFEITURES");
            Map(m => m.EnrollmentId).Index(8).Name("ENROLLMENT ID");
        }
    }
}
