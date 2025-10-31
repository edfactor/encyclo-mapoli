using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record YearEndProfitSharingReportDetail : IIsExecutive, IProfitYearRequest
{
    public required int BadgeNumber { get; set; }
    public required short ProfitYear { get; set; }
    public required short PriorProfitYear { get; set; }
    [MaskSensitive] public required string EmployeeName { get; set; }
    public required short StoreNumber { get; set; }
    public required char EmployeeTypeCode { get; set; }
    public required string EmployeeTypeName { get; set; }
    [MaskSensitive] public required DateOnly DateOfBirth { get; set; }
    public required byte Age { get; set; }
    public required string Ssn { get; set; }
    public required decimal Wages { get; set; }
    public required decimal PriorBalance { get; set; }
    public required decimal Hours { get; set; }

    [MaskSensitive]
    public short Points { get; set; }
    public required bool IsUnder21 { get; set; }
    public required bool IsNew { get; set; }
    public char? EmployeeStatus { get; set; }
    public required decimal Balance { get; set; }
    public required short YearsInPlan { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public short? FirstContributionYear { get; set; }
    public bool IsExecutive { get; set; }

    public static YearEndProfitSharingReportDetail ResponseExample()
    {
        return new YearEndProfitSharingReportDetail()
        {
            BadgeNumber = 135,
            PriorProfitYear = 2023,
            ProfitYear = 2024,
            EmployeeName = "John Doe",
            StoreNumber = 23,
            EmployeeTypeCode = 'p',
            EmployeeTypeName = "Part Time",
            DateOfBirth = new DateOnly(1996, 9, 21),
            Age = 28,
            Ssn = "XXX-XX-1234",
            Wages = 26527,
            PriorBalance = 12345,
            Hours = 1475,
            Points = 2653,
            IsUnder21 = false,
            IsNew = false,
            EmployeeStatus = ' ',
            Balance = 51351.55m,
            YearsInPlan = 1,
            TerminationDate = null
        };
    }
}
