namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record ForfeituresAndPointsForYearResponse
{
    public required long EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public required string EmployeeSsn { get; set; }
    public decimal? Forfeitures { get; set; }
    public required short ForfeitPoints { get; set; }
    public required int EarningPoints { get; set; }
    public string? BeneficiaryPsn { get; set; }

    public static ForfeituresAndPointsForYearResponse ResponseExample()
    {
        return new ForfeituresAndPointsForYearResponse
        {
            EmployeeId = 1234,
            EmployeeName = "Jane Doe",
            EmployeeSsn = "XXX-XX-9295",
            Forfeitures = 200.25m,
            ForfeitPoints = 229,
            EarningPoints = 2048,
        };
    }
}
