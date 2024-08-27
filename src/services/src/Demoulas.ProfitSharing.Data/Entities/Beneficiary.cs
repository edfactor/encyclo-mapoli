namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Beneficiary
{
    public required long Psn { get; set; }
    public required long Ssn { get; set; }

    public required string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }= string.Empty;

    public required DateOnly DateOfBirth { get; set; }

    public required Address Address { get; set; }
    public required ContactInfo ContactInfo { get; set; }

    public required char KindId { get; set; }
    public required BeneficiaryKind Kind { get; set; }

    public decimal Distribution{ get; set; }
    public decimal Amount { get; set; }
    public decimal Earnings { get; set; }
    public decimal SecondaryEarnings { get; set; }

    public List<PayProfit>? PayProfits { get; set; }
}
