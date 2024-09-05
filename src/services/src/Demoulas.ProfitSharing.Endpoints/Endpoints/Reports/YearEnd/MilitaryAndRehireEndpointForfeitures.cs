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
        }
    }
}
