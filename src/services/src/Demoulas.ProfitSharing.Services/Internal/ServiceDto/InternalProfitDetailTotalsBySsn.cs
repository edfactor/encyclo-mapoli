namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;

internal sealed class InternalProfitDetailTotalsBySsn
{
    public int Ssn { get; set; }
    public decimal DistributionsTotal { get; set; }
    public decimal ForfeitsTotal { get; set; }
    public decimal AllocationsTotal { get; set; }
    public decimal PaidAllocationsTotal { get; set; }
    public decimal MilitaryTotal { get; set; }
    public decimal ClassActionFundTotal { get; set; }
}
