namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record ForfeituresAndPointsForYearResponseWithTotals : ReportResponseBase<ForfeituresAndPointsForYearResponse>
{
    public decimal TotalForfeitures { get; set; }
    public int TotalForfeitPoints { get; set; }
    public int TotalEarningPoints { get; set; }
    public decimal? TotalProfitSharingBalance { get; set; }
    

    public decimal? DistributionTotals { get; set; }
    public decimal? AllocationToTotals { get; set; }
    public decimal? AllocationsFromTotals { get; set; }

    public static ForfeituresAndPointsForYearResponseWithTotals ResponseExample()
    {
        return new ForfeituresAndPointsForYearResponseWithTotals
        {
            ReportName = "Forfeitures and Points for Year",
            ReportDate = DateTimeOffset.Now,
            TotalForfeitures = 1234.56m,
            TotalForfeitPoints = 15000,
            TotalEarningPoints = 85000,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Response = new Demoulas.Common.Contracts.Contracts.Response.PaginatedResponseDto<ForfeituresAndPointsForYearResponse>
            {
                Results = new List<ForfeituresAndPointsForYearResponse> { ForfeituresAndPointsForYearResponse.ResponseExample() }
            }
        };
    }
}
