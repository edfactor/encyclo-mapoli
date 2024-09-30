
namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class Distribution
{
    public long Id { get; set; } 
    
    public required long Ssn { get; set; }
    public required byte PaymentSequence{ get; set; }
    
    public required string EmployeeName { get; set; }
    public required char FrequencyId { get; set; }
    public DistributionFrequency? Frequency { get; set; }
    public required char StatusId { get; set; }
    public required DistributionStatus Status { get; set; }
    
    public DistributionPayee? Payee { get; set; }
    public required int PayeeId { get; set; }

    public DistributionThirdPartyPayee? ThirdPartyPayee { get; set; }
    public required int ThirdPartyPayeeId { get; set; }

    public string? ForTheBenefitOfPayee { get; set; }
    public string? ForTheBenefitOfAccountType { get; set; }
    public bool Tax1099ForEmployee { get; set; }
    public bool Tax1099ForBeneficiary { get; set; }
    public decimal FederalTaxPercentage { get; set; }
    public decimal StateTaxPercentage { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal FederalTaxAmount { get; set; }
    public decimal StateTaxAmount { get; set; }
    public decimal CheckAmount { get; set; }
    public char TaxCodeId { get; set; }
    public required TaxCode TaxCode { get; set; }
    public bool IsDeceased { get; set; }
    public char? GenderId { get; set; }
    public Gender? Gender { get; set; }
    public bool QualifiedDomesticRelationsOrder { get; set; }
    public string? Memo { get; set; }
    public bool RothIra { get; set; }
    
}
