using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Data.Entities.Navigations;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class ProfitShareEditEndpoint
    : EndpointWithCsvTotalsBase<ProfitShareUpdateRequest,
        ProfitShareEditResponse,
        ProfitShareEditMemberRecordResponse,
        ProfitShareEditEndpoint.ProfitShareEditClassMap>
{
    private readonly IProfitShareEditService _editService;

    public ProfitShareEditEndpoint(IProfitShareEditService editService)
        : base(Navigation.Constants.ProfitShareReportEditRun)
    {
        _editService = editService;
    }

    public override string ReportFileName => "profit-share-edit-report";

    public override void Configure()
    {
        Get("profit-share-edit");
        Summary(s =>
        {
            s.Summary = "profit share edit";
            s.Description =
                "Returns per member transactions based on user specified contribution/incoming forfeit/earnings parameters";
            s.ExampleRequest = ProfitShareUpdateRequest.RequestExample();
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();

        base.Configure();
    }

    public override Task<ProfitShareEditResponse> GetResponse(ProfitShareUpdateRequest req, CancellationToken ct)
    {
        return _editService.ProfitShareEdit(req, ct);
    }

    public sealed class ProfitShareEditClassMap : ClassMap<ProfitShareEditMemberRecordResponse>
    {
        internal ProfitShareEditClassMap()
        {
            int dex = 0;
            Map(m => m.Psn).Index(dex++).Name("Number");
            Map(m => m.Name).Index(dex++).Name("Name");
            Map(m => m.Code).Index(dex++).Name("Code");
            Map(m => m.ContributionAmount).Index(dex++).Name("Contribution Amount");
            Map(m => m.EarningsAmount).Index(dex++).Name("Earnings Amount");
            Map(m => m.ForfeitureAmount).Index(dex++).Name("Incoming Forfeitures");
            Map(m => m.Remark).Index(dex).Name("Reason");
        }
    }
}
