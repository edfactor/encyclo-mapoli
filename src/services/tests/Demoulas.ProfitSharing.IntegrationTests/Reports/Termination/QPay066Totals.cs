namespace Demoulas.ProfitSharing.IntegrationTests.Reports.Termination;

/// <summary>
///     Represents the totals section from the QPAY066 Termination - Profit Sharing report.
/// </summary>
public sealed record QPay066Totals
{
    public required decimal AmountInProfitSharing { get; init; }
    public required decimal VestedAmount { get; init; }
    public required decimal TotalForfeitures { get; init; }
    public required decimal TotalBeneficiaryAllocations { get; init; }
}
