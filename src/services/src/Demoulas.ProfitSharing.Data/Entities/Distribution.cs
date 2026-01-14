
using System.Diagnostics;
using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;


[DebuggerDisplay("Id={Id} EmployeeName={EmployeeName} CheckAmount={CheckAmount}")]
public sealed class Distribution : ModifiedBase
{
    public long Id { get; set; }

    public required int Ssn { get; set; }
    public required byte PaymentSequence { get; set; }

    public required string EmployeeName { get; set; }
    public required char FrequencyId { get; set; }
    public DistributionFrequency? Frequency { get; set; }
    public required char StatusId { get; set; }
    public DistributionStatus? Status { get; set; }

    public DistributionPayee? Payee { get; set; }
    public int? PayeeId { get; set; }

    public DistributionThirdPartyPayee? ThirdPartyPayee { get; set; }
    public int? ThirdPartyPayeeId { get; set; }

    public string? ForTheBenefitOfPayee { get; set; }
    public string? ForTheBenefitOfAccountType { get; set; }
    public bool Tax1099ForEmployee { get; set; }
    public bool Tax1099ForBeneficiary { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal FederalTaxPercentage { get => FederalTaxAmount / GrossAmount; }
    public decimal StateTaxPercentage { get => StateTaxAmount / GrossAmount; }
    public decimal FederalTaxAmount { get; set; }
    public decimal StateTaxAmount { get; set; }
    public decimal CheckAmount { get => GrossAmount - FederalTaxAmount - StateTaxAmount; }
    public char TaxCodeId { get; set; }
    public TaxCode? TaxCode { get; set; }
    public bool IsDeceased { get; set; }
    public char? GenderId { get; set; }
    public Gender? Gender { get; set; }
    public bool QualifiedDomesticRelationsOrder { get; set; }
    public string? Memo { get; set; }
    public bool RothIra { get; set; }
    public string? ThirdPartyPayeeAccount { get; set; }
    public string? ManualCheckNumber { get; set; }

}
