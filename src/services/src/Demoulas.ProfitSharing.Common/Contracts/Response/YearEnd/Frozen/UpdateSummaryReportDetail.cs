using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed class UpdateSummaryReportDetail
{
    public int BadgeNumber { get; set; }
    public short StoreNumber { get; set; }
    public required string Name { get; set; }
    public bool IsEmployee { get; set; }
    public required UpdateSummaryReportPointInTimeDetail Before { get; set; }
    public required UpdateSummaryReportPointInTimeDetail After { get; set; }

    public static UpdateSummaryReportDetail ResponseExample()
    {
        return new UpdateSummaryReportDetail()
        {
            BadgeNumber = 2002,
            StoreNumber = 10,
            Name = "Oscar Taylor",
            Before = new UpdateSummaryReportPointInTimeDetail()
            {
                ProfitSharingAmount = 100,
                VestedProfitSharingAmount = 80,
                YearsInPlan = 6,
                EnrollmentId = 2
            },
            After = new UpdateSummaryReportPointInTimeDetail()
            {
                ProfitSharingAmount = 200,
                VestedProfitSharingAmount = 200,
                YearsInPlan = 7,
                EnrollmentId = 2
            }
        };
    }
}

public sealed class UpdateSummaryReportPointInTimeDetail
{
    public Decimal ProfitSharingAmount { get; set; }
    public Decimal VestedProfitSharingAmount { get; set; }
    public byte YearsInPlan { get; set; }
    public byte EnrollmentId { get; set; }
}
