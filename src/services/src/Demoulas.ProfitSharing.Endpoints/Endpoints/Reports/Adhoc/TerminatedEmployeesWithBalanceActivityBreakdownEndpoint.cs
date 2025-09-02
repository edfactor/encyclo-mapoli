using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.Adhoc;
public sealed class TerminatedEmployeesWithBalanceActivityBreakdownEndpoint : EndpointWithCsvBase<TerminatedEmployeesWithBalanceBreakdownRequest, MemberYearSummaryDto, TerminatedEmployeesWithBalanceActivityBreakdownEndpoint.BreakdownEndpointMap>
{
    private readonly IBreakdownService _breakdownService;

    public TerminatedEmployeesWithBalanceActivityBreakdownEndpoint(IBreakdownService breakdownService)
        : base(Navigation.Constants.QPAY066AdHocReports)
    {
        _breakdownService = breakdownService;
    }

    public override string ReportFileName => "Breakdown by Store - Terminated Employees with Balance Activity";

    public override void Configure()
    {
        Get("/breakdown-by-store/terminated/withbalanceactivity");
        Summary(s =>
        {
            s.Summary = "Breakdown terminated managers and associates for all stores who have a balance";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<ReportResponseBase<MemberYearSummaryDto>> GetResponse(TerminatedEmployeesWithBalanceBreakdownRequest breakdownByStoreRequest, CancellationToken ct)
    {
        return _breakdownService.GetTerminatedMembersWithBalanceActivityByStore(breakdownByStoreRequest, ct);
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
