namespace Demoulas.ProfitSharing.Data.Entities.Virtual;

public class ParticipantTotalVestingBalance
{
    public int Ssn { get; set; }
    public int Id { get; set; }
    public decimal? VestedBalance { get; set; }
    public decimal? VestingPercent { get; set; }
    public decimal? CurrentBalance { get; set; }
    public byte? YearsInPlan { get; set; }
    public decimal? AllocationsToBeneficiary { get; set; }
    public decimal? AllocationsFromBeneficiary { get; set; }
}
