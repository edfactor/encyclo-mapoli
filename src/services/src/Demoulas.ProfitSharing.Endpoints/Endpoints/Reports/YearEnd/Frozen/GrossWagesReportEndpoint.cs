using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
public sealed class GrossWagesReportEndpoint : EndpointWithCsvTotalsBase<GrossWagesReportRequest, GrossWagesReportResponse, GrossWagesReportDetail, GrossWagesReportEndpoint.GrossReportMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public override string ReportFileName => GrossWagesReportResponse.REPORT_NAME;

    public GrossWagesReportEndpoint(IFrozenReportService frozenReportService)
        : base(Navigation.Constants.ProfShareGrossRpt)
    {
        _frozenReportService = frozenReportService;
    }

    public override void Configure()
    {
        Get("frozen/grosswages");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of employees whose wages exceeded the input amount";
            s.ExampleRequest = GrossWagesReportRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200, GrossWagesReportResponse.ResponseExample()
                }
            };
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<GrossWagesReportResponse> GetResponse(GrossWagesReportRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetGrossWagesReport(req, ct);
    }

    public class GrossReportMapper : ClassMap<GrossWagesReportDetail>
    {
        public GrossReportMapper()
        {
            Map(m => m.BadgeNumber).Index(0).Name("BADGE #");
            Map(m => m.EmployeeName).Index(1).Name("EMPLOYEE NAME");
            Map(m => m.Ssn).Index(2).Name("SSN NUM");
            Map(m => m.DateOfBirth).Index(3).Name("D-O-B");
            Map(m => m.GrossWages).Index(4).Name("PS WAGES");
            Map(m => m.ProfitSharingAmount).Index(5).Name("PS AMOUNT");
            Map(m => m.Loans).Index(6).Name("LOANS");
            Map(m => m.Forfeitures).Index(7).Name("FORFEITURES");
            Map(m => m.EnrollmentId).Index(8).Name("ENROLLMENT ID");
        }
    }
}
