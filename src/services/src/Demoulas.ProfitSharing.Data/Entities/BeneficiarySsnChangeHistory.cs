namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class BeneficiarySsnChangeHistory : SsnChangeHistory
{
    public int BeneficiaryContactId { get; set; }
    public BeneficiaryContact BeneficiaryContact { get; set; } = null!;
}
