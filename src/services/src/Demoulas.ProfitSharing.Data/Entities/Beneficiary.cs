namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Beneficiary
{
    public required long PSN { get; set; }
    public required long SSN { get; set; }

    public required string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }= string.Empty;

    public required DateOnly DateOfBirth { get; set; }

    public required Address Address { get; set; }
    public required ContactInfo ContactInfo { get; set; }

    public required byte BeneficiaryTypeId { get; set; }
    public required BeneficiaryType BeneficiaryType { get; set; }

    public decimal Distribution{ get; set; }
    public decimal Amount { get; set; }
    public decimal Earnings { get; set; }
    public decimal SecondaryEarnings { get; set; }
}
