namespace Demoulas.ProfitSharing.Common.Contracts.Services;
public class ParticipantTotalVestingBalanceDto
{
    public int Ssn { get; set; }
    public decimal VestedBalance { get; set; }
    public decimal TotalDistributions { get; set; } 
    public decimal Etva { get; set; }
    public decimal VestingPercent { get; set; }
    public decimal CurrentBalance { get; set; }
}
