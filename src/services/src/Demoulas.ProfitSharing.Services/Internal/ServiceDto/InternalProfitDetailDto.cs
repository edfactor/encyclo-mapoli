namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
internal sealed record InternalProfitDetailDto
{
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
}
