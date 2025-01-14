using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
public class ForfeituresAndPointsForYearEndpoint:EndpointWithCsvBase<ProfitYearRequest, ForfeituresAndPointsForYearResponse, ForfeituresAndPointsForYearEndpoint.ForfeituresAndPointsForYearEndpointMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public ForfeituresAndPointsForYearEndpoint(IFrozenReportService frozenReportService)
    {
        _frozenReportService = frozenReportService;
    }
    public override string ReportFileName => "Forfeitures and Points for Year";

    public override void Configure()
    {
        Get("frozen/forfeitures-and-points");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description =
                "This report produces a list of members showing their points, and any forfeitures over the year";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<ForfeituresAndPointsForYearResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<ForfeituresAndPointsForYearResponse>
                        {
                            Results = new List<ForfeituresAndPointsForYearResponse> { ForfeituresAndPointsForYearResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<ReportResponseBase<ForfeituresAndPointsForYearResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _frozenReportService.GetForfeituresAndPointsForYearAsync(req, ct);
    }

    public class ForfeituresAndPointsForYearEndpointMapper:ClassMap<ForfeituresAndPointsForYearResponse>
    {
        public ForfeituresAndPointsForYearEndpointMapper()
        {
            Map(m => m.BadgeNumber).Index(0).Name("EMPLOYEE_BADGE");
            Map(m => m.EmployeeName).Index(1).Name("EMPLOYEE_NAME");
            Map(m => m.EmployeeSsn).Index(2).Name("EMPLOYEE_SSN");
            Map(m => m.Forfeitures).Index(3).Name("FORFEITURES");
            Map(m => m.ForfeitPoints).Index(4).Name("FORFEIT_POINTS");
            Map(m => m.EarningPoints).Index(5).Name("EARNING_POINTS");
            Map(m => m.BeneficiaryPsn).Index(6).Name("BENEFICIARY_SSN");
        }
    }
}
