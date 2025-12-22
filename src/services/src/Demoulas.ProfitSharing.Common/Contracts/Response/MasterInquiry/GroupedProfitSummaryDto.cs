using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;

public sealed record GroupedProfitSummaryDto : IProfitYearRequest
{
    public short ProfitYear { get; set; }
    public byte MonthToDate { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalForfeiture { get; set; }
    public decimal TotalPayment { get; set; }
    public int TransactionCount { get; set; }

    public static GroupedProfitSummaryDto ResponseExample()
    {
        return new GroupedProfitSummaryDto
        {
            ProfitYear = 2024,
            MonthToDate = 6,
            TotalContribution = 5000.00m,
            TotalEarnings = 1250.00m,
            TotalForfeiture = 250.00m,
            TotalPayment = 500.00m,
            TransactionCount = 15
        };
    }
}
