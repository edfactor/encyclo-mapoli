using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;

public sealed record MemberProfitPlanDetails : MemberDetails
{
    public int YearsInPlan { get; init; }
    [Unmask] public decimal PercentageVested { get; init; }
    public decimal? BeginPSAmount { get; set; }
    public decimal? CurrentPSAmount { get; set; }
    public decimal? BeginVestedAmount { get; set; }
    public decimal? CurrentVestedAmount { get; set; }
    public decimal AllocationToAmount { get; set; }
    public decimal AllocationFromAmount { get; set; }
}
