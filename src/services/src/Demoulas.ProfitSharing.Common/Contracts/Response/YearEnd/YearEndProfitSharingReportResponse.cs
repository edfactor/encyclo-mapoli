namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record YearEndProfitSharingReportResponse
{
    public required long BadgeNumber { get; set; }
    public required string EmployeeName { get; set; }
    public required short StoreNumber { get; set; }
    public required char EmployeeTypeCode { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required byte Age { get; set; }
    public required string EmployeeSsn { get; set; }
    public required decimal Wages { get; set; }
    public required decimal Hours { get; set; }
    public short? Points { get; set; }
    public required bool IsUnder21 { get; set; }
    public required bool IsNew { get; set; }
    public char? EmployeeStatus { get; set; }
    public required Decimal Balance { get; set; }
    public required short YearsInPlan { get; set; }

    public static YearEndProfitSharingReportResponse ResponseExample()
    {
        return new YearEndProfitSharingReportResponse()
        {
            BadgeNumber = 135,
            EmployeeName = "John Doe",
            StoreNumber = 23,
            EmployeeTypeCode = 'a',
            DateOfBirth = new DateOnly(1996, 9, 21),
            Age = 28,
            EmployeeSsn = "XXX-XX-1234",
            Wages = 26527,
            Hours = 1475,
            Points = 2653,
            IsUnder21 = false,
            IsNew = false,
            EmployeeStatus = ' ',
            Balance = 51351.55m,
            YearsInPlan = 1
        };
    }
}
