
namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class Distribution
{
    public required long SSN { get; set; }
    public required int SequenceNumber{ get; set; }
    public required string EmployeeName { get; set; }
    public required char FrequencyId { get; set; }
    public required DistributionFrequency Frequency { get; set; }
    public required char StatusId { get; set; }
    public required DistributionStatus Status { get; set; }
    public long PayeeSSN { get; set; }
    public required string PayeeName { get; set; }
    public required Address PayeeAddress { get; set; }
    public string? ThirdPartyPayee { get; set; }
    public string? ThirdPartyName { get; set; }
    public string? ThirdPartyAccount { get; internal set; }
    public required Address ThirdPartyAddress { get; set; }
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
    public bool Deceased { get; set; }
    public char? GenderId { get; set; }
    public Gender? Gender { get; set; }
    public bool QualifiedDomesticRelationsOrder { get; set; }
    public string? Memo { get; set; }
    public bool RothIRA { get; set; }
}
