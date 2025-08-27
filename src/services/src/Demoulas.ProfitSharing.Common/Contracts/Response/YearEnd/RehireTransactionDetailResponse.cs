namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record RehireTransactionDetailResponse
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
}
