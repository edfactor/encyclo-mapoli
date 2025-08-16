using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.PostFrozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.PostFrozen;

public sealed class ProfitSharingUnder21BreakdownByStoreEndpoint:
    EndpointWithCsvBase<
        ProfitYearRequest, 
        ProfitSharingUnder21BreakdownByStoreResponse, 
        ProfitSharingUnder21BreakdownByStoreEndpoint.ProfitSharingUnder21BreakdownByStoreClassMap>
{
    private readonly IPostFrozenService _postFrozenService;

    public ProfitSharingUnder21BreakdownByStoreEndpoint(IPostFrozenService postFrozenService)
    {
        _postFrozenService = postFrozenService;
    }

    public override string ReportFileName => ProfitSharingUnder21BreakdownByStoreResponse.REPORT_NAME;

    public override void Configure()
    {
        Get("post-frozen/under-21-breakdown-by-store");
        Summary(s =>
        {
            s.Summary = ReportFileName;
            s.Description = "Produces a list of active participants under 21, sorted by store number";
            s.ExampleRequest = ProfitYearRequest.RequestExample();
            s.ResponseExamples = new Dictionary<int, object>
            {
                {200,  ProfitSharingUnder21BreakdownByStoreResponse.ResponseExample()}
            };
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
    }

    public override Task<ReportResponseBase<ProfitSharingUnder21BreakdownByStoreResponse>> GetResponse(ProfitYearRequest req, CancellationToken ct)
    {
        return _postFrozenService.ProfitSharingUnder21BreakdownByStore(req, ct);
    }

    public class ProfitSharingUnder21BreakdownByStoreClassMap: ClassMap<ProfitSharingUnder21BreakdownByStoreResponse>
    {
        public ProfitSharingUnder21BreakdownByStoreClassMap()
        {
            int idx = 0;
            Map(m => m.StoreNumber).Index(idx++).Name("STORE");
            Map(m => m.BadgeNumber).Index(idx++).Name("BADGE #");
            Map(m => m.FullName).Index(idx++).Name("EMPLOYEE NAME");
            Map(m => m.BeginningBalance).Index(idx++).Name("BEGINNING BALANCE");
            Map(m => m.Earnings).Index(idx++).Name("EARNINGS");
            Map(m => m.Contributions).Index(idx++).Name("CONTR.");
            Map(m => m.Forfeitures).Index(idx++).Name("FORFEIT");
            Map(m => m.Distributions).Index(idx++).Name("DISTR.");
            Map(m => m.EndingBalance).Index(idx++).Name("ENDING BALANCE");
            Map(m => m.VestedAmount).Index(idx++).Name("VESTED AMOUNT");
            Map(m => m.VestingPercentage).Index(idx++).Name("%");
            Map(m => m.Age).Index(idx++).Name("AGE");
            Map(m => m.EnrollmentId).Index(idx).Name("EC");
        }
    }
}