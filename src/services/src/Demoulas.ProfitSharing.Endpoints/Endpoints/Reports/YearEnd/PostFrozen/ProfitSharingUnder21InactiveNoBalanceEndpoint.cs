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

public sealed class ProfitSharingUnder21InactiveNoBalanceEndpoint :
    EndpointWithCsvBase<
        ProfitYearRequest,
        ProfitSharingUnder21InactiveNoBalanceResponse,
        ProfitSharingUnder21InactiveNoBalanceEndpoint.ProfitSharingUnder21InaactiveNoBalanceClassMap>
{
    private readonly IPostFrozenService _postFrozenService;
    private readonly ILogger<ProfitSharingUnder21InactiveNoBalanceEndpoint> _logger;

    public ProfitSharingUnder21InactiveNoBalanceEndpoint(IPostFrozenService postFrozenService, ILogger<ProfitSharingUnder21InactiveNoBalanceEndpoint> logger)
        : base(Navigation.Constants.QPAY066Under21)
    {
        _postFrozenService = postFrozenService;
        _logger = logger;
    }

    public override string ReportFileName => ProfitSharingUnder21InactiveNoBalanceResponse.REPORT_NAME;

    public override void Configure()
    {
        Get("post-frozen/under-21-inactive");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description = "Produces a list of inactive participants under 21 who have no balance";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {200,  ProfitSharingUnder21InactiveNoBalanceResponse.SampleResponse()}
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override async Task<ReportResponseBase<ProfitSharingUnder21InactiveNoBalanceResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var result = await _postFrozenService.ProfitSharingUnder21InactiveNoBalance(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit_sharing_under_21_inactive_no_balance"),
                new("profit_year", req.ProfitYear.ToString()));

            var recordCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new("record_type", "post-frozen-under-21-inactive-no-balance"),
                new("endpoint", "ProfitSharingUnder21InactiveNoBalanceEndpoint"));

            _logger.LogInformation("Year-end post-frozen under-21 inactive no-balance report generated: {Count} inactive participants (correlation: {CorrelationId})",
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

    public sealed class ProfitSharingUnder21InaactiveNoBalanceClassMap : ClassMap<ProfitSharingUnder21InactiveNoBalanceResponse>
    {
        public ProfitSharingUnder21InaactiveNoBalanceClassMap()
        {
            int idx = 0;
            Map(m => m.BadgeNumber).Index(idx++).Name("BADGE #");
            Map(m => m.LastName).Index(idx++).Name("LAST NAME");
            Map(m => m.FirstName).Index(idx++).Name("FIRST NAME");
            Map(m => m.BirthDate).Index(idx++).Name("BIRTHDTE");
            Map(m => m.HireDate).Index(idx++).Name("HIREDATE");
            Map(m => m.TerminationDate).Index(idx++).Name("TERMDATE");
            Map(m => m.Age).Index(idx++).Name("AGE");
            Map(m => m.EnrollmentId).Index(idx).Name("EC");
        }
    }
}
