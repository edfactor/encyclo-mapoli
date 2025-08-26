using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;
using static Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages.CurrentYearWagesEndpoint;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages;
public class CurrentYearWagesEndpoint : EndpointWithCsvBase<ProfitYearRequest, WagesCurrentYearResponse, WagesCurrentYearResponseMap>
{
    private readonly IWagesService _reportService;

    public CurrentYearWagesEndpoint(IWagesService reportService) : base(Navigation.Constants.YTDWagesExtract)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("wages-current-year");  
        Summary(s =>
        {
            s.Summary = "Wages for the specified year";
            s.Description =
                "Provides a report on employees' wages for the specified year. This report helps identify potential issues that need to be addressed before running profit sharing. The endpoint can be executed multiple times.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<WagesCurrentYearResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        Response = new PaginatedResponseDto<WagesCurrentYearResponse>
                        {
                            Results = new List<WagesCurrentYearResponse> { WagesCurrentYearResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "YTD Wages Extract (PROF-DOLLAR-EXTRACT)";

    public override Task<ReportResponseBase<WagesCurrentYearResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _reportService.GetWagesReportAsync(req, ct);
    }


    public sealed class WagesCurrentYearResponseMap : ClassMap<WagesCurrentYearResponse>
    {
        public WagesCurrentYearResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.HoursCurrentYear).Index(3).Name("HOURS YR");
            Map(m => m.IncomeCurrentYear).Index(4).Name("DOLLARS YR");
        }
    }
}
