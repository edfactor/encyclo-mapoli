using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class ProfitShareEditEndpoint
    : EndpointWithCsvTotalsBase<ProfitShareUpdateRequest,
        ProfitShareEditResponse,
        ProfitShareEditMemberRecordResponse,
        ProfitShareEditEndpoint.ProfitShareEditClassMap>
{
    private readonly IProfitShareEditService _editService;
    private readonly ILogger<ProfitShareEditEndpoint> _logger;

    public ProfitShareEditEndpoint(IProfitShareEditService editService, ILogger<ProfitShareEditEndpoint> logger)
        : base(Navigation.Constants.ProfitShareReportEditRun)
    {
        _editService = editService;
        _logger = logger;
    }

    public override string ReportFileName => "profit-share-edit-report";

    public override void Configure()
    {
        Get("profit-share-edit");
        Summary(s =>
        {
            s.Summary = "profit share edit";
            s.Description =
                "Returns per member transactions based on user specified contribution/incoming forfeit/earnings parameters";
            s.ExampleRequest = ProfitShareUpdateRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }

    public override async Task<ProfitShareEditResponse> GetResponse(ProfitShareUpdateRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, req);

            var result = await _editService.ProfitShareEdit(req, ct);

            // Record year-end profit share edit metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-profit-share-edit"),
                new("endpoint", "ProfitShareEditEndpoint"),
                new("report_type", "profit-share-edit"),
                new("edit_type", "member-transactions"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "profit-share-edit-members"),
                new("endpoint", "ProfitShareEditEndpoint"));

            _logger.LogInformation("Year-end profit share edit report generated, returned {Count} member records (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ProfitShareEditResponse
            {
                ReportName = ReportFileName,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today),
                Response = new() { Results = [] },
                BeginningBalanceTotal = 0,
                ContributionGrandTotal = 0,
                IncomingForfeitureGrandTotal = 0,
                EarningsGrandTotal = 0
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

    public sealed class ProfitShareEditClassMap : ClassMap<ProfitShareEditMemberRecordResponse>
    {
        internal ProfitShareEditClassMap()
        {
            int dex = 0;
            Map(m => m.Psn).Index(dex++).Name("Number");
            Map(m => m.Name).Index(dex++).Name("Name");
            Map(m => m.Code).Index(dex++).Name("Code");
            Map(m => m.ContributionAmount).Index(dex++).Name("Contribution Amount");
            Map(m => m.EarningsAmount).Index(dex++).Name("Earnings Amount");
            Map(m => m.ForfeitureAmount).Index(dex++).Name("Incoming Forfeitures");
            Map(m => m.Remark).Index(dex).Name("Reason");
        }
    }
}
