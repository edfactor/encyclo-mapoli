using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record RehireTransactionDetailResponse : IProfitYearRequest
{
    public required short ProfitYear { get; set; }
    public required decimal Forfeiture { get; set; }
    public required string? Remark { get; set; }
    public byte ProfitCodeId { get; set; }
    public decimal? WagesTransactionYear { get; set; }
    public required decimal? HoursTransactionYear { get; set; }
    public decimal? SuggestedUnforfeiture { get; set; }

    // Need a property for the profit detail id from the PROFIT_DETAIL database table
    public int ProfitDetailId { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static RehireTransactionDetailResponse ResponseExample()
    {
        return new RehireTransactionDetailResponse
        {
            ProfitYear = 2022,
            Forfeiture = 3254.14m,
            Remark = "Rehire forfeiture recovery",
            ProfitCodeId = 2,
            WagesTransactionYear = 15000.00m,
            HoursTransactionYear = 800.00m,
            SuggestedUnforfeiture = 3254.14m,
            ProfitDetailId = 1001
        };
    }
}
