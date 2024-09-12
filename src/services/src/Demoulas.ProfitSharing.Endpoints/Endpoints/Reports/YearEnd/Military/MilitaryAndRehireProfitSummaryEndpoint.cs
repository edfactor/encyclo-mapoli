using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;

public class MilitaryAndRehireProfitSummaryEndpoint :
    EndpointWithCsvBase<MilitaryAndRehireRequest, MilitaryAndRehireProfitSummaryResponse, MilitaryAndRehireProfitSummaryEndpoint.MilitaryRehireProfitSharingResponseMap>
{
    private readonly IMilitaryAndRehireService _reportService;

    public MilitaryAndRehireProfitSummaryEndpoint(IMilitaryAndRehireService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("military-and-rehire-profit-summary/{reportingYear}");
        Summary(s =>
        {
            s.Summary = "Military and Rehire Profit Sharing Data Report Endpoint";
            s.Description =
                "The Military and Rehire Profit Sharing Data endpoint produces a comprehensive report on employees who are either currently on military leave or have been rehired, focusing on their eligibility for forfeiture adjustments in profit-sharing. The report contains employee information, such as badge number, rehire date, and current year-to-date hours, along with profit-sharing records, including profit year, forfeiture amounts, and comments. This report supports multiple executions and is primarily used to address forfeiture discrepancies before profit sharing is finalized.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<MilitaryAndRehireProfitSummaryResponse>
                        {
                            Results = new List<MilitaryAndRehireProfitSummaryResponse> { MilitaryAndRehireProfitSummaryResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "MILITARY TERM-REHIRE";

    public override async Task<ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>> GetResponse(MilitaryAndRehireRequest req, CancellationToken ct)
    {
        return await _reportService.GetMilitaryAndRehireProfitSummaryReport(req, ct);
    }

    public sealed class MilitaryRehireProfitSharingResponseMap : ClassMap<MilitaryAndRehireProfitSummaryResponse>
    {
        public MilitaryRehireProfitSharingResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.Ssn).Index(3).Name("SSN");
            Map(m => m.FullName).Index(4).Name("EMPLOYEE NAME");
            Map(m => m.ReHiredDate).Index(5).Name("REHIRED").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.CompanyContributionYears).Index(6).Name("PY-YRS");
            Map(m => m.HoursCurrentYear).Index(7).Name("YTD HOURS").TypeConverterOption.Format("0.00");
            Map().Index(8).Name("EC"); // Assuming EC is blank, leave an empty column
        }
    }
}
    
