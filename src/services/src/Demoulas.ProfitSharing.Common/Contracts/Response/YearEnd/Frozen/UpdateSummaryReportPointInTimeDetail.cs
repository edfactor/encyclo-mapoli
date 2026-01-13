namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed class UpdateSummaryReportPointInTimeDetail
{
    public decimal ProfitSharingAmount { get; set; }
    public decimal VestedProfitSharingAmount { get; set; }
    public byte YearsInPlan { get; set; }
    public byte EnrollmentId { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static UpdateSummaryReportPointInTimeDetail ResponseExample()
    {
        return new UpdateSummaryReportPointInTimeDetail
        {
            ProfitSharingAmount = 75000.00m,
            VestedProfitSharingAmount = 67500.00m,
            YearsInPlan = 10,
            EnrollmentId = 2
        };
    }
}
