using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.DistributionsByAgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class DistributionsByAgeEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, DistributionsByAge, DistributionsByAgeDetail, ProfitSharingDistributionsByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;
    private readonly ILogger<DistributionsByAgeEndpoint> _logger;

    public DistributionsByAgeEndpoint(IFrozenReportService frozenReportService, ILogger<DistributionsByAgeEndpoint> logger)
        : base(Navigation.Constants.DistributionsByAge)
    {
        _frozenReportService = frozenReportService;
        _logger = logger;
    }

    public override string ReportFileName => "PROFIT SHARING DISTRIBUTIONS BY AGE";

    public override void Configure()
    {
        Get("frozen/distributions-by-age");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their distribution over the year grouped by age and aggregated to separate regular from hardships";

            s.ExampleRequest = FrozenReportsByAgeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, DistributionsByAge.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<DistributionsByAge> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var result = await _frozenReportService.GetDistributionsByAgeYearAsync(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "distributions_by_age"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "frozen-distributions-by-age"),
                new("endpoint", "DistributionsByAgeEndpoint"));

            if (result?.TotalEmployees > 0)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(result.TotalEmployees,
                    new("operation", "distributions_by_age"),
                    new("metric_type", "total_employees"));

                EndpointTelemetry.RecordCountsProcessed.Record((long)result.DistributionTotalAmount,
                    new("operation", "distributions_by_age"),
                    new("metric_type", "distribution_total_amount"));
            }

            _logger.LogInformation("Year-end frozen distributions by age report generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new DistributionsByAge
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] },
                TotalEmployees = 0,
                RegularTotalEmployees = 0,
                HardshipTotalEmployees = 0,
                RegularTotalAmount = 0M,
                HardshipTotalAmount = 0M
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

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, DistributionsByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingDistributionsByAgeMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        // Write out totals
        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("");
        csvWriter.WriteField(report.RegularTotalEmployees);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("HARDSHIP");
        csvWriter.WriteField(report.HardshipTotalEmployees);
        csvWriter.WriteField(report.HardshipTotalAmount);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("DIST TTL");
        csvWriter.WriteField("");
        csvWriter.WriteField(report.DistributionTotalAmount);

        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<DistributionsByAgeDetail>();
        await csvWriter.NextRecordAsync();


    }


    public class ProfitSharingDistributionsByAgeMapper : ClassMap<DistributionsByAgeDetail>
    {
        public ProfitSharingDistributionsByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.Amount).Index(2).Name("AMOUNT");
        }
    }
}
