namespace Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;

public sealed record RecipientBeneficiary
{
    public short PsnSuffix { get; init; }
    public decimal? Percentage { get; init; }
    public decimal? Amount { get; init; }
}
