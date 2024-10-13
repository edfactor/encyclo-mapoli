namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryContact
{
    public required int Id { get; set; }

    public string? FullName { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }

    public string? PhoneNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string? EmailAddress { get; set; }

    public required long Ssn { get; set; }

    public required DateOnly DateOfBirth { get; set; }

    public required Address Address { get; set; }
    
    public required DateOnly CreatedDate { get; set; }
    
    public List<Beneficiary>? Beneficiaries { get; set; }
}
