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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

/// <summary>
/// Endpoint for generating account history reports showing member account activity by profit year.
/// Allows users to set start and end dates for export and condense data into one line per plan year.
/// </summary>
public sealed class AccountHistoryReportEndpoint : ProfitSharingEndpoint<AccountHistoryReportRequest, Results<Ok<AccountHistoryReportPaginatedResponse>, ProblemHttpResult>>
{
    private readonly IAccountHistoryReportService _accountHistoryReportService;
    private readonly ILogger<AccountHistoryReportEndpoint> _logger;

    public AccountHistoryReportEndpoint(IAccountHistoryReportService accountHistoryReportService, ILogger<AccountHistoryReportEndpoint> logger)
        : base(Navigation.Constants.AccountHistoryReport)
    {
        _accountHistoryReportService = accountHistoryReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/divorce-report");
        Group<AdhocReportsGroup>();
        Summary(s =>
        {
            s.Summary = "Account History Report";
            s.Description = "Returns a report of member account activity condensed by profit year. Includes contributions, earnings, forfeitures, withdrawals, and ending balances for each plan year.";
            s.ExampleRequest = new AccountHistoryReportRequest
            {
                BadgeNumber = 700518,
                StartDate = new DateOnly(2017, 1, 1),
                EndDate = new DateOnly(2024, 12, 31)
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new AccountHistoryReportPaginatedResponse
                    {
                        ReportName = "Account History Report",
                        ReportDate = DateTimeOffset.UtcNow,
                        StartDate = new DateOnly(2017, 1, 1),
                        EndDate = new DateOnly(2024, 12, 31),
                        Response = new PaginatedResponseDto<AccountHistoryReportResponse>
                        {
                            Results = new List<AccountHistoryReportResponse>
                            {
                                new AccountHistoryReportResponse
                                {
                                    Id = 123,
                                    BadgeNumber = 700518,
                                    ProfitYear = 2017,
                                    Contributions = 50000,
                                    Earnings = 5000,
                                    Forfeitures = 0,
                                    Withdrawals = 0,
                                    EndingBalance = 55000
                                },
                                new AccountHistoryReportResponse
                                {
                                    Id = 234,
                                    BadgeNumber = 700518,
                                    ProfitYear = 2008,
                                    Contributions = 55000,
                                    Earnings = 6000,
                                    Forfeitures = 0,
                                    Withdrawals = 0,
                                    EndingBalance = 116000
                                }
                            },
                            Total = 2
                        },
                        CumulativeTotals = new AccountHistoryReportTotals
                        {
                            TotalContributions = 105000,
                            TotalEarnings = 11000,
                            TotalForfeitures = 0,
                            TotalWithdrawals = 0
                        }
                    }
                }
            };
            s.Responses[200] = "Account history report generated successfully";
            s.Responses[400] = "Bad Request. Invalid badge number or date range.";
            s.Responses[500] = "Internal Server Error.";
        });
    }

    public override async Task<Results<Ok<AccountHistoryReportPaginatedResponse>, ProblemHttpResult>> ExecuteAsync(AccountHistoryReportRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            // Record request metrics - this endpoint accesses SSN data
            this.RecordRequestMetrics(HttpContext, _logger, req, "Ssn");

            if (req.BadgeNumber == 0)
            {
                _logger.LogWarning("Account history report requested without valid badge number (correlation: {CorrelationId})",
                    HttpContext.TraceIdentifier);

                var emptyResult = new AccountHistoryReportPaginatedResponse
                {
                    ReportName = "Account History Report",
                    StartDate = req.StartDate ?? new DateOnly(2017, 1, 1),
                    EndDate = req.EndDate ?? DateOnly.FromDateTime(DateTime.Today),
                    Response = new PaginatedResponseDto<AccountHistoryReportResponse>
                    {
                        Results = [],
                        Total = 0
                    },
                    CumulativeTotals = new AccountHistoryReportTotals
                    {
                        TotalContributions = 0,
                        TotalEarnings = 0,
                        TotalForfeitures = 0,
                        TotalWithdrawals = 0
                    }
                };

                this.RecordResponseMetrics(HttpContext, _logger, emptyResult);
                return TypedResults.Ok(emptyResult);
            }

            // Pass the request directly to the service with pagination parameters
            var result = await _accountHistoryReportService.GetAccountHistoryReportAsync(req.BadgeNumber, req, ct);

            // Record account history report business metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "account-history-report-generation"),
                new("endpoint", "AccountHistoryReportEndpoint"),
                new("report_type", "account-history"),
                new("date_range_years", $"{(req.StartDate?.Year ?? 2017)}-{(req.EndDate?.Year ?? DateTime.Today.Year)}"));

            var recordCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new("record_type", "account-history-report-years"),
                new("endpoint", "DivorceReportEndpoint"));

            _logger.LogInformation("Account history report generated for badge {BadgeNumber}, returned {Count} years of activity (correlation: {CorrelationId})",
                req.BadgeNumber, recordCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return TypedResults.Ok(result);
            }

            var emptyReportResult = new AccountHistoryReportPaginatedResponse
            {
                ReportName = "Account History Report",
                StartDate = req.StartDate ?? new DateOnly(2017, 1, 1),
                EndDate = req.EndDate ?? DateOnly.FromDateTime(DateTime.Today),
                Response = new PaginatedResponseDto<AccountHistoryReportResponse>
                {
                    Results = [],
                    Total = 0
                },
                CumulativeTotals = new AccountHistoryReportTotals
                {
                    TotalContributions = 0,
                    TotalEarnings = 0,
                    TotalForfeitures = 0,
                    TotalWithdrawals = 0
                }
            };

            this.RecordResponseMetrics(HttpContext, _logger, emptyReportResult);
            return TypedResults.Ok(emptyReportResult);
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }
}
