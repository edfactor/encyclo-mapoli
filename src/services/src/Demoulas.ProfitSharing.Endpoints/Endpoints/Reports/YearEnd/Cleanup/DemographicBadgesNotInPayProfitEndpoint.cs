using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class DemographicBadgesNotInPayProfitEndpoint : EndpointWithCsvBase<ProfitYearRequest, DemographicBadgesNotInPayProfitResponse,
    DemographicBadgesNotInPayProfitEndpoint.DemographicBadgesNotInPayProfitResponseMap>
{
    private readonly ICleanupReportService _cleanupReportService;
    private readonly ILogger<DemographicBadgesNotInPayProfitEndpoint> _logger;

    public DemographicBadgesNotInPayProfitEndpoint(ICleanupReportService cleanupReportService, ILogger<DemographicBadgesNotInPayProfitEndpoint> logger)
        : base(Navigation.Constants.DemographicBadgesNotInPayProfit)
    {
        _cleanupReportService = cleanupReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("demographic-badges-not-in-payprofit");
        Summary(s =>
        {
            s.Summary = "Demographic badges not in Payprofit";
            s.Description = "Returns a list of employees who have demographic badges but are not in Payprofit.";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<DemographicBadgesNotInPayProfitResponse>()
                        {
                            Results = new List<DemographicBadgesNotInPayProfitResponse>
                            {
                                new DemographicBadgesNotInPayProfitResponse()
                                {
                                    BadgeNumber = 47425,
                                    Ssn = "XXX-XX-7425",
                                    EmployeeName = "John",
                                    Status = EmploymentStatus.Constants.Active
                                },
                                new DemographicBadgesNotInPayProfitResponse
                                {
                                    BadgeNumber = 82424,
                                    Ssn = "XXX-XX-2424",
                                    EmployeeName = "Jane",
                                    Status = EmploymentStatus.Constants.Delete
                                },
                                new DemographicBadgesNotInPayProfitResponse
                                {
                                    BadgeNumber = 85744,
                                    Ssn = "XXX-XX-5744",
                                    EmployeeName = "Tim",
                                    Status = EmploymentStatus.Constants.Inactive
                                },
                                new DemographicBadgesNotInPayProfitResponse
                                {
                                    BadgeNumber = 94861,
                                    Ssn = "XXX-XX-4861",
                                    EmployeeName = "Sally",
                                    Status = EmploymentStatus.Constants.Terminated,
                                    Store = 4
                                }
                            }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "DEMOGRAPHIC BADGES NOT IN PAYPROFIT";

    public override async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _cleanupReportService.GetDemographicBadgesNotInPayProfitAsync(req, ct);

            // Record year-end cleanup report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-cleanup-demographic-badges"),
                new("endpoint", "DemographicBadgesNotInPayProfitEndpoint"),
                new("report_type", "cleanup"),
                new("cleanup_type", "demographic-badges-not-in-payprofit"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "demographic-badges-cleanup"),
                new("endpoint", "DemographicBadgesNotInPayProfitEndpoint"));

            _logger.LogInformation("Year-end cleanup report for demographic badges not in payprofit generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ReportResponseBase<DemographicBadgesNotInPayProfitResponse>
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

    public sealed class DemographicBadgesNotInPayProfitResponseMap : ClassMap<DemographicBadgesNotInPayProfitResponse>
    {
        public DemographicBadgesNotInPayProfitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("DEM SSN");
            Map(m => m.EmployeeName).Index(4).Name("Name");
            Map(m => m.Store).Index(5).Name("Store");
            Map(m => m.Status).Index(6).Name("StatusEnum");
        }
    }
}
