using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.ContributionsByAgeEndpoint;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class ContributionsByAgeEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, ContributionsByAge, ContributionsByAgeDetail, ProfitSharingContributionsByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;
    private readonly ILogger<ContributionsByAgeEndpoint> _logger;

    public ContributionsByAgeEndpoint(IFrozenReportService frozenReportService, ILogger<ContributionsByAgeEndpoint> logger)
        : base(Navigation.Constants.ContributionsByAge)
    {
        _frozenReportService = frozenReportService;
        _logger = logger;
    }

    public override string ReportFileName => "PROFIT SHARING CONTRIBUTIONS BY AGE";

    public override void Configure()
    {
        Get("frozen/contributions-by-age");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their contributions over the year grouped by age";

            s.ExampleRequest = FrozenReportsByAgeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, ContributionsByAge.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<ContributionsByAge> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var result = await _frozenReportService.GetContributionsByAgeYearAsync(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "contributions_by_age"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "frozen-contributions-by-age"),
                new("endpoint", "ContributionsByAgeEndpoint"));

            if (result?.TotalEmployees > 0)
            {
                EndpointTelemetry.RecordCountsProcessed.Record(result.TotalEmployees,
                    new("operation", "contributions_by_age"),
                    new("metric_type", "total_employees"));

                EndpointTelemetry.RecordCountsProcessed.Record((long)result.TotalAmount,
                    new("operation", "contributions_by_age"),
                    new("metric_type", "total_amount"));
            }

            _logger.LogInformation("Year-end frozen contributions by age report generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ContributionsByAge
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] },
                TotalEmployees = 0,
                TotalAmount = 0M
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

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, ContributionsByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingContributionsByAgeMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("CONT TTL");
        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalAmount);

        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<ContributionsByAgeDetail>();
        await csvWriter.NextRecordAsync();


    }


    public class ProfitSharingContributionsByAgeMapper : ClassMap<ContributionsByAgeDetail>
    {
        public ProfitSharingContributionsByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.Amount).Index(2).Name("AMOUNT");
        }
    }
}
