namespace Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;

/// <summary>
///     A summary of financial information about a Beneficiary
/// </summary>
internal sealed record BeneficiaryFinancials
{
    public required string? Psn { get; set; } // PIC 9(11).
    public required int Ssn { get; set; } // PIC 9(9).
    public required string? Name { get; set; } // PIC X(25).
    public required decimal BeginningBalance { get; set; } // PIC S9(7)V99.
}
