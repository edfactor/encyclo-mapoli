namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
public sealed record InternalProfitDetailDto
{
    public int Ssn { get; set; }
    public long OracleHcmId { get; set; }
    public int BadgeNumber { get; set; }

    public decimal TotalContributions { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalForfeitures { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal TotalFederalTaxes { get; set; }
    public decimal TotalStateTaxes { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal Distribution { get; set; }
    public decimal BeneficiaryAllocation { get; set; }
    public short ProfitYear { get; set; }
}
