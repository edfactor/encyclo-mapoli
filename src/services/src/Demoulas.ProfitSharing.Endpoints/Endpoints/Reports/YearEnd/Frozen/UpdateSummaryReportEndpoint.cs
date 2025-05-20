using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
public sealed  class UpdateSummaryReportEndpoint:EndpointWithCsvTotalsBase<FrozenProfitYearRequest, UpdateSummaryReportResponse, UpdateSummaryReportDetail, UpdateSummaryReportEndpoint.UpdateSummaryReportMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public UpdateSummaryReportEndpoint(IFrozenReportService frozenReportService)
    {
        _frozenReportService = frozenReportService;
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
        return _frozenReportService.GetUpdateSummaryReport(req, ct);
    }


    public class UpdateSummaryReportMapper : ClassMap<UpdateSummaryReportDetail>
    {
        public UpdateSummaryReportMapper()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE#");
            Map(m => m.Name).Index(1).Name("NAME");
            Map(m => m.StoreNumber).Index(2).Name("STR");
            Map(m => m.Before.ProfitSharingAmount).Index(3).Name("BEFORE_P/S_AMOUNT");
            Map(m => m.Before.VestedProfitSharingAmount).Index(4).Name("BEFORE_P/S_VESTED");
            Map(m => m.Before.YearsInPlan).Index(5).Name("YEARS");
            Map(m => m.Before.EnrollmentId).Index(6).Name("ENROLL");
            Map(m => m.After.ProfitSharingAmount).Index(3).Name("BEFORE_P/S_AMOUNT");
            Map(m => m.After.ProfitSharingAmount).Index(3).Name("BEFORE_P/S_AMOUNT");
            Map(m => m.After.YearsInPlan).Index(5).Name("YEARS");
            Map(m => m.After.EnrollmentId).Index(6).Name("ENROLL");
        }
    }
}
