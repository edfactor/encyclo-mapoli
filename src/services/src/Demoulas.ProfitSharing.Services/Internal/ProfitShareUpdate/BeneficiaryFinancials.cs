namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

/// <summary>
///     A summary of financial information about a Beneficiary
/// </summary>
internal sealed record BeneficiaryFinancials
{
    public long Psn { get; set; } // PIC 9(11).
    public int Ssn { get; set; } // PIC 9(9).
    public string? Name { get; set; } // PIC X(25).
    public decimal CurrentAmount { get; set; } // PIC S9(7)V99.
    public decimal Earnings { get; set; } // PIC S9(7)V99.
    public decimal SecondaryEarnings { get; set; } // PIC S9(7)V99.
}
