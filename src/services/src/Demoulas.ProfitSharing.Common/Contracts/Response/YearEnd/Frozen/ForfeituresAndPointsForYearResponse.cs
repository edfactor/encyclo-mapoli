namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record ForfeituresAndPointsForYearResponse
{
    public required int BadgeNumber { get; set; }
    public string? EmployeeName { get; set; }
    public required string Ssn { get; set; }
    public decimal? Forfeitures { get; set; }
    public required short ContForfeitPoints { get; set; }
    public required int EarningPoints { get; set; }
    public string? BeneficiaryPsn { get; set; }
    public required bool IsExecutive { get; set; }

    public static ForfeituresAndPointsForYearResponse ResponseExample()
    {
        return new ForfeituresAndPointsForYearResponse
        {
            BadgeNumber = 1234,
            EmployeeName = "Jane Doe",
            Ssn = "XXX-XX-9295",
            Forfeitures = 200.25m,
            ContForfeitPoints = 229,
            EarningPoints = 2048,
            IsExecutive = false,
        };
    }
}
