using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

public sealed class RetiredEmployeesWithBalanceActivityEndpoint : EndpointWithCsvBase<TerminatedEmployeesWithBalanceBreakdownRequest, MemberYearSummaryDto, RetiredEmployeesWithBalanceActivityEndpoint.BreakdownEndpointMap>
{
    private readonly IBreakdownService _breakdownService;
    private readonly ILogger<RetiredEmployeesWithBalanceActivityEndpoint> _logger;

    public RetiredEmployeesWithBalanceActivityEndpoint(IBreakdownService breakdownService, ILogger<RetiredEmployeesWithBalanceActivityEndpoint> logger)
        : base(Navigation.Constants.QPAY066AdHocReports)
    {
        _breakdownService = breakdownService;
        _logger = logger;
    }

    public override string ReportFileName => "Breakdown by Store - QPAY066i - Retired employees with balance activity over the year";

    public override void Configure()
    {
        Get("/breakdown-by-store/retired/withbalanceactivity");
        Summary(s =>
        {
            s.Summary = "Breakdown retired managers and associates for all stores who have had balance activity";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<ReportResponseBase<MemberYearSummaryDto>> GetResponse(TerminatedEmployeesWithBalanceBreakdownRequest breakdownByStoreRequest, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, breakdownByStoreRequest);

            var result = await _breakdownService.GetRetiredEmployessWithBalanceActivity(breakdownByStoreRequest, ct);

            // Record retired employees with balance activity metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "retired-employees-balance-activity"),
                new("endpoint", "RetiredEmployeesWithBalanceActivityEndpoint"),
                new("report_type", "breakdown"),
                new("employee_status", "retired"),
                new("activity_filter", "balance-activity"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "retired-employees-active"),
                new("endpoint", "RetiredEmployeesWithBalanceActivityEndpoint"));

            _logger.LogInformation("Retired employees with balance activity report generated, returned {Count} employees (correlation: {CorrelationId})",
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
