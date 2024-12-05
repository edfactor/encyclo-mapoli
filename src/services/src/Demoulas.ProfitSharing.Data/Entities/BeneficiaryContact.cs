namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryContact
{
    public required int Id { get; set; }

    public required int Ssn { get; set; }

    public required DateOnly DateOfBirth { get; set; }

    public required Address Address { get; set; }
    public required ContactInfo ContactInfo { get; set; }

    public required DateOnly CreatedDate { get; set; }

    public List<Beneficiary>? Beneficiaries { get; set; }
}
