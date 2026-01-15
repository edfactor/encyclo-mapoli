
using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

// This set of totals reflects the effect of running PAY444 with a user selected set of parameters (ie, contribution = 11.5%, ...)
public record ProfitShareUpdateTotals
{
    public decimal BeginningBalance { get; set; }
    public decimal TotalContribution { get; set; }
    public decimal Earnings { get; set; }
    public decimal Earnings2 { get; set; }
    [YearEndArchiveProperty]
    public decimal Forfeiture { get; set; }
    [YearEndArchiveProperty]
    public decimal Distributions { get; set; }
    public decimal Military { get; set; }
    public decimal EndingBalance { get; set; }

    public decimal Allocations { get; set; }
    public decimal PaidAllocations { get; set; }
    public decimal ClassActionFund { get; set; }

    [MaskSensitive]
    public long ContributionPoints { get; set; }

    [MaskSensitive]
    public long EarningPoints { get; set; }

    public decimal MaxOverTotal { get; set; }
    public long MaxPointsTotal { get; set; }

    public long TotalEmployees { get; set; }
    public long TotalBeneficaries { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static ProfitShareUpdateTotals ResponseExample()
    {
        return new ProfitShareUpdateTotals
        {
            BeginningBalance = 5000000.00m,
            TotalContribution = 450000.00m,
            Earnings = 300000.00m,
            Earnings2 = 150000.00m,
            Forfeiture = 75000.00m,
            Distributions = 200000.00m,
            Military = 50000.00m,
            EndingBalance = 5375000.00m,
            Allocations = 125000.00m,
            PaidAllocations = 100000.00m,
            ClassActionFund = 25000.00m,
            ContributionPoints = 10000,
            EarningPoints = 5000,
            MaxOverTotal = 0m,
            MaxPointsTotal = 15000,
            TotalEmployees = 2500,
            TotalBeneficaries = 850
        };
    }
}
