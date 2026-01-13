using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class ProfitSharingUnder21BreakdownByStoreEndpoint :
    EndpointWithCsvBase<
        ProfitYearRequest,
        ProfitSharingUnder21BreakdownByStoreResponse,
        ProfitSharingUnder21BreakdownByStoreEndpoint.ProfitSharingUnder21BreakdownByStoreClassMap>
{
    private readonly IPostFrozenService _postFrozenService;
    private readonly ILogger<ProfitSharingUnder21BreakdownByStoreEndpoint> _logger;

    public ProfitSharingUnder21BreakdownByStoreEndpoint(IPostFrozenService postFrozenService, ILogger<ProfitSharingUnder21BreakdownByStoreEndpoint> logger)
        : base(Navigation.Constants.QPAY066TA)
    {
        _postFrozenService = postFrozenService;
        _logger = logger;
    }

    public override string ReportFileName => ProfitSharingUnder21BreakdownByStoreResponse.REPORT_NAME;

    public override void Configure()
    {
        Get("post-frozen/under-21-breakdown-by-store");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description = "Produces a list of active participants under 21, sorted by store number";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {200,  ProfitSharingUnder21BreakdownByStoreResponse.ResponseExample()}
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task<ReportResponseBase<ProfitSharingUnder21BreakdownByStoreResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var result = await _postFrozenService.ProfitSharingUnder21BreakdownByStore(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit_sharing_under_21_breakdown_by_store"),
                new("profit_year", req.ProfitYear.ToString()));

            var recordCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new("record_type", "post-frozen-under-21-breakdown-by-store"),
                new("endpoint", "ProfitSharingUnder21BreakdownByStoreEndpoint"));

            _logger.LogInformation("Year-end post-frozen under-21 breakdown by store generated: {Count} store records (correlation: {CorrelationId})",
                recordCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
            }

            return result!;
        }
        catch (Exception ex)
        {
            this.RecordException(HttpContext, _logger, ex, activity);
            throw;
        }
    }

    public class ProfitSharingUnder21BreakdownByStoreClassMap : ClassMap<ProfitSharingUnder21BreakdownByStoreResponse>
    {
        public ProfitSharingUnder21BreakdownByStoreClassMap()
        {
            int idx = 0;
            Map(m => m.StoreNumber).Index(idx++).Name("STORE");
            Map(m => m.BadgeNumber).Index(idx++).Name("BADGE #");
            Map(m => m.FullName).Index(idx++).Name("EMPLOYEE NAME");
            Map(m => m.BeginningBalance).Index(idx++).Name("BEGINNING BALANCE");
            Map(m => m.Earnings).Index(idx++).Name("EARNINGS");
            Map(m => m.Contributions).Index(idx++).Name("CONTR.");
            Map(m => m.Forfeitures).Index(idx++).Name("FORFEIT");
            Map(m => m.Distributions).Index(idx++).Name("DISTR.");
            Map(m => m.EndingBalance).Index(idx++).Name("ENDING BALANCE");
            Map(m => m.VestedAmount).Index(idx++).Name("VESTED AMOUNT");
            Map(m => m.VestingPercentage).Index(idx++).Name("%");
            Map(m => m.Age).Index(idx++).Name("AGE");
            Map(m => m.EnrollmentId).Index(idx).Name("EC");
        }
    }
}
