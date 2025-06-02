namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
public sealed record MemberProfitPlanDetails : MemberDetails
{
    public int YearsInPlan { get; init; }
    public decimal PercentageVested { get; init; }
    public bool ContributionsLastYear { get; init; }
    public decimal BeginPSAmount { get; set; }
    public decimal CurrentPSAmount { get; set; }
    public decimal BeginVestedAmount { get; set; }
    public decimal CurrentVestedAmount { get; set;}
}
