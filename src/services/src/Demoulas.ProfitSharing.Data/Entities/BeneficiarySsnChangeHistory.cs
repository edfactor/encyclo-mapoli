namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class BeneficiarySsnChangeHistory : SsnChangeHistory
{
    public int BeneficiaryId { get; set; }
    public BeneficiaryContact BeneficiaryContact { get; set; } = null!;
}
