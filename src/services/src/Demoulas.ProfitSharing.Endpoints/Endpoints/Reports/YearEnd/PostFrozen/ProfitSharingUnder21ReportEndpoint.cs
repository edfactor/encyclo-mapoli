using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Demoulas.ProfitSharing.Common.Telemetry;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public class ProfitSharingUnder21ReportEndpoint : EndpointWithCsvTotalsBase<ProfitYearRequest,
    ProfitSharingUnder21ReportResponse,
    ProfitSharingUnder21ReportDetail,
    ProfitSharingUnder21ReportEndpoint.ProfitSharingUnder21ReportClassMap>
{
    private readonly IPostFrozenService _postFrozenService;
    private readonly ILogger<ProfitSharingUnder21ReportEndpoint> _logger;

    public ProfitSharingUnder21ReportEndpoint(IPostFrozenService postFrozenService, ILogger<ProfitSharingUnder21ReportEndpoint> logger)
        : base(Navigation.Constants.QPAY066Under21)
    {
        _postFrozenService = postFrozenService;
        _logger = logger;
    }
    public override string ReportFileName => ProfitSharingUnder21ReportResponse.REPORT_NAME;

    public override void Configure()
    {
        Get("post-frozen-under-21");
        Summary(s =>
        {
            s.Summary = "Under 21 report";
            s.Description =
                "Produces a report of participants under 21 who are in the profit sharing system.  With years and/or vested amount greater than zero.";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, ProfitSharingUnder21ReportResponse.ResponseExample()
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }

    public override async Task<ProfitSharingUnder21ReportResponse> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        using var activity = this.StartEndpointActivity(HttpContext);
        this.RecordRequestMetrics(HttpContext, _logger, req);

        try
        {
            var result = await _postFrozenService.ProfitSharingUnder21Report(req, ct);

            // Record business operation metrics
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "profit_sharing_under_21_report"),
                new("profit_year", req.ProfitYear.ToString()));

            var recordCount = result?.Response?.Results?.Count() ?? 0;
            EndpointTelemetry.RecordCountsProcessed.Record(recordCount,
                new("record_type", "post-frozen-under-21-participants"),
                new("endpoint", "ProfitSharingUnder21ReportEndpoint"));

            _logger.LogInformation("Year-end post-frozen under-21 report generated: {Count} participants under 21 with PS benefits (correlation: {CorrelationId})",
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

    public class ProfitSharingUnder21ReportClassMap : ClassMap<ProfitSharingUnder21ReportDetail>
    {
        public ProfitSharingUnder21ReportClassMap()
        {
            Map(m => m.StoreNumber).Index(0).Name("STR");
            Map(m => m.BadgeNumber).Index(1).Name("BADGE #");
            Map(m => m.FirstName).Index(2).Name("EMPLOYEE FIRST");
            Map(m => m.LastName).Index(3).Name("EMPLOYEE LAST");
            Map(m => m.Ssn).Index(4).Name("SOC.SEC.#");
            Map(m => m.ProfitSharingYears).Index(5).Name("PS YRS");
            Map(m => m.IsNew).Index(6).Name("NE");
            Map(m => m.ThisYearHours).Index(7).Name("THIS YR P-HOURS");
            Map(m => m.LastYearHours).Index(8).Name("LAST YR P-HOURS");
            Map(m => m.HireDate).Index(9).Name("HIRE DATE");
            Map(m => m.FullTimeDate).Index(10).Name("FULL DATE");
            Map(m => m.TerminationDate).Index(11).Name("TERM DATE");
            Map(m => m.DateOfBirth).Index(12).Name("BIRTH DATE");
            Map(m => m.Age).Index(13).Name("AGE");
            Map(m => m.EmploymentStatusId).Index(14).Name("SC");
            Map(m => m.CurrentBalance).Index(15).Name("PS-AMT");
            Map(m => m.EnrollmentId).Index(16).Name("EC");
        }
    }
}
