namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record ForfeituresAndPointsForYearResponse
{
    public required int BadgeNumber { get; set; }
    public string? EmployeeName { get; set; }
    public required string Ssn { get; set; }
    public decimal? Forfeitures { get; set; }
    public required short ForfeitPoints { get; set; }
    public required int EarningPoints { get; set; }
    public string? BeneficiaryPsn { get; set; }

    public static ForfeituresAndPointsForYearResponse ResponseExample()
    {
        return new ForfeituresAndPointsForYearResponse
        {
            BadgeNumber = 1234,
            EmployeeName = "Jane Doe",
            Ssn = "XXX-XX-9295",
            Forfeitures = 200.25m,
            ForfeitPoints = 229,
            EarningPoints = 2048,
        };
    }
}

public sealed record ForfeituresAndPointsForYearResponseWithTotals : ReportResponseBase<ForfeituresAndPointsForYearResponse>
{
    public decimal TotalForfeitures { get; set; }
    public int TotalForfeitPoints { get; set; }
    public int TotalEarningPoints { get; set; }

    public static ForfeituresAndPointsForYearResponseWithTotals ResponseExample()
    {
        return new ForfeituresAndPointsForYearResponseWithTotals
        {
            ReportName = "Forfeitures and Points for Year",
            ReportDate = DateTimeOffset.Now,
            TotalForfeitures = 1234.56m,
            TotalForfeitPoints = 15000,
            TotalEarningPoints = 85000,
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<ForfeituresAndPointsForYearResponse>
            {
                Results = new List<ForfeituresAndPointsForYearResponse> { ForfeituresAndPointsForYearResponse.ResponseExample() }
            }
        };
    }
}
