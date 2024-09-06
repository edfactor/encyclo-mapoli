using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Endpoints.TypeConverters;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class MilitaryAndRehireEndpointForfeitures :
    EndpointWithCsvBase<PaginationRequestDto, MilitaryRehireProfitSharingResponse, MilitaryAndRehireEndpointForfeitures.MilitaryRehireProfitSharingResponseMap>
{
    private readonly IMilitaryAndRehireService _reportService;

    public MilitaryAndRehireEndpointForfeitures(IMilitaryAndRehireService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("military-and-rehire-forfeitures");
        Summary(s =>
        {
            s.Summary = "Military and Rehire Report Endpoint";
            s.Description =
                "Provides a report on employees who are on military leave or have been rehired. This report helps identify potential issues that need to be addressed before running profit sharing. The endpoint can be executed multiple times.";

            s.ExampleRequest = SimpleExampleRequest;
            s.ResponseExamples = new Dictionary<int, object>
            {
                {
                    200,
                    new ReportResponseBase<MilitaryAndRehireReportResponse>
                    {
                        ReportName = ReportFileName,
                        ReportDate = DateTimeOffset.Now,
                        Response = new PaginatedResponseDto<MilitaryAndRehireReportResponse>
                        {
                            Results = new List<MilitaryAndRehireReportResponse> { MilitaryAndRehireReportResponse.ResponseExample() }
                        }
                    }
                }
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override string ReportFileName => "EMPLOYEES ON MILITARY LEAVE";

    public override async Task<ReportResponseBase<MilitaryRehireProfitSharingResponse>> GetResponse(PaginationRequestDto req, CancellationToken ct)
    {
        return await _reportService.FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYears(req, ct);
    }

    public sealed class MilitaryRehireProfitSharingResponseMap : ClassMap<MilitaryRehireProfitSharingResponse>
    {
        public MilitaryRehireProfitSharingResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.BadgeNumber).Index(2).Name("BADGE");
            Map(m => m.FullName).Index(3).Name("EMPLOYEE NAME");
            Map(m => m.Ssn).Index(4).Name("SSN");
            Map(m => m.ReHiredDate).Index(5).Name("REHIRED").TypeConverter<YearMonthDayTypeConverter>();
            Map(m => m.CompanyContributionYears).Index(6).Name("PY-YRS");
            Map(m => m.HoursCurrentYear).Index(7).Name("YTD HOURS").TypeConverterOption.Format("0.00");
            Map().Index(8).Name("EC"); // Assuming EC is blank, leave an empty column
            
            Map(m => m.Details)
        }
    }

    public sealed class DetailsMap : ClassMap<MilitaryRehireProfitSharingDetailResponse>
    {
        public DetailsMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map().Index(2).Convert(_ => string.Empty);
            Map().Index(3).Convert(_ => string.Empty);
            Map().Index(4).Convert(_ => string.Empty);
            Map().Index(5).Convert(_ => string.Empty);
            Map().Index(6).Convert(_ => string.Empty);

            Map(m => m.ProfitYear).Index(7).Name("YEAR");
            Map(m => m.Forfeiture).Index(8).Name("FORFEITURES").TypeConverterOption.Format("0.00");
            Map(m => m.Remark).Index(9).Name("COMMENT");
        }
    }
}
    
