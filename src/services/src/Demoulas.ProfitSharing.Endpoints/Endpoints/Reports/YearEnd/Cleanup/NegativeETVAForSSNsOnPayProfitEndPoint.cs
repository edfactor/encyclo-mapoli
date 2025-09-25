using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class NegativeEtvaForSsNsOnPayProfitEndPoint : EndpointWithCsvBase<ProfitYearRequest, NegativeEtvaForSsNsOnPayProfitResponse, NegativeEtvaForSsNsOnPayProfitEndPoint.NegativeEtvaForSsNsOnPayProfitResponseMap>
{
    private readonly INegativeEtvaReportService _reportService;
    private readonly ILogger<NegativeEtvaForSsNsOnPayProfitEndPoint> _logger;

    public NegativeEtvaForSsNsOnPayProfitEndPoint(INegativeEtvaReportService reportService, ILogger<NegativeEtvaForSsNsOnPayProfitEndPoint> logger)
        : base(Navigation.Constants.NegativeETVA)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("negative-evta-ssn");
        Summary(s =>
        {
            s.Summary = "Negative ETVA for SSNs on PayProfit";
            s.Description = "ETVA = Early Termination Vested Amount";
            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<NegativeEtvaForSsNsOnPayProfitResponse>()
                        {
                            Results = new List<NegativeEtvaForSsNsOnPayProfitResponse>
                            {
                                new NegativeEtvaForSsNsOnPayProfitResponse { BadgeNumber = 47425, Ssn = "XXX-XX-7425", EtvaValue = -1293.43m }
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

    public override async Task<ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _reportService.GetNegativeETVAForSsNsOnPayProfitResponseAsync(req, ct);

            // Record year-end cleanup report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-cleanup-negative-etva"),
                new("endpoint", "NegativeEtvaForSsNsOnPayProfitEndPoint"),
                new("report_type", "cleanup"),
                new("cleanup_type", "negative-etva-ssns"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "negative-etva-cleanup"),
                new("endpoint", "NegativeEtvaForSsNsOnPayProfitEndPoint"));

            _logger.LogInformation("Year-end cleanup report for negative ETVA SSNs generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ReportResponseBase<NegativeEtvaForSsNsOnPayProfitResponse>
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

    public override string ReportFileName => "ETVA-LESS-THAN-ZERO";

    public sealed class NegativeEtvaForSsNsOnPayProfitResponseMap : ClassMap<NegativeEtvaForSsNsOnPayProfitResponse>
    {
        public NegativeEtvaForSsNsOnPayProfitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("SSN");
            Map(m => m.EtvaValue).Index(4).Name("ETVA");
        }
    }
}
