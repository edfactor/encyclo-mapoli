namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Beneficiary
{
    public required int Id { get; set; }
    public required long Psn { get; set; }
   
    public BeneficiaryContact? Contact { get; set; }
    public required int BeneficiaryContactId { get; set; }

    public string? Relationship { get; set; }
    public  char? KindId { get; set; }
    public  BeneficiaryKind? Kind { get; set; }
    public decimal Distribution{ get; set; }
    public decimal Amount { get; set; }
    public decimal Earnings { get; set; }
    public decimal SecondaryEarnings { get; set; }
    public required decimal Percent { get; set; }
    
}
