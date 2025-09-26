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
using Microsoft.Extensions.Logging;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup.DistributionsAndForfeitureEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Cleanup;

public class DistributionsAndForfeitureEndpoint : EndpointWithCsvTotalsBase<DistributionsAndForfeituresRequest,
    DistributionsAndForfeitureTotalsResponse,
    DistributionsAndForfeitureResponse,
    DistributionsAndForfeitureResponseMap>
{
    private readonly ICleanupReportService _cleanupReportService;
    private readonly ILogger<DistributionsAndForfeitureEndpoint> _logger;

    public DistributionsAndForfeitureEndpoint(ICleanupReportService cleanupReportService, ILogger<DistributionsAndForfeitureEndpoint> logger)
        : base(Navigation.Constants.DistributionsAndForfeitures)
    {
        _cleanupReportService = cleanupReportService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("distributions-and-forfeitures");
        Summary(s =>
        {
            s.Summary = "Lists distributions and forfeitures for a date range";
            s.ExampleRequest = new DistributionsAndForfeituresRequest() { Skip = SimpleExampleRequest.Skip, Take = SimpleExampleRequest.Take };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<DistributionsAndForfeitureResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<DistributionsAndForfeitureResponse>
                        {
                            Results = new List<DistributionsAndForfeitureResponse> { DistributionsAndForfeitureResponse.ResponseExample() }
                        }
                    }
                }
            };

        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "DistributionsAndForfeitures";

    public override async Task<DistributionsAndForfeitureTotalsResponse> GetResponse(DistributionsAndForfeituresRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _cleanupReportService.GetDistributionsAndForfeitureAsync(req, ct);

            // Record year-end cleanup report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-cleanup-distributions-forfeitures"),
                new("endpoint", "DistributionsAndForfeitureEndpoint"),
                new("report_type", "cleanup"),
                new("cleanup_type", "distributions-and-forfeitures"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "distributions-forfeitures-cleanup"),
                new("endpoint", "DistributionsAndForfeitureEndpoint"));

            _logger.LogInformation("Year-end cleanup report for distributions and forfeitures generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new DistributionsAndForfeitureTotalsResponse
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] },
                DistributionTotal = 0,
                StateTaxTotal = 0,
                FederalTaxTotal = 0,
                ForfeitureTotal = 0,
                StateTaxTotals = new Dictionary<string, decimal>()
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

    public sealed class DistributionsAndForfeitureResponseMap : ClassMap<DistributionsAndForfeitureResponse>
    {
        public DistributionsAndForfeitureResponseMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE #");
            Map(m => m.EmployeeName).Index(1).Name("NAME");
            Map(m => m.Ssn).Index(2).Name("SSN");
            Map(m => m.Date).Index(3).Name("DATE");
            Map(m => m.DistributionAmount).Index(4).Name("DISTRIBUTION AMOUNT");
            Map(m => m.TaxCode).Index(5).Name("TC");
            Map(m => m.StateTax).Index(6).Name("STATE TAX");
            Map(m => m.FederalTax).Index(7).Name("FEDERAL TAX");
            Map(m => m.ForfeitAmount).Index(8).Name("FORFEIT AMOUNT");
            Map(m => m.Age).Index(9).Name("AGE");
            Map(m => m.OtherName).Index(10).Name("OTHER NAME");
            Map(m => m.OtherSsn).Index(11).Name("OTHER SSN");
            Map(m => m.HasForfeited).Index(12).Name("Forfeited");
        }
    }
}
