using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;
public sealed class InactiveEmployeesWithVestedBalanceBreakdown : EndpointWithCsvBase<BreakdownByStoreRequest, MemberYearSummaryDto, InactiveEmployeesWithVestedBalanceBreakdown.BreakdownEndpointMap>
{
    private readonly IBreakdownService _breakdownService;

    public InactiveEmployeesWithVestedBalanceBreakdown(IBreakdownService breakdownService)
    {
        _breakdownService = breakdownService;
    }

    public override string ReportFileName => "Breakdown by Store - QPAY066i - Inactive employees with vested balance";

    public override void Configure()
    {
        Get("/breakdown-by-store/inactive/withvestedbalance");
        Summary(s =>
        {
            s.Summary = "Breakdown inactive managers and associates for all stores";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<ReportResponseBase<MemberYearSummaryDto>> GetResponse(BreakdownByStoreRequest breakdownByStoreRequest, CancellationToken ct)
    {
        return _breakdownService.GetInactiveMembersWithVestedBalanceByStore(breakdownByStoreRequest, ct);
    }

    public sealed class BreakdownEndpointMap : ClassMap<MemberYearSummaryDto>
    {
        public BreakdownEndpointMap()
        {
            Map(m => m.StoreNumber).Index(0).Name("StoreNumber");
            Map(m => m.BadgeNumber).Index(2).Name("BadgeNumber");
            Map(m => m.FullName).Index(4).Name("FullName");
            Map(m => m.PayClassificationId).Index(7).Name("PayClassificationId");
            Map(m => m.BeginningBalance).Index(8).Name("BeginningBalance");
            Map(m => m.Earnings).Index(9).Name("Earnings");
            Map(m => m.Contributions).Index(10).Name("Contributions");
            Map(m => m.Distributions).Index(12).Name("Distributions");
            Map(m => m.EndingBalance).Index(13).Name("EndingBalance");
            Map(m => m.VestedAmount).Index(14).Name("VestedAmount");
            Map(m => m.VestedPercent).Index(15).Name("VestedPercentage");
            Map(m => m.PayClassificationName).Index(16).Name("PayClassificationName");
        }
    }
}

