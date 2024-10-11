namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryContact : ContactInfo
{
    public required int Id { get; set; }

    public required long Ssn { get; set; }

    public required DateOnly DateOfBirth { get; set; }

    public required Address Address { get; set; }
    
    public required DateOnly CreatedDate { get; set; }
    
    public List<Beneficiary>? Beneficiaries { get; set; }
}
