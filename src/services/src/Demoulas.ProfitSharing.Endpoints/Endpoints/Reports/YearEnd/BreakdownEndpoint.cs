using CsvHelper.Configuration;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Base;
using Demoulas.ProfitSharing.Endpoints.Groups;
using Demoulas.ProfitSharing.Security;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd;

public class BreakdownEndpoint : EndpointWithCsvBase<BreakdownByStoreRequest, MemberYearSummaryDto, BreakdownEndpoint.BreakdownEndpointMap>
{
    private readonly IBreakdownService _breakdownService;

    public BreakdownEndpoint(IBreakdownService breakdownService)
    {
        _breakdownService = breakdownService;
    }

    public override string ReportFileName => "Breakdown by Store - QPAY066TA";

    public override void Configure()
    {
        Get("/breakdown-by-store");
        Summary(s =>
        {
            s.Summary = "Breakdown managers and associates for all stores";
            s.Responses[403] = $"Forbidden.  Requires roles of {Role.ADMINISTRATOR} or {Role.FINANCEMANAGER}";
        });
        Group<YearEndGroup>();
        base.Configure();
    }

    public override Task<ReportResponseBase<MemberYearSummaryDto>> GetResponse(BreakdownByStoreRequest breakdownByStoreRequest, CancellationToken ct)
    {
        return _breakdownService.GetActiveMembersByStore(breakdownByStoreRequest, ct);
    }

    public sealed class BreakdownEndpointMap : ClassMap<MemberYearSummaryDto>
    {
        public BreakdownEndpointMap()
        {
            Map(m => m.StoreNumber).Index(0).Name("StoreNumber");
            Map(m => m.EnrollmentId).Index(1).Name("EnrollmentId");
            Map(m => m.BadgeNumber).Index(2).Name("BadgeNumber");
            Map(m => m.Ssn).Index(3).Name("Ssn");
            Map(m => m.FullName).Index(4).Name("FullName");
            Map(m => m.PayFrequencyId).Index(5).Name("PayFrequencyId");
            Map(m => m.DepartmentId).Index(6).Name("DepartmentId");
            Map(m => m.PayClassificationId).Index(7).Name("PayClassificationId");
            Map(m => m.BeginningBalance).Index(8).Name("BeginningBalance");
            Map(m => m.Earnings).Index(9).Name("Earnings");
            Map(m => m.Contributions).Index(10).Name("Contributions");
            Map(m => m.Forfeiture).Index(11).Name("Forfeiture");
            Map(m => m.Distributions).Index(12).Name("Distributions");
            Map(m => m.EndingBalance).Index(13).Name("EndingBalance");
            Map(m => m.VestedAmount).Index(14).Name("VestedAmount");
            Map(m => m.VestedPercentage).Index(15).Name("VestedPercentage");
            Map(m => m.EmploymentStatusId).Index(16).Name("EmploymentStatusId");
        }
    }
}
