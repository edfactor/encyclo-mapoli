using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Groups;
using FastEndpoints;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;
public class PayProfitBadgesNotInDemographicsEndpoint: EndpointWithCSVBase<EmptyRequest,PayProfitBadgesNotInDemographicsResponse, PayProfitBadgesNotInDemographicsEndpoint.PayProfitBadgesNotInDemographicsResponseMap>
{
    private readonly IYearEndService _yearEndService;

    public PayProfitBadgesNotInDemographicsEndpoint(IYearEndService yearEndService)
    {
        _yearEndService = yearEndService;
    }

    public override void Configure()
    {
        AllowAnonymous();
        Get("payprofit-badges-without-demographics");
        Summary(s =>
        {
            s.Summary = "Payprofit Badges not in Demographics";
        });
        Group<YearEndGroup>();
    }

    public override string ReportFileName => "PAYPROFIT-WITOUT-DEMOGRAPHICS";

    public override async Task<ReportResponseBase<PayProfitBadgesNotInDemographicsResponse>> GetResponse(CancellationToken ct)
    {
        return await _yearEndService.GetPayProfitBadgesNotInDemographics(ct);
    }

    public sealed class PayProfitBadgesNotInDemographicsResponseMap : ClassMap<PayProfitBadgesNotInDemographicsResponse>
    {
        public PayProfitBadgesNotInDemographicsResponseMap()
        {
            Map().Index(0).Convert(_ => string.Empty);
            Map().Index(1).Convert(_ => string.Empty);
            Map(m => m.EmployeeBadge).Index(2).Name("BADGE");
            Map(m => m.EmployeeSSN).Index(3).Name("SSN");
        }
    }
}
