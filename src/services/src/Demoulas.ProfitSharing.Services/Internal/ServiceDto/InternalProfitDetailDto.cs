namespace Demoulas.ProfitSharing.Services.Internal.ServiceDto;
internal sealed record InternalProfitDetailDto
{
    internal long OracleHcmId { get; set; }
    internal int BadgeNumber { get; set; }

    internal decimal TotalContributions { get; set; }
    internal decimal TotalEarnings { get; set; }
    internal decimal TotalForfeitures { get; set; }
    internal decimal TotalPayments { get; set; }
    internal decimal TotalFederalTaxes { get; set; }
    internal decimal TotalStateTaxes { get; set; }
    internal decimal CurrentAmount { get; set; }
    internal decimal Distribution { get; set; }
    internal decimal BeneficiaryAllocation { get; set; }
}
