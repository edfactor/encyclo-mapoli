using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed class GrossWagesReportDetail : IIsExecutive
{
    public required int BadgeNumber { get; set; }
    [MaskSensitive] public required string EmployeeName { get; set; }
    [MaskSensitive] public required DateOnly DateOfBirth { get; set; }
    public required string Ssn { get; set; }
    public required decimal GrossWages { get; set; }
    public required decimal ProfitSharingAmount { get; set; }
    public required decimal Loans { get; set; }
    public required decimal Forfeitures { get; set; }
    public int EnrollmentId { get; set; }
    public required bool IsExecutive { get; set; }

    public static GrossWagesReportDetail ResponseExample()
    {
        return new GrossWagesReportDetail()
        {
            BadgeNumber = 123456,
            EmployeeName = "Sam Hughes",
            DateOfBirth = new DateOnly(1993, 4, 28),
            Ssn = "xxx-xx-1942",
            GrossWages = 52005.15m,
            ProfitSharingAmount = 150023.55m,
            Loans = 0,
            Forfeitures = 5001m,
            IsExecutive = false,
        };
    }
}
