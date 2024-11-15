using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen.ReportByAgeEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;

public class ReportByAgeEndpoint : EndpointWithCsvBase<ProfitYearRequest, ProfitSharingDistributionsByAgeDetail, ProfitSharingDistributionsByAgeMapper>
{
    private readonly IFrozenReportService _frozenReportService;

    public ReportByAgeEndpoint(IFrozenReportService frozenReportService)
    {
        _frozenReportService = frozenReportService;
    }

    public override string ReportFileName => "PROFIT SHARING DISTRIBUTIONS BY AGE";

    public override void Configure()
    {
        Get("frozen/report-by-age");
        Summary(s =>
        {
            s.Summary = "PROFIT SHARING DISTRIBUTIONS BY AGE";
            s.Description =
                "This report produces a list of members showing their points, and any forfeitures over the year";

            s.ExampleRequest = new ProfitYearRequest
            {
                ProfitYear = 2023,
                Skip = 0,
                Take = 25
            };
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<ProfitSharingDistributionsByAge>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<ProfitSharingDistributionsByAge>
                        {
                            Results = new List<ProfitSharingDistributionsByAge> { ProfitSharingDistributionsByAge.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override async Task<ReportResponseBase<ProfitSharingDistributionsByAgeDetail>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return await _frozenReportService.GetDistributionsByAgeYear(req, ct);
    }

    public class ProfitSharingDistributionsByAgeMapper : ClassMap<ProfitSharingDistributionsByAgeDetail>
    {
        public ProfitSharingDistributionsByAgeMapper()
        {
#pragma warning disable S125
            //Map(m => m.Age).Index(0).Name("AGE");
#pragma warning restore S125
            //Map(m => m.EmployeeCount).Index(1).Name("EMPS");
            //Map(m => m.Amount).Index(2).Name("AMOUNT");
        }
    }
}
