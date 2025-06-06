namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
public sealed record GroupedProfitSummaryDto
{
    public int ProfitYear { get; set; }
    public byte MonthToDate { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalForfeiture { get; set; }
    public decimal TotalPayment { get; set; }
    public int TransactionCount { get; set; }
}
