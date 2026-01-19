using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public class ProfitSharingUnder21TotalsEndpoint : ProfitSharingEndpoint<ProfitYearRequest, ProfitSharingUnder21TotalsResponse>
{
    private readonly IPostFrozenService _postFrozenService;
    private readonly ILogger<ProfitSharingUnder21TotalsEndpoint> _logger;

    public ProfitSharingUnder21TotalsEndpoint(IPostFrozenService postFrozenService, ILogger<ProfitSharingUnder21TotalsEndpoint> logger)
        : base(Navigation.Constants.QPAY066TAUnder21)
    {
        _postFrozenService = postFrozenService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("post-frozen/totals");
        Summary(s =>
        {
            s.Summary = "Totals lines for under 21 reports";
            s.Description = "Produces a series of totals related to participants under 21";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {200,  ProfitSharingUnder21TotalsResponse.ResponseExample()}
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    protected override async Task<ProfitSharingUnder21TotalsResponse> HandleRequestAsync(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var result = await _postFrozenService.GetUnder21Totals(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit_sharing_under_21_totals"),
                new("profit_year", req.ProfitYear.ToString()));

            _logger.LogInformation("Year-end post-frozen under-21 totals calculated for year {ProfitYear} (correlation: {CorrelationId})",
                req.ProfitYear, HttpContext.TraceIdentifier);

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
}
