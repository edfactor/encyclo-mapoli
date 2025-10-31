using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Report;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareReport;

/// <summary>
/// Endpoint for generating the year-end profit sharing report. Returns a list of employees eligible for profit sharing, with filtering options and CSV export support.
/// </summary>
public class YearEndProfitSharingReportEndpoint : EndpointWithCsvTotalsBase<YearEndProfitSharingReportRequest, YearEndProfitSharingReportResponse, YearEndProfitSharingReportDetail, YearEndProfitSharingReportEndpoint.YearEndProfitSharingReportClassMap>
{
    private readonly IProfitSharingSummaryReportService _cleanupReportService;
    private readonly IAuditService _auditService;
    private readonly ILogger<YearEndProfitSharingReportEndpoint> _logger;
    private const string Report_Name = "Yearend Profit Sharing Report";

    public YearEndProfitSharingReportEndpoint(IProfitSharingSummaryReportService cleanupReportService, IAuditService auditService, ILogger<YearEndProfitSharingReportEndpoint> logger)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _cleanupReportService = cleanupReportService;
        _auditService = auditService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("yearend-profit-sharing-report");
        Summary(s =>
        {
            s.Summary = Report_Name;
            s.Description = @"Returns a list of employees who will participate in the profit sharing this year, as well as their qualifying attributes.\n\nRequest parameters allow filtering by age, hours, employment status, and more. The endpoint supports CSV export if the Accept header is set to 'text/csv'.\n\n" +
                "ReportId options (see enum YearEndProfitSharingReportId):\n" +
                string.Join("\n", Enum.GetValues(typeof(YearEndProfitSharingReportId))
                    .Cast<YearEndProfitSharingReportId>()
                    .Select(e => $"{(int)e}: {GetEnumDescription(e)}"));

            s.ExampleRequest = new YearEndProfitSharingReportRequest() { ProfitYear = 2025, ReportId = YearEndProfitSharingReportId.Age18To20With1000Hours, Skip = SimpleExampleRequest.Skip, Take = SimpleExampleRequest.Take };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    YearEndProfitSharingReportResponse.ResponseExample()
                }
            };
            s.Responses[400] = "Bad request. Invalid or missing parameters.";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }

    /// <summary>
    /// Handles the request and returns the year-end profit sharing report response.
    /// </summary>
    public override async Task<YearEndProfitSharingReportResponse> GetResponse(YearEndProfitSharingReportRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // This endpoint accesses SSN data, so mark as sensitive
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            var response = await _auditService.ArchiveCompletedReportAsync(
                Report_Name,
                req.ProfitYear,
                req,
                (archiveReq, _, cancellationToken) => _cleanupReportService.GetYearEndProfitSharingReportAsync(archiveReq, cancellationToken),
                ct);

            // Record year-end profit sharing report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-profit-sharing-report"),
                new("endpoint", "YearEndProfitSharingReportEndpoint"),
                new("profit_year", req.ProfitYear.ToString()),
                new("report_id", req.ReportId.ToString()));

            // Record counts processed
            var recordCount = response.Response?.Results?.Count() ?? 0;

            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new("endpoint", "YearEndProfitSharingReportEndpoint"),
                new("entity_type", "year-end participants"),
                new("profit_year", req.ProfitYear.ToString()));

            this.RecordResponseMetrics(HttpContext, _logger, response);

            return response;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex);
            throw;
        }
    }

    public override string ReportFileName => "yearend-profit-sharing-report";

    private static string GetEnumDescription(YearEndProfitSharingReportId value)
    {
        var field = typeof(YearEndProfitSharingReportId).GetField(value.ToString());
        var attr = (System.ComponentModel.DescriptionAttribute?)Attribute.GetCustomAttribute(field!, typeof(System.ComponentModel.DescriptionAttribute));
        return attr?.Description ?? value.ToString();
    }

    public class YearEndProfitSharingReportClassMap : ClassMap<YearEndProfitSharingReportDetail>
    {
        public YearEndProfitSharingReportClassMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("Badge Number");
            Map(m => m.EmployeeName).Index(1).Name("Employee Name");
            Map(m => m.StoreNumber).Index(2).Name("Store Number");
            Map(m => m.EmployeeTypeCode).Index(3).Name("Employee Type");
            Map(m => m.DateOfBirth).Index(4).Name("Date of Birth");
            Map(m => m.Age).Index(5).Name("Age");
            Map(m => m.Ssn).Index(6).Name("SSN");
            Map(m => m.Wages).Index(7).Name("Wages");
            Map(m => m.Hours).Index(8).Name("Hours");
            Map(m => m.Points).Index(9).Name("Points");
            Map(m => m.IsUnder21).Index(10).Name("Is under 21");
            Map(m => m.IsNew).Index(11).Name("Is new");
            Map(m => m.EmployeeStatus).Index(12).Name("Employee Status");
            Map(m => m.Balance).Index(13).Name("Balance");
            Map(m => m.YearsInPlan).Index(14).Name("Years in Plan");
            Map(m => m.TerminationDate).Index(15).Name("Inactive date");
        }
    }

}
