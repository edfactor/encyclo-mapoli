using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages.PreviousYearWagesEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages;
public class PreviousYearWagesEndpoint : EndpointWithCsvBase<PaginationRequestDto, WagesPreviousYearResponse, WagesPreviousYearResponseMap>
{
    private readonly IWagesService _reportService;

    public PreviousYearWagesEndpoint(IWagesService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("wages-previous-year");
        Summary(s =>
        {
            s.Summary = "Wages for the current year";
            s.Description =
                "Provides a report on employees' wages for the previous year. This report helps identify potential issues that need to be addressed before running profit sharing. The endpoint can be executed multiple times.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<WagesPreviousYearResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<WagesPreviousYearResponse>
                        {
                            Results = new List<WagesPreviousYearResponse> { WagesPreviousYearResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "EJR PROF-DOLLAR-EXTRACT YEAR=LAST";

    public override async Task<ReportResponseBase<WagesPreviousYearResponse>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _reportService.GetWagesPreviousYearReport(req, ct);
    }



    public sealed class WagesPreviousYearResponseMap : ClassMap<WagesPreviousYearResponse>
    {
        public WagesPreviousYearResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.HoursLastYear).Index(3).Name("HOURS LASTYR");
            Map(m => m.IncomeLastYear).Index(4).Name("DOLLARS LASTYR");
        }
    }
}
