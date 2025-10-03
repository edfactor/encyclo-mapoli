using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ProfitShareUpdate;

public class ProfitShareUpdateEndpoint
    : EndpointWithCsvTotalsBase<ProfitShareUpdateRequest,
        ProfitShareUpdateResponse,
        ProfitShareUpdateMemberResponse,
        ProfitShareUpdateEndpoint.ProfitShareUpdateClassMap>
{
    private readonly IProfitShareUpdateService _profitShareUpdateService;

    public ProfitShareUpdateEndpoint(IProfitShareUpdateService profitShareUpdateService)
        : base(Navigation.Constants.ProfitShareReportEditRun)
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
            s.ExampleRequest = ProfitShareUpdateRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }

    public override Task<ProfitShareUpdateResponse> GetResponse(ProfitShareUpdateRequest req, CancellationToken ct)
    {
        return _profitShareUpdateService.ProfitShareUpdate(req, ct);
    }

    public class ProfitShareUpdateClassMap : ClassMap<ProfitShareUpdateMemberResponse>
    {
        public ProfitShareUpdateClassMap()
        {
            Map(m => m.Psn).Index(0).Name("Number");
            Map(m => m.Name).Index(1).Name("Name");
            Map(m => m.BeginningAmount).Index(2).Name("Beginning Balance");
            Map(m => m.Contributions).Index(3).Name("Contributions");
            Map(m => m.AllEarnings).Index(4).Name("Earning");
            Map(m => m.AllSecondaryEarnings).Index(5).Name("Earning2");
            Map(m => m.IncomingForfeitures).Index(6).Name("Forfeits");
            Map(m => m.Distributions).Index(7).Name("Distributions");
            Map(m => m.Military).Index(8).Name("Military / Paid Alloc");
            Map(m => m.EndingBalance).Index(9).Name("Ending Balance");
        }
    }
}
