using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class ProfitSharingUnder21InactiveNoBalanceEndpoint :
    EndpointWithCsvBase<
        ProfitYearRequest,
        ProfitSharingUnder21InactiveNoBalanceResponse,
        ProfitSharingUnder21InactiveNoBalanceEndpoint.ProfitSharingUnder21InaactiveNoBalanceClassMap>
{
    private readonly IPostFrozenService _postFrozenService;

    public ProfitSharingUnder21InactiveNoBalanceEndpoint(IPostFrozenService postFrozenService)
    {
        _postFrozenService = postFrozenService;
    }

    public override string ReportFileName => ProfitSharingUnder21InactiveNoBalanceResponse.REPORT_NAME;

    public override void Configure()
    {
        Get("post-frozen/under-21-inactive");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description = "Produces a list of inactive participants under 21 who have no balance";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {200,  ProfitSharingUnder21InactiveNoBalanceResponse.SampleResponse()}
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override Task<ReportResponseBase<ProfitSharingUnder21InactiveNoBalanceResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _postFrozenService.ProfitSharingUnder21InactiveNoBalance(req, ct);
    }

    public sealed class ProfitSharingUnder21InaactiveNoBalanceClassMap: ClassMap<ProfitSharingUnder21InactiveNoBalanceResponse>
    {
        public ProfitSharingUnder21InaactiveNoBalanceClassMap()
        {
            int idx = 0;
            Map(m => m.BadgeNumber).Index(idx++).Name("BADGE #");
            Map(m => m.LastName).Index(idx++).Name("LAST NAME");
            Map(m => m.FirstName).Index(idx++).Name("FIRST NAME");
            Map(m => m.BirthDate).Index(idx++).Name("BIRTHDTE");
            Map(m => m.HireDate).Index(idx++).Name("HIREDATE");
            Map(m => m.TerminationDate).Index(idx++).Name("TERMDATE");
            Map(m => m.Age).Index(idx++).Name("AGE");
            Map(m => m.EnrollmentId).Index(idx).Name("EC");
        }
    }
}
