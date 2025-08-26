using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
public sealed  class UpdateSummaryReportEndpoint:EndpointWithCsvTotalsBase<FrozenProfitYearRequest, UpdateSummaryReportResponse, UpdateSummaryReportDetail, UpdateSummaryReportEndpoint.UpdateSummaryReportMapper>
{
    private readonly IFrozenReportService _frozenReportService;
    private readonly IAuditService _auditService;

    public UpdateSummaryReportEndpoint(IFrozenReportService frozenReportService, IAuditService auditService)
        : base(Navigation.Constants.ProfitShareReportFinalRun)
    {
        _frozenReportService = frozenReportService;
        _auditService = auditService;
    }

    public override string ReportFileName => "UPDATE SUMMARY FOR PROFIT SHARING";

    public override void Configure()
    {
        Get("frozen/updatesummary");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description = "This report produces a list of members showing last year's balance, compared to this years";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>()
            {
                {
                    200, UpdateSummaryReportResponse.ResponseExample()
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<UpdateSummaryReportResponse> GetResponse(FrozenProfitYearRequest req, CancellationToken ct)
    {
        return _auditService.ArchiveCompletedReportAsync(ReportFileName, req.ProfitYear, req,
            (audit, _, cancellationToken) => _frozenReportService.GetUpdateSummaryReport(audit, cancellationToken),
            ct);
    }


    public class UpdateSummaryReportMapper : ClassMap<UpdateSummaryReportDetail>
    {
        public UpdateSummaryReportMapper()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE#");
            Map(m => m.Name).Index(2).Name("NAME");
            Map(m => m.StoreNumber).Index(3).Name("STR");

            Map(m => m.Before.ProfitSharingAmount).Index(4).Name("BEFORE_P/S_AMOUNT");
            Map(m => m.Before.VestedProfitSharingAmount).Index(5).Name("BEFORE_P/S_VESTED");
            Map(m => m.Before.YearsInPlan).Index(6).Name("YEARS");
            Map(m => m.Before.EnrollmentId).Index(7).Name("ENROLL");

            Map(m => m.After.ProfitSharingAmount).Index(4).Name("AFTER_P/S_AMOUNT");
            Map(m => m.After.VestedProfitSharingAmount).Index(5).Name("AFTER_P/S_VESTED");
            Map(m => m.After.YearsInPlan).Index(6).Name("YEARS");
            Map(m => m.After.EnrollmentId).Index(7).Name("ENROLL");
        }
    }
}
