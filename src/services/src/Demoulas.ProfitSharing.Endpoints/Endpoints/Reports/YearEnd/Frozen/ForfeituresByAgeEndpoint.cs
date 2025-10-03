using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.ForfeituresByAgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class ForfeituresByAgeEndpoint : EndpointWithCsvTotalsBase<FrozenReportsByAgeRequest, ForfeituresByAge, ForfeituresByAgeDetail, ProfitSharingForfeituresByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;
    private readonly ILogger<ForfeituresByAgeEndpoint> _logger;

    public ForfeituresByAgeEndpoint(IFrozenReportService frozenReportService, ILogger<ForfeituresByAgeEndpoint> logger)
        : base(Navigation.Constants.ForfeituresByAge)
    {
        _frozenReportService = frozenReportService;
        _logger = logger;
    }

    public override string ReportFileName => "PROFIT SHARING FORFEITURE BY AGE";

    public override void Configure()
    {
        Get("frozen/forfeitures-by-age");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their forfeitures over the year grouped by age";

            s.ExampleRequest = FrozenReportsByAgeRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, ForfeituresByAge.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<ForfeituresByAge> GetResponse(FrozenReportsByAgeRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var result = await _frozenReportService.GetForfeituresByAgeYearAsync(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "forfeitures_by_age"),
                new("profit_year", req.ProfitYear.ToString()));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "frozen-forfeitures-by-age"),
                new("endpoint", "ForfeituresByAgeEndpoint"));

            _logger.LogInformation("Year-end frozen forfeitures by age report generated, returned {Count} records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            // Create empty result if needed
            var emptyResult = new ForfeituresByAge
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

    protected internal override async Task GenerateCsvContent(CsvWriter csvWriter, ForfeituresByAge report, CancellationToken cancellationToken)
    {
        // Register the class map for the main member data
        csvWriter.Context.RegisterClassMap<ProfitSharingForfeituresByAgeMapper>();

        await base.GenerateCsvContent(csvWriter, report, cancellationToken);

        await csvWriter.NextRecordAsync();
        csvWriter.WriteField("FORF TTL");
        csvWriter.WriteField("");
        csvWriter.WriteField(report.TotalAmount);

        await csvWriter.NextRecordAsync();

        // Write the headers
        csvWriter.WriteHeader<ForfeituresByAgeDetail>();
        await csvWriter.NextRecordAsync();


    }


    public class ProfitSharingForfeituresByAgeMapper : ClassMap<ForfeituresByAgeDetail>
    {
        public ProfitSharingForfeituresByAgeMapper()
        {
            Map(m => m.Age).Index(0).Name("AGE");
            Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            Map(m => m.Amount).Index(2).Name("AMOUNT");
        }
    }
}
