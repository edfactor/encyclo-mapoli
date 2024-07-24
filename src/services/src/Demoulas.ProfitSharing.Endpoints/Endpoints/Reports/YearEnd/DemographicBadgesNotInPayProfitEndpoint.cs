using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Csv;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
public class DemographicBadgesNotInPayProfitEndpoint:EndpointWithCSVBase<EmptyRequest, DemographicBadgesNotInPayProfitResponse, DemographicBadgesNotInPayProfitEndpoint.DemographicBadgesNotInPayProfitResponseMap>
{
    private readonly IYearEndService _yearEndService;

    public DemographicBadgesNotInPayProfitEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("demographic-badges-not-in-payprofit");
        Summary(s =>
        {
            s.Summary = "Demographic badges not in Payprofit";
        });
        Group<YearEndGroup>();
    }

    public override string ReportFileName => "DEMOGRAPHIC-WITHOUT-DEMOGRAPHICS";

    public override async Task<ReportResponseBase<DemographicBadgesNotInPayProfitResponse>> GetResponse(CancellationToken ct)
    {
        return await _yearEndService.GetDemographicBadgesNotInPayProfit(ct);
    }

    public sealed class DemographicBadgesNotInPayProfitResponseMap: ClassMap<DemographicBadgesNotInPayProfitResponse>
    {
        public DemographicBadgesNotInPayProfitResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.EmployeeSSN).Index(3).Name("DEM SSN");
            Map(m => m.EmployeeName).Index(4).Name("Name");
            Map(m => m.Store).Index(5).Name("Store");
            Map(m => m.Status).Index(6).Name("Status");
        }
    }
}
