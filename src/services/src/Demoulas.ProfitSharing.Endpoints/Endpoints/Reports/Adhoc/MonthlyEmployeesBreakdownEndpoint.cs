using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;

public sealed class MonthlyEmployeesBreakdownEndpoint : EndpointWithCsvBase<TerminatedEmployeesWithBalanceBreakdownRequest, MemberYearSummaryDto, MonthlyEmployeesBreakdownEndpoint.MonthlyEmployeesMap>
{
    private readonly IBreakdownService _breakdownService;
    private readonly ILogger<MonthlyEmployeesBreakdownEndpoint> _logger;

    public MonthlyEmployeesBreakdownEndpoint(IBreakdownService breakdownService, ILogger<MonthlyEmployeesBreakdownEndpoint> logger)
        : base(Navigation.Constants.QPAY066AdHocReports)
    {
        _breakdownService = breakdownService;
        _logger = logger;
    }

    public override string ReportFileName => "Breakdown by Store - QPAY066M - Monthly Employees with Activity";

    public override void Configure()
    {
        Get("/breakdown-by-store/monthly");
        Summary(s =>
        {
            s.Summary = "QPAY066M: Breakdown all monthly employees with distribution, forfeit, or contribution for profit year";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<AdhocReportsGroup>();
        base.Configure();
    }

    public override async Task<ReportResponseBase<MemberYearSummaryDto>> GetResponse(TerminatedEmployeesWithBalanceBreakdownRequest breakdownByStoreRequest, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);

        try
        {
            this.RecordRequestMetrics(HttpContext, _logger, breakdownByStoreRequest);

            var result = await _breakdownService.GetMonthlyEmployeesWithActivity(breakdownByStoreRequest, ct);

            // Record monthly employees breakdown metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "monthly-employees-breakdown"),
                new("endpoint", "MonthlyEmployeesBreakdownEndpoint"),
                new("report_type", "breakdown"),
                new("pay_frequency", "monthly"),
                new("activity_filter", "distribution-forfeit-contribution"));

            var resultCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(resultCount,
                new("record_type", "monthly-employees"),
                new("endpoint", "MonthlyEmployeesBreakdownEndpoint"));

            _logger.LogInformation("Monthly employees breakdown report generated, returned {Count} employees (correlation: {CorrelationId})",
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

    public sealed class MonthlyEmployeesMap : ClassMap<MemberYearSummaryDto>
    {
        public MonthlyEmployeesMap()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BadgeNumber");
            Map(m => m.FullName).Index(1).Name("Name");
            Map(m => m.BeginningBalance).Index(2).Name("BeginningBalance");
            Map(m => m.BeneficiaryAllocation).Index(3).Name("BeneficiaryAllocation");
            Map(m => m.Distributions).Index(4).Name("Distributions");
            Map(m => m.Forfeitures).Index(5).Name("Forfeit");
            Map(m => m.EndingBalance).Index(6).Name("EndingBalance");
            Map(m => m.VestedAmount).Index(7).Name("VestedBalance");
            Map(m => m.TerminationDate).Index(8).Name("DateTerminated");
            Map(m => m.ProfitShareHours).Index(9).Name("YTDProfitSharingHours");
            Map(m => m.VestedPercent).Index(10).Name("VestedPercentage");
            Map(m => m.Age).Index(11).Name("Age");
            Map(m => m.EnrollmentId).Index(12).Name("EnrollmentCode");
        }
    }
}
