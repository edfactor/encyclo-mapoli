using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;

public class ProfitShareUpdateEndpoint
    : EndpointWithCsvTotalsBase<ProfitSharingUpdateRequest,
        ProfitShareUpdateResponse,
        MemberFinancialsResponse,
        ProfitShareUpdateEndpoint.ProfitShareUpdateClassMap>
{
    private readonly IProfitShareUpdateService _profitShareUpdateService;

    public ProfitShareUpdateEndpoint(IProfitShareUpdateService profitShareUpdateService)
    {
        _profitShareUpdateService = profitShareUpdateService;
    }

    public override string ReportFileName => "profit-sharing-update-report";

    public override void Configure()
    {
        Get("profit-sharing-update");
        Summary(s =>
        {
            s.Summary = "profit sharing update";
            s.Description =
                "Updates plan members based on points provided.";
            s.ExampleRequest = ProfitSharingUpdateRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }

    public override Task<ProfitShareUpdateResponse> GetResponse(ProfitSharingUpdateRequest req,
        CancellationToken ct)
    {
        return _profitShareUpdateService.ProfitSharingUpdate(req, ct);
    }

    public class ProfitShareUpdateClassMap : ClassMap<MemberFinancialsResponse>
    {
        public ProfitShareUpdateClassMap()
        {
            Map(m => m.Psn).Index(0).Name("Number");
            Map(m => m.Name).Index(1).Name("Name");
            Map(m => m.CurrentAmount).Index(2).Name("Beginning Balance");
            Map(m => m.Contributions).Index(3).Name("Contributions");
            Map(m => m.Earnings).Index(4).Name("Earning");
            Map(m => m.SecondaryEarnings).Index(5).Name("Earning2");
            Map(m => m.IncomingForfeitures).Index(6).Name("Forfeits");
            Map(m => m.Distributions).Index(7).Name("Distributions");
            Map(m => m.Military).Index(8).Name("Military / Paid Alloc");
            Map(m => m.EndingBalance).Index(9).Name("Ending Balance");
        }
    }
}
