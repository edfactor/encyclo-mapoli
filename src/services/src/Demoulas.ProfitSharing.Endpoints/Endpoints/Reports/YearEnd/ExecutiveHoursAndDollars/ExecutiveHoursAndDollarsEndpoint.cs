using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
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

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;

public class ExecutiveHoursAndDollarsEndpoint :
    EndpointWithCsvBase<ExecutiveHoursAndDollarsRequest, ExecutiveHoursAndDollarsResponse, ExecutiveHoursAndDollarsEndpoint.ExecutiveHoursAndDollarsMap
    >
{
    private readonly IExecutiveHoursAndDollarsService _reportService;
    private readonly IAuditService _auditService;
    private readonly ILogger<ExecutiveHoursAndDollarsEndpoint> _logger;

    public ExecutiveHoursAndDollarsEndpoint(IExecutiveHoursAndDollarsService reportService, IAuditService auditService, ILogger<ExecutiveHoursAndDollarsEndpoint> logger)
        : base(Navigation.Constants.ManageExecutiveHours)
    {
        _reportService = reportService;
        _auditService = auditService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("executive-hours-and-dollars");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "The Executive Hours and Dollars Endpoint endpoint produces a list of executives with hours and dollars.";

            s.ExampleRequest = SimpleExampleRequest;
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "Executive Hours and Dollars";

    public override async Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetResponse(ExecutiveHoursAndDollarsRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _auditService.ArchiveCompletedReportAsync(ReportFileName,
                req.ProfitYear,
                req,
                (archiveReq, isArchiveRequest, cancellationToken) =>
                {
                    if (isArchiveRequest)
                    {
                        archiveReq = archiveReq with
                        {
                            BadgeNumber = null,
                            FullNameContains = null,
                            Ssn = null,
                            HasExecutiveHoursAndDollars = true,
                            IsMonthlyPayroll = true
                        };
                    }

                    return _reportService.GetExecutiveHoursAndDollarsReportAsync(archiveReq, cancellationToken);
                },
                ct);

            // Record year-end executive hours and dollars report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-executive-hours-dollars"),
                new("endpoint", "ExecutiveHoursAndDollarsEndpoint"),
                new("report_type", "executive-hours-dollars"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "executive-hours-dollars"),
                new("endpoint", "ExecutiveHoursAndDollarsEndpoint"));

            _logger.LogInformation("Year-end executive hours and dollars report generated for year {ProfitYear}, returned {Count} records (correlation: {CorrelationId})",
                req.ProfitYear, resultCount, HttpContext?.TraceIdentifier ?? "test-correlation");

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);

                // DEBUG: Before returning, validate each record can be serialized
                if (result.Response?.Results?.Any() == true)
                {
                    var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase };
                    foreach (var record in result.Response.Results)
                    {
#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both
                        try
                        {
                            // Try to serialize each record to catch which one fails
                            var json = System.Text.Json.JsonSerializer.Serialize(record, jsonOptions);
                            _logger.LogDebug("Badge {Badge} serialized successfully: {JsonLength} bytes", record.BadgeNumber, json.Length);
                        }
                        catch (Exception serEx)
                        {
                            _logger.LogError(serEx, "Badge {Badge} FAILED to serialize. FullName: {FullName}, HoursExecutive: {Hours}, IncomeExecutive: {Income}, CurrentHoursYear: {CurHours}, CurrentIncomeYear: {CurIncome}",
                                record.BadgeNumber, record.FullName, record.HoursExecutive, record.IncomeExecutive, record.CurrentHoursYear, record.CurrentIncomeYear);
                            throw;
                        }
#pragma warning restore S2139 // Exceptions should be either logged or rethrown but not both
                    }
                }

                return result;
            }

            var emptyResult = new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
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

    public sealed class ExecutiveHoursAndDollarsMap : ClassMap<ExecutiveHoursAndDollarsResponse>
    {
        public ExecutiveHoursAndDollarsMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE");
            Map(m => m.FullName).Index(1).Name("NAME");
            Map(m => m.StoreNumber).Index(2).Name("STR");
            Map(m => m.HoursExecutive).Index(3).Name("EXEC HRS");
            Map(m => m.IncomeExecutive).Index(4).Name("EXEC DOLS");
            Map(m => m.CurrentHoursYear).Index(5).Name("ORA HRS CUR");
            Map(m => m.CurrentIncomeYear).Index(6).Name("ORA DOLS CUR");
            Map(m => m.PayFrequencyId).Index(7).Name("FREQ");
            Map(m => m.EmploymentStatusId).Index(8).Name("STATUS");
            Map(m => m.Ssn).Index(9).Name("SSN");
        }
    }
}

