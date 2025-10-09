using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownEndpoint : EndpointWithCsvBase<BreakdownByStoreRequest, MemberYearSummaryDto, BreakdownEndpoint.BreakdownEndpointMap>
{
    private readonly IBreakdownService _breakdownService;
    private readonly ILogger<BreakdownEndpoint> _logger;

    public BreakdownEndpoint(IBreakdownService breakdownService, ILogger<BreakdownEndpoint> logger)
        : base(Navigation.Constants.QPAY066TA)
    {
        _breakdownService = breakdownService;
        _logger = logger;
    }

    public override string ReportFileName => "Breakdown by Store - QPAY066TA";

    public override void Configure()
    {
        Get("/breakdown-by-store");
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates for all stores";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<ReportResponseBase<MemberYearSummaryDto>> GetResponse(BreakdownByStoreRequest breakdownByStoreRequest, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, breakdownByStoreRequest);

            var result = await _breakdownService.GetActiveMembersByStore(breakdownByStoreRequest, ct);

            // Record year-end breakdown report metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "year-end-breakdown-by-store"),
                new("endpoint", "BreakdownEndpoint"),
                new("report_type", "breakdown"),
                new("member_status", "active"),
                new("report_code", "QPAY066TA"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "active-members-year-end"),
                new("endpoint", "BreakdownEndpoint"));

            _logger.LogInformation("Year-end breakdown by store report generated, returned {Count} active members (correlation: {CorrelationId})",
                resultCount, HttpContext.TraceIdentifier);

            if (result != null)
            {
                this.RecordResponseMetrics(HttpContext, _logger, result);
                return result;
            }

            var emptyResult = new ReportResponseBase<MemberYearSummaryDto>
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

    public sealed class BreakdownEndpointMap : ClassMap<MemberYearSummaryDto>
    {
        public BreakdownEndpointMap()
        {
            Map(m => m.StoreNumber).Index(0).Name("StoreNumber");
            Map(m => m.BadgeNumber).Index(2).Name("BadgeNumber");
            Map(m => m.FullName).Index(4).Name("FullName");
            Map(m => m.PayClassificationId).Index(7).Name("PayClassificationId");
            Map(m => m.BeginningBalance).Index(8).Name("BeginningBalance");
            Map(m => m.Earnings).Index(9).Name("Earnings");
            Map(m => m.Contributions).Index(10).Name("Contributions");
            Map(m => m.Distributions).Index(12).Name("Distributions");
            Map(m => m.EndingBalance).Index(13).Name("EndingBalance");
            Map(m => m.VestedAmount).Index(14).Name("VestedAmount");
            Map(m => m.VestedPercent).Index(15).Name("VestedPercentage");
            Map(m => m.PayClassificationName).Index(16).Name("PayClassificationName");
        }
    }
}
