namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed class UpdateSummaryReportPointInTimeDetail
{
    public decimal ProfitSharingAmount { get; set; }
    public decimal VestedProfitSharingAmount { get; set; }
    public byte YearsInPlan { get; set; }
    public byte EnrollmentId { get; set; }
}
