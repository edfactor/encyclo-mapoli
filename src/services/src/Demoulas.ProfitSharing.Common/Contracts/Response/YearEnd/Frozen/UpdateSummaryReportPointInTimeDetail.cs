namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed class UpdateSummaryReportPointInTimeDetail
{
    public Decimal ProfitSharingAmount { get; set; }
    public Decimal VestedProfitSharingAmount { get; set; }
    public byte YearsInPlan { get; set; }
    public byte EnrollmentId { get; set; }
}