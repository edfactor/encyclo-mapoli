using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts; // Result, Error
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages.CurrentYearWagesEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages;

public class CurrentYearWagesEndpoint : EndpointWithCsvBase<ProfitYearRequest, WagesCurrentYearResponse, WagesCurrentYearResponseMap>
{
    private readonly IWagesService _reportService;
    private readonly ILogger<CurrentYearWagesEndpoint> _logger;

    public CurrentYearWagesEndpoint(IWagesService reportService, ILogger<CurrentYearWagesEndpoint> logger) : base(Navigation.Constants.YTDWagesExtract)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("wages-current-year");
        Summary(s =>
        {
            s.Summary = "Wages for the specified year";
            s.Description =
                "Provides a report on employees' wages for the specified year. This report helps identify potential issues that need to be addressed before running profit sharing. The endpoint can be executed multiple times.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<WagesCurrentYearResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<WagesCurrentYearResponse>
                        {
                            Results = new List<WagesCurrentYearResponse> { WagesCurrentYearResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "YTD Wages Extract (PROF-DOLLAR-EXTRACT)";

    public override async Task<ReportResponseBase<WagesCurrentYearResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _reportService.GetWagesReportAsync(req, ct);

            // Record year-end current year wages report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-current-year-wages"),
                new("endpoint", "CurrentYearWagesEndpoint"),
                new("report_type", "wages"),
                new("report_code", "YTD-WAGES-EXTRACT"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "current-year-wages"),
                new("endpoint", "CurrentYearWagesEndpoint"));

            _logger.LogInformation("Year-end current year wages report generated for year {ProfitYear}, returned {Count} wage records (correlation: {CorrelationId})",
                req.ProfitYear, resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ReportResponseBase<WagesCurrentYearResponse>
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
            throw new InvalidOperationException($"Failed to retrieve current year wages: {ex.Message}", ex);
        }
    }


    public sealed class WagesCurrentYearResponseMap : ClassMap<WagesCurrentYearResponse>
    {
        public WagesCurrentYearResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.HoursCurrentYear).Index(3).Name("HOURS YR");
            Map(m => m.IncomeCurrentYear).Index(4).Name("DOLLARS YR");
        }
    }
}
