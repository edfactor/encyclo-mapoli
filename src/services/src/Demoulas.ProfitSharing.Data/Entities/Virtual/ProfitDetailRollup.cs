namespace Demoulas.ProfitSharing.Data.Entities.Virtual;

public sealed class ProfitDetailRollup
{
    public int Ssn { get; set; }
    public decimal TotalContributions { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalForfeitures { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal Distribution { get; set; }
    public decimal BeneficiaryAllocation { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal MilitaryTotal { get; set; }
    public decimal ClassActionFundTotal { get; set; }
    public decimal PaidAllocationsTotal { get; set; }
    public decimal DistributionsTotal { get; set; }
    public decimal AllocationsTotal { get; set; }
    public decimal ForfeitsTotal { get; set; }
}
