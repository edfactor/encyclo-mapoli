using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Microsoft.Extensions.Logging;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc.TerminatedEmployeesReportEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

public sealed class TerminatedEmployeesReportEndpoint : EndpointWithCsvBase<StartAndEndDateRequest, AdhocTerminatedEmployeeResponse, TerminatedEmployeesReportResponseMap>
{
    private readonly IAdhocTerminatedEmployeesService _adhocTerminatedEmployeesService;
    private readonly ILogger<TerminatedEmployeesReportEndpoint> _logger;

    public TerminatedEmployeesReportEndpoint(IAdhocTerminatedEmployeesService adhocTerminatedEmployeesService, ILogger<TerminatedEmployeesReportEndpoint> logger)
        : base(Navigation.Constants.RecentlyTerminated)
    {
        _adhocTerminatedEmployeesService = adhocTerminatedEmployeesService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("adhoc-terminated-employees-report");
        Summary(s =>
        {
            s.Summary = "Adhoc Terminated Employees Report";
            s.Description = "Returns a report of terminated employees within a specified profit year.";
            s.ExampleRequest = new StartAndEndDateRequest
            {
                Skip = 0,
                Take = 100,
                SortBy = "TerminationDate",
                IsSortDescending = false,
                BeginningDate = new DateOnly(2023, 1, 1),
                EndingDate = new DateOnly(2023, 12, 31)
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<AdhocTerminatedEmployeeResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.MinValue,
                        EndDate = DateOnly.MaxValue,
                        Response = new PaginatedResponseDto<AdhocTerminatedEmployeeResponse>
                        {
                            Results = new List<AdhocTerminatedEmployeeResponse>
                            {
                                new AdhocTerminatedEmployeeResponse
                                {
                                    BadgeNumber = 12345,
                                    FullName = "John Doe",
                                    Ssn = "123-45-6789",
                                    TerminationDate = new DateOnly(2023, 5, 15),
                                    TerminationCodeId = 'A',
                                    TerminationCode = "Active"
                                }
                            },
                            Total = 1
                        }
                    }
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }
    public override string ReportFileName => "Adhoc Terminated Employees Report";

    public override async Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetResponse(StartAndEndDateRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // This endpoint accesses SSN data, so mark as sensitive
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            var result = await _adhocTerminatedEmployeesService.GetTerminatedEmployees(req, ct);

            // Record terminated employees report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "terminated-employees-report"),
                new("endpoint", "TerminatedEmployeesReportEndpoint"),
                new("report_type", "terminated-employees"),
                new("employee_status", "terminated"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "terminated-employees"),
                new("endpoint", "TerminatedEmployeesReportEndpoint"));

            _logger.LogInformation("Terminated employees report generated, returned {Count} employees (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ReportResponseBase<AdhocTerminatedEmployeeResponse>
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

    public class TerminatedEmployeesReportResponseMap : ClassMap<AdhocTerminatedEmployeeResponse>
    {
        public TerminatedEmployeesReportResponseMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("Badge Number");
            Map(m => m.FullName).Index(1).Name("Full Name");
            Map(m => m.Ssn).Index(2).Name("SSN");
            Map(m => m.TerminationDate).Index(3).Name("Termination Date").TypeConverterOption.Format("MM/dd/yyyy");
            Map(m => m.TerminationCodeId).Index(4).Name("Termination Code");
        }
    }
}
