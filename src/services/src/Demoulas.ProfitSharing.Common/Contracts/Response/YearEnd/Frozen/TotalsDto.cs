
namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

// This set of totals reflects the effect of running PAY444 with a user selected set of parameters (ie, contribution = 11.5%, ...)
public record TotalsDto
{
    public decimal BeginningBalance { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal Earnings { get; set; }
    public decimal Earnings2 { get; set; }
    public decimal Forfeiture { get; set; }
    public decimal Distributions { get; set; }
    public decimal Military { get; set; }
    public decimal EndingBalance { get; set; }

    public decimal Allocations { get; set; }
    public decimal PaidAllocations { get; set; }
    public decimal ClassActionFund { get; set; }
    
    public long ContributionPoints { get; set; }
    public long EarningPoints { get; set; }

    public decimal MaxOverTotal { get; set; }
    public long MaxPointsTotal { get; set; }
}
