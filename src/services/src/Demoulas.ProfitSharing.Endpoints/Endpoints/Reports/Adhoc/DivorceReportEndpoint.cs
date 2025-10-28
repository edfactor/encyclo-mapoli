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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

/// <summary>
/// Endpoint for generating divorce reports showing member account activity by profit year.
/// Allows users to set start and end dates for export and condense data into one line per plan year.
/// </summary>
public sealed class DivorceReportEndpoint : ProfitSharingEndpoint<DivorceReportRequest, ReportResponseBase<DivorceReportResponse>>
{
    private readonly IDivorceReportService _divorceReportService;
    private readonly ILogger<DivorceReportEndpoint> _logger;

    public DivorceReportEndpoint(IDivorceReportService divorceReportService, ILogger<DivorceReportEndpoint> logger)
        : base(Navigation.Constants.DivorceReport)
    {
        _divorceReportService = divorceReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("divorce-report");
        Group<AdhocReportsGroup>();
        Summary(s =>
        {
            s.Summary = "Divorce Report";
            s.Description = "Returns a report of member account activity condensed by profit year, suitable for divorce proceedings. Includes contributions, withdrawals, distributions, and ending balances for each plan year.";
            s.ExampleRequest = new DivorceReportRequest
            {
                BadgeNumber = 700006,
                StartDate = new DateOnly(2007, 1, 1),
                EndDate = new DateOnly(2024, 12, 31)
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<DivorceReportResponse>
                    {
                        ReportName = "Divorce Report",
                        ReportDate = DateTimeOffset.Now,
                        StartDate = new DateOnly(2007, 1, 1),
                        EndDate = new DateOnly(2024, 12, 31),
                        Response = new PaginatedResponseDto<DivorceReportResponse>
                        {
                            Results = new List<DivorceReportResponse>
                            {
                                new DivorceReportResponse
                                {
                                    BadgeNumber = 700006,
                                    FullName = "John Doe",
                                    Ssn = "123-45-6789",
                                    ProfitYear = 2007,
                                    TotalContributions = 50000,
                                    TotalWithdrawals = 0,
                                    TotalDistributions = 0,
                                    TotalDividends = 5000,
                                    TotalForfeitures = 0,
                                    EndingBalance = 55000,
                                    CumulativeContributions = 50000,
                                    CumulativeWithdrawals = 0,
                                    CumulativeDistributions = 0
                                },
                                new DivorceReportResponse
                                {
                                    BadgeNumber = 700006,
                                    FullName = "John Doe",
                                    Ssn = "123-45-6789",
                                    ProfitYear = 2008,
                                    TotalContributions = 55000,
                                    TotalWithdrawals = 0,
                                    TotalDistributions = 0,
                                    TotalDividends = 6000,
                                    TotalForfeitures = 0,
                                    EndingBalance = 116000,
                                    CumulativeContributions = 105000,
                                    CumulativeWithdrawals = 0,
                                    CumulativeDistributions = 0
                                }
                            },
                            Total = 2
                        }
                    }
                }
            };
        });
    }

    public override async Task HandleAsync(DivorceReportRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics - this endpoint accesses SSN data
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            if (req.BadgeNumber == 0)
            {
                _logger.LogWarning("Divorce report requested without valid badge number (correlation: {CorrelationId})",
                    HttpContext.TraceIdentifier);

                var emptyResult = new ReportResponseBase<DivorceReportResponse>
                {
                    ReportName = "Divorce Report",
                    StartDate = req.StartDate ?? new DateOnly(2007, 1, 1),
                    EndDate = req.EndDate ?? DateOnly.FromDateTime(DateTime.Today),
                    Response = new PaginatedResponseDto<DivorceReportResponse> { Results = [] }
                };

                this.RecordResponseMetrics(HttpContext, _logger, emptyResult);
                Response = emptyResult;
                return;
            }

            // Build the StartAndEndDateRequest from the DivorceReportRequest
            var serviceRequest = new StartAndEndDateRequest
            {
                ProfitYear = (short)DateTime.Now.Year,
                Skip = 0,
                Take = 1000,
                SortBy = "ProfitYear",
                IsSortDescending = true,
                BeginningDate = req.StartDate ?? new DateOnly(2007, 1, 1),
                EndingDate = req.EndDate ?? DateOnly.FromDateTime(DateTime.Today)
            };

            var result = await _divorceReportService.GetDivorceReportAsync(req.BadgeNumber, serviceRequest, ct);

            // Record divorce report business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "divorce-report-generation"),
                new("endpoint", "DivorceReportEndpoint"),
                new("report_type", "divorce"),
                new("date_range_years", $"{serviceRequest.BeginningDate.Year}-{serviceRequest.EndingDate.Year}"));

            var recordCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new("record_type", "divorce-report-years"),
                new("endpoint", "DivorceReportEndpoint"));

            _logger.LogInformation("Divorce report generated for badge {BadgeNumber}, returned {Count} years of activity (correlation: {CorrelationId})",
                req.BadgeNumber, recordCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                Response = result;
                return;
            }

            var emptyReportResult = new ReportResponseBase<DivorceReportResponse>
            {
                ReportName = "Divorce Report",
                StartDate = serviceRequest.BeginningDate,
                EndDate = serviceRequest.EndingDate,
                Response = new PaginatedResponseDto<DivorceReportResponse> { Results = [] }
            };

            this.RecordResponseMetrics(HttpContext, _logger, emptyReportResult);
            Response = emptyReportResult;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
