using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed class UpdateSummaryReportDetail : IIsExecutive
{
    public long BadgeNumber { get; set; }
    public short StoreNumber { get; set; }
    [MaskSensitive] public required string Name { get; set; }
    public bool IsEmployee { get; set; }
    public bool IsExecutive { get; set; }
    public required UpdateSummaryReportPointInTimeDetail Before { get; set; }
    public required UpdateSummaryReportPointInTimeDetail After { get; set; }

    public static UpdateSummaryReportDetail ResponseExample()
    {
        return new UpdateSummaryReportDetail()
        {
            BadgeNumber = 2002,
            StoreNumber = 10,
            Name = "Oscar Taylor",
            Before = new UpdateSummaryReportPointInTimeDetail
            {
                ProfitSharingAmount = 100,
                VestedProfitSharingAmount = 80,
                YearsInPlan = 6,
                EnrollmentId = 2,
            },
            After = new UpdateSummaryReportPointInTimeDetail
            {
                ProfitSharingAmount = 200,
                VestedProfitSharingAmount = 200,
                YearsInPlan = 7,
                EnrollmentId = 2,
            }
        };
    }
}
