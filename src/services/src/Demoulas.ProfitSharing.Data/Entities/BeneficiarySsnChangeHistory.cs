namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class BeneficiarySsnChangeHistory : SsnChangeHistory
{
    public int BeneficiaryId { get; set; }
    public Beneficiary Beneficiary { get; set; } = null!;
}